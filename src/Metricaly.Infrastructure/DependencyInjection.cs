using AutoMapper;
using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Common.Models;
using Metricaly.Infrastructure.Data;
using Metricaly.Infrastructure.Identity;
using Metricaly.Infrastructure.Services;
using Metricaly.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Reflection;
using System.Text;

namespace Metricaly.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration conf)
        {
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(conf["Redis:Host"]));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(conf.GetConnectionString("ApplicationConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                  .AddEntityFrameworkStores<ApplicationDbContext>()
                  .AddDefaultTokenProviders();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddScoped<ITokenClaimsService, IdentityTokenClaimService>();
            services.AddScoped<IApiKeyGenerator, ApiKeyGenerator>();
            services.AddScoped<ICreateApplicationService, CreateApplicationService>();

            services.AddSingleton<IMetricsRetriever, RedisMetricsRetriever>();
            services.AddSingleton<IMetricDownSampler, MetricDownSampler>();

            services.Configure<JwtSettings>(conf.GetSection("Jwt"));
            services.Configure<RedisSettings>(conf.GetSection("Redis"));

            return services;
        }
    }
}
