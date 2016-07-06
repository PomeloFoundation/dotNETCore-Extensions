using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Pomelo.AspNetCore.Localization;

namespace Microsoft.AspNetCore.Builder
{
    public static class FrontendLocalizer
    {
        public static IApplicationBuilder UseFrontendLocalizer(this IApplicationBuilder self, string scriptUrl = "/scripts/localizer.js")
        {
            return self.Map(scriptUrl, config =>
            {
                config.Run(async context =>
                {
                    var cultureProvider = self.ApplicationServices.GetRequiredService<ICultureProvider>();
                    var cultureSet = self.ApplicationServices.GetRequiredService<ICultureSet>();
                    var strings = cultureSet.GetLocalizedStrings(cultureProvider.DetermineCulture());
                    var json = JsonConvert.SerializeObject(strings);

                    var js = new StringBuilder("var __dictionary = {};");
                    js.AppendLine("__dictionary = " + json + ";");
                    js.AppendLine(@"
function __replaceAll(str0, str1, str2)
{
	return str0.replace(new RegExp(str1, 'gm'), str2);
}
function SR()
{
    var key = arguments[0];
	if (arguments.length == 1)
		return __dictionary[key] || key;
	else
	{
		var ret = __dictionary[key] || key;
		for (var i = 0; i < params.length; i++)
			ret = __replaceAll(ret, '\\{' + i + '\\}', arguments[i + 1]);
		return ret;
	}
}");
                    context.Response.Headers["Cache-Control"] = $"max-age={ 60 * 24 * 7 }";
                    await context.Response.WriteAsync(js.ToString());
                    return;
                });
            });
        }
    }
}
