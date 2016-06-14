using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Pomelo.AspNetCore.AntiXSS.EntityFramework
{
    public class WhiteListTag
    {
        [MaxLength(64)]
        public string Id { get; set; }

        [MaxLength(128)]
        public string RoleRequired { get; set; }
        
        public virtual ICollection<WhiteListAttribute> Attributes { get; set; } = new List<WhiteListAttribute>();
    }
}
