# stackly-backend

Small ASP.NET Core Web API project.

## Quick links
- App entry: [Program.cs](Program.cs) — [`Program`](Program.cs)  
- DB context: [DbContext.cs](DbContext.cs) — [`AppDbContext`](DbContext.cs)  

## Prerequisites
- .NET 8 SDK installed

## Install & run
1. Restore, build and create database files (by default in build directory):
```bash
dotnet restore
dotnet build
dotnet ef migrations add InitialCreate
dotnet ef database update
```

2. Run the app from the project root:
```bash
dotnet run
```

3. Access Swagger UI to see and interact with API
- /swagger/index.html
