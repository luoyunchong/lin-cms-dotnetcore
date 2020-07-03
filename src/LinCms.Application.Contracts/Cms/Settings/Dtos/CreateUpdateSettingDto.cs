using System.ComponentModel.DataAnnotations;

namespace LinCms.Application.Contracts.Cms.Settings.Dtos
{
    public class CreateUpdateSettingDto
    {
        /// <summary>
        /// 键
        /// </summary>
        [StringLength(128)]
        public string Name { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        [StringLength(2048)]
        public string Value { get; set; }

        /// <summary>
        /// 提供者键
        /// </summary>
        [StringLength(64)]
        public string ProviderName { get; set; }

        /// <summary>
        /// 提供者值
        /// </summary>
        [StringLength(64)]
        public string ProviderKey { get; set; }
    }
}
