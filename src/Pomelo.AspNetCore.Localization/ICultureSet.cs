namespace Pomelo.AspNetCore.Localization
{
    public interface ICultureSet
    {
        ILocalizedStringStore Default { get; set; }

        string DefaultCulture { get; set; }

        ILocalizedStringStore GetLocalizedStrings(string Culture);

        void AddCulture(string[] Cultures, ILocalizedStringStore strings, bool IsDefault = false);

        string SimplifyCulture(string Culture);
    }
}
