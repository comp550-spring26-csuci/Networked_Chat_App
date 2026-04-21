#----------------------------------------------
# Description: Script to setup the environment
# Developer: Ivana Bavin-Gomez-San Basilio
# Date: March 7th 2026
#----------------------------------------------

# Importing the script with path variables
. "$PSScriptRoot\init-paths.ps1"

Write-Host "Starting environment Setup" -ForegroundColor Cyan

# 1. Docker
Write-Host "Spinning up Docker containers" 
docker-compose -f $Global:DockerPath up -d

# 2. Build
Write-Host "Building the Backend" 
dotnet build $Global:CsprojPath

Write-Host "Environment is synchronized" -ForegroundColor Green

