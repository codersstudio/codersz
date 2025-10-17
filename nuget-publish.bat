@echo off
if not defined NUGET_API_KEY (
    echo "Error: NUGET_API_KEY environment variable is not set."
    exit /b 1
)

set VERSION=3.0.1
set SOURCE=https://api.nuget.org/v3/index.json

set PROJ=coders\coders.csproj
set PKG_NAME=coders

set OUTPUT_DIR=nupkg

REM Clean up previous packages
if exist %OUTPUT_DIR% (
    rmdir /s /q %OUTPUT_DIR%
)
mkdir %OUTPUT_DIR%

REM Pack the project
dotnet pack %PROJ% -c Release -o %OUTPUT_DIR%

REM Push the package
dotnet nuget push "%OUTPUT_DIR%\%PKG_NAME%.%VERSION%.nupkg" --api-key %NUGET_API_KEY%

echo "Publishing complete."