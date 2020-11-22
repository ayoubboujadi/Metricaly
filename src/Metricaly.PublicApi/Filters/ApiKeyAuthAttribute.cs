using Metricaly.Infrastructure.Data;
using Metricaly.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Metricaly.Angular.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthAttribute : Attribute, IAsyncAuthorizationFilter //, IActionFilter
    {
        public static readonly object HttpContextItemsMiddlewareKey = new Object();

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(!context.HttpContext.Request.Headers.TryGetValue("ApiKey", out var apiKey) || string.IsNullOrWhiteSpace(apiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var applicationDbContext = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();

            var application = await applicationDbContext.Applications
                .FirstOrDefaultAsync(a => a.ApiKey.Equals(apiKey));

            if(application == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("ApiKey", out var apiKey) || string.IsNullOrWhiteSpace(apiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var applicationRepository = context.HttpContext.RequestServices.GetRequiredService<IApplicationRepository>();

            var application = await applicationRepository.GetByApiKeyAsync(apiKey);

            if (application == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            context.HttpContext.Items[HttpContextItemsMiddlewareKey] = application;
        }

    }
}
