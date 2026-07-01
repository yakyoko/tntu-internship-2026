# US-012: Health Check Endpoints

## Story

As a **DevOps engineer / mentor**, I want **health check endpoints on both services**, so that **I can verify the APIs are running and optionally monitor dependency health**.

## Acceptance Criteria

- [ ] Given the service is running, when I call `GET /health`, then the API returns `200 OK` with a health status payload.
- [ ] Given Cosmos DB is reachable, when I call `/health`, then the response indicates the database check is healthy.
- [ ] Given Cosmos DB is unreachable, when I call `/health`, then the API returns `503 Service Unavailable`.
- [ ] Given Tasks.Api, when health check runs, then it optionally includes Projects.Api reachability as a degraded (not unhealthy) dependency — document chosen behavior.
- [ ] Health endpoints are mapped in both Projects.Api and Tasks.Api.

## API Contract

| | |
|---|---|
| **Method** | `GET` |
| **Path** | `/health` |
| **Success response** | `200 OK` — `{ "status": "Healthy" }` (or ASP.NET Core default health UI JSON) |
| **Unhealthy response** | `503 Service Unavailable` |

**Example response (custom writer optional):**

```json
{
  "status": "Healthy",
  "checks": [
    { "name": "cosmosdb", "status": "Healthy" }
  ]
}
```

## Technical Notes

- **Services:** Projects.Api and Tasks.Api
- **ASP.NET Core:** Use built-in health checks middleware
- **Cosmos DB check:** `AddCosmosDb` health check or custom `IHealthCheck` that pings the database
- **Registration:**

```csharp
builder.Services.AddHealthChecks()
    .AddCosmosDb(connectionString, name: "cosmosdb");

app.MapHealthChecks("/health");
```

## Definition of Done

- [ ] `/health` returns 200 when service and Cosmos are healthy
- [ ] Returns 503 when Cosmos is down (test with invalid connection string)
- [ ] Azure App Service can use health check URL (optional configuration)
- [ ] Documented in README

## References

- [Health checks in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks)
- [App Service health check](https://learn.microsoft.com/en-us/azure/app-service/monitor-instances-health-check)
