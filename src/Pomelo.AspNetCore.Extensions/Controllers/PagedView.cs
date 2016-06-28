using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Microsoft.AspNetCore.Mvc
{
    public abstract partial class BaseController : Controller
    {
        [NonAction]
        protected IActionResult PagedView<TModel>(
            IEnumerable<TModel> Source,
            int PageSize = 50,
            string ViewPath = null)
        {
            int? p;
            try
            {
                if (Request.Query["p"].Count > 0)
                {
                    p = int.Parse(Request.Query["p"].ToString());
                }
                else if (RouteData.Values["p"] != null)
                {
                    p = int.Parse(RouteData.Values["p"].ToString());
                }
                else
                {
                    p = 1;
                }
            }
            catch
            {
                p = 1;
            }
            ViewData["PagingInfo"] = Paging.Divide(ref Source, PageSize, p.Value);
            if (string.IsNullOrEmpty(ViewPath))
                return View(Source);
            else
                return View(ViewPath, Source);
        }
    }
}
