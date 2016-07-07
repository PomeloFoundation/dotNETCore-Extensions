using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.AspNetCore.Localization
{
    public interface ITranslatedCaching
    {
        void Set(string key, string culture, string dst);

        string Get(string key, string culture);
    }
}
