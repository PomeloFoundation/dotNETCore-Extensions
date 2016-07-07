using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Pomelo.AspNetCore.Localization.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static string Culture(this IHtmlHelper self)
        {
            var services = self.ViewContext.HttpContext.RequestServices;
            return services.GetRequiredService<ICultureProvider>().DetermineCulture();
        }

        public static string Localize(this IHtmlHelper self, string localizedString)
        {
            try
            {
                var services = self.ViewContext.HttpContext.RequestServices;
                var cache = services.GetRequiredService<ITranslatedCaching>();
                var translator = services.GetRequiredService<ITranslator>();
                var culture = services.GetRequiredService<ICultureProvider>().DetermineCulture();
                var json = JsonConvert.DeserializeObject<IDictionary<string, string>>(localizedString);
                if (json.ContainsKey(culture))
                {
                    return json[culture];
                }
                else
                {
                    var key = json.Keys.FirstOrDefault();
                    if (key == null)
                    {
                        return localizedString;
                    }
                    else
                    {
                        var cachedString = cache.Get(json[key], culture);
                        if (cachedString == null)
                        {
                            var translateTask = translator.TranslateAsync(key, culture, json[key]);
                            translateTask.Wait();
                            cache.Set(json[key], culture, translateTask.Result);
                            return translateTask.Result;
                        }
                        else
                        {
                            return cachedString;
                        }
                    }
                }
            }
            catch
            {
                return localizedString;
            }
        }

        public static string Translate(this IHtmlHelper self, string src)
        {
            var services = self.ViewContext.HttpContext.RequestServices;
            var cache = services.GetRequiredService<ITranslatedCaching>();
            var translator = services.GetRequiredService<ITranslator>();
            var culture = services.GetRequiredService<ICultureProvider>().DetermineCulture();
            var ret = cache.Get(src, culture);
            if (ret == null)
            {
                var task = translator.TranslateAsync("", culture, src);
                task.Wait();
                cache.Set(src, culture, src);
                return task.Result;
            }
            else
            {
                return ret;
            }
        }
    }
}
