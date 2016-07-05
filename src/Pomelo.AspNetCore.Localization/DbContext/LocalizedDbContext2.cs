using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Pomelo.AspNetCore.Localization;
using System.Threading;

namespace Microsoft.AspNetCore.Identity.EntityFrameworkCore
{
    public class LocalizedDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TUser : IdentityUser<TKey, TUserClaim, TUserRole, TUserLogin>
        where TRole : IdentityRole<TKey, TUserRole, TRoleClaim>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>
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
                var type = x.GetType();
                var properties = type.GetProperties().Where(y => y.PropertyType == typeof(string) && y.GetCustomAttribute<LocalizedAttribute>() != null);
                foreach(var y in properties)
                {
                    var origin = x.Property(y.Name).OriginalValue;
                    var current = x.Property(y.Name).CurrentValue;
                    var cultureProvider = services.GetRequiredService<ICultureProvider>();
                    var culture = cultureProvider.DetermineCulture();
                    culture = set.SimplifyCulture(culture);
                    var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(origin.ToString());
                    if (json.ContainsKey(culture))
                        json[culture] = current.ToString();
                    else
                        json.Add(culture, current.ToString());
                    y.SetValue(x.Entity, JsonConvert.SerializeObject(json));
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

    public class LocalizedDbContext<TUser, TRole, TKey> : LocalizedDbContext<TUser, TRole, TKey, IdentityUserClaim<TKey>, IdentityUserRole<TKey>, IdentityUserLogin<TKey>, IdentityRoleClaim<TKey>, IdentityUserToken<TKey>>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        public LocalizedDbContext(IServiceProvider services)
            : base(services)
        {
        }

        public LocalizedDbContext(IServiceProvider services, DbContextOptions opt)
            : base(services, opt)
        {
        }
    }

    public class LocalizedDbContext<TUser> : LocalizedDbContext<TUser, IdentityRole, string>
        where TUser : IdentityUser
    {
        public LocalizedDbContext(IServiceProvider services)
            : base(services)
        {
        }

        public LocalizedDbContext(IServiceProvider services, DbContextOptions opt)
            : base(services, opt)
        {
        }
    }

    public class LocalizedDbContext : LocalizedDbContext<IdentityUser>
    {
        public LocalizedDbContext(IServiceProvider services)
            : base(services)
        {
        }

        public LocalizedDbContext(IServiceProvider services, DbContextOptions opt)
            : base(services, opt)
        {
        }
    }
}
