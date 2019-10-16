using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using SampleWebApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SampleWebApi
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
            services.AddDbContext<TodoContext>(opt =>
                opt.UseInMemoryDatabase("TodoList"));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ToDo API",
                    Description = "A simple example ASP.NET Core Web API",
                    Contact = new OpenApiContact
                    {
                        Name = "EuroForm",
                        Email = string.Empty
                    },
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.SchemaFilter<CustomXmlSchemaFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseMvc();
        }
    }

    public class CustomXmlSchemaFilter : ISchemaFilter
    {
        private const string _SCHEMA_ARRAY_TYPE = "array";
        private const string _SCHEMA_STRING_TYPE = "string";
        private const string _PREFIX_ARRAY = "ArrayOf";

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.ApiModel.Type.IsValueType)
                return;

            if (schema.Type == _SCHEMA_STRING_TYPE)
                return;

            schema.Xml = new OpenApiXml
            {
                Name = context.ApiModel.Type.Name
            };

            if (schema.Type == _SCHEMA_ARRAY_TYPE)
            {
                var itemName = string.Empty;
                if (schema.Items.Reference != null)
                {
                    itemName = schema.Items.Reference.Id;
                }
                else
                {
                    schema.Items.Xml = new OpenApiXml
                    {
                        Name = schema.Items.Type,
                    };
                    itemName = schema.Items.Type;
                }

                schema.Xml = new OpenApiXml
                {
                    Name = $"{_PREFIX_ARRAY}{itemName}",
                    Wrapped = true,
                };
            }

            if (schema.Properties == null)
            {
                return;
            }

            foreach (var property in schema.Properties.Where(x => x.Value.Type == _SCHEMA_ARRAY_TYPE))
            {
                property.Value.Items.Xml = new OpenApiXml
                {
                    Name = property.Value.Items.Type,
                };
                property.Value.Xml = new OpenApiXml
                {
                    Name = property.Key,
                    Wrapped = true
                };
            }
        }
    }
}
