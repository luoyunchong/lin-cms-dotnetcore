using System;
using IGeekFan.FreeKit.Extras.AuditEntity;

namespace LinCms.Base.BaseItems;

public class BaseItemDto : EntityDto<long>
{
    public long BaseTypeId { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public bool Status { get; set; }
    public int? SortCode { get; set; }
    public DateTime CreateTime { get; set; }
}