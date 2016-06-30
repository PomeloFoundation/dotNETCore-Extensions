using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.AspNetCore.Statistics
{
    public interface IGeolocationProvider
    {
        Task<IpInformation> GeolocateAsync(string ip);
    }
}
