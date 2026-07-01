# US-008: View Task Details by ID

## Story

As a **team member**, I want to **view the details of a specific task**, so that **I can see its full information including status, assignee, and due date**.

## Acceptance Criteria

- [ ] Given a valid project ID and task ID that exist, when I request the task, then the API returns `200 OK` with the full task object.
- [ ] Given a task ID that does not exist within the project, when I request the task, then the API returns `404 Not Found`.
- [ ] Given a project ID that does not exist, when I request a task, then the API returns `404 Not Found`.
- [ ] Given a task that belongs to a different project than specified in the URL, when I request the task, then the API returns `404 Not Found`.

## API Contract

| | |
|---|---|
| **Method** | `GET` |
| **Path** | `/api/v1/projects/{projectId}/tasks/{taskId}` |
| **Path parameters** | `projectId` — GUID, `taskId` — GUID |
| **Success response** | `200 OK` — `TaskItem` object |
| **Error responses** | `404 Not Found` |

**Response example:**

```json
{
  "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "projectId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "Implement create project endpoint",
  "description": "US-001",
  "status": "InProgress",
  "assignee": "student@tntu.edu.ua",
  "dueDate": "2026-07-15T00:00:00Z",
  "createdAt": "2026-07-01T10:00:00Z",
  "updatedAt": "2026-07-03T14:30:00Z"
}
```

## Technical Notes

- **Service:** Tasks.Api
- **Cosmos query:** Use partition key `projectId` + document `id` for efficient point read
- **Security note:** Verify `task.projectId == projectId` from URL to prevent cross-project access

## Definition of Done

- [ ] Endpoint returns task for valid IDs
- [ ] Returns 404 when task not found or project mismatch
- [ ] Tests cover found and not-found scenarios
- [ ] Swagger updated

## References

- [EF Core Cosmos DB queries](https://learn.microsoft.com/en-us/ef/core/providers/cosmos/querying)
