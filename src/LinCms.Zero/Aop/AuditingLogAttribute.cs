using System;

namespace LinCms.Zero.Aop
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
