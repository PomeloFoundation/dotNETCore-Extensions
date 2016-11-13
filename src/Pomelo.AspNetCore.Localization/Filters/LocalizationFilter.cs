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
                foreach(var vd in result.ViewData)
                {
                    if (vd.Value is IEnumerable)
                    {
                        var model = (IEnumerable<object>)vd.Value;
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
                                            if (disabler.IsDisabled())
                                                y.SetValue(x, json[key]);
                                            (context.Result as ViewResult).ViewData["__IsTranslated"] = true;
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
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                }
                            }
                        }
                    }
                    else
                    {
                        var type = vd.Value.GetType();
                        var properties = type.GetProperties().Where(y => y.PropertyType == typeof(string) && y.GetCustomAttribute<LocalizedAttribute>() != null);
                        foreach (var y in properties)
                        {
                            try
                            {
                                var jsonStr = y.GetValue(vd.Value);
                                var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr.ToString());
                                if (json.ContainsKey(culture))
                                    y.SetValue(vd.Value, json[culture]);
                                else
                                {
                                    var key = json.Keys.FirstOrDefault();
                                    if (key == null)
                                    {
                                        y.SetValue(vd.Value, "");
                                    }
                                    else
                                    {
                                        var cachedString = cache.Get(json[key], culture);
                                        if (cachedString == null)
                                        {
                                            var translateTask = translator.TranslateAsync(key, culture, json[key]);
                                            translateTask.Wait();
                                            cache.Set(json[key], culture, translateTask.Result);
                                            y.SetValue(vd.Value, translateTask.Result);
                                        }
                                        else
                                        {
                                            y.SetValue(vd.Value, cachedString);
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
                if (result.Model != null)
                {
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
                                            if (disabler.IsDisabled())
                                                y.SetValue(x, json[key]);
                                            (context.Result as ViewResult).ViewData["__IsTranslated"] = true;
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
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
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
}
