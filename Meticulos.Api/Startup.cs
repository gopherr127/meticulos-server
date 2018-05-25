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
using Meticulos.Api.App.Locations;
using Meticulos.Api.App.Dashboard;
using Meticulos.Api.App.AppUsers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

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

            // Auth0 Configuration
            string domain = $"https://{Configuration["Auth0:Domain"]}/";
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = domain;
                options.Audience = Configuration["Auth0:ApiIdentifier"];
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("read:projects", policy =>
                    policy.Requirements.Add(new HasScopeRequirement("read:projects", domain)));
                options.AddPolicy("modify:projects", policy =>
                    policy.Requirements.Add(new HasScopeRequirement("modify:projects", domain)));
                options.AddPolicy("delete:projects", policy =>
                    policy.Requirements.Add(new HasScopeRequirement("delete:projects", domain)));
            });

            // register the scope authorization handler
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

            services.AddSingleton(Configuration);

            // Supply access to the Settings.cs file for application settings
            services.Configure<Settings>(options =>
            {
                //options.ConnectionString = Configuration.GetSection("MongoConnection:ConnectionString").Value;
                options.ConnectionString = Configuration["ConnectionStrings:Mongo:ConnectionString"];
                options.DefaultDatabase = Configuration["MongoConnection:Database"];
                options.TheNounProject_ConsumerKey = "154191dca3c842dea2b447c49cb1888b";
                options.TheNounProject_ConsumerKeySecret = "3d3c23d79b5e46f0a66bbdaf22a51c08";
            });

            // Dependency Injection Registrations
            services.AddSingleton<IFunctionRegistry, FunctionRegistry>();

            services.AddTransient<IAppUserRepository, AppUserRepository>();
            services.AddTransient<IDashboardPanelRepository, DashboardPanelRepository>();
            services.AddTransient<IItemRepository, ItemRepository>();
            services.AddTransient<IItemTypeRepository, ItemTypeRepository>();
            services.AddTransient<IItemLocationRepository, ItemLocationRepository>();
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
        public async void Configure(IApplicationBuilder app,
            IServiceProvider serviceProvider, 
            IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //TODO: app.UseExceptionHandler("...rel path to error page...");
            }
            app.UseStaticFiles();

            // Auth0 Configuration
            app.UseAuthentication();

            app.UseCors("AllowAllHeaders");

            app.UseMvc();

            // Ensure all required functions exist as system defaults
            var functionRegistry = serviceProvider.GetService<IFunctionRegistry>();
            var workflowFunctionRepository = serviceProvider.GetService<IWorkflowFunctionRepository>();

            foreach (WorkflowFunction func in functionRegistry.GetDefaultFunctions())
            {
                var wfFunc = await workflowFunctionRepository.Get(func.Id);
                if (wfFunc == null)
                {
                    await workflowFunctionRepository.Add(wfFunc);
                }
            }
        }
    }
}
