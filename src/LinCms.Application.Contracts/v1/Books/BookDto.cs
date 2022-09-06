using System;
using IGeekFan.FreeKit.Extras.AuditEntity;

namespace LinCms.v1.Books;

public class BookDto : EntityDto
{
    public string Author { get; init; }
    public string Image { get; init; }
    public string Summary { get; init; }
    public string Title { get; init; }
    public DateTime CreateTime { get; init; }
    public DateTime UpdateTime { get; init; }
}