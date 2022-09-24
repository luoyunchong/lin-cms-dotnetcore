using System;
using IGeekFan.FreeKit.Extras.AuditEntity;

namespace LinCms.Base.BaseTypes;

public class BaseTypeDto : EntityDto<long>
{
    public string TypeCode { get; set; }
    public string FullName { get; set; }
    public int? SortCode { get; set; }
    public DateTime CreateTime { get; set; }
}