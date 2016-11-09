using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.Data.Excel.Infrastructure
{
    public class Row : List<string>
    {
        private Header _header;

        public string Spans { get; private set; }

        public void Add(string item, string pos)
        {
            var numStr = "";
            foreach (var x in pos)
                if ("QWERTYUIOPASDFGHJKLZXCVBNM".Contains(x))
                    numStr += x;
            var num = new ColNumber(numStr);
            var intvalue = Convert.ToInt64(ColNumber.FromNumberSystem26(num)) - 1;
            while (this.LongCount() != intvalue +1)
                this.Add(null);
            this[(int)intvalue] = item;
        }

        public Row(Header header = null)
        {
            _header = header;
        }

        public string this[string index]
        {
            get
            {
                try
                {
                    return this[_header.IndexOf(index)];
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
