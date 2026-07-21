# Copilot Instructions for TNTU Internship 2026 Task Board

This document provides guidance for GitHub Copilot and other developers on the structure, conventions, and practices used in this repository.

---

## Project Overview

**TNTU Internship 2026** is a 1-month educational microservices project building a minimal task board using **ASP.NET Core**. The system consists of two independent APIs that communicate via HTTP and store data in **Azure Cosmos DB**.

- **Duration**: 4-week internship project
- **Architecture**: Microservices (two independently deployable services)
- **Target**: Azure free tier deployment with CI/CD automation

---

## Repository Structure

```
tntu-internship-2026/
├── .github/
│   ├── workflows/              # GitHub Actions CI/CD pipelines
│   └── copilot-instructions.md # This file
├── docs/
│   ├── architecture/           # System design and ADRs
│   ├── prerequisites/          # Development setup
│   ├── domain/                 # Business domain and models
│   ├── internship-plan/        # Week-by-week schedule
│   └── user-stories/           # Feature specifications
├── src/
│   ├── Projects.Api/           # Projects microservice
│   ├── Projects.Api.Tests/     # Unit and integration tests for Projects.Api
│   ├── Tasks.Api/              # Tasks microservice
│   ├── Tasks.Api.Tests/        # Unit and integration tests for Tasks.Api
│   └── TntuTaskboard.sln       # Solution file
└── README.md
```

### Service Layout

Each service follows this internal structure:

```
<Service>.Api/
├── Program.cs                  # ASP.NET Core app setup
├── appsettings.json           # Configuration
├── appsettings.Development.json
├── Controllers/               # API endpoints (e.g., ProjectsController.cs)
├── Models/                    # Entity models and DTOs
├── Data/                      # DbContext, migrations
├── Services/                  # Business logic
└── <Service>.Api.csproj       # Project file
```

---

## Tech Stack

| Layer | Technology | Version | Notes |
|-------|-----------|---------|-------|
| **Runtime** | .NET | 8.0 | LTS; verify SDK installed |
| **Framework** | ASP.NET Core Web API | 8.0.* | Uses Minimal APIs in some projects |
| **ORM** | Entity Framework Core | 8.0.* | With Cosmos DB provider |
| **Database** | Azure Cosmos DB | Free tier | No authentication in v1; open APIs |
| **Mapping** | AutoMapper | 16.2.0 | Maps DTOs ↔ entity models |
| **Testing** | xUnit | 2.4.2 | Unit and integration tests |
| **Mocking** | Moq | 4.20.72 | Mock interfaces and DbContext |
| **Test Utilities** | WebApplicationFactory | 8.0.28 | In-process HTTP testing |
| **Deployment** | GitHub Actions | — | CI/CD; build, test, deploy to Azure |
| **Hosting** | Azure App Service | F1 (free) | One slot per service |
| **Observability** | Application Insights | — | Optional; added in Week 3 |

---

## Coding Conventions

### C# Naming and Formatting

- **Namespaces**: `<Company>.<Project>` (e.g., `Projects.Api.Models`, `Tasks.Api.Services`)
- **Classes and Methods**: PascalCase (e.g., `ProjectsController`, `CreateProjectAsync`)
- **Local variables and parameters**: camelCase (e.g., `projectId`, `validationErrors`)
- **Constants**: PascalCase (e.g., `MaxProjectNameLength`)
- **Private fields**: camelCase with underscore prefix (e.g., `_logger`, `_dbContext`)

### Nullable Reference Types

Nullable reference types are **enabled** in all projects (`<Nullable>enable</Nullable>` in .csproj):

- Use `string?` for nullable strings
- Use `null!` only when you are certain a value is not null
- Use null-coalescing operators: `value ?? fallback`
- All parameters without `?` are non-nullable

Example:

```csharp
public async Task<Project?> GetProjectAsync(Guid projectId)
{
    var project = await _dbContext.Projects.FindAsync(projectId);
    return project;  // Can return null; caller knows this from the ? return type
}
```

### Global Using Statements

Common namespaces are declared globally in `GlobalUsings.cs` in each project:

```csharp
global using System;
global using System.Collections.Generic;
global using System.Threading.Tasks;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
```

- Add to `GlobalUsings.cs` for namespaces used across multiple files
- Reduces repetitive `using` statements in individual files

### DTOs vs. Entity Models

- **Entity models** (`Project.cs`, `Task.cs`): Map to database schema via EF Core
- **DTOs** (`CreateProjectDto.cs`, `ProjectDto.cs`): Contract for API requests/responses
- **Validation**: Applied to DTOs using `[Required]`, `[StringLength]`, `[Range]` attributes
- **Mapping**: AutoMapper profiles in `MappingProfile.cs` (e.g., `CreateProjectDto` → `Project`)

Example:

```csharp
// Entity model (stored in database)
public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}

// Request DTO (validates input)
public class CreateProjectDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = null!;
}

// Response DTO (shaped for API clients)
public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
```

### Code Organization

- **One class per file** (except nested types if architecturally justified)
- **File name matches class name** (e.g., `ProjectsController.cs` contains `ProjectsController`)
- **Namespace aligns with folder structure** (e.g., `Projects.Api.Models.Project` in `src/Projects.Api/Models/Project.cs`)

---

## Test Framework: xUnit with Moq

### Test Project Structure

```
<Service>.Api.Tests/
├── GlobalUsings.cs
├── <Feature>/
│   ├── <FeatureTests>.cs      # Unit tests (arrange-act-assert)
│   ├── <Feature>IntegrationTests.cs  # Integration tests (with WebApplicationFactory)
└── Models/
    └── <ModelValidationTests>.cs
```

### Test Naming and Organization

- **Test method names**: `MethodName_Condition_ExpectedResult` (e.g., `GetProject_WithValidId_ReturnsOkWithProject`)
- **Test class names**: `<ClassUnderTest>Tests` or `<ClassUnderTest>IntegrationTests`
- **Arrange-Act-Assert pattern**:

```csharp
[Fact]
public async Task CreateProject_WithValidRequest_ReturnsCreatedAtAction()
{
    // Arrange
    var request = new CreateProjectDto { Name = "Test Project" };
    var client = _factory.CreateClient();

    // Act
    var response = await client.PostAsJsonAsync("/api/projects", request);

    // Assert
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
}
```

### Using Moq

Mock interfaces and DbContext for unit tests:

```csharp
[Fact]
public async Task GetProject_WhenProjectNotFound_ReturnsNull()
{
    // Arrange
    var mockDbContext = new Mock<TaskBoardDbContext>();
    mockDbContext.Setup(db => db.Projects.FindAsync(It.IsAny<Guid>()))
        .ReturnsAsync((Project?)null);
    var service = new ProjectService(mockDbContext.Object);

    // Act
    var result = await service.GetProjectAsync(Guid.NewGuid());

    // Assert
    Assert.Null(result);
}
```

### WebApplicationFactory for Integration Tests

Use `WebApplicationFactory<T>` to test endpoints in-process:

```csharp
public class ProjectsControllerIntegrationTests : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        _factory.Dispose();
        _client.Dispose();
    }

    [Fact]
    public async Task GetProjects_ReturnsOkWithProjects()
    {
        var response = await _client.GetAsync("/api/projects");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```

---

## Building and Running

### Prerequisites

- **.NET 8 SDK** or later ([download](https://dotnet.microsoft.com/en-us/download))
- **Git** for version control
- **Visual Studio 2022**, **Rider**, or **VS Code** (optional but recommended)

Verify installation:

```bash
dotnet --version
```

### Build Commands

```bash
# Restore dependencies and build the entire solution
dotnet build

# Build a specific service
dotnet build src/Projects.Api/Projects.Api.csproj

# Build and include only errors (no warnings)
dotnet build --verbosity minimal
```

### Running Services Locally

Each service runs on a different port:

```bash
# Terminal 1: Start Projects.Api
cd src/Projects.Api
dotnet run
# Runs on http://localhost:5201 (HTTPS) or http://localhost:5200 (HTTP)

# Terminal 2: Start Tasks.Api
cd src/Tasks.Api
dotnet run
# Runs on http://localhost:5202 (HTTPS) or http://localhost:5203 (HTTP)
```

Both services use in-memory Cosmos DB when running locally (configured in `appsettings.Development.json`).

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests for a specific project
dotnet test src/Projects.Api.Tests/Projects.Api.Tests.csproj

# Run with verbose output
dotnet test --verbosity normal

# Run a specific test class
dotnet test --filter "ClassName=Projects.Api.Tests.ProjectsControllerIntegrationTests"

# Generate code coverage report (if coverlet is configured)
dotnet test /p:CollectCoverage=true
```

### Configuration

Each service uses `appsettings.json` and `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "CosmosDb": {
    "Account": "https://<account>.documents.azure.com:443/",
    "Key": "<access-key>"
  }
}
```

For local development, use in-memory database (configured automatically in Development environment).

---

## Architecture Principles

### Service Independence

- Each service **owns its data**; no direct database access across services
- Services communicate only via **HTTP** (no message bus or event sourcing in v1)
- Each service has its own Cosmos DB container(s)

Example: Tasks.Api validates a project exists by calling Projects.Api:

```csharp
public async Task<bool> ValidateProjectExistsAsync(Guid projectId)
{
    var response = await _httpClient.GetAsync($"{_projectsApiBaseUrl}/api/projects/{projectId}");
    return response.IsSuccessStatusCode;
}
```

### API Design

- **RESTful endpoints**: Follow HTTP conventions (GET, POST, PUT, DELETE)
- **Resource-based routes**: `/api/projects`, `/api/projects/{id}/tasks`
- **Status codes**: 200 OK, 201 Created, 204 No Content, 400 Bad Request, 404 Not Found, 500 Internal Server Error
- **DTOs for contracts**: Always return DTOs; never expose entity models to clients

### Data Validation

Validation is applied at the **DTO level** using data annotations:

```csharp
public class CreateProjectDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be 3-100 characters")]
    public string Name { get; set; } = null!;
}
```

---

## Development Workflow

### Branching Strategy

- **main**: Stable, production-ready code
- **develop**: Integration branch (optional; can push to main for this project)
- **feature/<us-###>**: Feature branches per user story (e.g., `feature/us-001-create-project`)

```bash
# Create and switch to feature branch
git checkout -b feature/us-001-create-project

# Make changes, commit, push
git add .
git commit -m "Implement US-001: Create Project endpoint"
git push origin feature/us-001-create-project

# Create pull request on GitHub
```

### Common Workflow

1. **Select a user story** from `/docs/user-stories/`
2. **Create a feature branch** with the user story ID
3. **Implement the feature** in the appropriate service
4. **Write unit and integration tests** (required for acceptance)
5. **Test locally** with `dotnet run` and `dotnet test`
6. **Commit and push**; GitHub Actions will run CI/CD
7. **Create a pull request** for code review

---

## Key Files and Their Purpose

| File | Purpose |
|------|---------|
| `Program.cs` | Configures ASP.NET Core services (DI, routing, Cosmos DB, CORS) |
| `Controllers/<Name>Controller.cs` | HTTP endpoint definitions; typically uses dependency injection for services |
| `Models/<Entity>.cs` | EF Core entity models; represents database schema |
| `Models/<Dto>.cs` | Data transfer objects for API contracts (requests/responses) |
| `Services/<Service>.cs` | Business logic; injected into controllers |
| `Data/<Service>DbContext.cs` | EF Core DbContext; manages entity relationships and migrations |
| `MappingProfile.cs` | AutoMapper profiles for DTO ↔ entity conversion |
| `appsettings.json` | Configuration (secrets, database, logging) |
| `<Project>.csproj` | Project metadata, NuGet dependencies, build settings |

---

## Cross-Service HTTP Communication

When Tasks.Api needs to call Projects.Api:

1. **Inject HttpClient** in the service constructor:

```csharp
public class TaskService
{
    private readonly HttpClient _httpClient;
    private readonly string _projectsApiUrl;

    public TaskService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _projectsApiUrl = config["ProjectsApi:BaseUrl"]; // From appsettings.json
    }

    public async Task<bool> ValidateProjectExistsAsync(Guid projectId)
    {
        var response = await _httpClient.GetAsync($"{_projectsApiUrl}/api/projects/{projectId}");
        return response.IsSuccessStatusCode;
    }
}
```

2. **Register in Program.cs**:

```csharp
builder.Services.AddHttpClient<TaskService>();
```

3. **Handle failures gracefully**:

```csharp
try
{
    var projectExists = await _taskService.ValidateProjectExistsAsync(projectId);
    if (!projectExists)
        return NotFound("Project not found");
}
catch (HttpRequestException)
{
    return StatusCode(503, "Projects service is unavailable");
}
```

---

## Testing Best Practices

1. **Unit tests**: Test services in isolation with mocked dependencies
2. **Integration tests**: Test full endpoint flow using WebApplicationFactory
3. **Test data**: Use realistic but minimal data; keep tests fast
4. **Assertions**: Be specific; assert both success and failure cases
5. **Naming**: Make test intent obvious from method name
6. **Isolation**: Each test should be independent and repeatable

---

## Deployment and CI/CD

GitHub Actions automatically:

1. **Build**: Restores packages and compiles the solution
2. **Test**: Runs all unit and integration tests
3. **Deploy**: Publishes successful builds to Azure App Service

See `.github/workflows/` for pipeline definitions.

---

## Documentation and Resources

- **Architecture Overview**: [docs/architecture/architecture-and-tech-stack.md](../docs/architecture/architecture-and-tech-stack.md)
- **Prerequisites Checklist**: [docs/prerequisites/development-prerequisites.md](../docs/prerequisites/development-prerequisites.md)
- **System Domain Model**: [docs/domain/system-overview.md](../docs/domain/system-overview.md)
- **User Stories**: [docs/user-stories/README.md](../docs/user-stories/README.md)
- **Internship Schedule**: [docs/internship-plan/one-month-schedule.md](../docs/internship-plan/one-month-schedule.md)

---

## Common Issues and Troubleshooting

### Issue: `dotnet build` fails with "Project not found"

**Solution**: Ensure you are running from the repository root or specifying the correct path to `.sln`:

```bash
cd tntu-internship-2026
dotnet build
```

### Issue: Tests fail with "DbContext is null"

**Solution**: Ensure test projects properly initialize `WebApplicationFactory` or mock the DbContext in arrange phase.

### Issue: `NullReferenceException` in cross-service calls

**Solution**: Check that HttpClient is injected and that the remote service base URL is configured in `appsettings.json`.

### Issue: Cosmos DB connection errors locally

**Solution**: Verify `appsettings.Development.json` is configured to use in-memory database for local development.

---

## Quick Reference

| Task | Command |
|------|---------|
| Build solution | `dotnet build` |
| Run all tests | `dotnet test` |
| Start Projects.Api | `cd src/Projects.Api && dotnet run` |
| Start Tasks.Api | `cd src/Tasks.Api && dotnet run` |
| Restore packages | `dotnet restore` |
| Format code | `dotnet format` (if configured) |
| Clean build artifacts | `dotnet clean` |

---

## Contributing

1. Read the relevant **user story** in `docs/user-stories/`
2. Create a **feature branch**: `git checkout -b feature/us-###-description`
3. **Implement** the feature with tests
4. **Commit** with clear messages
5. **Push** and create a pull request
6. **GitHub Actions** will validate your changes

Thank you for contributing to this project!
