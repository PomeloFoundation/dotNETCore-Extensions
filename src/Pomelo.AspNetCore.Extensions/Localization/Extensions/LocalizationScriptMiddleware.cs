using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Pomelo.AspNetCore.Extensions.Localization;

namespace Microsoft.AspNetCore.Builder
{
    public static class LocalizationScriptMiddleware
    {
        public static IApplicationBuilder UseJavascriptLocalization(this IApplicationBuilder self, string scriptUrl = "/scripts/localization.js")
        {
            var SR = self.ApplicationServices.GetService<ILocalizationStringCollection>();
            var CultureProvider = self.ApplicationServices.GetService<IRequestCultureProvider>();
            var culture = SR.Collection.Where(x => x.Culture.Contains(SR.SingleCulture(CultureProvider.DetermineRequestCulture()))).First();
            var json = JsonConvert.SerializeObject(culture.LocalizedStrings);
            return self.Map(scriptUrl, config => 
            {
                var js = new StringBuilder("var __dictionary = {};");
                config.Run(async context => 
                {
                    js.AppendLine("__dictionary = " + json + ";");
                    js.AppendLine(@"
function __replaceAll(str0, str1, str2)
{
	return str0.replace(new RegExp(str1, 'gm'), str2);
}

function SR(key, params)
{
	if (!params)
		return __dictionary[key] || key;
	else
	{
		var ret = __dictionary[key] || key;
		for (var i = 0; i < params.length; i++)
			ret = __replaceAll(ret, '\\{' + i + '\\}', params[i]);
		return ret;
	}
}");
                    await context.Response.WriteAsync(js.ToString());
                    return;
                });
            });
        }
    }
}
