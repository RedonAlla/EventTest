# EventTest

A Web API solution built on Clean Architecture principles, with a strict separation between domain logic and infrastructure details such as databases or external services.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) or later

## Getting Started

Restore, build, and run the API from the solution root:

```bash
dotnet restore
dotnet build
dotnet run --project src/Presentation/EventTest.Api/EventTest.Api.csproj
```

The API listens on `http://localhost:5039` with the default profile. To use the HTTPS profile instead:

```bash
dotnet run --project src/Presentation/EventTest.Api/EventTest.Api.csproj --launch-profile https
```

Useful endpoints:

- Scalar API documentation: `http://localhost:5039/openapi-ui` (mapped outside the Production environment)
- Health checks: `http://localhost:5039/health` (mapped when a database provider is configured)

## Configuration

When database providers are configured, set one connection string per provider in `src/Presentation/EventTest.Api/appsettings.json`:

```json
"ConnectionStrings": {
  "EventTestSqlServerConnectionString": "your-connection-string"
}
```

Each configured provider has its own `DbContext`, connection string (`EventTestSqlServerConnectionString`, `EventTestOracleConnectionString`, `EventTestPostgresConnectionString`, `EventTestSqliteConnectionString`), health check, and dependency injection registration. Only the entries for the providers selected at generation time are present.

In the Development environment the application creates and initializes the databases on startup. For local development you can keep secrets out of source control by moving the connection strings to `appsettings.Development.json`, user secrets, or environment variables (e.g. `ConnectionStrings__EventTestSqlServerConnectionString`).

## Running Tests

```bash
dotnet test
```

The `EventTest.ArchitectureTests` project uses NetArchTest to enforce layer boundaries, entity conventions, and feature naming rules (inputs, handlers, validators, and decorators).

## Solution Structure

```
EventTest.sln
src/
  Core/
    EventTest.Domain/            # Entities, value objects, domain constants — no project dependencies
    EventTest.Application/       # Feature handlers, validators, DTOs, data abstractions — depends on Domain
  Infrastructure/
    EventTest.Infrastructure/    # Wires up the other infrastructure projects
    EventTest.Persistence/       # EF Core DbContext, entity configurations, initializer
    EventTest.Integration/       # HTTP client integrations with request/response logging
  Presentation/
    EventTest.Api/               # ASP.NET Core host, middleware and endpoint registration
    EventTest.Api.Contracts/     # Shared API contracts — no project dependencies
tests/
  EventTest.ArchitectureTests/   # NetArchTest-based architecture enforcement
```

Depending on the options selected when the project was generated, some of these projects may not be present: the Persistence project is omitted when no database provider is used, and the Integration project is omitted when HTTP integrations are disabled. JWT authorization wiring is likewise included only when authorization was enabled.

## Architecture

Layers, from inner to outer: **Domain → Application → Infrastructure → Presentation**. Dependencies only point inward.

- **Domain** holds business entities and domain constants and depends on nothing else.
- **Application** orchestrates the domain logic with CQRS-style features (requests, handlers, validators, pipeline decorators) and defines per-provider data abstractions such as `IEventTestSqlServerDbContext`.
- **Infrastructure** implements the external concerns: EF Core persistence and HTTP client integrations, each registered by its own `DependencyInjection` class.
- **Presentation** contains the ASP.NET Core host. `StartupExtensions.AddServices` composes the layers, and `UsePipelines` configures the request pipeline.

Each layer exposes a `DependencyInjection` class with `IServiceCollection` extension methods, so the composition root stays free of infrastructure details.
