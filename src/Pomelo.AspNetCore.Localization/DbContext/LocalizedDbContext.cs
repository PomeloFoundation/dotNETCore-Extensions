using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Pomelo.AspNetCore.Localization;
using System.Threading;

namespace Microsoft.EntityFrameworkCore
{
    public class LocalizedDbContext : DbContext
    {
        public LocalizedDbContext(IServiceProvider services)
        {
            this.services = services;
        }

        public LocalizedDbContext(IServiceProvider services, DbContextOptions opt)
            : base(opt)
        {
            this.services = services;
        }

        private IServiceProvider services { get; set; }

        public virtual void OnSavingChanges()
        {
            var entities = ChangeTracker
                .Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified && x.Entity.GetType().GetProperties().Any(y => y.GetCustomAttribute<LocalizedAttribute>() != null))
                .ToList();
            var set = services.GetRequiredService<ICultureSet>();

            foreach(var x in entities)
            {
                var type = x.Entity.GetType();
                var properties = type.GetProperties().Where(y => y.GetCustomAttribute<LocalizedAttribute>() != null).ToList();
                foreach(var y in properties)
                {
                    var origin = x.Property(y.Name).OriginalValue;
                    var current = x.Property(y.Name).CurrentValue;
                    var cultureProvider = services.GetRequiredService<ICultureProvider>();
                    var culture = cultureProvider.DetermineCulture();
                    culture = set.SimplifyCulture(culture);
                    Dictionary<string, string> json;
                    if (x.State == EntityState.Added)
                        json = new Dictionary<string, string>();
                    else
                        json = JsonConvert.DeserializeObject<Dictionary<string, string>>(origin.ToString());
                    if (json.ContainsKey(culture))
                        json[culture] = current.ToString();
                    else
                        json.Add(culture, current.ToString());
                    x.Property(y.Name).CurrentValue = JsonConvert.SerializeObject(json);
                }
            }
        }
        
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnSavingChanges();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnSavingChanges();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
