using System;
using System.Collections.Generic;
using System.Text;

namespace LinCms.Zero.Domain
{
   public class LinOauth2:FullAduitEntity<Guid>
    {
        public string IdentityType { get; set; }
        public string Identifier { get; set; }
        public string Credential { get; set; }
    }
}
