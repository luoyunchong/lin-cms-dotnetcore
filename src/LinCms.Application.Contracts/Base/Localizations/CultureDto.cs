using System.ComponentModel.DataAnnotations;
using IGeekFan.FreeKit.Extras.AuditEntity;

namespace LinCms.Base.Localizations;

public class CultureDto : EntityDto<long>
{
    [Required]
    [StringLength(50,ErrorMessage = "名称最大长度为50")]
    public string Name { get; set; }

    [Required]
    [StringLength(50,ErrorMessage = "显示名称最大长度为50")]
    public string DisplayName { get; set; }
}