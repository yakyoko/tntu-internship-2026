# US-011: Delete a Task

## Story

As a **team member**, I want to **delete a task that is no longer needed**, so that **the task board stays clean and relevant**.

## Acceptance Criteria

- [ ] Given an existing task, when I send a delete request, then the API returns `204 No Content` and the task is permanently removed from Cosmos DB.
- [ ] Given a task ID that does not exist, when I send a delete request, then the API returns `404 Not Found`.
- [ ] Given a project ID that does not exist, when I send a delete request, then the API returns `404 Not Found`.
- [ ] Given a deleted task, when I request it by ID, then the API returns `404 Not Found`.

## API Contract

| | |
|---|---|
| **Method** | `DELETE` |
| **Path** | `/api/v1/projects/{projectId}/tasks/{taskId}` |
| **Request body** | None |
| **Success response** | `204 No Content` |
| **Error responses** | `404 Not Found` |

## Technical Notes

- **Service:** Tasks.Api
- **EF Core:** `Remove(entity)` + `SaveChangesAsync`
- **Hard delete:** Unlike projects, tasks are permanently deleted (domain rule BR-T08)
- **Partition key:** Ensure delete uses correct `projectId` partition

## Definition of Done

- [ ] Delete removes document from Cosmos
- [ ] Subsequent GET returns 404
- [ ] Tests verify delete and not-found
- [ ] Swagger updated

## References

- Domain rule [BR-T08](../domain/system-overview.md#business-rules)
- [EF Core deleting data](https://learn.microsoft.com/en-us/ef/core/saving/cascade-delete)
