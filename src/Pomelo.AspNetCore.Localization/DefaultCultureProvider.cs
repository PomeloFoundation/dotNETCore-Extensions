using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Pomelo.AspNetCore.Localization
{
    public class DefaultCultureProvider : ICultureProvider
    {
        private IServiceProvider services { get; set; }

        public DefaultCultureProvider(IServiceProvider services)
        {
            this.services = services;
        }

        public string DetermineCulture()
        {
            var httpContext = services.GetRequiredService<IHttpContextAccessor>().HttpContext;
            var actionContext = services.GetRequiredService<IActionContextAccessor>().ActionContext;
            if (httpContext.Request.Cookies.ContainsKey("ASPNET_LANG"))
            {
                return httpContext.Request.Cookies["ASPNET_LANG"];
            }
            else if (actionContext.RouteData.Values.ContainsKey("culture"))
            {
                return actionContext.RouteData.Values["culture"].ToString();
            }
            else if (httpContext.Request.Query.ContainsKey("lang"))
            {
                return httpContext.Request.Query["lang"];
            }
            else
            {
                return httpContext.Request.Headers["Accept-Language"].FirstOrDefault();
            }
        }
    }
}
