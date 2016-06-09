using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace Pomelo.AspNetCore.Localization.Json
{
    public class JsonCollection : LocalizationStringCollection
    {
        private string _ResourcesPath { get; set; }

        public List<CultureInfo> _Collection { get; set; }

        public override void SetString(string culture, string identifier, string Content)
        {
            var obj = _Collection.Where(x => x.Culture.Contains(culture)).FirstOrDefault();
            if (obj == null)
                throw new FileNotFoundException();
            obj.LocalizedStrings[identifier] = Content;
            var path = obj.Identifier;
            var json = JsonConvert.SerializeObject(obj);
            File.WriteAllText(path, json);
        }

        public JsonCollection(string resourcesPath, IRequestCultureProvider cultureProvider, IHostingEnvironment env) : base(cultureProvider)
        {
            _Collection = new List<CultureInfo>();
            _ResourcesPath = env.ContentRootPath + resourcesPath;
            Refresh();
        }

        public override IEnumerable<CultureInfo> Collection
        {
            get
            {
                return _Collection;
            }
        }

        public override void Refresh()
        {
            _Collection = new List<CultureInfo>();
            var files = Directory.GetFiles(_ResourcesPath);
            foreach (var file in files)
            {
                if (Path.GetExtension(file) == ".json")
                {
                    try
                    {
                        var json = JsonConvert.DeserializeObject<CultureInfo>(File.ReadAllText(file));
                        json.Identifier = file;
                        _Collection.Add(json);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public override void RemoveString(string Identifier)
        {
            foreach(var x in _Collection)
            {
                try
                {
                    x.LocalizedStrings.Remove(Identifier);
                }
                catch
                {
                }
                var obj = x;
                var path = obj.Identifier;
                obj.Identifier = null;
                var json = JsonConvert.SerializeObject(obj);
                File.WriteAllText(path, json);
            }
        }
    }
}
