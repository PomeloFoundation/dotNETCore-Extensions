using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pomelo.AspNetCore.AntiXSS.EntityFramework
{
    public class WhiteListAttribute
    {
        public Guid Id { get; set; }

        [MaxLength(64)]
        public string Attribute { get; set; }

        [MaxLength(128)]
        public string RoleRequired { get; set; }

        [MaxLength(128)]
        public string ClaimRequired { get; set; }

        [MaxLength(64)]
        [ForeignKey("Tag")]
        public string TagId { get; set; }

        public virtual WhiteListTag Tag { get; set; }
    }
}
