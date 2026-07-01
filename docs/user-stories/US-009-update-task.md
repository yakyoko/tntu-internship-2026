# US-009: Update Task Details

## Story

As a **team member**, I want to **update a task's title, description, assignee, and due date**, so that **I can keep task information current without changing its status**.

## Acceptance Criteria

- [ ] Given an existing task, when I send a valid update request, then the API returns `200 OK` with the updated task and a new `updatedAt` timestamp.
- [ ] Given a task that does not exist, when I send an update request, then the API returns `404 Not Found`.
- [ ] Given an empty or whitespace title, when I submit, then the API returns `400 Bad Request`.
- [ ] Given a title longer than 200 characters, when I submit, then the API returns `400 Bad Request`.
- [ ] Given a description longer than 2000 characters, when I submit, then the API returns `400 Bad Request`.
- [ ] Given an assignee longer than 200 characters, when I submit, then the API returns `400 Bad Request`.
- [ ] Given a valid update, when the response is returned, then `status` is unchanged (status changes use US-010).

## API Contract

| | |
|---|---|
| **Method** | `PUT` |
| **Path** | `/api/v1/projects/{projectId}/tasks/{taskId}` |
| **Request body** | `{ "title": "string", "description": "string (optional)", "assignee": "string (optional)", "dueDate": "ISO8601 (optional)" }` |
| **Success response** | `200 OK` — updated `TaskItem` |
| **Error responses** | `400 Bad Request`, `404 Not Found` |

## Technical Notes

- **Service:** Tasks.Api
- **Fields updated:** `title`, `description`, `assignee`, `dueDate`, `updatedAt`
- **Fields not updated:** `status` (see US-010), `projectId`, `id`, `createdAt`
- **Validation:** Reuse constraints from domain [BR-T02](../domain/system-overview.md#business-rules), [BR-T03](../domain/system-overview.md#business-rules), [BR-T06](../domain/system-overview.md#business-rules)

## Definition of Done

- [ ] Endpoint updates allowed fields only
- [ ] `updatedAt` set to current UTC on every update
- [ ] Validation tests for field constraints
- [ ] Swagger updated

## References

- Domain rules [BR-T02](../domain/system-overview.md#business-rules) through [BR-T07](../domain/system-overview.md#business-rules)
- [EF Core saving data](https://learn.microsoft.com/en-us/ef/core/saving/)
