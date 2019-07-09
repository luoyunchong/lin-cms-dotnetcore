namespace LinCms.Web.Data.Enums
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
        /// <summary>
        /// 普通用户
        /// </summary>
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
        /// <summary>
        /// 激活状态
        /// </summary>
        Active = 1,
        /// <summary>
        /// 非激活状态
        /// </summary>
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
        /// <summary>
        /// access token
        /// </summary>
        public static string Access = "access";
        /// <summary>
        /// refresh token
        /// </summary>
        public static string Refresh = "refresh";
    }
}
