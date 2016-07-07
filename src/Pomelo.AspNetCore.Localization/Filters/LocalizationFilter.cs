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
            var cache = services.GetRequiredService<ITranslatedCaching>();
            var translator = services.GetRequiredService<ITranslator>();
            var culture = services.GetRequiredService<ICultureProvider>().DetermineCulture();
            if (context.Result is ViewResult)
            {
                var result = (ViewResult)context.Result;
                if (result.Model == null)
                    return;

                if (result.Model is IEnumerable)
                {
                    var model = (IEnumerable<object>)result.Model;
                    foreach (var x in model)
                    {
                        var type = x.GetType();
                        var properties = type.GetProperties().Where(y => y.PropertyType == typeof(string) && y.GetCustomAttribute<LocalizedAttribute>() != null);
                        foreach (var y in properties)
                        {
                            try
                            {
                                var jsonStr = y.GetValue(x);
                                var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr.ToString());
                                if (json.ContainsKey(culture))
                                    y.SetValue(x, json[culture]);
                                else
                                {
                                    var key = json.Keys.FirstOrDefault();
                                    if (key == null)
                                    {
                                        y.SetValue(x, "");
                                    }
                                    else
                                    {
                                        var cachedString = cache.Get(json[key], culture);
                                        if (cachedString == null)
                                        {
                                            var translateTask = translator.TranslateAsync(key, culture, json[key]);
                                            translateTask.Wait();
                                            cache.Set(json[key], culture, translateTask.Result);
                                            y.SetValue(x, translateTask.Result);
                                        }
                                        else
                                        {
                                            y.SetValue(x, cachedString);
                                        }
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                else
                {
                    var type = result.Model.GetType();
                    var properties = type.GetProperties().Where(y => y.PropertyType == typeof(string) && y.GetCustomAttribute<LocalizedAttribute>() != null);
                    foreach (var y in properties)
                    {
                        try
                        {
                            var jsonStr = y.GetValue(result.Model);
                            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr.ToString());
                            if (json.ContainsKey(culture))
                                y.SetValue(result.Model, json[culture]);
                            else
                            {
                                var key = json.Keys.FirstOrDefault();
                                if (key == null)
                                {
                                    y.SetValue(result.Model, "");
                                }
                                else
                                {
                                    var cachedString = cache.Get(json[key], culture);
                                    if (cachedString == null)
                                    {
                                        var translateTask = translator.TranslateAsync(key, culture, json[key]);
                                        translateTask.Wait();
                                        cache.Set(json[key], culture, translateTask.Result);
                                        y.SetValue(result.Model, translateTask.Result);
                                    }
                                    else
                                    {
                                        y.SetValue(result.Model, cachedString);
                                    }
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }
    }
}
