# US-007: List All Tasks for a Project

## Story

As a **team member**, I want to **list all tasks belonging to a project**, so that **I can see the current workload and task statuses for that project**.

## Acceptance Criteria

- [ ] Given a project with tasks, when I request the task list for that project, then the API returns `200 OK` with an array of tasks.
- [ ] Given a project with no tasks, when I request the task list, then the API returns `200 OK` with an empty array `[]`.
- [ ] Given a project ID that does not exist, when I request the task list, then the API returns `404 Not Found` (after validating project via Projects.Api).
- [ ] Given multiple tasks, when I request the task list, then tasks are ordered by `createdAt` descending.

## API Contract

| | |
|---|---|
| **Method** | `GET` |
| **Path** | `/api/v1/projects/{projectId}/tasks` |
| **Query parameters** | None in this story (see US-016 for status filter) |
| **Success response** | `200 OK` — array of `TaskItem` objects |
| **Error responses** | `404 Not Found`, `502 Bad Gateway` |

**Response example:**

```json
[
  {
    "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "projectId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "title": "Implement create project endpoint",
    "description": "US-001",
    "status": "ToDo",
    "assignee": "student@tntu.edu.ua",
    "dueDate": null,
    "createdAt": "2026-07-01T10:00:00Z",
    "updatedAt": "2026-07-01T10:00:00Z"
  }
]
```

## Technical Notes

- **Service:** Tasks.Api
- **Query:** Filter by `projectId` (partition key) for efficient Cosmos query
- **Cross-service:** Validate project exists via Projects.Api before returning tasks (or return empty list — document chosen behavior; recommended: validate and return 404 if project missing)

## Definition of Done

- [ ] Endpoint returns tasks for valid project
- [ ] Returns 404 for non-existent project
- [ ] Tests cover empty and populated lists
- [ ] Swagger updated

## References

- Domain rule [BR-X01](../domain/system-overview.md#business-rules)
- [Partition key queries in Cosmos DB](https://learn.microsoft.com/en-us/azure/cosmos-db/partitioning-overview)
