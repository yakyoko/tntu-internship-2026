# US-004: Update Project Name and Description

## Story

As a **team member**, I want to **update a project's name and description**, so that **I can keep project information accurate as work evolves**.

## Acceptance Criteria

- [ ] Given an existing project, when I send a valid update request, then the API returns `200 OK` with the updated project object.
- [ ] Given a project ID that does not exist, when I send an update request, then the API returns `404 Not Found`.
- [ ] Given an empty or whitespace name in the update, when I submit, then the API returns `400 Bad Request`.
- [ ] Given a name longer than 100 characters, when I submit, then the API returns `400 Bad Request`.
- [ ] Given a description longer than 500 characters, when I submit, then the API returns `400 Bad Request`.
- [ ] Given an archived project, when I send an update request, then the API returns `409 Conflict` (optional — document chosen behavior).

## API Contract

| | |
|---|---|
| **Method** | `PUT` |
| **Path** | `/api/v1/projects/{id}` |
| **Request body** | `{ "name": "string", "description": "string (optional)" }` |
| **Success response** | `200 OK` — updated `Project` object |
| **Error responses** | `400 Bad Request`, `404 Not Found`, `409 Conflict` |

## Technical Notes

- **Service:** Projects.Api
- **EF Core:** Load entity, apply changes, `SaveChangesAsync`
- **Validation:** Reuse same rules as US-001 for name and description

## Definition of Done

- [ ] Endpoint updates and returns project
- [ ] Validation tests for name constraints
- [ ] 404 tested for missing project
- [ ] Swagger updated

## References

- Domain rules [BR-P01](../domain/system-overview.md#business-rules), [BR-P02](../domain/system-overview.md#business-rules)
- [EF Core saving data](https://learn.microsoft.com/en-us/ef/core/saving/)
