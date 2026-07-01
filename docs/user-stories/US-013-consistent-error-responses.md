# US-013: Consistent Error Responses (RFC 7807)

## Story

As an **API consumer**, I want **all error responses to follow a consistent Problem Details format**, so that **I can programmatically handle errors across both services**.

## Acceptance Criteria

- [ ] Given any validation error (`400`), when the API responds, then the body conforms to RFC 7807 Problem Details (`type`, `title`, `status`, `detail`, `instance`).
- [ ] Given a not-found error (`404`), when the API responds, then Problem Details are returned with a clear `detail` message.
- [ ] Given a business rule violation (`409`), when the API responds, then Problem Details describe the conflict.
- [ ] Given an upstream service failure (`502`), when Tasks.Api responds, then Problem Details indicate Projects.Api is unavailable.
- [ ] Given a successful response, when the API responds, then Problem Details are **not** included.
- [ ] Content-Type for errors is `application/problem+json`.

## API Contract

All error responses follow this structure:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "Project name is required.",
  "instance": "/api/v1/projects"
}
```

For validation with multiple errors, optional `errors` extension:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation failed",
  "status": 400,
  "errors": {
    "name": ["The Name field is required."]
  },
  "instance": "/api/v1/projects"
}
```

## Technical Notes

- **Services:** Projects.Api and Tasks.Api
- **ASP.NET Core:** Enable Problem Details services:

```csharp
builder.Services.AddProblemDetails();
```

- **Controller behavior:** Use `ValidationProblem()`, `NotFound()`, `Conflict()`, or throw `BadHttpRequestException`
- **Global exception handler:** Optional middleware for unhandled exceptions → `500` with generic Problem Details (hide stack trace in production)

## Definition of Done

- [ ] All endpoints return Problem Details for error cases
- [ ] Manual test matrix: 400, 404, 409, 502 scenarios verified
- [ ] No plain-text or empty error bodies
- [ ] Swagger documents error response schemas (optional)

## References

- [RFC 7807 — Problem Details](https://datatracker.ietf.org/doc/html/rfc7807)
- [Problem Details in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/web-api/#problem-details)
- [Handle errors in ASP.NET Core APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling)
