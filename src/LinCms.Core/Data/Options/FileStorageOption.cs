namespace LinCms.Core.Data.Options
{
    public class FileStorageOption
    {
        public string ServiceName { get; set; }
        public LocalFileOption LocalFile { get; set; }
        public QiniuOptions Qiniu { get; set; }
    }
}