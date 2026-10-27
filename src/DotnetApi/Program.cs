using System.Text;
using DotnetApi.Repositories.Users;
using DotnetApi.Services.Authentication;
using Microsoft.EntityFrameworkCore;
using DotnetApi.Contexts;
using DotnetApi.Helpers;

// Used for implementing JWT authentication and authorization in future
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
// add swagger for API documentation
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", 
policy =>
            {
                policy.WithOrigins("http://localhost:3000/", "http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            }
    );
});

// Authentication and authorization
// Attach service that implements the IAuthenticationService
// Responsible for authenticating a user, or responding when an unauthenticated user tries to access a restricted resource
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
        options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JwtIssuer"],
                ValidAudience = builder.Configuration["JwtAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        builder.Configuration["JwtKey"]
                        ?? throw new InvalidOperationException("JwtSecret is not configured")
                    )
                )
            };
        });

builder.Services.AddAuthorization();

// Add the database context to the services container (the dependency injection container). (Register the context)
builder.Services.AddDbContext<BudgetContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the authentication service
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Swagger configuration
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Automatically detect and apply new migrations on startup
    // (useful for keeping Postgres up to date automatically)
    app.ApplyMigrations();
}

// register the middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseCors("CorsPolicy");

app.MapSwagger().RequireAuthorization();

app.UseAuthentication(); // decodes the JWT token and sets the user principal for the request
app.UseAuthorization(); // evaluates permissions against the user principal and the endpoint's authorization policies

app.MapControllers();

app.Run();
