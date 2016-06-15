using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EFAntiXssSite.Models;

namespace EFAntiXssSite
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContext<EFAntiXssContext>(x => x.UseInMemoryDatabase());
            services.AddIdentity<IdentityUser, IdentityRole>(x =>
            {
                x.Password.RequireDigit = false;
                x.Password.RequiredLength = 0;
                x.Password.RequireLowercase = false;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequireUppercase = false;
                x.User.AllowedUserNameCharacters = null;
            })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<EFAntiXssContext>();

            services.AddAntiXss()
                .AddEntityFrameworkWhiteList<EFAntiXssContext, IdentityUser, string>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvcWithDefaultRoute();

            SampleData.InitDB(app.ApplicationServices);
        }
    }
}
