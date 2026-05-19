docker-compose down -v
docker-compose up -d --remove-orphans

cd Backend-Service/Backend.API
dotnet ef database update
