using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Pomelo.AspNetCore.Localization.EntityFramework
{
    public interface ILocalizationDbContext<TKey>
        where TKey : IEquatable<TKey>
    {
        DbSet<CultureInfo<TKey>> LocalizationCultureInfo { get; set; }
        DbSet<Culture<TKey>> LocalizationCulture { get; set; }
        DbSet<LocalizedString<TKey>> LocalizationString { get; set; }
        int SaveChanges();
        DatabaseFacade Database { get; }
    }
}
