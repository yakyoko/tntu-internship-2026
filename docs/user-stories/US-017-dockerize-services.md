# US-017: Dockerize Each API Service

## Story

As a **developer**, I want **a Dockerfile for each API service**, so that **I can build and run the services in containers consistently across machines**.

## Priority

**Could** — Week 4 stretch goal

## Acceptance Criteria

- [ ] Given Projects.Api, when I run `docker build`, then a container image is produced that runs the API on port 8080.
- [ ] Given Tasks.Api, when I run `docker build`, then a container image is produced that runs the API on port 8080.
- [ ] Given a built image, when I run the container with required environment variables, then the API starts and `/health` returns `200 OK`.
- [ ] Dockerfiles use multi-stage builds (SDK for build, aspnet runtime for run).
- [ ] `.dockerignore` excludes `bin/`, `obj/`, and secrets.

## API Contract

Not applicable — this is an infrastructure user story.

## Technical Notes

- **Files:**
  - `src/Projects.Api/Dockerfile`
  - `src/Tasks.Api/Dockerfile`
  - `.dockerignore` at repository root
- **Base images:** `mcr.microsoft.com/dotnet/sdk:8.0` (build), `mcr.microsoft.com/dotnet/aspnet:8.0` (runtime)
- **Port:** Expose 8080 (ASP.NET Core default in containers)
- **Environment:** Pass Cosmos and ProjectsApi settings via `-e` or env file

**Example Dockerfile (Projects.Api):**

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Projects.Api/Projects.Api.csproj", "Projects.Api/"]
RUN dotnet restore "Projects.Api/Projects.Api.csproj"
COPY src/Projects.Api/ .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Projects.Api.dll"]
```

## Definition of Done

- [ ] Both Dockerfiles build successfully
- [ ] Containers run locally with health check passing
- [ ] README documents `docker build` and `docker run` commands
- [ ] Images are not pushed to a registry (optional bonus)

## References

- [Containerize a .NET app](https://learn.microsoft.com/en-us/dotnet/core/docker/build-container)
- [Dockerfile reference](https://docs.docker.com/reference/dockerfile/)
- [Architecture — Optional Docker](../architecture/architecture-and-tech-stack.md#optional-docker-support)
