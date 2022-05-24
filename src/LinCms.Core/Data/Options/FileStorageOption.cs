namespace LinCms.Data.Options
{
    public class FileStorageOption
    {
        /// <summary>
        /// 上传文件总大小
        /// </summary>
        public long MaxFileSize { get; set; }
        /// <summary>
        /// 多文件上传时，支持的最大文件数量
        /// </summary>
        public int NumLimit { get; set; }
        /// <summary>
        /// 允许某些类型文件上传，文件格式以,隔开
        /// </summary>
        public string Include { get; set; }
        /// <summary>
        /// 禁止某些类型文件上传，文件格式以,隔开
        /// </summary>
        public string Exclude { get; set; }

        public string ServiceName { get; set; }
        public LocalFileOption LocalFile { get; set; }
        public QiniuOptions Qiniu { get; set; }
    }
}