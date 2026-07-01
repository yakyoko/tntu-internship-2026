# US-015: GitHub Actions CD — Deploy to Azure

## Story

As a **developer**, I want **both APIs to deploy automatically to Azure when code is merged to main**, so that **the cloud environment always reflects the latest working version**.

## Acceptance Criteria

- [ ] Given a push to `main` with passing tests, when the CD workflow runs, then Projects.Api is deployed to its App Service.
- [ ] Given a push to `main`, when the CD workflow runs, then Tasks.Api is deployed to its App Service.
- [ ] Given deployment succeeds, when I call `/health` on both Azure URLs, then both return `200 OK`.
- [ ] Given deployment fails, when the workflow completes, then the workflow is marked failed with actionable logs.
- [ ] Azure credentials are stored in GitHub Secrets (OIDC or publish profile) — never in source code.
- [ ] App Service application settings for Cosmos and `ProjectsApi__BaseUrl` are configured in Azure Portal.

## API Contract

Not applicable — this is an infrastructure user story.

## Technical Notes

- **Workflow file:** `.github/workflows/cd.yml`
- **Trigger:** `push` to `main` branch only
- **Deployment target:**
  - `app-tntu-projects-api` (or your chosen name)
  - `app-tntu-tasks-api`
- **Recommended auth:** [OIDC federated credentials](https://learn.microsoft.com/en-us/azure/developer/github/connect-from-azure)

**Example jobs structure:**

```yaml
name: CD

on:
  push:
    branches: [main]

jobs:
  deploy-projects:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      - run: dotnet publish src/Projects.Api -c Release -o ./publish/projects
      - name: Deploy to Azure Web App
        uses: azure/webapps-deploy@v3
        with:
          app-name: app-tntu-projects-api
          package: ./publish/projects

  deploy-tasks:
    runs-on: ubuntu-latest
    needs: deploy-projects
    steps:
      # Similar steps for Tasks.Api
```

- **Order:** Deploy Projects.Api before Tasks.Api (Tasks depends on Projects URL)

## Definition of Done

- [ ] CD workflow deploys both services on merge to main
- [ ] Health checks pass in Azure after deployment
- [ ] [Demo script](../internship-plan/one-month-schedule.md#minimum-viable-demo-script) passes against Azure URLs
- [ ] Secrets documented in prerequisites (not values)

## References

- [Deploy to Azure from GitHub Actions](https://learn.microsoft.com/en-us/azure/app-service/deploy-github-actions)
- [OIDC with Azure](https://learn.microsoft.com/en-us/azure/developer/github/connect-from-azure)
- [Development Prerequisites — GitHub Secrets](../prerequisites/development-prerequisites.md#secrets-for-deployment-week-3--mentor-assists)
