namespace Pomelo.AspNetCore.Localization
{
    public class DefaultStringReader : IStringReader
    {
        public ICultureProvider culture { get; set; }

        public ICultureSet set { get; set; }

        public DefaultStringReader(ICultureProvider culture, ICultureSet set)
        {
            this.culture = culture;
            this.set = set;
        }

        public string this[string src, params object[] args]
        {
            get
            {
                var s = set.GetLocalizedStrings(culture.DetermineCulture());
                return s.Localize(src, args);
            }
        }
    }
}
