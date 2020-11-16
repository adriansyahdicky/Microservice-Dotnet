using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Catalog.API.Infrastructure.Filters;
using Catalog.API.Infrastructure.Repositories;
using Catalog.API.Infrastructure.Services;
using EventBus;
using EventBus.Abstractions;
using EventBusRabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Catalog.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .AddCustomMVC(Configuration)
                .AddIntegrationServices(Configuration)
                .AddEventBus(Configuration);


            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddTransient<ICatalogService, CatalogService>();
            services.AddTransient<ICatalogRepository, CatalogRepository>();
            //services.AddOptions();

            var container = new ContainerBuilder();
            //container.RegisterType<CatalogService>().As<ICatalogService>();
            container.Populate(services);

            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //var swaggerOptions = new SwaggerOptions();
            //Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);

            //app.UseSwagger(option =>
            //{
            //    option.RouteTemplate = swaggerOptions.JsonRoute;
            //});

            //app.UseSwaggerUI(option =>
            //{
            //    option.SwaggerEndpoint(swaggerOptions.UIEndpoint, swaggerOptions.Description);
            //});

            app.UseRouting();

            //app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
            });
        }

    }

    public static class CustomExtensionMethods
    {
        
        public static IServiceCollection AddCustomMVC(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            }).AddNewtonsoftJson();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            return services;
        }

        public static IServiceCollection AddIntegrationServices(this IServiceCollection services, IConfiguration configuration)
        {
            
                services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                    var factory = new ConnectionFactory()
                    {
                        HostName = configuration["EventBus:EventBusConnection"],
                        Port = int.Parse(configuration["EventBus:EventBusPort"]),
                        DispatchConsumersAsync = true
                    };

                    if (!string.IsNullOrEmpty(configuration["EventBus:EventBusUserName"]))
                    {
                        factory.UserName = configuration["EventBus:EventBusUserName"];
                    }

                    if (!string.IsNullOrEmpty(configuration["EventBus:EventBusPassword"]))
                    {
                        factory.Password = configuration["EventBus:EventBusPassword"];
                    }

                    var retryCount = 5;
                    if (!string.IsNullOrEmpty(configuration["EventBus:EventBusRetryCount"]))
                    {
                        retryCount = int.Parse(configuration["EventBus:EventBusRetryCount"]);
                    }

                    return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
                });
            

            return services;
        }

        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            var subscriptionClientName = configuration["SubscriptionClientName"];

                services.AddSingleton<IEventBus, EventBusRabbitMQQ>(sp =>
                {
                    var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQQ>>();
                    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                    var retryCount = 5;
                    if (!string.IsNullOrEmpty(configuration["EventBus:EventBusRetryCount"]))
                    {
                        retryCount = int.Parse(configuration["EventBus:EventBusRetryCount"]);
                    }

                    return new EventBusRabbitMQQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
                });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            return services;
        }
    }

}
