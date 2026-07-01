# US-006: Create a Task in a Project

## Story

As a **team member**, I want to **create a task within an existing project**, so that **I can track individual units of work**.

## Acceptance Criteria

- [ ] Given an existing non-archived project, when I submit a valid create task request, then the API returns `201 Created` with a task in `ToDo` status, generated `id`, `createdAt`, and `updatedAt`.
- [ ] Given a project ID that does not exist, when I submit a create request, then the API returns `404 Not Found`.
- [ ] Given an archived project, when I submit a create request, then the API returns `409 Conflict`.
- [ ] Given Projects.Api is unavailable, when I submit a create request, then the API returns `502 Bad Gateway`.
- [ ] Given a request without a title (or empty/whitespace), when I submit, then the API returns `400 Bad Request`.
- [ ] Given a title longer than 200 characters, when I submit, then the API returns `400 Bad Request`.
- [ ] Given optional fields (`description`, `assignee`, `dueDate`), when omitted, then the task is created with null/default values.

## API Contract

| | |
|---|---|
| **Method** | `POST` |
| **Path** | `/api/v1/projects/{projectId}/tasks` |
| **Request body** | `{ "title": "string", "description": "string (optional)", "assignee": "string (optional)", "dueDate": "ISO8601 (optional)" }` |
| **Success response** | `201 Created` — `TaskItem` object |
| **Error responses** | `400 Bad Request`, `404 Not Found`, `409 Conflict`, `502 Bad Gateway` |

**Response example:**

```json
{
  "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "projectId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "Implement create project endpoint",
  "description": null,
  "status": "ToDo",
  "assignee": null,
  "dueDate": null,
  "createdAt": "2026-07-01T10:00:00Z",
  "updatedAt": "2026-07-01T10:00:00Z"
}
```

## Technical Notes

- **Service:** Tasks.Api
- **Entity:** `TaskItem` in Cosmos container `tasks`, partition key `/projectId`
- **Cross-service call:** `GET {ProjectsApi}/api/v1/projects/{projectId}` before insert
  - `200` + `isArchived == false` → proceed
  - `404` → return 404
  - `200` + `isArchived == true` → return 409
  - HTTP error / timeout → return 502
- **HttpClient:** Register typed client with `ProjectsApi:BaseUrl` from configuration

## Definition of Done

- [ ] Endpoint creates task after project validation
- [ ] All error paths tested (404, 409, 502, 400)
- [ ] Unit tests mock Projects.Api HTTP responses
- [ ] Swagger updated

## References

- Domain rules [BR-T01](../domain/system-overview.md#business-rules), [BR-T02](../domain/system-overview.md#business-rules), [BR-T04](../domain/system-overview.md#business-rules), [BR-X01](../domain/system-overview.md#business-rules)
- [HttpClientFactory in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests)
- [Cross-service flow](../architecture/architecture-and-tech-stack.md#cross-service-communication)
