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
        protected IActionResult AjaxPagedView<TView, TModel>(
            IEnumerable<TModel> Source,
            string ContentSelector,
            int PageSize = 50,
            AjaxPerformanceType AjaxPerformance = AjaxPerformanceType.WaterFallFlow,
            string PagerDomId = "pager-outer",
            string FormSelector = "form",
            string ViewPath = null)
            where TModel : IConvertible<TView>
        {
            if (Request.Query["raw"] == "info")
            {
                var info = Pager.GetPagerInfo(ref Source, PageSize, string.IsNullOrEmpty(Request.Query["p"]) ? 1 : Convert.ToInt32(Request.Query["p"]));
                return Json(info);
            }
            else if (Request.Query["raw"] == "true")
            {
                Pager.PlainDivide(ref Source, PageSize, string.IsNullOrEmpty(Request.Query["p"]) ? 1 : Convert.ToInt32(Request.Query["p"]));
                if (string.IsNullOrEmpty(ViewPath))
                    return View("/_" + ControllerContext.ActionDescriptor.ActionName, Source);
                else
                {
                    var last = ViewPath.LastIndexOf('/');
                    var tmp = ViewPath.Substring(0, last);
                    var tmp2 = ViewPath.Substring(last + 1, ViewPath.Length - 1 - last);
                    var ret = new List<TView>();
                    foreach (var item in Source)
                    {
                        var tmp3 = (item as IConvertible<TView>).ToType();
                        ret.Add(tmp3);
                    }
                    return View(tmp + "/_" + tmp2, ret);
                }
            }
            else
            {
                ViewData["__Performance"] = (int)AjaxPerformance;
                ViewData["__PagerDomId"] = PagerDomId;
                ViewData["__ContentSelector"] = ContentSelector;
                ViewData["__FormSelector"] = FormSelector;
                if (string.IsNullOrEmpty(ViewPath))
                    return View();
                else
                    return View(ViewPath);
            }
        }
    }
}
