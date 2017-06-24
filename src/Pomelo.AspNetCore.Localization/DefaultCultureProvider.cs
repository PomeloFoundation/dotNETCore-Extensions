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

        private string _DetermineCulture()
        {
            var httpContextAccessor = services.GetRequiredService<IHttpContextAccessor>();
            var actionContextAccessor = services.GetRequiredService<IActionContextAccessor>();

            try 
            {
                if (httpContextAccessor.HttpContext.Request.Cookies.ContainsKey("ASPNET_LANG"))
                    return httpContextAccessor.HttpContext.Request.Cookies["ASPNET_LANG"];
                else if (actionContextAccessor.ActionContext.RouteData.Values.ContainsKey("culture"))
                    return actionContextAccessor.ActionContext.RouteData.Values["culture"].ToString();
                else if (httpContextAccessor.HttpContext.Request.Query.ContainsKey("lang"))
                    return httpContextAccessor.HttpContext.Request.Query["lang"].ToString();
                else
                {
                    var raw = httpContextAccessor.HttpContext.Request.Headers["Accept-Language"].ToString().Split(';')[0].Split(',').Select(x => x.Trim());
                    return raw.FirstOrDefault();  
                }
            }
            catch
            {
                var raw = httpContextAccessor.HttpContext.Request.Headers["Accept-Language"].ToString().Split(';')[0].Split(',').Select(x => x.Trim());
                return raw.FirstOrDefault();
            }
        }

        public string DetermineCulture()
        {
            var culture = _DetermineCulture();
            var cs = services.GetRequiredService<ICultureSet>();
            return cs.SimplifyCulture(culture);
        }
    }
}
