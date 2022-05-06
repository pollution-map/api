using Microsoft.EntityFrameworkCore;

namespace PollutionMapAPI.Helpers;

public static class MigrationsHelper
{
    public static IServiceProvider MigrateDatabase<T>(this IServiceProvider services) where T : DbContext
    {
        using (var scope = services.CreateScope())
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                var db = scope.ServiceProvider.GetRequiredService<T>();

                db.Database.Migrate();
                logger.LogInformation("Migration applied.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating the database.");
            }
        }

        return services;
    }
}
