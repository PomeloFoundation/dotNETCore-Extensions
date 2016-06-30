using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.AspNetCore.Statistics
{
    public class IpInformation
    {
        public string Ip { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string ISP { get; set; }

        public double? Longitude { get; set; }

        public double? Latitude { get; set; }
    }
}
