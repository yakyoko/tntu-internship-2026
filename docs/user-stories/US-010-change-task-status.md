# US-010: Change Task Status

## Story

As a **team member**, I want to **move a task through its lifecycle statuses**, so that **the team can track progress from To Do through In Progress to Done**.

## Acceptance Criteria

- [ ] Given a task in `ToDo` status, when I request status change to `InProgress`, then the API returns `200 OK` with updated task.
- [ ] Given a task in `InProgress` status, when I request status change to `Done`, then the API returns `200 OK` with updated task.
- [ ] Given a task in `ToDo` status, when I request status change to `Done` (skipping InProgress), then the API returns `409 Conflict`.
- [ ] Given a task in `InProgress` status, when I request status change to `ToDo`, then the API returns `409 Conflict`.
- [ ] Given a task in `Done` status, when I request any status change, then the API returns `409 Conflict`.
- [ ] Given an invalid status value, when I submit, then the API returns `400 Bad Request`.
- [ ] Given a successful status change, when the response is returned, then `updatedAt` is updated.

## API Contract

| | |
|---|---|
| **Method** | `PATCH` |
| **Path** | `/api/v1/projects/{projectId}/tasks/{taskId}/status` |
| **Request body** | `{ "status": "ToDo" \| "InProgress" \| "Done" }` |
| **Success response** | `200 OK` — updated `TaskItem` |
| **Error responses** | `400 Bad Request`, `404 Not Found`, `409 Conflict` |

## Technical Notes

- **Service:** Tasks.Api
- **State machine:** Implement transition validation per [domain lifecycle](../domain/system-overview.md#task-status-lifecycle)

| From | To | Allowed |
|------|-----|---------|
| ToDo | InProgress | Yes |
| InProgress | Done | Yes |
| All others | — | No |

- **Implementation tip:** Encapsulate rules in a `TaskStatusTransition` validator or domain method

## Definition of Done

- [ ] All allowed transitions work
- [ ] All disallowed transitions return 409
- [ ] Unit tests cover every transition combination
- [ ] Swagger updated

## References

- Domain rule [BR-T05](../domain/system-overview.md#business-rules)
- [Task status lifecycle diagram](../domain/system-overview.md#task-status-lifecycle)
