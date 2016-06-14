using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.AntiXSS
{
    public interface ITagAuthorizationProvider
    {
        bool IsAbleToUse(string tag);

        bool IsAbleToUse(string tag, string attribute);
    }
}
