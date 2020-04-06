using System;
using LinCms.Core.Entities;

namespace LinCms.Application.Contracts.v1.Books.Dtos
{
    public class BookDto:EntityDto
    {
        public string Author { get; set; }
        public string Image { get; set; }
        public string Summary { get; set; }
        public string Title { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
