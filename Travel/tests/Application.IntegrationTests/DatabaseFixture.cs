using System;
using System.IO;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Respawn;
using Travel.Data.Contexts;
using Travel.WebApi;

namespace Application.IntegrationTests
{
    public class DatabaseFixture  : IDisposable
    {
        private static IConfigurationRoot _configuration;
        private static IServiceScopeFactory _scopeFactory;
        private static Checkpoint _checkPoint;
        static  void Init()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables();
            _configuration = builder.Build();
            var startup = new Startup(_configuration);
            var services = new ServiceCollection();
            services.AddSingleton(Mock.Of<IWebHostEnvironment>(w =>
                w.EnvironmentName == "Development" && w.ApplicationName == "Travel.WebApi"
            ));
            services.AddLogging();
            startup.ConfigureServices(services);
            _scopeFactory = services.BuildServiceProvider().GetService<IServiceScopeFactory>();
            _checkPoint = new Checkpoint
            {
                TablesToIgnore = new[] { "__EFMigrationHistory" }
            };
        }
        public DatabaseFixture()
        {
            // create config root
            Init();
            EnsureDatabase();


        }

        private static void EnsureDatabase()
        {
            if(_scopeFactory is null)
            {
                Init();
            }
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetService<TravelDbContext>();
            context.Database.Migrate();
        }
        private static void DeEnsureDatabase()
        {
            if (_scopeFactory is null)
            {
                Init();
            }
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetService<TravelDbContext>();
            var result = context.Database.EnsureDeleted();
            context.SaveChanges();
        }
        public static async Task ResetState()
        {
            if (_checkPoint is null)
            {
                Init();
            }
           // config.ConnectionString = builder.ConnectionString;
            await _checkPoint.Reset(_configuration.GetConnectionString("DefaultConnection"));
        }

        public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request )
        {
            if (_scopeFactory is null)
            {
                Init();
            }
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            return await mediator.Send(request);
        }
        public static async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            if (_scopeFactory is null)
            {
                Init();
            }
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetService<TravelDbContext>();
            context.Add(entity);
            await context.SaveChangesAsync();
        }

        public static async Task<TEntity> FindAsync<TEntity>(int id)
            where TEntity : class
        {
            if (_scopeFactory is null)
            {
                Init();
            }
            using var scope = _scopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetService<TravelDbContext>();

            return await context.FindAsync<TEntity>(id);
        }

        public void Dispose()
        {

            DeEnsureDatabase();
        }

    }
}
