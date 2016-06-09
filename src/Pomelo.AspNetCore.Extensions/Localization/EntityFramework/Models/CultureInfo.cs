using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pomelo.AspNetCore.Extensions.Localization.EntityFramework
{
    [Table("AspNetLocalizationCultureInformations")]
    public class CultureInfo<TKey> : CultureInfo
        where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }

        public virtual ICollection<Culture<TKey>> _Culture { get; set; } = new List<Culture<TKey>>();

        public virtual ICollection<LocalizedString<TKey>> _Strings { get; set; } = new List<LocalizedString<TKey>>();
    }
}
