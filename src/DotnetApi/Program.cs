using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Add the database context to the services container (the dependency injection container). (Register the context)
builder.Services.AddDbContext<TodoContext>(opt =>
    opt.UseInMemoryDatabase("TodoList"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/test-config", (IConfiguration config) =>
{
    // This will return the environment variable value for the connection string. Otherwise, it will return the value from appsettings.json.
    var connectionString = config.GetConnectionString("DefaultConnection");
    return Results.Ok(connectionString);
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
