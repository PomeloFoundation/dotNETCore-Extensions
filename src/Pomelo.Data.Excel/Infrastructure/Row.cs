using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.Data.Excel.Infrastructure
{
    public class Row : Header
    {
        private readonly Header _header;

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
