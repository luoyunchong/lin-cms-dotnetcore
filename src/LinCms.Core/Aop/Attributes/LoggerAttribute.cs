using System;

namespace LinCms.Aop.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class LoggerAttribute(string template) : Attribute
    {
        public string Template { get; } = template ?? throw new ArgumentNullException(nameof(template));
    }
}
