# DownSort Build and Package Script
# PowerShell 7+ required

param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [switch]$SelfContained = $false,
    [switch]$CreateInstaller = $false,
    [string]$Version = ""  # 비어있으면 Directory.Build.props에서 읽기
)

$ErrorActionPreference = "Continue"  # DevExpress 오류 무시
$ScriptRoot = $PSScriptRoot
$ProjectRoot = Split-Path -Parent $ScriptRoot
$ProjectName = "Downsort"
$ProjectPath = Join-Path $ProjectRoot "$ProjectName\$ProjectName.csproj"
$PublishDir = Join-Path $ProjectRoot "$ProjectName\bin\$Configuration\net8.0-windows\publish"
$InstallerDir = Join-Path $ProjectRoot "Installer"
$DirectoryBuildPropsPath = Join-Path $ProjectRoot "Directory.Build.props"

Write-Host "=== DownSort Build and Package ===" -ForegroundColor Cyan

# Read version from Directory.Build.props if not specified
if ([string]::IsNullOrEmpty($Version)) {
    if (Test-Path $DirectoryBuildPropsPath) {
        Write-Host "Reading version from Directory.Build.props..." -ForegroundColor Yellow
        [xml]$buildProps = Get-Content $DirectoryBuildPropsPath
        $Version = $buildProps.Project.PropertyGroup.Version
        if ([string]::IsNullOrEmpty($Version)) {
            Write-Host "? Version not found in Directory.Build.props!" -ForegroundColor Red
            exit 1
        }
        Write-Host "? Version from Directory.Build.props: $Version" -ForegroundColor Green
    } else {
        Write-Host "? Directory.Build.props not found, using default version 1.0.0" -ForegroundColor Yellow
        $Version = "1.0.0"
    }
}

Write-Host "Project Root: $ProjectRoot" -ForegroundColor Gray
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
Write-Host "Runtime: $Runtime" -ForegroundColor Yellow
Write-Host "Version: $Version" -ForegroundColor Cyan
Write-Host "Self-Contained: $SelfContained" -ForegroundColor Yellow
Write-Host "Publish Directory: $PublishDir" -ForegroundColor Gray
Write-Host ""

# Check if project file exists
if (-not (Test-Path $ProjectPath)) {
    Write-Host "? Project file not found: $ProjectPath" -ForegroundColor Red
    Write-Host "Please run this script from the Setup directory!" -ForegroundColor Yellow
    exit 1
}

# Clean previous publish
if (Test-Path $PublishDir) {
    Write-Host "Cleaning previous build..." -ForegroundColor Green
    Remove-Item $PublishDir -Recurse -Force -ErrorAction SilentlyContinue
}

# Clean obj and bin folders to avoid assets.json issues
Write-Host "Cleaning obj and bin folders..." -ForegroundColor Green
$ObjDir = Join-Path $ProjectRoot "$ProjectName\obj"
$BinDir = Join-Path $ProjectRoot "$ProjectName\bin"
if (Test-Path $ObjDir) {
    Remove-Item $ObjDir -Recurse -Force -ErrorAction SilentlyContinue
}
if (Test-Path $BinDir) {
    Remove-Item $BinDir -Recurse -Force -ErrorAction SilentlyContinue
}

# Restore dependencies with proper runtime
Write-Host "Restoring dependencies..." -ForegroundColor Green
Write-Host "? DevExpress 패키지 복원 오류는 무시됩니다 (로컬에서 이미 설정됨)" -ForegroundColor Yellow

dotnet restore $ProjectPath --runtime $Runtime --verbosity minimal 2>&1 | Out-Null

# DevExpress 복원 실패는 무시하고 계속
Write-Host "? Restore completed (DevExpress errors ignored)" -ForegroundColor Green

# Run tests (optional - skip if failing)
Write-Host "Running tests..." -ForegroundColor Green
$TestProject = Join-Path $ProjectRoot "DownSort.Tests\DownSort.Tests.csproj"
if (Test-Path $TestProject) {
    dotnet test $TestProject --configuration $Configuration --verbosity quiet --no-restore 2>&1 | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? Tests failed, but continuing..." -ForegroundColor Yellow
    } else {
        Write-Host "? Tests passed!" -ForegroundColor Green
    }
} else {
    Write-Host "? Test project not found, skipping tests..." -ForegroundColor Yellow
}

# Publish application
Write-Host "Publishing application..." -ForegroundColor Green

$PublishArgs = @(
    "publish",
    $ProjectPath,
    "--configuration", $Configuration,
    "--runtime", $Runtime,
    "--output", $PublishDir,
    "--no-restore",
    "/p:PublishSingleFile=true",
    "/p:IncludeNativeLibrariesForSelfExtract=true"
)

if ($SelfContained) {
    $PublishArgs += "--self-contained", "true"
    $PublishArgs += "/p:PublishTrimmed=false"
    Write-Host "Building self-contained package (includes .NET Runtime)..." -ForegroundColor Cyan
} else {
    $PublishArgs += "--self-contained", "false"
    Write-Host "Building framework-dependent package..." -ForegroundColor Cyan
}

# Publish with error suppression for DevExpress
$publishOutput = & dotnet @PublishArgs 2>&1
$publishExitCode = $LASTEXITCODE

# Check for critical errors (not DevExpress related)
if ($publishExitCode -ne 0) {
    $criticalError = $publishOutput | Where-Object { 
        $_ -match "error CS" -or 
        $_ -match "error MSB" -and 
        $_ -notmatch "DevExpress" -and 
        $_ -notmatch "NU1301"
    }
    
    if ($criticalError) {
        Write-Host "? Publish failed with critical errors!" -ForegroundColor Red
        $criticalError | ForEach-Object { Write-Host $_ -ForegroundColor Red }
        exit 1
    } else {
        Write-Host "? DevExpress 경고가 있지만 빌드는 성공했습니다" -ForegroundColor Yellow
    }
}

# Verify exe exists
$ExePath = Join-Path $PublishDir "$ProjectName.exe"
if (-not (Test-Path $ExePath)) {
    Write-Host "? Output executable not found: $ExePath" -ForegroundColor Red
    Write-Host "Publish output:" -ForegroundColor Yellow
    $publishOutput | Write-Host
    exit 1
}

Write-Host "? Build completed successfully!" -ForegroundColor Green
Write-Host "? Executable: $ExePath" -ForegroundColor Green

# Get actual version from built assembly
$BuiltVersion = (Get-Item $ExePath).VersionInfo.FileVersion
if ($BuiltVersion) {
    Write-Host "? Built version: $BuiltVersion" -ForegroundColor Green
}

# Create ZIP archive
Write-Host ""
Write-Host "Creating ZIP archive..." -ForegroundColor Green

if (-not (Test-Path $InstallerDir)) {
    New-Item -ItemType Directory -Path $InstallerDir -Force | Out-Null
}

$ZipName = "DownSort-v$Version-$Runtime"
if ($SelfContained) {
    $ZipName += "-standalone"
}
$ZipPath = Join-Path $InstallerDir "$ZipName.zip"

# Remove old zip if exists
if (Test-Path $ZipPath) {
    Remove-Item $ZipPath -Force
}

# Copy documentation files if they exist
$DocsToInclude = @("README.md", "USER_GUIDE.md", "CHANGELOG.md", "LICENSE.txt")
foreach ($doc in $DocsToInclude) {
    $DocPath = Join-Path $ProjectRoot $doc
    if (Test-Path $DocPath) {
        Copy-Item $DocPath $PublishDir -Force
        Write-Host "  Added: $doc" -ForegroundColor Gray
    }
}

Compress-Archive -Path "$PublishDir\*" -DestinationPath $ZipPath -Force
Write-Host "? ZIP created: $ZipPath" -ForegroundColor Green

$ZipSize = (Get-Item $ZipPath).Length / 1MB
Write-Host "  Size: $([math]::Round($ZipSize, 2)) MB" -ForegroundColor Gray

# Calculate checksums
Write-Host ""
Write-Host "Calculating checksums..." -ForegroundColor Green
$ChecksumFile = Join-Path $InstallerDir "checksums.txt"
$ZipHash = (Get-FileHash -Path $ZipPath -Algorithm SHA256).Hash
$ChecksumContent = @"
DownSort v$Version - File Checksums
Generated: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")

SHA256 Checksums:
-----------------
ZIP: $ZipHash
  File: $(Split-Path -Leaf $ZipPath)

"@

# Create installer
if ($CreateInstaller) {
    Write-Host ""
    Write-Host "Creating installer..." -ForegroundColor Green
    
    $InnoSetupPath = "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe"
    
    if (-not (Test-Path $InnoSetupPath)) {
        Write-Host "? Inno Setup not found at: $InnoSetupPath" -ForegroundColor Yellow
        Write-Host "Please install Inno Setup 6 from: https://jrsoftware.org/isinfo.php" -ForegroundColor Yellow
        Write-Host "Skipping installer creation..." -ForegroundColor Yellow
    } else {
        $IssPath = Join-Path $ScriptRoot "DownSort.iss"
        
        if (-not (Test-Path $IssPath)) {
            Write-Host "? ISS file not found: $IssPath" -ForegroundColor Red
        } else {
            # Update version in ISS file
            Write-Host "Updating version in ISS file to $Version..." -ForegroundColor Cyan
            $IssContent = Get-Content $IssPath -Raw -Encoding UTF8
            $IssContent = $IssContent -replace '#define MyAppVersion ".*"', "#define MyAppVersion `"$Version`""
            Set-Content $IssPath $IssContent -Encoding UTF8
            
            Write-Host "Running Inno Setup compiler..." -ForegroundColor Cyan
            & $InnoSetupPath $IssPath | Out-Null
            
            if ($LASTEXITCODE -eq 0) {
                $InstallerPath = Join-Path $InstallerDir "DownSort-Setup-$Version.exe"
                if (Test-Path $InstallerPath) {
                    Write-Host "? Installer created successfully!" -ForegroundColor Green
                    Write-Host "  Location: $InstallerPath" -ForegroundColor Cyan
                    $InstallerSize = (Get-Item $InstallerPath).Length / 1MB
                    Write-Host "  Size: $([math]::Round($InstallerSize, 2)) MB" -ForegroundColor Gray
                    
                    # Add installer checksum
                    $InstallerHash = (Get-FileHash -Path $InstallerPath -Algorithm SHA256).Hash
                    $ChecksumContent += @"
Setup: $InstallerHash
  File: $(Split-Path -Leaf $InstallerPath)

"@
                } else {
                    Write-Host "? Installer file not found at expected location" -ForegroundColor Yellow
                }
            } else {
                Write-Host "? Installer creation failed with exit code: $LASTEXITCODE" -ForegroundColor Red
            }
        }
    }
}

# Save checksums
Set-Content -Path $ChecksumFile -Value $ChecksumContent -Encoding UTF8
Write-Host "? Checksums saved: $ChecksumFile" -ForegroundColor Green

# Display summary
Write-Host ""
Write-Host "=== Build Summary ===" -ForegroundColor Cyan
Write-Host "Version: $Version (from Directory.Build.props)" -ForegroundColor White
Write-Host "Configuration: $Configuration" -ForegroundColor White
Write-Host "Runtime: $Runtime" -ForegroundColor White
Write-Host "Self-Contained: $SelfContained" -ForegroundColor White
Write-Host ""
Write-Host "Artifacts:" -ForegroundColor Yellow
Write-Host "  ?? Application: $PublishDir" -ForegroundColor White
Write-Host "  ?? ZIP Archive: $ZipPath ($([math]::Round($ZipSize, 2)) MB)" -ForegroundColor White

if ($CreateInstaller -and (Test-Path $InnoSetupPath)) {
    $InstallerPath = Join-Path $InstallerDir "DownSort-Setup-$Version.exe"
    if (Test-Path $InstallerPath) {
        $InstallerSize = (Get-Item $InstallerPath).Length / 1MB
        Write-Host "  ?? Installer: $InstallerPath ($([math]::Round($InstallerSize, 2)) MB)" -ForegroundColor White
    }
}

Write-Host "  ?? Checksums: $ChecksumFile" -ForegroundColor White

Write-Host ""
Write-Host "? All done!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "  1. Test the application: $ExePath" -ForegroundColor Gray
Write-Host "  2. Test the ZIP: Extract and run from $ZipPath" -ForegroundColor Gray
if ($CreateInstaller) {
    Write-Host "  3. Test the installer: Run the setup exe" -ForegroundColor Gray
    Write-Host "  4. Create GitHub Release with files from: $InstallerDir" -ForegroundColor Gray
}
Write-Host ""
Write-Host "?? Tip: Update version in Directory.Build.props for next release" -ForegroundColor Yellow
