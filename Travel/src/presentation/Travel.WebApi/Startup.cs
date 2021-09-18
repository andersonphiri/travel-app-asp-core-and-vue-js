using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Travel.Application;
using Travel.Data;
using Travel.Data.Contexts;
using Travel.Identity;
using Travel.Identity.Helpers;
using Travel.Shared;
using Travel.WebApi.Extensions;
using Travel.WebApi.Filters;
using Travel.WebApi.Helpers;
using VueCliMiddleware;

namespace Travel.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddApplication(Configuration);
            services.AddInfrastructureData("Data Source=TravelTourDatabase.sqlite3");
            services.AddInfrastructureShared(Configuration);
            services.AddInfrastructureIdentity(Configuration);
            services.AddHttpContextAccessor();

            services.AddControllers();
            services.AddControllersWithViews(options =>
                    options.Filters.Add(new ApiExceptionFilter()));
            services.Configure<ApiBehaviorOptions>(options =>
                options.SuppressModelStateInvalidFilter = true
            );
            services.AddApiVersioningExtension();
            services.AddVersionedApiExplorerExtension();
            services.AddSwaggerGenExtension();
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "../vue-app/dist";
            });
            services.AddCors();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerExtension(provider);
            }
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }
            app.UseCors(b =>
            {
                b.AllowAnyOrigin();
                b.AllowAnyHeader();
                b.AllowAnyMethod();
            });
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseMiddleware<JwtMiddleware>();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "../vue-app";
                if(env.IsDevelopment())
                {
                    spa.UseVueCli(npmScript: "serve");
                }
            });
        }
    }
}
