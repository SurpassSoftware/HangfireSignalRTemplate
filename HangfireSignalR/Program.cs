using HangfireSignalR.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HangfireSignalR
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            //Apply migration
            var serviceScope = (IServiceScopeFactory)host.Services.GetService(typeof(IServiceScopeFactory));
            ApplyMigration(serviceScope);

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        private static void ApplyMigration(IServiceScopeFactory serviceScope)
        {
            using (var scope = serviceScope.CreateScope())
            {
                using (var baseDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                {
                    baseDbContext.Database.Migrate();
                }
            }
        }
    }
}
