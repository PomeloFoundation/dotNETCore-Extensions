using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.Data.Excel.Infrastructure
{
    public class Header : List<string>
    {
        public void Add(string item, string pos)
        {
            var numStr = "";
            foreach (var x in pos)
                if ("QWERTYUIOPASDFGHJKLZXCVBNM".Contains(x))
                    numStr += x;
            var num = new ColNumber(numStr);
            var intvalue = Convert.ToInt64(ColNumber.FromNumberSystem26(num)) - 1;
            while (this.LongCount() != intvalue + 1)
                this.Add(null);
            this[(int)intvalue] = item;
        }
    }
}
