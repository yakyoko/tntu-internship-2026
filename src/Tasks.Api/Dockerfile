FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Tasks.Api/Tasks.Api.csproj", "Tasks.Api/"]
RUN dotnet restore "Tasks.Api/Tasks.Api.csproj"
COPY ["src/Tasks.Api/", "Tasks.Api/"]
RUN dotnet publish Tasks.Api/Tasks.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Tasks.Api.dll"]
