using System;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using System.Net.Http;
using Newtonsoft.Json;

namespace Pomelo.AspNetCore.Localization
{
    public class BaiduTranslator : ITranslator
    {
        private ITranslatorDisabler disabler { get; set; }

        public BaiduTranslator(ITranslatorDisabler disabler)
        {
            this.disabler = disabler;
        }

        public async Task<string> TranslateAsync(string from, string to, string src)
        {
            try
            {
                if (disabler.IsDisabled())
                    return src;

                if (from.IndexOf('-') >= 0)
                    from = from.Split('-')[0];
                if (to.IndexOf('-') >= 0)
                    to = to.Split('-')[0];

                if (from == to)
                    return src;

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
            catch
            {
                return src;
            }
        }
    }
}
