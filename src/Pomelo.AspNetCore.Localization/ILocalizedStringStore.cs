using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.AspNetCore.Localization
{
    public interface ILocalizedStringStore : IDictionary<string, string>
    {
        string Localize(string src, params object[] args);
    }
}
