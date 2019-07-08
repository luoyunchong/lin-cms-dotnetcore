namespace LinCms.Domain
{
    /**
     * 枚举
     */

    /**
     * 是否为超级管理员的枚举
     * COMMON 代表 普通用户
     * ADMIN 代表 超级管理员
     */
    public enum UserAdmin
    {
        Common = 1,
        /// <summary>
        /// 管理员
        /// </summary>
        Admin = 2
    }

    /**
    * 当前用户是否为激活状态的枚举
    * ACTIVE 代表 激活状态
    * NOT_ACTIVE 代表 非激活状态
    */
    public enum UserActive
    {
        Active = 1,
        NotActive = 2
    }


    /**
     * 令牌的类型
     * ACCESS 代表 access token
     * REFRESH 代表 refresh token
     */
    //public enum TokenType
    //{
    //    ACCESS,
    //    REFRESH
    //}

    /*
     * 令牌类型静态类
     */
    public class TokenType
    {
        public static string Access = "access";
        public static string Refresh = "refresh";
    }
}
