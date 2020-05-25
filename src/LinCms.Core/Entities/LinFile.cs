using FreeSql.DataAnnotations;

namespace LinCms.Core.Entities
{
    [Table(Name = "lin_file")]
    public class LinFile : FullAduitEntity
    {
        /// <summary>
        /// 后缀
        /// </summary>
        [Column(DbType = "varchar(50)")]
        public string Extension { get; set; } = string.Empty;

        /// <summary>
        /// 图片md5值，防止上传重复图片
        /// </summary>
        [Column(DbType = "varchar(40)")]
        public string Md5 { get; set; } = string.Empty;

        /// <summary>
        /// 名称
        /// </summary>
        [Column(DbType = "varchar(300)")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 路径
        /// </summary>
        [Column(DbType = "varchar(500)")]
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// 大小
        /// </summary>
        public long? Size { get; set; }

        /// <summary>
        /// 1 local，2 代表七牛云 3 其他表示其他地方
        /// </summary>
        public short? Type { get; set; }

        public static string LocalFileService = "LocalFileService";
        public static string QiniuService = "QiniuService";

    }
}
