using System;
using FreeSql.DataAnnotations;

namespace LinCms.Entities.Settings
{
    /// <summary>
    /// 配置项
    /// </summary>
    [Table(Name = "lin_settings")]
    public class LinSetting : FullAuditEntity<Guid>
    {
        /// <summary>
        /// 键
        /// </summary>
        [Column(StringLength = 128)]
        public string Name { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        [Column(StringLength = 2048)]
        public string Value { get; set; }

        /// <summary>
        /// 提供者
        /// </summary>

        [Column(StringLength = 64)]
        public string ProviderName { get; set; }

        /// <summary>
        /// 提供者值
        /// </summary>
        [Column(StringLength = 64)]
        public string ProviderKey { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()}, Name = {Name}, Value = {Value}, ProviderName = {ProviderName}, ProviderKey = {ProviderKey}";
        }
    }
}
