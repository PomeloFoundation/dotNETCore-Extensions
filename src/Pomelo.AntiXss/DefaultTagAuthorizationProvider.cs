using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.AntiXSS
{
    public class DefaultTagAuthorizationProvider : ITagAuthorizationProvider
    {
        public bool IsAbleToUse(string tag)
        {
            return true;
        }
        public bool IsAbleToUse(string tag, string attribute)
        {
            return true;
        }
    }
}
