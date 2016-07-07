using System.Linq;

namespace Pomelo.AspNetCore.Localization
{
    public class DefaultStringReader : IStringReader
    {
        public ICultureProvider culture { get; set; }

        public ICultureSet set { get; set; }

        public ITranslator translator { get; set; }

        public ITranslatedCaching cache { get; set; }

        public DefaultStringReader(ICultureProvider culture, ICultureSet set, ITranslator translator, ITranslatedCaching cache)
        {
            this.culture = culture;
            this.set = set;
            this.translator = translator;
            this.cache = cache;
        }

        public string this[string src, params object[] args]
        {
            get
            {
                var culture = this.culture.DetermineCulture();
                var s = set.GetLocalizedStrings(culture);
                if (s.ContainsKey(src) || translator is NonTranslator)
                    return s.Localize(src, args);
                else
                {
                    var key = set.DefaultCulture;
                    var cachedString = cache.Get(set.Default.ContainsKey(key) ? set.Default[key] : src, culture);
                    if (cachedString == null)
                    {
                        var translateTask = translator.TranslateAsync(key, culture, set.Default.ContainsKey(key) ? set.Default[key] : src);
                        translateTask.Wait();
                        cache.Set(set.Default.ContainsKey(key) ? set.Default[key] : src, culture, translateTask.Result);
                        return string.Format(translateTask.Result, args);
                    }
                    else
                    {
                        return string.Format(cachedString, args);
                    }
                }
            }
        }
    }
}
