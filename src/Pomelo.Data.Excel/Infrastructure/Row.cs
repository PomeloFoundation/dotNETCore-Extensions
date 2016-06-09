using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeComb.Data.Excel.Infrastructure
{
    public class Row : List<string>
    {
        private Header header;

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

        public Row(Header Header = null)
        {
            header = Header;
        }

        public string this[string index]
        {
            get
            {
                try
                {
                    return this[header.IndexOf(index)];
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
