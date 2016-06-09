using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Pomelo.AspNetCore.Localization.EntityFramework
{
    public class LocalizationDbContext : LocalizationDbContext<Guid>
    {
    }

    public class LocalizationDbContext<TKey> : DbContext, ILocalizationDbContext<TKey>
        where TKey : IEquatable<TKey>
    {
        public DbSet<CultureInfo<TKey>> LocalizationCultureInfo { get; set; }
        public DbSet<Culture<TKey>> LocalizationCulture { get; set; }
        public DbSet<LocalizedString<TKey>> LocalizationString { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.BuildLocalization<TKey>();
        }
    }
}
