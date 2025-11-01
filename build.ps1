# DownSort Build Script
# PowerShell script for building, testing, and packaging the application

param(
    [Parameter()]
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    
    [Parameter()]
    [switch]$SkipTests,
    
    [Parameter()]
    [switch]$Package
)

$ErrorActionPreference = "Stop"

Write-Host "====================================" -ForegroundColor Cyan
Write-Host " DownSort Build Script" -ForegroundColor Cyan
Write-Host "====================================" -ForegroundColor Cyan
Write-Host ""

# Variables
$SolutionFile = "DownSort.sln"
$OutputDir = ".\bin\$Configuration"
$PublishDir = ".\publish"
$ProjectName = "Downsort"

# Step 1: Clean
Write-Host "[1/5] Cleaning previous builds..." -ForegroundColor Yellow
if (Test-Path $OutputDir) {
    Remove-Item -Path $OutputDir -Recurse -Force
}
if (Test-Path $PublishDir) {
    Remove-Item -Path $PublishDir -Recurse -Force
}

dotnet clean $SolutionFile --configuration $Configuration
Write-Host "Clean completed successfully!" -ForegroundColor Green
Write-Host ""

# Step 2: Restore
Write-Host "[2/5] Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore $SolutionFile
if ($LASTEXITCODE -ne 0) {
    Write-Host "Restore failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}
Write-Host "Restore completed successfully!" -ForegroundColor Green
Write-Host ""

# Step 3: Build
Write-Host "[3/5] Building solution..." -ForegroundColor Yellow
dotnet build $SolutionFile --configuration $Configuration --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}
Write-Host "Build completed successfully!" -ForegroundColor Green
Write-Host ""

# Step 4: Test
if (-not $SkipTests) {
    Write-Host "[4/5] Running tests..." -ForegroundColor Yellow
    dotnet test $SolutionFile --configuration $Configuration --no-build --verbosity normal `
        --collect:"XPlat Code Coverage" `
        --results-directory:".\TestResults"
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Tests failed!" -ForegroundColor Red
        exit $LASTEXITCODE
    }
    Write-Host "Tests completed successfully!" -ForegroundColor Green
    Write-Host ""
    
    # Display coverage summary
    Write-Host "Code Coverage Report:" -ForegroundColor Cyan
    Write-Host "Check .\TestResults folder for detailed coverage reports" -ForegroundColor Gray
    Write-Host ""
} else {
    Write-Host "[4/5] Skipping tests..." -ForegroundColor Gray
    Write-Host ""
}

# Step 5: Publish
if ($Package) {
    Write-Host "[5/5] Publishing application..." -ForegroundColor Yellow
    
    # Publish for Windows x64
    dotnet publish ".\$ProjectName\$ProjectName.csproj" `
        --configuration $Configuration `
        --runtime win-x64 `
        --self-contained false `
        --output "$PublishDir\win-x64" `
        /p:PublishSingleFile=true `
        /p:IncludeNativeLibrariesForSelfExtract=true
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Publish failed!" -ForegroundColor Red
        exit $LASTEXITCODE
    }
    
    Write-Host "Publish completed successfully!" -ForegroundColor Green
    Write-Host "Output directory: $PublishDir\win-x64" -ForegroundColor Gray
    Write-Host ""
    
    # Create zip package
    Write-Host "Creating distribution package..." -ForegroundColor Yellow
    $Version = "1.0.0"
    $ZipFile = "$PublishDir\DownSort-$Version-win-x64.zip"
    
    Compress-Archive -Path "$PublishDir\win-x64\*" -DestinationPath $ZipFile -Force
    Write-Host "Package created: $ZipFile" -ForegroundColor Green
    
} else {
    Write-Host "[5/5] Skipping publish..." -ForegroundColor Gray
}

Write-Host ""
Write-Host "====================================" -ForegroundColor Cyan
Write-Host " Build Completed Successfully!" -ForegroundColor Green
Write-Host "====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  - Run the application: dotnet run --project $ProjectName\$ProjectName.csproj" -ForegroundColor Gray
if (-not $SkipTests) {
    Write-Host "  - View test results: .\TestResults" -ForegroundColor Gray
}
if ($Package) {
    Write-Host "  - Distribution package: $ZipFile" -ForegroundColor Gray
}
Write-Host ""
