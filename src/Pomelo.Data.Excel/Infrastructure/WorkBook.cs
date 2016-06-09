using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeComb.Data.Excel.Infrastructure
{
    public class WorkBook
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public string FileName
        {
            get
            {
                return $"sheet{Id}.xml";
            }
        }
    }
}
