# US-005: Archive a Project

## Story

As a **team member**, I want to **archive a completed or cancelled project**, so that **it no longer appears in the active project list but its data is preserved**.

## Acceptance Criteria

- [ ] Given an existing active project, when I archive it, then the API returns `200 OK` with the project object where `isArchived` is `true`.
- [ ] Given a project ID that does not exist, when I archive it, then the API returns `404 Not Found`.
- [ ] Given an already archived project, when I archive it again, then the API returns `200 OK` (idempotent) or `409 Conflict` — document chosen behavior.
- [ ] Given an archived project, when I list projects (US-002), then it does not appear.
- [ ] Given an archived project, when Tasks.Api attempts to create a task (US-006), then it receives `409 Conflict`.

## API Contract

| | |
|---|---|
| **Method** | `PATCH` |
| **Path** | `/api/v1/projects/{id}/archive` |
| **Request body** | None (or empty `{}`) |
| **Success response** | `200 OK` — `Project` with `isArchived: true` |
| **Error responses** | `404 Not Found` |

## Technical Notes

- **Service:** Projects.Api
- **Implementation:** Set `isArchived = true`; do not delete the document
- **Soft delete:** Aligns with domain rule BR-P04

## Definition of Done

- [ ] Archive endpoint sets `isArchived` to true
- [ ] Archived project excluded from US-002 list
- [ ] Test verifies idempotent or conflict behavior
- [ ] Swagger updated

## References

- Domain rules [BR-P04](../domain/system-overview.md#business-rules), [BR-P05](../domain/system-overview.md#business-rules), [BR-T09](../domain/system-overview.md#business-rules)
- [Partial updates in REST](https://learn.microsoft.com/en-us/azure/architecture/best-practices/api-design)
