using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace MultiTemplateSite.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Change(string id)
        {
            Cookies["ASPNET_TEMPLATE"] = id;
            return Redirect("/");
        }
    }
}
