using Humanizer;
using System.Collections.Generic;
using System.Linq;

namespace LinCms.Scaffolding.Entities
{
    public class EntityInfo
    {
        public EntityInfo(string @namespace, string name, string baseType, string primaryKey, string relativeDirectory)
        {
            Namespace = @namespace;
            Name = name;
            BaseType = baseType;
            PrimaryKey = primaryKey;
            RelativeDirectory = relativeDirectory.Replace('\\', '/');
        }

        /// <summary>
        /// 实体类应用服务等的命名空间
        /// </summary>
        public string Namespace { get; }
        public string RelativeNamespace => RelativeDirectory.Replace('/', '.');
        public string RelativeDirectory { get; }
        public string NamespaceLastPart => Namespace.Split('.').Last();
        public string Name { get; }

        /// <summary>
        /// 复数
        /// </summary>
        public string NamePluralized => Name.Pluralize();
        /// <summary>
        /// 首字母小写
        /// </summary>
        public string NameCamelize => Name.Camelize();

        /// <summary>
        /// 小写+复数
        /// </summary>
        public string NameCamelizePluralized => Name.Camelize().Pluralize();


        public string BaseType { get; }
        /// <summary>
        /// 主键类型
        /// </summary>
        public string PrimaryKey { get; }
        /// <summary>
        /// 类的属性键值对。
        /// </summary>
        public List<PropertyInfo> Properties { get; } = new List<PropertyInfo>();

        /// <summary>
        /// 类的备注
        /// </summary>
        public string EntityRemark { get; set; } = "";
    }
}
