using ContosoUniversity.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace ContosoUniversity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run
            var host = CreateHostBuilder(args).Build();
            //CreateDbIfNotExists(host);
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    // Get a database context instance from the dependency injection container.
                    var context = services.GetRequiredService<SchoolContext>();
                    // Seed data if not exist
                    DbInitializer.Initialize(context); 
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB");
                }
            }
            host.Run();
        }

        //The first time the app is run, the database is created and loaded with test data.
        //Whenever the data model changes:
        //Delete the database.
        //Update the seed method, and start afresh with a new database.

        //private static void CreateDbIfNotExists(IHost host)
        //{
        //    using (var scope = host.Services.CreateScope())
        //    {
        //        var services = scope.ServiceProvider;
        //        try
        //        {
        //            var context = services.GetRequiredService<SchoolContext>();
        //            DbInitializer.Initialize(context);
        //        }
        //        catch (Exception ex)
        //        {
        //            var logger = services.GetRequiredService<ILogger<Program>>();
        //            logger.LogError(ex, "An error occurred creating the DB");
        //        }
        //    }
        //    //throw new NotImplementedException();
        //}

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
