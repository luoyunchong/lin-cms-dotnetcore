namespace LinCms.Core.Common
{
    public static class LinConsts
    {
        public static class Group
        {
            public static int Admin = 1;
            public static int CmsAdmin = 2;
            public static int User = 3;
        }

        public static class Claims
        {
            public const string BIO = "urn:github:bio";
            public const string AvatarUrl = "urn:github:avatar_url";
            public const string BlogAddress = "urn:github:blog";
        }
    }
}
