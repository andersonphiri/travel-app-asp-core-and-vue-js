using System;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Travel.WebApi.Helpers
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;
        /// <summary>
        /// adds a Swagger document for every discovered API version
        /// </summary>
        /// <param name="options"></param>
        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }

        /// <summary>
        /// Basically, this code is for the Swagger information such as title, version, description, contact name, contact email, and URL of the application:
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo
            {
                Title = "Travel Tour",
                Version = description.ApiVersion.ToString(),
                Description = "Web Service for Travel Tour.",
                Contact = new OpenApiContact
                {
                    Name = "IT Department",
                    Email = "andersonp@chitataunga.com",
                    Url = new Uri("https://chitataunga.com/support")
                }
            };

            if (description.IsDeprecated)
                info.Description += " <strong>This API version of Travel Tour has been deprecated.</strong>";

            return info;
        }
    }
}
