using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc
{
    public class QueryStringRequestTemplateProvider : IRequestTemplateProvider
    {
        public HttpContext HttpContext { get; set; }

        public string QueryField { get; }

        public IServiceProvider Services { get; }

        public QueryStringRequestTemplateProvider(IServiceProvider provider, string queryField = "template")
        {
            QueryField = queryField;
            Services = provider;
        }

        public string DetermineRequestTemplate()
        {
            try
            {
                HttpContext = Services.GetRequiredService<IHttpContextAccessor>().HttpContext;
                return HttpContext.Request.Query[QueryField].ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}
