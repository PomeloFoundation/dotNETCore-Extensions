using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.AspNetCore.Localization.Extensions
{
    public static class ILocalizationStringCollectionExtensions
    {
        public static Task RefreshAsync(this ILocalizationStringCollection self)
        {
            return Task.Factory.StartNew(() => { self.Refresh(); });
        }

        public static Task RemoveStringAsync(this ILocalizationStringCollection self, string identifier)
        {
            return Task.Factory.StartNew(() => { self.RemoveString(identifier); });
        }
    }
}
