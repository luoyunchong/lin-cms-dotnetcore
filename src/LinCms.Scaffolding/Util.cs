using LinCms.Scaffolding.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace LinCms.Scaffolding
{
    public class Util
    {
        /// <summary>
        /// 获取类的属性的注释文本，通过 xml和（类的命名空间+类名）读取。获取类的注释
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Dict：key=属性名，value=注释</returns>
        public static string GetEntityRemarkBySummary(string xmlPath, List<PropertyInfo> propertyInfos, string className)
        {
            var entityRemark =String.Empty;
            var sReader = new StringReader(File.ReadAllText(xmlPath));
            using (var xmlReader = XmlReader.Create(sReader))
            {
                XPathDocument xpath = null;
                try
                {
                    xpath = new XPathDocument(xmlReader);
                }
                catch
                {
                    return null;
                }
                var xmlNav = xpath.CreateNavigator();

                var node = xmlNav.SelectSingleNode($"/doc/members/member[@name='T:{className}']/summary");
                if (node != null)
                {
                    var comment = node.InnerXml.Trim(' ', '\r', '\n', '\t');
                    if (string.IsNullOrEmpty(comment) == false) entityRemark=comment; //class注释
                }

                foreach (var prop in propertyInfos)
                {
                    node = xmlNav.SelectSingleNode($"/doc/members/member[@name='P:{className}.{prop.Name}']/summary");
                    if (node == null) continue;
                    var comment = node.InnerXml.Trim(' ', '\r', '\n', '\t');
                    if (string.IsNullOrEmpty(comment)) continue;

                    prop.Remarks = comment;
                }
            }
            return entityRemark;
        }
    }
}
