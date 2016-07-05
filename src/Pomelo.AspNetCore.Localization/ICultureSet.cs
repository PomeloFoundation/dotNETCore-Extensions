namespace Pomelo.AspNetCore.Localization
{
    public interface ICultureSet
    {
        ILocalizedStringStore GetLocalizedStrings(string Culture);

        void AddCulture(string[] Cultures, ILocalizedStringStore strings, bool IsDefault = false);

        string SimplifyCulture(string Culture);
    }
}
