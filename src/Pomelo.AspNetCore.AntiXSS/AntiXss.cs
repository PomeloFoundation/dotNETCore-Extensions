using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.AntiXSS;

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
            return self.AddSingleton<IWhiteListProvider>(new DefaultWhiteListProvider())
                .AddSingleton<AntiXSS>();
        }
    }
}
