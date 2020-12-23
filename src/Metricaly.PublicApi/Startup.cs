using FluentValidation.AspNetCore;
using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure;
using Metricaly.Infrastructure.Interfaces;
using Metricaly.PublicApi.Filters;
using Metricaly.PublicApi.Services;
using Metricaly.PublicApi.Validators;
using Metricaly.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Metricaly.PublicApi
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
            services.AddInfrastructure(Configuration);
            services.AddControllers(options =>
                {
                    options.Filters.Add<ValidationFilter>();
                })
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CollectSingleMetricRequestValidator>());

            services.AddMemoryCache();

            services.AddSingleton<IMetricsCollectionService, MetricsCollectionService>();

            services.AddScoped<IApplicationRepository, CachedApplicationRepository>();
            services.AddScoped<IMetricRepository, CachedMetricRepository>();
            services.AddScoped<MetricRepository>();
            services.AddScoped<ApplicationRepository>();

            // Register the Swagger services
            services.AddSwaggerDocument(settings =>
            {
                settings.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Metricaly Public API";
                    document.Info.Description = "REST API for Metricaly.";
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Register the Swagger generator and the Swagger UI middlewares
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
