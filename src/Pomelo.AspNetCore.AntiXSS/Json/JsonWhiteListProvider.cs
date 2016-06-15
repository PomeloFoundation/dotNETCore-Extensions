using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Pomelo.AntiXSS;

namespace Pomelo.AspNetCore.AntiXSS.Json
{
    public class JsonWhiteListProvider : IWhiteListProvider
    {
        public string Path { get; private set; }

        public Dictionary<string, dynamic> RawWhiteList { get; private set; }

        public IDictionary<string, string[]> WhiteList { get; set; }

        public JsonWhiteListProvider(IHostingEnvironment env , string RelativePath = "xss.json")
        {
            Path = System.IO.Path.Combine(env.ContentRootPath, RelativePath);
            Refresh();
        }

        public void Refresh()
        {
            var ret = new Dictionary<string, string[]>();
            var jsonStr = System.IO.File.ReadAllText(Path);
            var RawWhiteList = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonStr);
            foreach(var x in RawWhiteList)
            {
                if (!ret.ContainsKey(x.Key))
                    ret.Add(x.Key, new string[0]);
                var attributes = new List<string>();
                // 如果子项是数组
                if (x.Value.Type == Newtonsoft.Json.Linq.JTokenType.Array)
                {
                    foreach (var y in x.Value)
                    {
                        if (y.Type == Newtonsoft.Json.Linq.JTokenType.String)
                        {
                            attributes.Add(y.ToString());
                        }
                        else
                        {
                            attributes.Add(y.attribute.ToString());
                        }
                    }
                }
                else
                {
                    try
                    {
                        foreach (var y in x.Value.attributes)
                        {
                            if (y.Type == Newtonsoft.Json.Linq.JTokenType.String)
                            {
                                attributes.Add(y.ToString());
                            }
                            else
                            {
                                attributes.Add(y.attribute.ToString());
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                ret[x.Key] = attributes.ToArray();
            }
            WhiteList = ret;
        }
    }
}

/*
{
    "tr": ["class", "style"],
    "td": 
    {
        "role": "admin",
        "attributes": ["class", "style"]
    },
    "div":
    {
        "role": "admin",
        "attributes": [ { "attribute": "style", "role": "admin" } ]
    }
} 
*/
