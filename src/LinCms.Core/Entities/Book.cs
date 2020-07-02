using FreeSql.DataAnnotations;

namespace LinCms.Core.Entities
{
    [Table(Name = "book")]
    public class Book : FullAduitEntity
    {
        [Column(StringLength = 20)]
        public string Author { get; set; } = string.Empty;

        [Column(StringLength = 50)]
        public string Image { get; set; } = string.Empty;

        [Column(StringLength = 1000)]
        public string Summary { get; set; } = string.Empty;

        [Column(StringLength = 50)]
        public string Title { get; set; } = string.Empty;

    }

}
