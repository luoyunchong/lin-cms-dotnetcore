using System.ComponentModel.DataAnnotations;
using LinCms.Entities;

namespace LinCms.Base.Localizations
{
    public class ResourceDto : EntityDto<long>
    {
        [StringLength(50)]
        public string Key { get; set; }
        [StringLength(50)]
        public string Value { get; set; }
        public long CultureId { get; set; }
    }
}
