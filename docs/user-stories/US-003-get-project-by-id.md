# US-003: View Project Details by ID

## Story

As a **team member**, I want to **view the details of a specific project by its ID**, so that **I can confirm project information before adding tasks or sharing with the team**.

## Acceptance Criteria

- [ ] Given a project ID that exists, when I request that project, then the API returns `200 OK` with the full project object (including archived projects).
- [ ] Given a project ID that does not exist, when I request that project, then the API returns `404 Not Found` with Problem Details.
- [ ] Given an invalid GUID format in the URL, when I request that project, then the API returns `400 Bad Request`.

## API Contract

| | |
|---|---|
| **Method** | `GET` |
| **Path** | `/api/v1/projects/{id}` |
| **Path parameter** | `id` — GUID |
| **Success response** | `200 OK` — `Project` object |
| **Error responses** | `400 Bad Request`, `404 Not Found` |

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
- **EF Core:** `FindAsync(id)` or `FirstOrDefaultAsync(p => p.Id == id)` with partition key = `id`
- **Note:** This endpoint is also called by Tasks.Api for project validation (US-006)

## Definition of Done

- [ ] Endpoint returns correct project for valid ID
- [ ] Returns 404 for non-existent ID
- [ ] Tests cover found and not-found scenarios
- [ ] Swagger documents path parameter

## References

- Domain rule [BR-P06](../domain/system-overview.md#business-rules)
- [Routing in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing)
