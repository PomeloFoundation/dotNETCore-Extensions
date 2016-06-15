using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.AntiXSS;
using Pomelo.AspNetCore.AntiXSS.Json;
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
        public static HtmlString Sanitize(this IHtmlHelper self, string Html, object UserId = null)
        {
            var AntiXSS = self.ViewContext.HttpContext.RequestServices.GetService<AntiXSS>();
            return new HtmlString(AntiXSS.Sanitize(Html, UserId));
        }

        /// <summary>
        /// 过滤Html代码，仅保留白名单中的标签与属性。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="Html">欲被过滤的Html代码</param>
        /// <returns></returns>
        public static HtmlString Sanitize(this IHtmlHelper self, HtmlString Html, object UserId = null)
        {
            return self.Sanitize(Html.ToString(), UserId);
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

        public static IServiceCollection AddEntityFrameworkWhiteList<TContext, TUser, TKey>(this IServiceCollection self, string ClaimType = "AntiXSS WhiteList")
            where TContext : IAntiXSSDbContext
            where TUser : IdentityUser<TKey>
            where TKey : IEquatable<TKey>

        {
            return self.AddHttpContextAccessor()
                .AddScoped<IWhiteListProvider, EntityFrameworkWhiteListProvider<TContext>>()
                .AddScoped<ITagAuthorizationProvider, EntityFrameworkTagAuthorizationProvider<TContext, TUser, TKey>>(x => new EntityFrameworkTagAuthorizationProvider<TContext, TUser, TKey>(x, ClaimType))
                .AddSingleton<AntiXSS>();
        }

        public static IServiceCollection AddEntityFrameworkWhiteList<TContext>(this IServiceCollection self)
            where TContext : IAntiXSSDbContext
        {
            return self.AddScoped<IWhiteListProvider, EntityFrameworkWhiteListProvider<TContext>>()
                .AddScoped<ITagAuthorizationProvider, DefaultTagAuthorizationProvider>()
                .AddSingleton<AntiXSS>();
        }

        public static IServiceCollection AddJsonWhiteList<TUser, TKey>(this IServiceCollection self, string RelativePath = "xss.json", string ClaimType = "AntiXSS WhiteList")
            where TUser : IdentityUser<TKey>
            where TKey : IEquatable<TKey>

        {
            return self.AddHttpContextAccessor()
                .AddSingleton<IWhiteListProvider, JsonWhiteListProvider>()
                .AddScoped<ITagAuthorizationProvider, JsonTagAuthorizationProvider<TUser, TKey>>(x => new JsonTagAuthorizationProvider<TUser, TKey>(x.GetRequiredService<IWhiteListProvider>(), x, ClaimType))
                .AddSingleton<AntiXSS>();
        }

        public static IServiceCollection AddJsonWhiteList(this IServiceCollection self)
        {
            return self.AddSingleton<IWhiteListProvider, JsonWhiteListProvider>()
                .AddScoped<ITagAuthorizationProvider, DefaultTagAuthorizationProvider>()
                .AddSingleton<AntiXSS>();
        }
    }
}
