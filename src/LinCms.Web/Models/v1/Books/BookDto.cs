using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Zero.Domain;

namespace LinCms.Web.Models.v1.Books
{
    public class BookDto:Entity 
    {
        public string Author { get; set; }
        public string Image { get; set; }
        public string Summary { get; set; }
        public string Title { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
