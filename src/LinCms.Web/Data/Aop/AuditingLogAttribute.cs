using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinCms.Web.Data.Aop
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AuditingLogAttribute : Attribute
    {
        public string Template { get; }

        public AuditingLogAttribute(string template)
        {
            Template = template ?? throw new ArgumentNullException(nameof(template));
        }
    }
}
