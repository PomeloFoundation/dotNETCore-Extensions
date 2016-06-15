using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.AntiXSS
{
    public class DefaultTagAuthorizationProvider : ITagAuthorizationProvider
    {
        public bool IsAbleToUse(string tag, object UserId = null)
        {
            return true;
        }
        public bool IsAbleToUse(string tag, string attribute, object UserId = null)
        {
            return true;
        }
    }
}
