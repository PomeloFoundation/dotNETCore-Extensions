using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Pomelo.AspNetCore.Localization.Filters
{
    public class LocalizationFilter : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            var services = context.HttpContext.RequestServices;
            var disabler = services.GetRequiredService<ITranslatorDisabler>();
            var cache = services.GetRequiredService<ITranslatedCaching>();
            var translator = services.GetRequiredService<ITranslator>();
            var culture = services.GetRequiredService<ICultureProvider>().DetermineCulture();
            if (context.Result is ViewResult)
            {
                var result = (ViewResult)context.Result;
                var visited = new List<int>();
                HandleLocalization(result.Model, disabler, cache, translator, culture, context, visited);
                foreach (var key in result.ViewData.Keys.ToList())
                {
                    result.ViewData[key] = HandleLocalization(result.ViewData[key], disabler, cache, translator, culture, context, visited);
                }
            }
        }

        private object HandleLocalization(object src, ITranslatorDisabler disabler, ITranslatedCaching cache, ITranslator translator, string culture, ResultExecutingContext context, List<int> visited)
        {
            if (src == null)
                return null;
            if (src.GetType().GetTypeInfo().BaseType == typeof(ValueType))
                return src;
            if (src is IDictionary)
                return src;
            if (src is string)
                return src;
            if (src is IEnumerable)
            {
                var model = src is IList ? src : ((dynamic)src).ToList();
                if (!visited.Any(x => x == src.GetHashCode()))
                {
                    for (var i = 0; i < model.Count; i++)
                    {
                        var tmp = model[i];
                        var hc = (int)model[i].GetHashCode();
                        if (!visited.Any(x => x == hc))
                        {
                            HandleLocalizationForObject(ref tmp, disabler, cache, translator, culture, context, visited);
                            model[i] = tmp;
                        }
                    }
                    return model;
                }
                else
                {
                    return model;
                }
            }
            else
            {
                if (!visited.Any(x => x == src.GetHashCode()))
                {
                    visited.Add(src.GetHashCode());
                    return HandleLocalizationForObject(ref src, disabler, cache, translator, culture, context, visited);
                }
                else
                {
                    return src;
                }
            }
        }

        private object HandleLocalizationForObject(ref object src, ITranslatorDisabler disabler, ITranslatedCaching cache, ITranslator translator, string culture, ResultExecutingContext context, List<int> visited)
        {
            if (src == null)
                return null;
            if (src.GetType().GetTypeInfo().BaseType == typeof(ValueType))
                return src;
            var type = src.GetType().GetTypeInfo();
            var properties = type.DeclaredProperties;
            foreach (var y in properties)
            {
                if (y.PropertyType.GetTypeInfo().BaseType == typeof(ValueType))
                {
                    continue;
                }
                else if (y.PropertyType == typeof(string))
                {
                    if (y.GetCustomAttribute<LocalizedAttribute>() != null)
                    {
                        var jsonStr = y.GetValue(src);
                        var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr.ToString());
                        if (json.ContainsKey(culture))
                            y.SetValue(src, json[culture]);
                        else
                        {
                            var key = json.Keys.FirstOrDefault();
                            if (key == null)
                            {
                                y.SetValue(src, "");
                            }
                            else
                            {
                                if (disabler.IsDisabled())
                                    y.SetValue(src, json[key]);
                                (context.Result as ViewResult).ViewData["__IsTranslated"] = true;
                                var cachedString = cache.Get(json[key], culture);
                                if (cachedString == null)
                                {
                                    var translateTask = translator.TranslateAsync(key, culture, json[key]);
                                    translateTask.Wait();
                                    cache.Set(json[key], culture, translateTask.Result);
                                    y.SetValue(src, translateTask.Result);
                                }
                                else
                                {
                                    y.SetValue(src, cachedString);
                                }
                            }
                        }
                    }
                }
                else
                {
                    HandleLocalization(y.GetValue(src), disabler, cache, translator, culture, context, visited);
                }
            }
            return src;
        }
    }
}
