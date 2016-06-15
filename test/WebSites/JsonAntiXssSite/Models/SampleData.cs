using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JsonAntiXssSite.Models
{
    public static class SampleData
    {
        public static async void InitDB(IServiceProvider services)
        {
            var DB = services.GetRequiredService<JsonAntiXssContext>();
            var RoleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = services.GetRequiredService<UserManager<IdentityUser>>();
            await RoleManager.CreateAsync(new IdentityRole("Root"));
            var user = new IdentityUser { Id = "00000000-0000-0000-0000-000000000000", UserName = "admin" };
            await UserManager.CreateAsync(user, "123456");
            await UserManager.AddToRoleAsync(user, "Root");
        }
    }
}
