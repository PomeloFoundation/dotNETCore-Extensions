using Microsoft.AspNetCore.Http;

namespace Pomelo.AspNetCore.Localization
{
    public interface ICultureProvider
    {
        string DetermineCulture();
    }
}
