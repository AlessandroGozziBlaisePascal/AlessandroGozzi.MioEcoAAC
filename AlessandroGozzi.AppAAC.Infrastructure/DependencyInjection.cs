using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Application.Interfaces;
using AlessandroGozzi.AppAAC.Domain.Repositories;
using AlessandroGozzi.AppAAC.Infrastructure.Persistance;
using AlessandroGozzi.AppAAC.Infrastructure.Repositories;
using AlessandroGozzi.AppAAC.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlessandroGozzi.AppAAC.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration? configuration = null)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var defaultDbPath = Path.Combine(appData, "AppAAC.db");
            var connectionString = configuration?.GetConnectionString("DefaultConnection")
                                   ?? $"Data Source={defaultDbPath}";

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(connectionString));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IRoutineRepository, RoutineRepository>();

            services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();

            return services;
        }

        public static async Task UseInfrastructureDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
            await initializer.InitializeAndSeedAsync();
        }
    }
}
