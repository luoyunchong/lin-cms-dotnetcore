using System.ComponentModel.DataAnnotations;
using LinCms.Entities;

namespace LinCms.Base.Localizations
{
    public class CultureDto : EntityDto<long>
    {
        [Required]
        [StringLength(10)]
        public string Name { get; set; }

        [Required]
        [StringLength(10)]
        public string DisplayName { get; set; }
    }
}
