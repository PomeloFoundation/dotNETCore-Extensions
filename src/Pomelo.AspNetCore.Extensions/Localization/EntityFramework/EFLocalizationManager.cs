using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Pomelo.AspNetCore.Extensions.Localization.EntityFramework
{
    public class EFLocalizationManager<TKey>
        where TKey : IEquatable<TKey>
    {
        private EFCollection<TKey> _localizationStringCollection;
        private IRequestCultureProvider _cultureProvider;
        public EFLocalizationManager(ILocalizationStringCollection collection, IRequestCultureProvider provider)
        {
            _localizationStringCollection = collection as EFCollection<TKey>;
            _cultureProvider = provider;
        }

        public void SetLocalizedString(string culture, string identifier, string text)
        {
            _localizationStringCollection.SetString(culture, identifier, text);
        }

        public virtual string this[string identifier, params object[] objects]
        {
            get
            {
                return _localizationStringCollection[identifier, objects];
            }
        }

        public void SetLocalizedString(string identifier, string text)
        {
            _localizationStringCollection.SetString(_localizationStringCollection.SingleCulture(_cultureProvider.DetermineRequestCulture()), identifier, text);
        }

        public string AddLocalizedString(string text)
        {
            var identifier = Guid.NewGuid().ToString();
            var culture = _localizationStringCollection._DbContext.LocalizationCultureInfo
                .Include(x => x._Culture)
                .Select(x => x._Culture.First().CultureString)
                .ToList();
            foreach (var x in culture)
                SetLocalizedString(x, identifier, text);
            return identifier;
        }
    }
}
