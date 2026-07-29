FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Projects.Api/Projects.Api.csproj", "Projects.Api/"]
RUN dotnet restore "Projects.Api/Projects.Api.csproj"
COPY ["src/Projects.Api/", "Projects.Api/"]
RUN dotnet publish Projects.Api/Projects.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Projects.Api.dll"]
