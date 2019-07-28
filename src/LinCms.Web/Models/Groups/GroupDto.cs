using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Zero.Domain;

namespace LinCms.Web.Models.Groups
{
    public class GroupDto:Entity
    {
        public List<string> Auths { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
    }
}
