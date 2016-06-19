using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.Net.Smtp;

namespace SmtpSite
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSmtpEmailSender("smtp.exmail.qq.com", 25, "Mano Cloud", "noreply@mano.cloud", "noreply@mano.cloud", "ManoCloud123456");
        }

        public void Configure(IApplicationBuilder app)
        {
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });

            var Env = app.ApplicationServices.GetRequiredService<IHostingEnvironment>();
            var Email = app.ApplicationServices.GetRequiredService<IEmailSender>();
            var atta = new Attachment { FileName = "Avatar.png", File = System.IO.File.ReadAllBytes(System.IO.Path.Combine(Env.ContentRootPath, "avatar.png")) };
            Email.SendEmailAsync("1@1234.sh", null, "911574351@qq.com, 18903616070@189.cn", "测试不带有附件的电子邮件发送", "<h2>Email testing without attachment</h2>");
            Email.SendEmailAsync("1@1234.sh", null, "911574351@qq.com, 18903616070@189.cn", "测试带有附件的电子邮件发送", "<h2>Email testing with attachment</h2>", atta);
        }
    }
}
