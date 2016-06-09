using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Mvc
{
    public class QueryStringRequestTemplateProvider : IRequestTemplateProvider
    {
        public HttpContext HttpContext { get; }

        public string QueryField { get; }

        public QueryStringRequestTemplateProvider(IHttpContextAccessor accessor, string queryField = "template")
        {
            QueryField = queryField;
            HttpContext = accessor.HttpContext;
        }

        public string DetermineRequestTemplate()
        {
            try
            {
                return HttpContext.Request.Query[QueryField].ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}
