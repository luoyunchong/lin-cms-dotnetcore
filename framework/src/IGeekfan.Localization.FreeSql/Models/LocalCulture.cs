
using System.Collections;
using System.Collections.Generic;
using FreeSql.DataAnnotations;

namespace IGeekFan.Localization.FreeSql.Models
{
    public class LocalCulture
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public long Id { get; set; }
        [Column(StringLength = 500)]
        public string Name { get; set; }
        public virtual ICollection<LocalResource> Resources { get; set; }
    }
}
