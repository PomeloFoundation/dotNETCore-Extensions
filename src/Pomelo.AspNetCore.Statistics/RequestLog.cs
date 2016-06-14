using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.AspNetCore.Statistics
{
    public class RequestLog
    {
        public string IP { get; set; }

        public string Zone { get; set; }

        public DateTime Time { get; set; }

        public string UserName { get; set; }

        public string OperateSystem { get; set; }

        public string UserAgent { get; set; }

        public string Referer { get; set; }

        public string Browser { get; set; }
    }
}
