using System;
using System.Collections.Generic;
using LinCms.Core.Entities;

namespace LinCms.Plugins.Poem.Models
{
    public class PoemDto:EntityDto
    {
        public string Author { get; set; } 
        public List<List<string>> Content { get; set; } 
        public string Dynasty { get; set; } 
        public string Image { get; set; }
        public string Title { get; set; }
        public DateTime CreateTime { get; set; }                            
        public DateTime UpdateTime { get; set; }
    }
}
