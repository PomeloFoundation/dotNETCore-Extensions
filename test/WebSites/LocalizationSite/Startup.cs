using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Pomelo.AspNetCore.Localization;
using LocalizationSite.Models;

namespace LocalizationSite
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddDynamicLocalizer();

            services.AddDbContext<TestContext>(x =>x.UseInMemoryDatabase());

            services.AddPomeloLocalization(x =>
            {
                x.AddCulture(new[] { "zh", "zh-CN", "zh-Hans" }, new JsonLocalizedStringStore(Path.Combine("Localization", "zh-CN.json")));
                x.AddCulture(new[] { "en-US" }, new JsonLocalizedStringStore(Path.Combine("Localization", "en-US.json")));
            })
                .AddBaiduTranslator();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            app.UseDeveloperExceptionPage();
            app.UseMvcWithDefaultRoute();
            app.UseFrontendLocalizer();

            var DB = app.ApplicationServices.GetRequiredService<TestContext>();
            DB.Database.EnsureCreated();
        }
    }
}
