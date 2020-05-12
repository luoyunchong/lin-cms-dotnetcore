
using System.Collections;
using System.Collections.Generic;
using FreeSql.DataAnnotations;

namespace IGeekFan.Localization.FreeSql.Models
{
    public class Culture
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Resource> Resources { get; set; }
    }
}
