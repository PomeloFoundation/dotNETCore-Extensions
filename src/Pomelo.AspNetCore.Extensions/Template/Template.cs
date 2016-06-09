using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Mvc
{
    public class TemplateManager
    {
        public TemplateCollection Collection { get; }

        public TemplateInfo Current
        {
            get
            {
                var ret = Collection.Templates.Where(x => x.Identifier == Provider.DetermineRequestTemplate()).FirstOrDefault();
                if (ret ==null)
                    ret = Collection.Templates.Where(x => x.IsDefault).FirstOrDefault();
                if (ret == null)
                    throw new FileNotFoundException();
                return ret;
            }
        }

        public TemplateInfo Default
        {
            get
            {
                var ret = Collection.Templates.Where(x => x.IsDefault).FirstOrDefault();
                if (ret == null)
                    ret = Collection.Templates.FirstOrDefault();
                if (ret == null)
                    throw new FileNotFoundException();
                return ret;
            }
        }

        public IRequestTemplateProvider Provider { get; private set; }

        public TemplateManager(IRequestTemplateProvider templateProvider, TemplateCollection collection)
        {
            Collection = collection;
            Provider = templateProvider;
        }
    }
}
