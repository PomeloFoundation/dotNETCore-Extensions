using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pomelo.AspNetCore.Extensions.Localization.EntityFramework;

namespace Microsoft.EntityFrameworkCore
{
    public static class ModelBuilderExtensions
    {
        public static void BuildLocalization<TKey>(this ModelBuilder self)
            where TKey : IEquatable<TKey>
        {
            self.Entity<CultureInfo<TKey>>(e =>
            {
                e.HasIndex(x => x.IsDefault);
                e.HasIndex(x => x.Set);
            });
        }
    }
}
