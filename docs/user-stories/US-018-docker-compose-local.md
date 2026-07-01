# US-018: Docker Compose for Local Development

## Story

As a **developer**, I want **a Docker Compose file to run both APIs together locally**, so that **I can test the full microservice workflow without starting each service manually**.

## Priority

**Could** — Week 4 stretch goal

## Acceptance Criteria

- [ ] Given `docker-compose.yml`, when I run `docker compose up`, then both Projects.Api and Tasks.Api start.
- [ ] Given both containers are running, when I create a project and then a task, then the cross-service call succeeds (Tasks → Projects).
- [ ] Given Compose configuration, when services start, then Tasks.Api has `ProjectsApi__BaseUrl` pointing to the Projects container hostname.
- [ ] Given Compose configuration, when services start, then both services have Cosmos DB connection settings (cloud Cosmos or host emulator).
- [ ] `docker compose down` stops and removes containers cleanly.

## API Contract

Not applicable — this is an infrastructure user story.

## Technical Notes

- **File:** `docker-compose.yml` at repository root (or `src/`)
- **Depends on:** US-017 Dockerfiles
- **Networking:** Use Compose service names for inter-container DNS (e.g., `http://projects-api:8080`)

**Example compose structure:**

```yaml
services:
  projects-api:
    build:
      context: .
      dockerfile: src/Projects.Api/Dockerfile
    ports:
      - "5001:8080"
    environment:
      - CosmosDb__Endpoint=${COSMOS_ENDPOINT}
      - CosmosDb__Key=${COSMOS_KEY}
      - CosmosDb__DatabaseName=TaskBoard

  tasks-api:
    build:
      context: .
      dockerfile: src/Tasks.Api/Dockerfile
    ports:
      - "5002:8080"
    environment:
      - CosmosDb__Endpoint=${COSMOS_ENDPOINT}
      - CosmosDb__Key=${COSMOS_KEY}
      - ProjectsApi__BaseUrl=http://projects-api:8080
    depends_on:
      - projects-api
```

- **Cosmos DB note:** The Linux emulator or cloud Cosmos is required — document which approach the team uses. Cosmos DB Emulator on Windows host can be reached via `host.docker.internal`.

## Definition of Done

- [ ] `docker compose up` starts both services
- [ ] End-to-end flow works: create project → create task → list tasks
- [ ] `.env.example` documents required variables (no real secrets)
- [ ] README documents Compose usage

## References

- [Docker Compose overview](https://docs.docker.com/compose/)
- [Use Cosmos DB Emulator from Docker](https://learn.microsoft.com/en-us/azure/cosmos-db/local-emulator)
- [US-017 Dockerize services](US-017-dockerize-services.md)
