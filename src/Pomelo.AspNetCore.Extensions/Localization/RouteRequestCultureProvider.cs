using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Pomelo.AspNetCore.Localization
{
    public class RouteRequestCultureProvider : IRequestCultureProvider
    {
        public ActionContext ActionContext { get; set; }

        private string RouteField;

        public RouteRequestCultureProvider(IActionContextAccessor actionContextAccessor, string RouteField = "lang")
        {
            this.RouteField = RouteField;
            ActionContext = actionContextAccessor.ActionContext;
        }

        public HttpContext HttpContext { get; set; }

        public string[] DetermineRequestCulture()
        {
            return new string[] { ActionContext.RouteData.Values[RouteField].ToString() };
        }
    }
}
