using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.PlatformAbstractions;

namespace Microsoft.AspNetCore.Mvc
{
    public abstract partial class BaseController : Controller
    {
        public string AppRoot { get { return HttpContext.RequestServices?.GetService<ApplicationEnvironment>().ApplicationBasePath; } }

        public string WebRoot { get { return HttpContext.RequestServices?.GetService<IHostingEnvironment>().WebRootPath; } }

        public IConfiguration Configuration { get { return HttpContext.RequestServices?.GetService<IConfiguration>(); } }
        
        public Http.SmartCookies Cookies { get { return HttpContext.RequestServices?.GetService<Http.SmartCookies>(); } }
        
        public TemplateManager TemplateManager { get { return HttpContext.RequestServices?.GetService<TemplateManager>(); } }

        public Pomelo.AspNetCore.Extensions.Others.Marked Marked { get; set; } = new Pomelo.AspNetCore.Extensions.Others.Marked();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Prepare();
            base.OnActionExecuting(context);
        }

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Prepare();
            return base.OnActionExecutionAsync(context, next);
        }

        public virtual void Prepare()
        {
        }
    }
}
