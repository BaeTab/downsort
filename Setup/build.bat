@echo off
REM DownSort Quick Build Script
REM This script builds the application and creates installer

echo ========================================
echo DownSort Quick Build
echo ========================================
echo.

cd /d "%~dp0"

REM Check if running from Setup directory
if not exist "build-installer.ps1" (
    echo Error: Please run this script from the Setup directory!
    pause
    exit /b 1
)

REM Option 1: Simple build without installer
echo [1] Build only (no installer)
echo [2] Build + Create installer
echo [3] Build self-contained + installer (includes .NET Runtime)
echo.
set /p choice="Select option (1-3): "

if "%choice%"=="1" (
    echo.
    echo Building application...
    powershell -ExecutionPolicy Bypass -File ".\build-installer.ps1"
) else if "%choice%"=="2" (
    echo.
    echo Building application and creating installer...
    powershell -ExecutionPolicy Bypass -File ".\build-installer.ps1" -CreateInstaller
) else if "%choice%"=="3" (
    echo.
    echo Building self-contained application and creating installer...
    powershell -ExecutionPolicy Bypass -File ".\build-installer.ps1" -SelfContained -CreateInstaller
) else (
    echo Invalid choice!
    pause
    exit /b 1
)

echo.
echo ========================================
echo Build completed!
echo ========================================
echo.
echo Check the Installer folder for output files.
echo.
pause
