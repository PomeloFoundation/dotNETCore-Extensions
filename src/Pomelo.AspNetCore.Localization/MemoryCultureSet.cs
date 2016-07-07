using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.AspNetCore.Localization
{
    public class MemoryCultureSet : ICultureSet
    {
        public ILocalizedStringStore Default { get; set; }

        public string DefaultCulture { get; set; }

        private IDictionary<IList<string>, ILocalizedStringStore> dic { get; set; } = new Dictionary<IList<string>, ILocalizedStringStore>();

        public void AddCulture(string[] Cultures, ILocalizedStringStore Strings, bool IsDefault = false)
        {
            if (IsDefault || dic.Count == 0)
            {
                Default = Strings;
                DefaultCulture = Cultures.First();
            }

            if (!dic.ContainsKey(Cultures.ToList()))
            {
                dic.Add(Cultures, Strings);
            }
            else
            {
                dic[Cultures] = Strings;
            }
        }

        public ILocalizedStringStore GetLocalizedStrings(string Culture)
        {
            if (dic.Where(x => x.Key.Contains(Culture)).Count() == 0)
                return Default;
            return dic.FirstOrDefault(x => x.Key.Contains(Culture)).Value;
        }

        public string SimplifyCulture(string Culture)
        {
            if (dic.Where(x => x.Key.Contains(Culture)).Count() == 0)
                return Culture;
            else
                return dic.Where(x => x.Key.Contains(Culture)).First().Key.First();
        }
    }
}
