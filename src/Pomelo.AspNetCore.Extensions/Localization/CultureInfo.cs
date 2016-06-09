using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Pomelo.AspNetCore.Localization
{
    public class CultureInfo
    {
        [NotMapped]
        [JsonIgnore]
        public string Identifier { get; set; }

        [MaxLength(128)]
        public string Set { get; set; }

        [NotMapped]
        public virtual List<string> Culture { get; set; }

        public bool IsDefault { get; set; }

        [NotMapped]
        public virtual Dictionary<string, string> LocalizedStrings { get; set; }
    }
}
