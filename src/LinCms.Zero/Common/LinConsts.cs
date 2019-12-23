namespace LinCms.Zero.Common
{
    public static class LinConsts
    {
        public static string SITE_DOMAIN = "SITE_DOMAIN";
        public static class File
        {
            public static string STORE_DIR = "FILE:STORE_DIR";
        }

        public static class Group
        {
            public static int Admin = 1;
            public static int CmsAdmin = 2;
            public static int User = 3;
        }
        public static class Qiniu
        {
            public static string AK = "Qiniu:AK";
            public static string SK = "Qiniu:SK";
            public static string Bucket = "Qiniu:Bucket";
            public static string PrefixPath = "Qiniu:PrefixPath";
            public static string Host = "Qiniu:Host";
            public static string UseHttps = "Qiniu:UseHttps";
        }

        public static class Claims
        {
            public const string BIO = "urn:github:bio";
            public const string AvatarUrl = "urn:github:avatar_url";
            public const string BlogAddress = "urn:github:blog";
        }
    }
}
