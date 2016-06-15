using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.AspNetCore.AntiXSS.EntityFramework;

namespace EFAntiXssSite.Models
{
    public static class SampleData
    {
        public static async void InitDB(IServiceProvider services)
        {
            var DB = services.GetRequiredService<EFAntiXssContext>();
            var RoleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = services.GetRequiredService<UserManager<IdentityUser>>();

            DB.WhiteListTags.Add(new WhiteListTag { Id = "div" });
            DB.WhiteListAttributes.Add(new WhiteListAttribute { TagId = "div", Attribute = "class", RoleRequired = "Root, Master" });
            DB.WhiteListAttributes.Add(new WhiteListAttribute { TagId = "div", Attribute = "style", RoleRequired = "Root, Master" });
            DB.SaveChanges();

            await RoleManager.CreateAsync(new IdentityRole("Root"));
            var user = new IdentityUser { Id = "00000000-0000-0000-0000-000000000000", UserName = "admin" };
            await UserManager.CreateAsync(user, "123456");
            await UserManager.AddToRoleAsync(user, "Root");
        }
    }
}
