using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Pomelo.AspNetCore.Localization
{
    public class RouteRequestCultureProvider : IRequestCultureProvider
    {
        public ActionContext ActionContext { get; set; }

        public HttpContext HttpContext { get; set; }

        public IServiceProvider Services { get; }

        private string RouteField;

        public RouteRequestCultureProvider(IServiceProvider provider, string RouteField = "lang")
        {
            this.RouteField = RouteField;
            Services = provider;
        }

        public string[] DetermineRequestCulture()
        {
            ActionContext = Services.GetRequiredService<IActionContextAccessor>().ActionContext;
            return new string[] { ActionContext.RouteData.Values[RouteField].ToString() };
        }
    }
}
