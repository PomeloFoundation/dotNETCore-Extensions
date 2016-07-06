using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LocalizationSite.Models;

namespace LocalizationSite.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index([FromServices]TestContext DB)
        {
            var ret = DB.Tests.ToList();
            return View(ret);
        }

        public IActionResult Insert(string MultiLangContent, [FromServices]TestContext DB)
        {
            DB.Tests.Add(new Test { MultiLangContent = MultiLangContent });
            DB.SaveChanges();
            return Redirect("/");
        }

        public IActionResult Lang(string id)
        {
            Response.Cookies.Append("ASPNET_LANG", id);
            return Redirect("/");
        }
    }
}
