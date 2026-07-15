using Tasks.Api.Clients;
using Tasks.Api.Infrastructure;
using Tasks.Api.Interfaces;
using Tasks.Api.Repositories;
using Tasks.Api.Services;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddCosmosInfrastructure(builder.Configuration);
}

builder.Services.AddHttpClient<IProjectApiClient, ProjectApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ProjectsApi:BaseUrl"]!);
});

builder.Services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TasksDbContext>();
    await context.Database.EnsureCreatedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
