# US-002: List All Non-Archived Projects

## Story

As a **team member**, I want to **list all active (non-archived) projects**, so that **I can see what projects exist and choose where to add tasks**.

## Acceptance Criteria

- [ ] Given projects exist in the database, when I request the project list, then the API returns `200 OK` with an array of projects where `isArchived` is `false`.
- [ ] Given no projects exist, when I request the project list, then the API returns `200 OK` with an empty array `[]`.
- [ ] Given archived projects exist, when I request the project list, then archived projects are **not** included in the response.
- [ ] Given multiple projects exist, when I request the project list, then projects are ordered by `createdAt` descending (newest first).

## API Contract

| | |
|---|---|
| **Method** | `GET` |
| **Path** | `/api/v1/projects` |
| **Request body** | None |
| **Success response** | `200 OK` — array of `Project` objects |

**Response example:**

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Internship Backend",
    "description": "Build the task board APIs",
    "isArchived": false,
    "createdAt": "2026-07-01T10:00:00Z"
  }
]
```

## Technical Notes

- **Service:** Projects.Api
- **Query:** `WHERE isArchived == false ORDER BY createdAt DESC`
- **EF Core:** Use LINQ `.Where(p => !p.IsArchived).OrderByDescending(p => p.CreatedAt)`

## Definition of Done

- [ ] Endpoint returns only non-archived projects
- [ ] Unit or integration test covers empty list and filtered list scenarios
- [ ] Swagger documents the endpoint

## References

- Domain rule [BR-P05](../domain/system-overview.md#business-rules)
- [LINQ in EF Core](https://learn.microsoft.com/en-us/ef/core/querying/)
