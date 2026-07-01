# US-014: GitHub Actions CI — Run Tests on PR

## Story

As a **developer**, I want **automated tests to run on every pull request and push**, so that **broken code is caught before it is merged to main**.

## Acceptance Criteria

- [ ] Given a pull request is opened or updated, when GitHub Actions runs, then `dotnet restore` and `dotnet test` execute for all test projects.
- [ ] Given all tests pass, when the workflow completes, then the check is green.
- [ ] Given any test fails, when the workflow completes, then the check is red and merge is blocked (if branch protection enabled).
- [ ] Given the workflow runs, when it completes, then it uses .NET 8 SDK on `ubuntu-latest`.
- [ ] Workflow file is committed at `.github/workflows/ci.yml`.

## API Contract

Not applicable — this is an infrastructure user story.

## Technical Notes

- **Workflow file:** `.github/workflows/ci.yml`
- **Test projects:** `src/Projects.Api.Tests`, `src/Tasks.Api.Tests`
- **No secrets required** for CI (tests should use mocks, not real Cosmos DB)

**Example workflow:**

```yaml
name: CI

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Restore
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore

      - name: Test
        run: dotnet test --no-build --verbosity normal
```

## Definition of Done

- [ ] CI workflow committed and enabled
- [ ] Green run on a test PR
- [ ] Failed test causes red check (verify intentionally)
- [ ] README mentions CI requirement

## References

- [GitHub Actions for .NET](https://docs.github.com/en/actions/automating-builds-and-tests/building-and-testing-net)
- [Workflow syntax](https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions)
- [Architecture — CI/CD](../architecture/architecture-and-tech-stack.md#cicd-pipeline)
