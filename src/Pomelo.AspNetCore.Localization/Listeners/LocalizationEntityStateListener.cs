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

        public LocalizationEntityStateListener(IServiceProvider services)
        {
            this.services = services;
        }

        public void StateChanging(InternalEntityEntry entry, EntityState newState)
        {
            if (newState != EntityState.Added && newState != EntityState.Modified)
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
                    Dictionary<string, string> json;
                    if (newState == EntityState.Added)
                        json = new Dictionary<string, string>();
                    else
                        json = JsonConvert.DeserializeObject<Dictionary<string, string>>(origin.ToString());
                    if (json.ContainsKey(culture))
                        json[culture] = current.ToString();
                    else
                        json.Add(culture, current.ToString());
                    entry.SetProperty(entry.EntityType.FindProperty(y.Name), JsonConvert.SerializeObject(json));
                }
                catch
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
