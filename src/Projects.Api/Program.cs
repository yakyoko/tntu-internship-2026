using Projects.Api.Infrastructure;
using Projects.Api.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExceptionToProblemDetailsHandler>();

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddCosmosInfrastructure(builder.Configuration);
}

if (builder.Environment.IsEnvironment("Production"))
{
    builder.Services.AddApplicationInsightsTelemetry();
}

builder.Services.AddProjectsHealthChecks();
builder.Services.AddProjectsAutoMapper();
builder.Services.AddProjectsApplicationServices();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (!builder.Environment.IsEnvironment("Testing"))
{
    await app.InitializeProjectsDatabaseAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapProjectsHealthChecks();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
