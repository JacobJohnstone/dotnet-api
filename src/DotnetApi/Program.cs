using Microsoft.EntityFrameworkCore;

// Used for implementing JWT authentication and authorization in future
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// Authentication and authorization
// Attach service that implements the IAuthenticationService
// Responsible for authenticating a user, or responding when an unauthenticated user tries to access a restricted resource
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, 
        options => builder.Configuration.Bind("JwtSettings", options))

builder.Services.AddAuthorization();

// add swagger for API documentation
builder.Services.AddSwaggerGen();

// Add the database context to the services container (the dependency injection container). (Register the context)
builder.Services.AddDbContext<BudgetContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// register the middleware pipeline
app.UseHttpsRedirection();

app.UseAuthentication(); // decodes the JWT token and sets the user principal for the request
app.UseAuthorization(); // evaluates permissions against the user principal and the endpoint's authorization policies

app.MapControllers();

app.Run();
