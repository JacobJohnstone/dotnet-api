using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TodoApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Add the database context to the services container (the dependency injection container). (Register the context)
builder.Services.AddDbContext<TodoContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add firebase authentication
var firebaseProjectId = builder.Configuration.GetValue<string>("Firebase:ProjectId");
var authorityUrl = $"https://securetoken.google.com/{firebaseProjectId}";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = authorityUrl;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = authorityUrl,
        ValidateAudience = true,
        ValidAudience = firebaseProjectId,
        ValidateLifetime = true
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// register the middleware pipeline
app.UseHttpsRedirection();

app.UseAuthentication(); // decodes the JWT token and sets the user principal for the request
app.UseAuthorization(); // evaluates permissions against the user principal and the endpoint's authorization policies

app.MapControllers();

app.Run();
