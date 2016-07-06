using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using System.Text.Encodings.Web;
using System.Net.Http;

namespace Pomelo.AspNetCore.Localization
{
    public class GoogleTranslator : ITranslator
    {
        public async Task<string> TranslateAsync(string from, string to, string src)
        {
            var ret = "";
            using (var client = new HttpClient() { BaseAddress = new Uri("https://translate.google.com") })
            {
                var result = await client.GetAsync($"/translate_a/t?client=t&text={ HtmlEncoder.Default.Encode(src) }&hl={ from }&sl={ from }&tl={ to }&ie=UTF-8&oe=UTF-8");
                var html = await result.Content.ReadAsStringAsync();
                throw new Exception(html);
                var index = html.IndexOf("]],");
                html = html.Substring(0, index + 1).Replace("[[", "");
                foreach (Match m in new Regex(@"\[""(.+?)"","".+?"","".+?"",""\w*""\]").Matches(html))
                {
                    var s = m.Value.TrimEnd(']') + ",";
                    var g2 = new Regex(@"""(.+?)"",").Match(s).Groups;
                    if (g2.Count >= 2)
                    {
                        ret += g2[1].Value;
                    }
                }
                return ret;
            }
        }
    }
}
