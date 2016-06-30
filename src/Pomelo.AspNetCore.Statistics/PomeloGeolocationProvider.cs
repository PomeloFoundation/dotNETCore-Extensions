using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace Pomelo.AspNetCore.Statistics
{
    public class PomeloGeolocationProvider : IGeolocationProvider
    {
        public async Task<IpInformation> GeolocateAsync(string ip)
        {
            var ret = new IpInformation();
            ret.Ip = ip;

            // 1. Try get informations from http://geoip.nekudo.com/
            using (var client = new HttpClient { BaseAddress = new Uri("http://geoip.nekudo.com/") })
            {
                var result = await client.GetAsync($"/api/{ip}/en/json");
                var jsonStr = await result.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<dynamic>(jsonStr);
                ret.City = json.city;
                ret.Country = json.country.name;
                ret.Longitude = json.location.longitude;
                ret.Latitude = json.location.latitude;
            }

            // 2. Get ISP informations for China visitors from http://ip.taobao.com/
            using (var client = new HttpClient { BaseAddress = new Uri("http://ip.taobao.com/") })
            {
                var result = await client.GetAsync($"/service/getIpInfo.php?ip={ip}");
                var jsonStr = await result.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<dynamic>(jsonStr);
                ret.ISP = json.data.isp;
            }

            return ret;
        }
    }
}
