using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Newtonsoft.Json;

namespace Pomelo.AspNetCore.Localization.Listeners
{
    public class LocalizationEntityStateListener : IEntityStateListener
    {
        private IServiceProvider services { get; set; }
        private static MethodInfo writePropertyValue = typeof(InternalEntityEntry).GetTypeInfo().DeclaredMethods.Single(x => x.Name == "WritePropertyValue");

        public LocalizationEntityStateListener(IServiceProvider services)
        {
            this.services = services;
        }

        public void StateChanging(InternalEntityEntry entry, EntityState newState)
        {
            if (!(newState == EntityState.Added && entry.EntityState == EntityState.Detached || newState == EntityState.Modified && entry.EntityState == EntityState.Unchanged))
                return;

            var set = services.GetRequiredService<ICultureSet>();
            var type = entry.Entity.GetType();
            var properties = type.GetProperties().Where(y => y.GetCustomAttribute<LocalizedAttribute>() != null).ToList();

            foreach (var y in properties)
            {
                try
                {
                    var origin = entry.GetOriginalValue(entry.EntityType.FindProperty(y.Name));
                    var current = entry.GetCurrentValue(entry.EntityType.FindProperty(y.Name));
                    var cultureProvider = services.GetRequiredService<ICultureProvider>();
                    var culture = cultureProvider.DetermineCulture();
                    culture = set.SimplifyCulture(culture);
                    Dictionary<string, string> dic;
                    if (newState == EntityState.Added)
                        dic = new Dictionary<string, string>();
                    else
                        dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(origin.ToString());
                    if (dic.ContainsKey(culture))
                        dic[culture] = current.ToString();
                    else
                        dic.Add(culture, current.ToString());
                    var json = JsonConvert.SerializeObject(dic);
                    writePropertyValue.Invoke(entry, new object[] { entry.EntityType.FindProperty(y.Name), json });
                }
                catch (Exception ex)
                {
                    break;
                }
            }
        }

        public void StateChanged(InternalEntityEntry entry, EntityState oldState, bool fromQuery)
        {
        }
    }
}
