using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Travel.Application.Common.Interfaces;
using Travel.Data.Contexts;


namespace Travel.Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureData(this IServiceCollection services, string sqliteConnectionString)
        {
            services.AddDbContext<TravelDbContext>(options => options
              .UseSqlite(sqliteConnectionString));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<TravelDbContext>());

            return services;
        }
    }
}

