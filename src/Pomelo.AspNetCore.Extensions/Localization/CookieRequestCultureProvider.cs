using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Pomelo.AspNetCore.Extensions.Localization
{
    public class CookieRequestCultureProvider : IRequestCultureProvider
    {
        public HttpContext HttpContext { get; set; }

        public IServiceProvider Services { get; set; }

        public string CookieField { get; set; }

        public CookieRequestCultureProvider(IServiceProvider provider, string CookieField = "ASPNET_LANG")
        {
            this.CookieField = CookieField;
            Services = provider;
        }

        public string[] DetermineRequestCulture()
        {
            HttpContext = Services.GetRequiredService<IHttpContextAccessor>().HttpContext;
            if (string.IsNullOrEmpty(HttpContext.Request.Cookies[CookieField]))
            {
                var ret = new List<string>();
                var tmp = HttpContext.Request.Headers["Accept-Language"].FirstOrDefault();
                if (tmp == null)
                    return new string[] { };
                var split = tmp.Split(',');
                foreach(var x in split)
                    ret.Add(x.Split(';')[0]);
                return ret.ToArray();
            }
            else
            {
                return new string[] { HttpContext.Request.Cookies[CookieField] };
            }
        }
    }
}
