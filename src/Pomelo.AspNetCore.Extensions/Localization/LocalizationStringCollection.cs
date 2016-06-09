using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Pomelo.AspNetCore.Extensions.Localization
{
    public abstract class LocalizationStringCollection : ILocalizationStringCollection
    {
        public virtual IRequestCultureProvider CultureProvider { get; set; }

        public HttpContext HttpContext
        {
            get { return CultureProvider.HttpContext; }
        }

        public LocalizationStringCollection(IRequestCultureProvider cultureProvider)
        {
            CultureProvider = cultureProvider;
        }

        public virtual string this[string identifier, params object[] objects]
        {
            get
            {
                return this.GetString(SingleCulture(CultureProvider.DetermineRequestCulture()), identifier, objects);
            }
        }

        public abstract IEnumerable<CultureInfo> Collection { get; }

        public abstract void Refresh();

        public abstract void RemoveString(string Identifier);

        public virtual string SingleCulture(string[] culture)
        {
            foreach(var x in culture)
            {
                if (Collection.Any(y => y.Culture.Contains(x)))
                {
                    var result = Collection.Where(y => y.Culture.Contains(x)).First().Culture.First();
                    return result;
                }
            }
            return Collection.Where(y => y.IsDefault).First().Culture.First();
        }

        public virtual string GetString(string culture, string identifier, params object[] objects)
        {
            var cultureInfo = Collection.Where(x => x.Culture.Contains(culture)).FirstOrDefault();
            if (cultureInfo == null)
                cultureInfo = Collection.Where(x => x.IsDefault).FirstOrDefault();
            if (cultureInfo == null)
                throw new FileNotFoundException();
            try
            {
                var ret = string.Format(cultureInfo.LocalizedStrings[identifier], objects);
                return ret;
            }
            catch
            {
                return string.Format(identifier, objects);
            }
        }

        public virtual void SetString(string culture, string identifier, string Content)
        {
            var obj = Collection.Where(x => x.Culture.Contains(culture)).FirstOrDefault();
            if (obj == null)
                throw new FileNotFoundException();
            obj.LocalizedStrings[identifier] = Content;
        }
    }
}
