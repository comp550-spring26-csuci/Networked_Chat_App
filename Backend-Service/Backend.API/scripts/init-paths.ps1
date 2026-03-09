#----------------------------------------------
# Description: Script to locate root folder 
# Developer: Ivana Bavin-Gomez-San Basilio
# Date: March 7th 2026
#----------------------------------------------

# Getting the directory of where this script is located
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition

# Go up one level to arrive to the Backend.API (where the .csproj for the back-end lives)
$Global:BackendPath = Split-Path -Parent $ScriptDir

# Go up one level to arrive to the Backend-Service folder
$BackendServiceDir = Split-Path -Parent $Global:BackendPath

# The project root is one level up. Here is where docker-compose.yml is.
$Global:ProjectRoot = [System.IO.Path]::GetFullPath((Join-Path $BackendServiceDir ".."))

# Defining common paths usable by other Scripts
$Global:CsprojPath = Join-Path $Global:BackendPath "Backend.API.csproj"
$Global:DockerPath = Join-Path $Global:ProjectRoot "docker-compose.yml"

Write-Host "Path Sync:" -ForgroundColor Gray
Write-Host "Project root: $Global:ProjectRoot" -ForegroundColor Gray
Write-Host "Project: $Global:ProjectRoot" -ForegroundColor Gray