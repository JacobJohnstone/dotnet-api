using Microsoft.EntityFrameworkCore;
using DotnetApi.Contexts;

namespace DotnetApi.Helpers;

public static class DevMigrationsHelper
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using BudgetContext dbContext = scope.ServiceProvider.GetRequiredService<BudgetContext>();

        dbContext.Database.Migrate();
    }
}