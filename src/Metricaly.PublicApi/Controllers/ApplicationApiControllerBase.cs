using Metricaly.Angular.Filters;
using Metricaly.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Metricaly.PublicApi.Controllers
{
    [ApiController]
    [ApiKeyAuth]
    public class ApplicationApiControllerBase : ControllerBase
    {
        public Application Application => GetAuthenticatedApplicationObject();

        public ApplicationApiControllerBase()
        {
        }

        private Application GetAuthenticatedApplicationObject()
        {
            HttpContext.Items.TryGetValue(ApiKeyAuthAttribute.HttpContextItemsMiddlewareKey, out var application);
            return (Application)application;
        }
    }
}
