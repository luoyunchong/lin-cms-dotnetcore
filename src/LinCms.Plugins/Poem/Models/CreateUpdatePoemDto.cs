using System;
using System.Collections.Generic;
using System.Text;

namespace LinCms.Plugins.Poem.Models
{
    public class CreateUpdatePoemDto
    {
        public string Author { get; set; }
        public string Content { get; set; }
        public string Dynasty { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
    }
}
