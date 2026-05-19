#--------------------------------------------------------------------
# Description: Script to update the database - automation
# Developer: Ivana Bavin-Gomez-San Basilio
#--------------------------------------------------------------------

# 1. Import the paths
. "$PSScriptRoot\init-paths.ps1"

Write-Host "`n====== Starting Database Migration Process ======" -ForegroundColor Cyan

# --- DEBUG CHECK ---
# Let's see what the computer thinks the path is
Write-Host "DEBUG: Looking for project at: $Global:CsprojPath" -ForegroundColor Magenta

if (-Not (Test-Path "$Global:CsprojPath")) {
    Write-Host "ERROR: The file '$Global:CsprojPath' does not exist!" -ForegroundColor Red
    Write-Host "Please check if your .csproj file is named exactly 'Backend.API.csproj'" -ForegroundColor Yellow
    Pause
    exit
}

# 2. Build the project (Using QUOTES to handle long paths)
Write-Host "[1/3] Building project..." -ForegroundColor Yellow
dotnet build "$Global:CsprojPath"

if ($LASTEXITCODE -ne 0)
{
    Write-Host "ERROR: Build failed. Check the error messages above." -ForegroundColor Red
    Pause
    exit
}

# 3. Migration Name
$migrationName = Read-Host "Enter a name for this migration"

if ([string]::IsNullOrWhiteSpace($migrationName)) {
    Write-Host "ERROR: Name cannot be empty!" -ForegroundColor Red
    Pause
    exit
}

# 4. Create Migration (Using QUOTES)
Write-Host "[2/3] Creating Migration..." -ForegroundColor Yellow
dotnet ef migrations add $migrationName --project "$Global:CsprojPath"

# 5. Update Database (Using QUOTES)
Write-Host "[3/3] Updating PostgreSQL..." -ForegroundColor Yellow
dotnet ef database update --project "$Global:CsprojPath"

# 6. Post-Flight Check: Verify the migration status
# This confirms if the migration actually landed in the DB
Write-Host "`n[4/4] Verifying Migration History..." -ForegroundColor Yellow
dotnet ef migrations list --project "$Global:CsprojPath"

Write-Host "`n--------------------------------------------------------" -ForegroundColor Green
Write-Host "SUCCESS: The 13th Dimension is in sync!" -ForegroundColor Green
Write-Host "NOTE: Ensure the migration says '(Applied)' above." -ForegroundColor Cyan
Write-Host "--------------------------------------------------------" -ForegroundColor Green

Pause