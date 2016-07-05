namespace Pomelo.AspNetCore.Localization
{
    public interface IStringReader
    {
        string this[string src, params object[] args] { get; }
    }
}
