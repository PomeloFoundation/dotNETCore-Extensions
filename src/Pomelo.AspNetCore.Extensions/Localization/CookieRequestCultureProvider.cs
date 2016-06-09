using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Pomelo.AspNetCore.Extensions.Localization
{
    public class CookieRequestCultureProvider : IRequestCultureProvider
    {
        public HttpContext HttpContext { get; set; }

        public string CookieField { get; set; }

        public CookieRequestCultureProvider(IHttpContextAccessor accessor, string CookieField = "ASPNET_LANG")
        {
            this.CookieField = CookieField;
            HttpContext = accessor.HttpContext;
        }

        public string[] DetermineRequestCulture()
        {
            if (HttpContext.Request.Cookies == null || string.IsNullOrEmpty(HttpContext.Request.Cookies[CookieField]))
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
