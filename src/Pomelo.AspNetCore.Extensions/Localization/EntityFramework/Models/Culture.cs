using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pomelo.AspNetCore.Localization.EntityFramework
{
    [Table("AspNetLocalizationCulture")]
    public class Culture<TKey>
        where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }

        [ForeignKey("CultureInfo")]
        public TKey CultureInfoId { get; set; }

        public virtual CultureInfo<TKey> CultureInfo { get; set; }

        [MaxLength(32)]
        public string CultureString { get; set; }
    }
}
