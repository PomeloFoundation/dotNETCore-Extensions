using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.AntiXSS
{
    public interface IWhiteListProvider
    {
        IDictionary<string, string[]> WhiteList { get; set; }
    }
}
