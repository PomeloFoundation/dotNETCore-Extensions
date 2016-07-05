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
            var services = context.HttpContext.RequestServices;
            var culture = services.GetRequiredService<ICultureProvider>().DetermineCulture();
            if (context.Result is ViewResult)
            {
                var result = (ViewResult)context.Result;
                if (result.Model is IEnumerable)
                {
                    var model = (IEnumerable<object>)result.Model;
                    foreach(var x in model)
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
                                    y.SetValue(x, key == null ? "" : json[key]);
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
                                y.SetValue(result.Model, key == null ? "" : json[key]);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
        }
    }
}
