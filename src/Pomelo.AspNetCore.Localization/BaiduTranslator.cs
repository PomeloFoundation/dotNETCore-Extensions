using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using System.Text.Encodings.Web;
using System.Net.Http;
using Newtonsoft.Json;

namespace Pomelo.AspNetCore.Localization
{
    public class BaiduTranslator : ITranslator
    {
        public async Task<string> TranslateAsync(string from, string to, string src)
        {
            using (var client = new HttpClient() { BaseAddress = new Uri("http://fanyi.baidu.com") })
            {
                var result = await client.GetAsync($"/v2transapi?from={ from }&query={ UrlEncoder.Default.Encode(src) }&to={ to }");
                var jsonStr = await result.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<dynamic>(jsonStr);
                if (json.trans_result.data.Count == 0)
                    return src;
                else
                    return json.trans_result.data.First.dst;
            }
        }
    }
}
