namespace LinCms.Data.Options
{
    public class QiniuOptions
    {
        public string AK { get; set; }
        public string SK { get; set; }
        public string Bucket { get; set; }
        public string PrefixPath { get; set; }
        public string Host { get; set; }
        public bool UseHttps { get; set; }
    }
}

