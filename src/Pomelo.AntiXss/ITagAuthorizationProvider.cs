using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.AntiXSS
{
    public interface ITagAuthorizationProvider
    {
        bool IsAbleToUse(string tag, object UserId = null);

        bool IsAbleToUse(string tag, string attribute, object UserId = null);
    }
}
