using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.AspNetCore.Localization
{
    public interface ITranslator
    {
        Task<string> TranslateAsync(string from, string to, string src);
    }
}
