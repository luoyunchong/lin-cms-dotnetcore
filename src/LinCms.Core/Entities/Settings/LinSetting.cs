using System;
using FreeSql.DataAnnotations;

namespace LinCms.Core.Entities.Settings
{
    [Table(Name = "lin_settings")]
    public class LinSetting : FullAduitEntity<Guid>
    {
        /// <summary>
        /// 键
        /// </summary>
        [Column(DbType = "varchar(128)", IsNullable=false)]
        public string Name { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        [Column(DbType = "varchar(2048)", IsNullable = false)]
        public string Value { get; set; }
        /// <summary>
        /// 提供者
        /// </summary>
        [Column(DbType = "varchar(64)")]
        public string ProviderName { get; set; }
        /// <summary>
        /// 提供者值
        /// </summary>
        [Column(DbType = "varchar(64)")]
        public string ProviderKey { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()}, Name = {Name}, Value = {Value}, ProviderName = {ProviderName}, ProviderKey = {ProviderKey}";
        }
    }
}
