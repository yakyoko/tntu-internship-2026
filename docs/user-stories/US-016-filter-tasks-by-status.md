# US-016: Filter Tasks by Status

## Story

As a **team member**, I want to **filter tasks by status within a project**, so that **I can quickly see only To Do, In Progress, or Done items**.

## Priority

**Could** — Week 4 stretch goal

## Acceptance Criteria

- [ ] Given a project with tasks in various statuses, when I request `GET /api/v1/projects/{projectId}/tasks?status=ToDo`, then only tasks with `ToDo` status are returned.
- [ ] Given the same project, when I request with `status=InProgress` or `status=Done`, then only matching tasks are returned.
- [ ] Given no status query parameter, when I request the task list, then all tasks are returned (same as US-007).
- [ ] Given an invalid status value, when I request with `?status=Invalid`, then the API returns `400 Bad Request`.
- [ ] Given a project that does not exist, when I filter tasks, then the API returns `404 Not Found`.

## API Contract

| | |
|---|---|
| **Method** | `GET` |
| **Path** | `/api/v1/projects/{projectId}/tasks` |
| **Query parameter** | `status` — optional: `ToDo`, `InProgress`, `Done` |
| **Success response** | `200 OK` — filtered array of `TaskItem` |
| **Error responses** | `400 Bad Request`, `404 Not Found` |

**Example:** `GET /api/v1/projects/3fa85f64-5717-4562-b3fc-2c963f66afa6/tasks?status=InProgress`

## Technical Notes

- **Service:** Tasks.Api
- **Extends:** US-007 list endpoint with optional query filter
- **EF Core:** `.Where(t => t.Status == status)` when parameter provided
- **Enum binding:** ASP.NET Core model binding for `TaskStatus` enum

## Definition of Done

- [ ] Filter works for all three statuses
- [ ] No parameter returns all tasks (backward compatible)
- [ ] Tests for filtered and unfiltered requests
- [ ] Swagger documents query parameter

## References

- [Model binding in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/model-binding)
- [US-007 List tasks](US-007-list-tasks-by-project.md)
