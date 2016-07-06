using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TemplateExtension
    {
        public static IMvcBuilder AddMultiTemplateEngine(this IMvcBuilder self)
        {
            self.Services
                .AddContextAccessor()
                .AddSingleton<TemplateCollection>()
                .AddSingleton<IRazorViewEngine, MultiTemplateEngine>()
                .AddScoped<TemplateManager>();

            return self.AddViewOptions(x =>
            {
                foreach (var v in x.ViewEngines)
                    if (!(v is MultiTemplateEngine))
                        x.ViewEngines.Remove(v);
            });
        }

        public static IMvcBuilder AddQueryStringTemplateProvider(this IMvcBuilder self, string QueryField = "template")
        {
            self.Services.AddScoped<IRequestTemplateProvider>(x => new QueryStringRequestTemplateProvider(x, QueryField));
            return self;
        }

        public static IMvcBuilder AddCookieTemplateProvider(this IMvcBuilder self, string CookieField = "ASPNET_TEMPLATE")
        {
            self.Services.AddScoped<IRequestTemplateProvider>(x => new CookieRequestTemplateProvider(x, CookieField));
            return self;
        }
    }
}
