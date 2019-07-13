using System.ComponentModel;

namespace LinCms.Zero.Data.Enums
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

    public enum ErrorCode
    {
        Success=0,
        /// <summary>
        /// 未知错误
        /// </summary>
        UnknownError= 1007,
        /// <summary>
        /// 服务器未知错误
        /// </summary>
        ServerUnknownError = 999,

        /// <summary>
        /// 认证失败
        /// </summary>
        AuthenticationFailed = 10000,
        /// <summary>
        /// 失败
        /// </summary>
        Fail =9999,
        /// <summary>
        /// refreshToken异常
        /// </summary>
        RefreshTokenError = 10100,
        /// <summary>
        /// 资源不存在
        /// </summary>
        NotFound = 10020,
        /// <summary>
        /// 参数错误
        /// </summary>
        [Description("参数错误")]
        ParameterError = 10030,
        /// <summary>
        /// 令牌失效
        /// </summary>
        [Description("令牌失效")]
        TokenInvalidation = 10040,
        /// <summary>
        /// 令牌过期
        /// </summary>
        TokenExpired = 10050,
        /// <summary>
        /// 字段重复
        /// </summary>
        RepeatField= 10060,
        /// <summary>
        /// 不可操作
        /// </summary>
        Inoperable = 10070,
    }
}
