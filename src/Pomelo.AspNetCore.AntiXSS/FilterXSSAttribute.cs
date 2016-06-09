using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Internal;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.AntiXSS;

namespace Microsoft.AspNetCore.Mvc
{
    public class FilterXSSAttribute : ActionFilterAttribute
    {
        public FilterXSSAttribute() { }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var qs = WebUtilities.QueryHelpers.ParseQuery(context.HttpContext.Request.QueryString.ToUriComponent());
            var ret = new Http.QueryString();
            var AntiXss = context.HttpContext.RequestServices.GetRequiredService<AntiXSS>();
            foreach (var k in qs.Keys)
            {
                for (var i = 0; i < qs[k].Count(); i++)
                {
                    try
                    {
                        ret.Add(k, AntiXss.Sanitize(qs[k][i]));
                    }
                    catch
                    {
                        ret.Add(k, qs[k][i]);
                    }
                }
            }
            context.HttpContext.Request.QueryString = ret;
            base.OnActionExecuting(context);
        }
    }
}
