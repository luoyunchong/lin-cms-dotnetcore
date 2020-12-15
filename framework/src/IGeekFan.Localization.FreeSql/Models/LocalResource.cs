using FreeSql.DataAnnotations;

namespace IGeekFan.Localization.FreeSql.Models
{
    public class LocalResource
    {
        public LocalResource()
        {
        }

        public LocalResource(string key, string value)
        {
            Key = key;
            Value = value;
        }

        [Column(IsIdentity = true, IsPrimary = true)]
        public long Id { get; set; }
        [Column(StringLength = 50)]
        public string Key { get; set; }
        [Column(StringLength = 50)]
        public string Value { get; set; }
        public long CultureId { get; set; }

        public virtual LocalCulture Culture { get; set; }

    }
}
