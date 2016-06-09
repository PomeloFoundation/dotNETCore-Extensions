using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.AspNetCore.Localization
{
    public interface ILocalizationStringCollection
    {
        IEnumerable<CultureInfo> Collection { get; }
        string this[string identifier, params object[] objects] { get; }
        string GetString(string culture, string identifier, params object[] objects);
        void SetString(string culture, string identifier, string Content);
        void Refresh();
        void RemoveString(string Identifier);
        string SingleCulture(string[] culture);
    }
}
