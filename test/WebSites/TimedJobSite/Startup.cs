using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TimedJobSite.Models;

namespace TimedJobSite
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TimedJobSiteContext>(x => x.UseInMemoryDatabase());

            services.AddTimedJob()
                .AddEntityFrameworkDynamicTimedJob<TimedJobSiteContext>();
        }
        
        public void Configure(IApplicationBuilder app)
        {
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });

            app.UseTimedJob();
            SampleData.InitDB(app.ApplicationServices);
        }
    }
}
