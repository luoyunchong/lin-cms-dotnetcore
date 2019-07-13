using FreeSql.DataAnnotations;

namespace LinCms.Zero.Domain
{
    [Table(Name = "image")]
    public class Image:Entity
    {
        public int From { get; set; }

        public string Url { get; set; } = string.Empty;

    }
}
