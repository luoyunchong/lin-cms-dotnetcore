
using FreeSql.DataAnnotations;

namespace IGeekFan.Localization.FreeSql.Models
{
    public class Resource
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public int CultureId { get; set; }

        public virtual Culture Culture { get; set; }
    }
}
