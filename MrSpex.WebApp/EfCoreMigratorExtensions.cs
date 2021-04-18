using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MrSpex.Data;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting
{
    public static class EfCoreMigratorExtensions
    {
        public static void RunWithMigrate(this IHost host)
        {
            // it should run somewhere in the github pipeline
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            using var context = services.GetRequiredService<AvailabilityDbContext>();
            Migrate(context);
            host.Run();
        }

        private static void Migrate(DbContext context)
        {
            Print("Database updating...");

            context.Database.Migrate();

            Print("Database updated!");
        }

        private static void Print(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine();
        }
    }
}