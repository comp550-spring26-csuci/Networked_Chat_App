#----------------------------------------------
# Description: Script for database reset
# Developer: Ivana Bavin-Gomez-San Basilio
# Date: May 1st 2026
#----------------------------------------------

Write-Host "!!! WARNING THIS WILL DELETE ALL DATA IN YOUR DATABASE !!!" -ForegroundColor Red
$confirmation = Read-Host "Are you absolutely sure? (y/n)"

if ($confirmation -ne 'y')
{
	Write-Host "Reset cancelled." -ForegroundColor Cyan
	exit
}

Write-Host "[1/2] Dropping existing database..." -ForegroundColor Yellow
dotnet ef database drop --force --project ../Backend.API/Backend.API.csproj

Write-Host "[2/2]Re-applying all migrations from scratch......" -ForegroundColor Yellow
dotnet ef database update --project ../Backend.API/Backend.API.csproj

Write-Host "`nDatabase has been reset to a clean state." -ForegroundColor Green
Pause