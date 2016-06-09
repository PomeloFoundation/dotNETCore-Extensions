using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pomelo.AspNetCore.Extensions.Localization.EntityFramework
{
    [Table("AspNetLocalizationStrings")]
    public class LocalizedString<TKey>
        where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        [ForeignKey("CultureInfo")]
        public TKey CultureInfoId { get; set; }

        public virtual CultureInfo<TKey> CultureInfo { get; set; }
    }
}
