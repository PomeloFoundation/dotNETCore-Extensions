using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Pomelo.AspNetCore.Localization.Filters
{
    public class DbContextModelBindingFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var pending = context.ActionArguments.Where(x => x.Value is DbContext).Select(x => x.Value as DbContext);
            foreach(var x in pending)
            {
                var type = typeof(DbContext);
                var field = type.GetRuntimeFields().Single(y => y.Name == "_options");
                var opt = new DbContextOptionsBuilder((DbContextOptions)field.GetValue(x));
                opt.UseInternalServiceProvider(context.HttpContext.RequestServices);
                field.SetValue(x, opt.Options);
            }
        }
    }
}
