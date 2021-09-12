using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;

namespace Travel.WebApi
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var name = Assembly.GetExecutingAssembly().GetName();
            var sqliteLogPath = Path.Combine(Environment.CurrentDirectory, "Logs", @"logs.sqlite3");
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Debug()
                            // add enrichers here before create
                            .Enrich.FromLogContext()
                            .Enrich.WithExceptionDetails()
                            .Enrich.WithMachineName()
                            .Enrich.WithProperty("Assembly", $"{name.Name}")
                            .Enrich.WithProperty("Assembly", $"{name.Version}")
                            .WriteTo.SQLite(
                                    sqliteLogPath,
                                    restrictedToMinimumLevel: LogEventLevel.Information,
                                    storeTimestampInUtc: true
                                
                                    )
                            .WriteTo.File(
                                new CompactJsonFormatter(),
                                Environment.CurrentDirectory + @"/Logs/log.json",
                                rollingInterval: RollingInterval.Day,
                                restrictedToMinimumLevel: LogEventLevel.Information
                                )
                            .WriteTo.Console()
                            .CreateLogger();
            try
            {
                Log.Information("Starting host");
                Log.Information("Sqlite Log Db Path: {SqliteDbPath}", sqliteLogPath);
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Host terminated unexpectedly");
                return 1;
            }
            finally {
                Log.CloseAndFlush();
            }
            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
