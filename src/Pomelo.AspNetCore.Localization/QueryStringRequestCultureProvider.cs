using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Pomelo.AspNetCore.Localization
{
    public class QueryStringRequestCultureProvider : IRequestCultureProvider
    {
        public string QueryField { get; set; }

        public IServiceProvider Services { get; }

        public QueryStringRequestCultureProvider(IServiceProvider provider, string QueryField = "lang")
        {
            this.QueryField = QueryField;
            Services = provider;
        }

        public HttpContext HttpContext { get; set; }

        public string[] DetermineRequestCulture()
        {
            HttpContext = Services.GetRequiredService<IHttpContextAccessor>().HttpContext;
            return HttpContext.Request.Query[QueryField];
        }
    }
}
