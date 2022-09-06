using System;

namespace IGeekFan.FreeKit.Extras.FreeSql
{
    [AttributeUsage(AttributeTargets.Method)]
    public class LoggerAttribute : Attribute
    {
        public string Template { get; }

        public LoggerAttribute(string template)
        {
            Template = template ?? throw new ArgumentNullException(nameof(template));
        }
    }
}
