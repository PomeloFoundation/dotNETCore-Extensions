using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.AntiXSS;
using Pomelo.AspNetCore.AntiXSS.EntityFramework;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class AntiXssExtension
    {
        /// <summary>
        /// 过滤Html代码，仅保留白名单中的标签与属性。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="Html">欲被过滤的Html代码</param>
        /// <returns></returns>
        public static HtmlString Sanitize(this IHtmlHelper self, string Html)
        {
            var AntiXSS = self.ViewContext.HttpContext.RequestServices.GetService<AntiXSS>();
            return new HtmlString(AntiXSS.Sanitize(Html));
        }
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AntiXssExtension
    {
        public static IServiceCollection AddAntiXss(this IServiceCollection self)
        {
            return self.AddScoped<IWhiteListProvider, DefaultWhiteListProvider>()
                .AddScoped<ITagAuthorizationProvider, DefaultTagAuthorizationProvider>()
                .AddSingleton<AntiXSS>();
        }

        public static IServiceCollection AddEntityFrameworkWhiteList<TContext, TUser, TKey>(this IServiceCollection self)
            where TContext : IAntiXSSDbContext
            where TUser : IdentityUserRole<TKey>
            where TKey : IEquatable<TKey>

        {
            return self.AddScoped<IWhiteListProvider, EntityFrameworkWhiteListProvider<TContext>>()
                .AddScoped<ITagAuthorizationProvider, EntityFrameworkTagAuthorizationProvider<TContext, TUser, TKey>>()
                .AddSingleton<AntiXSS>();
        }

        public static IServiceCollection AddEntityFrameworkWhiteList<TContext>(this IServiceCollection self)
            where TContext : IAntiXSSDbContext
        {
            return self.AddScoped<IWhiteListProvider, EntityFrameworkWhiteListProvider<TContext>>()
                .AddScoped<ITagAuthorizationProvider, DefaultTagAuthorizationProvider>()
                .AddSingleton<AntiXSS>();
        }
    }
}
