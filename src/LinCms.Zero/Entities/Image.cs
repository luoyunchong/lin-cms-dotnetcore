using FreeSql.DataAnnotations;

namespace LinCms.Core.Entities
{
    [Table(Name = "image")]
    public class Image:Entity
    {
        public int From { get; set; }

        public string Url { get; set; } = string.Empty;

    }
}
