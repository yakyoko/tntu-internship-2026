# US-001: Create a New Project

## Story

As a **team member**, I want to **create a new project with a name and optional description**, so that **I can organize related tasks under a single project**.

## Acceptance Criteria

- [ ] Given a valid project name, when I submit a create request, then a new project is persisted with a generated GUID, `isArchived = false`, and `createdAt` set to current UTC time.
- [ ] Given a request without a name (or empty/whitespace name), when I submit a create request, then the API returns `400 Bad Request` with a Problem Details body.
- [ ] Given a name longer than 100 characters, when I submit a create request, then the API returns `400 Bad Request`.
- [ ] Given a description longer than 500 characters, when I submit a create request, then the API returns `400 Bad Request`.
- [ ] Given a successful create, when the response is returned, then it includes a `Location` header pointing to the new project resource.

## API Contract

| | |
|---|---|
| **Method** | `POST` |
| **Path** | `/api/v1/projects` |
| **Request body** | `{ "name": "string", "description": "string (optional)" }` |
| **Success response** | `201 Created` — full `Project` object |
| **Error responses** | `400 Bad Request` — validation failure |

**Response example:**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Internship Backend",
  "description": "Build the task board APIs",
  "isArchived": false,
  "createdAt": "2026-07-01T10:00:00Z"
}
```

## Technical Notes

- **Service:** Projects.Api
- **Entity:** `Project` in Cosmos container `projects`, partition key `/id`
- **EF Core:** `DbSet<Project>`, `SaveChangesAsync` generates `id` and `createdAt` server-side
- **Validation:** Use Data Annotations or FluentValidation

## Definition of Done

- [ ] Endpoint implemented and returns correct status codes
- [ ] Unit tests for validation rules (empty name, max length)
- [ ] Integration test or manual verification against Cosmos DB
- [ ] Swagger/OpenAPI documents the endpoint
- [ ] No secrets in committed code

## References

- Domain rule [BR-P01](../domain/system-overview.md#business-rules), [BR-P02](../domain/system-overview.md#business-rules), [BR-P03](../domain/system-overview.md#business-rules)
- [Create web APIs with ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/web-api/)
- [EF Core Cosmos DB provider](https://learn.microsoft.com/en-us/ef/core/providers/cosmos/)
