using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LocalizationSite.Models
{
    public class Test
    {
        public int Id { get; set; }

        [Localized]
        public string MultiLangContent { get; set; }
    }
}
