using Tasks.Api.Infrastructure;
using Tasks.Api.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddCosmosInfrastructure(builder.Configuration);
}

builder.Services.AddTasksHealthChecks(builder.Configuration);
builder.Services.AddProjectApiClient(builder.Configuration);
builder.Services.AddTasksAutoMapper();
builder.Services.AddTasksApplicationServices();

builder.Services.AddTasksControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (!builder.Environment.IsEnvironment("Testing"))
{
    await app.InitializeTasksDatabaseAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapTasksHealthChecks();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
