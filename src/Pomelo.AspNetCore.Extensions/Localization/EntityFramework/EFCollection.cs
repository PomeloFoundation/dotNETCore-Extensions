using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pomelo.AspNetCore.Localization.EntityFramework;

namespace Pomelo.AspNetCore.Localization.EntityFramework
{
    public class EFCollection<TKey> : LocalizationStringCollection
        where TKey : IEquatable<TKey>
    {
        public ILocalizationDbContext<TKey> _DbContext { get; set; }

        public EFCollection(ILocalizationDbContext<TKey> DbContext, IRequestCultureProvider cultureProvider) : base(cultureProvider)
        {
            _DbContext = DbContext;
            Refresh();
        }

        public override IEnumerable<CultureInfo> Collection
        {
            get
            {
                return _DbContext.LocalizationCultureInfo;
            }
        }

        public override string GetString(string culture, string identifier , params object[] objects)
        {
            var tmp = _DbContext.LocalizationString
                .Include(x => x.CultureInfo)
                .ThenInclude(x => x._Culture)
                .Where(x => x.Key == identifier && x.CultureInfo._Culture.Where(y => y.CultureString == culture).Count() > 0)
                .Select(x => x.Value)
                .FirstOrDefault() ?? identifier;
            return string.Format(tmp, objects);
        }

        public override string SingleCulture(string[] culture)
        {
            foreach (var x in culture)
                if (_DbContext.LocalizationCulture.Where(y => y.CultureString == x).Count() > 0)
                    return x;
            return _DbContext.LocalizationCultureInfo
                .Include(x => x._Culture)
                .Where(x => x.IsDefault)
                .First()
                ._Culture.Select(x => x.CultureString)
                .First();
        }

        private Dictionary<string,string> ConvertToDictionary(ICollection<LocalizedString<TKey>> src)
        {
            var ret = new Dictionary<string, string>();
            foreach (var x in src)
            {
                ret[x.Key] = x.Value;
            }
            return ret;
        }

        public override void Refresh()
        {
        }

        public override void RemoveString(string Identifier)
        {
            var src = _DbContext.LocalizationString
                .Where(x => x.Key == Identifier)
                .ToList();
            foreach(var x in src)
            {
                _DbContext.LocalizationString.Remove(x);
            }
            _DbContext.SaveChanges();
        }

        public override void SetString(string culture, string identifier, string Content)
        {
            var obj = _DbContext.LocalizationCulture
                .Include(x => x.CultureInfo)
                .Where(x => x.CultureString == culture)
                .Select(x => x.CultureInfo)
                .FirstOrDefault();
            if (obj == null)
                throw new KeyNotFoundException();
            var id = obj.Id;
            var str = _DbContext.LocalizationString
                .Where(x => x.CultureInfoId.Equals(id) && x.Key == identifier)
                .ToList();
            if (str.Count > 0)
            {
                foreach (var x in str)
                {
                    x.Value = Content;
                }
            }
            else
            {
                dynamic cultureId;
                dynamic newId;
                if (typeof(TKey) == typeof(Guid))
                {
                    cultureId = Guid.Parse(obj.Id.ToString());
                    newId = Guid.NewGuid();
                }
                else if (typeof(TKey) == typeof(long))
                {
                    cultureId = Convert.ToInt64(obj.Id);
                    newId = -1;
                }
                else if (typeof(TKey) == typeof(int))
                {
                    cultureId = Convert.ToInt32(obj.Id);
                    newId = -1;
                }
                else
                {
                    cultureId = obj.Identifier.ToString();
                    newId = Guid.NewGuid().ToString();
                }
                _DbContext.LocalizationString.Add(new LocalizedString<TKey>
                {
                    Id = newId,
                    CultureInfoId = cultureId,
                    Key = identifier,
                    Value = Content
                });
            }
            _DbContext.SaveChanges();
        }
    }
}
