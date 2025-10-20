# stackly-backend

Small ASP.NET Core Web API project.

## Quick links
- App entry: [Program.cs](Program.cs) — [`Program`](Program.cs)  
- DB context: [DbContext.cs](DbContext.cs) — [`AppDbContext`](DbContext.cs)  
- Controller: [Controllers/ExampleController.cs](Controllers/ExampleController.cs) — [`ExampleController`](Controllers/ExampleController.cs)  
- Models: [Models/ExampleModel.cs](Models/ExampleModel.cs) — [`StacklyBackend.Models.Example`](Models/ExampleModel.cs)  
- ID generator: [Utils/DataGenerator.cs](Utils/DataGenerator.cs) — [`StacklyBackend.Utils.DataGenerator.Generator.GetRandomString`](Utils/DataGenerator.cs)  
- Example HTTP requests: [requests.http](requests.http), [stackly-backend.http](stackly-backend.http)  
- Project file: [stackly-backend.csproj](stackly-backend.csproj)  
- Launch settings: [Properties/launchSettings.json](Properties/launchSettings.json)  
- App settings: [appsettings.json](appsettings.json)

## Prerequisites
- .NET 8 SDK installed

## Install & run
1. Restore and build:
```bash
dotnet restore
dotnet build
```

2. Run the app from the project root:
```bash
dotnet run
```

By default the app will listen on the URLs defined in [Properties/launchSettings.json](Properties/launchSettings.json) (e.g. `http://localhost:5189`). Swagger UI is available at `http://localhost:5189/swagger` when running in Development (see [Program.cs](Program.cs)).

The SQLite DB file is created at the application base directory as configured in [DbContext.cs](DbContext.cs): `AppContext.BaseDirectory/database.db`.

## Implemented endpoints
Controller: [`ExampleController`](Controllers/ExampleController.cs)

Base path: `/api/example`

- GET /api/example  
  - Returns all examples. Response: 200 OK with list of [`StacklyBackend.Models.Example`](Models/ExampleModel.cs).

- GET /api/example/{id}  
  - Returns single example by id. Response: 200 OK with `Example` or 404 Not Found.

- POST /api/example  
  - Create new example. Body: JSON matching `ExampleCreate` in [Models/ExampleModel.cs](Models/ExampleModel.cs):
    ```json
    {
      "name": "string",
      "price": 0.0
    }
    ```
  - Server generates a 10-char id using [`Generator.GetRandomString`](Utils/DataGenerator.cs). Responses: 201 Created with created resource.

- PUT /api/example/{id}  
  - Update existing example. Body: full `Example` JSON. Responses: 204 No Content or 404 Not Found.

- DELETE /api/example/{id}  
  - Delete example. Responses: 204 No Content or 404 Not Found.

See implementation in [Controllers/ExampleController.cs](Controllers/ExampleController.cs).

## Example requests
Use the provided [requests.http](requests.http) or [stackly-backend.http](stackly-backend.http) files in the repo. Example curl for POST:
```bash
curl -X POST "http://localhost:5189/api/example" -H "Content-Type: application/json" -d '{"name":"Cherry","price":0.0}'
```

## Notes
- DB provider: SQLite configured in [DbContext.cs](DbContext.cs).
- ID generation uses [`StacklyBackend.Utils.DataGenerator.Generator.GetRandomString`](Utils/DataGenerator.cs) to avoid collisions with existing records.
