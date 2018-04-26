using Meticulos.Api.App;
using Meticulos.Api.App.ChangeHistory;
using Meticulos.Api.App.FieldOptions;
using Meticulos.Api.App.Fields;
using Meticulos.Api.App.Icons;
using Meticulos.Api.App.Items;
using Meticulos.Api.App.ItemTypes;
using Meticulos.Api.App.Screens;
using Meticulos.Api.App.WorkflowFunctions;
using Meticulos.Api.App.WorkflowNodes;
using Meticulos.Api.App.Workflows;
using Meticulos.Api.App.WorkflowTransitionFunctions;
using Meticulos.Api.App.WorkflowTransitions;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace meticulos_server
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllHeaders",
                      builder =>
                      {
                          builder.AllowAnyOrigin()
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();
                      });
            });

            services
                .AddMvc()
                .AddJsonOptions(options =>
                {
                    var settings = options.SerializerSettings;
                    settings.Converters = new List<JsonConverter> { new ObjectIdConverter() };
                });

            services.AddSingleton(Configuration);

            // Supply access to the Settings.cs file for application settings
            services.Configure<Settings>(options =>
            {
                //options.ConnectionString = Configuration.GetSection("MongoConnection:ConnectionString").Value;
                options.ConnectionString = Configuration.GetSection("ConnectionStrings:Mongo:ConnectionString").Value;
                options.Database = Configuration.GetSection("MongoConnection:Database").Value;
                options.TheNounProject_ConsumerKey = "154191dca3c842dea2b447c49cb1888b";
                options.TheNounProject_ConsumerKeySecret = "3d3c23d79b5e46f0a66bbdaf22a51c08";
            });

            // Dependency Injection Registrations
            services.AddTransient<IItemRepository, ItemRepository>();
            services.AddTransient<IItemTypeRepository, ItemTypeRepository>();
            services.AddTransient<IScreenRepository, ScreenRepository>();
            services.AddTransient<IFieldRepository, FieldRepository>();
            services.AddTransient<IFieldOptionRepository, FieldOptionRepository>();
            services.AddTransient<IFieldChangeGroupRepository, FieldChangeGroupRepository>();
            services.AddTransient<IWorkflowRepository, WorkflowRepository>();
            services.AddTransient<IWorkflowNodeRepository, WorkflowNodeRepository>();
            services.AddTransient<IWorkflowFunctionRepository, WorkflowFunctionRepository>();
            services.AddTransient<IWorkflowTransitionRepository, WorkflowTransitionRepository>();
            services.AddTransient<IWorkflowTransitionFunctionRepository, WorkflowTransitionFunctionRepository>();
            services.AddTransient<IIconSearcher, NounProjectIconSearcher>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowAllHeaders");

            app.UseMvc();
        }
    }
}
