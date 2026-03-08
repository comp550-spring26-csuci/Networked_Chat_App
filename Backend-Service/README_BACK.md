# backend.API Requirements: Netowrk Chat App

## Getting Started
1. Ensure you have **.NET 19 SDK** installed
2. Run `dotnet restore` to installn all requirements
3. Run `dotnet run` to start the API
4. Install Docker Desktop for windows AMD
1. To download the PostgresSQL go to the folder where the docker-compose.yml file is and run

```
docker compose up -d
```

## Requirements & Stack 
* **Framework:** .NET 10.0
* **Real-time:** SignalR Hubs
* **Database:** PostgreSQL (Core Data) & MongoDB (Chat History)
* **Messaging:** RabbitMQ (Event Bus)
* **Docker Desktop:** WSL 2 needs to be enabled for Windows users
* **Entity Framework Core Tools:**
	```
	  dotnet tool install --global dotnet-ef
	```

## Architecture
* **API:** Controllers and Hubs
* **Application:** Business logic and Services
* **Core:** Domain Entities and Interfaces

## Database Set Up
* **Docker:**
1.  Navigate to the root folder where ```docker-compose.yml``` is located
2. Spin up the PostgresSQL container:
	```
	docker-compose up -d
	```
3. The database will be available at ```localhost:5432``` with 
* **Development Secrets**:the connection string and local credentials are stored in `appsettings.Development.json`
* **Note**: this file is tipically excluded from source control in professional settings to protect credentials

## Building the Project
if you are setting this up for the first time or after moving files:

1. Clean and Restore:
	```
	dotnet clean
	```

2. Build:
	```
	dotnet build
	```


## Database Migration
To sync the database schema with the C# models:

1. Apply existing migratione:
	```
	dotnet ef database update
	```

2. Create a new migration (if there is a change in the model):
	```
	dotnet ef migrations add <MigrationName>
	```



## Automation & Scripts
This project has a modular PowerShell script folder with files that can be used for setting up 
the development environment.

Location: ```Backend.API/scripts/```

### Script Architecture
* ```init-paths.ps1```: It calculates the needed paths needed by the other scripts
* ```setup-env.ps1```: it imports the paths and spins up the Docker containers and builds the .NET project


### How to Run
To synchronize the environment from the project root (```Networked_Chat_App```)
1. Open PowerShell as Administrator
2. Set the Execution Policy (at least for the first time)
```
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

3. Run the Setup Script
```
.\Backend-Service\backend.API\scripts\setup-env.ps1
```