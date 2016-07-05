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
        public static string Localize(this IHtmlHelper self, string localizedString)
        {
            try
            {
                var services = self.ViewContext.HttpContext.RequestServices;
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
                        return localizedString;
                    return json.First().Value;
                }
            }
            catch
            {
                return localizedString;
            }
        }
    }
}
