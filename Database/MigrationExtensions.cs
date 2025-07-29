using Microsoft.EntityFrameworkCore;

namespace finance_management.Database
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            using PfmDbContext context = scope.ServiceProvider.GetRequiredService<PfmDbContext>();
            var pendingMigrations = context.Database.GetPendingMigrations();
            if (pendingMigrations.Any())
            {
                Console.WriteLine("Applying migrations...");
                context.Database.Migrate();

            }


        }
    }
}
