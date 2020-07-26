using Humanizer;
using System;

namespace LinCms.Scaffolding.Entities
{
    /// <summary>
    /// 属性
    /// </summary>
    public class PropertyInfo
    {
        public PropertyInfo(string type, string name)
        {
            Type = type;
            Name = name;
        }

        public PropertyInfo(string type, string name, string remarks) : this(type, name)
        {
            Remarks = remarks ?? throw new ArgumentNullException(nameof(remarks));
        }

        /// <summary>
        /// 属性类型
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// 属性名
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }


        /// <summary>
        /// 属性名转下划线
        /// </summary>
        public string NameUnderscore => Name.Underscore();


    }
}
