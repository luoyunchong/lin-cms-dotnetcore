# 基础配置项

本文档说明了 LinCms.Core 项目中主要配置文件 (`appsettings.json` 等) 的关键配置项。

## `appsettings.json`

这是 ASP.NET Core 的主配置文件，包含了各种环境通用的设置。

### `ConnectionStrings` (数据库和缓存连接)

*   `DefaultDB`: 指定默认使用的数据库类型，对应 `DataType` 中的数字 (例如，0 代表 MySql)。
*   `DataType`: 定义了支持的数据库类型及其对应的数字标识。
*   `MySql`, `SqlServer`, `PostgreSQL`, `Oracle`, `Sqlite`: 各种数据库的连接字符串。请根据您使用的数据库类型填写正确的连接信息。
*   `Redis`: Redis 缓存的连接字符串。格式通常为 `host:port,password=yourpassword,defaultDatabase=0,...`。

### `Basic` (Swagger Basic 认证)

*   `Enable`: 是否启用 Swagger UI 的 Basic 认证。
*   `ProtectPaths`: 需要 Basic 认证保护的路径列表 (例如 Swagger JSON 文件路径)。
*   `Realm`: Basic 认证的领域 (Realm)。
*   `UserName`, `UserPassword`: 用于 Basic 认证的用户名和密码。

### `Cache` (AOP 缓存)

*   `Enable`: 是否启用基于 `CacheableAttribute` 的 AOP 缓存。
*   `ExpireSeconds`: 缓存的默认过期时间（秒）。

### `Logging` (日志记录)

*   `LogLevel`: 配置不同日志源的最低日志级别 (例如 `Default`, `Microsoft`)。

### `AllowedHosts` (允许的主机)

*   指定允许访问本应用的主机名，`*` 表示允许所有主机。

### `LoginCaptcha` (登录验证码)

*   `Enabled`: 是否启用登录时的图形验证码。
*   `Salt`: 用于生成验证码 Tag 的盐值。

### `FileStorage` (文件存储)

*   `MaxFileSize`: 允许上传的单个文件的最大大小（字节）。
*   `NumLimit`: 单次允许上传的文件数量限制。
*   `Include`, `Exclude`: 允许或排除的文件扩展名列表。
*   `ServiceName`: 指定使用的文件存储服务 (`LocalFileService` 或 `QiniuService`)。
*   `LocalFile`: 本地文件存储的配置。
    *   `PrefixPath`: 文件存储的相对路径前缀。
    *   `Host`: 访问本地文件的基础 URL。
*   `Qiniu`: 七牛云对象存储的配置。
    *   `AK`, `SK`: 七牛云的 Access Key 和 Secret Key。
    *   `Bucket`: 七牛云存储空间 (Bucket) 名称。
    *   `PrefixPath`: 文件在 Bucket 中的路径前缀。
    *   `Host`: 七牛云存储的访问域名。
    *   `UseHttps`: 是否使用 HTTPS 访问。

### `Site` (站点信息)

*   `VVLogDomain`, `CMSDomain`, `ApiDomain`: 项目不同部分（如前端、API）的域名配置。
*   `Email`, `BlogUrl`, `DocUrl`: 站点相关的联系邮箱、博客地址、文档地址。

### `WithOrigins` (CORS 跨域配置)

*   允许跨域请求的来源 (Origin) 列表。

### `Service` (服务配置)

*   `Name`: 当前服务的名称。
*   `UseHttps`: 是否强制使用 HTTPS。

### `Authentication` (认证配置)

*   `JwtBearer`: JWT (JSON Web Token) 认证的核心配置。
    *   `SecurityKey`: 用于签名和验证 JWT 的密钥，**需要足够长且保密**。
    *   `Issuer`: JWT 的签发者。
    *   `Audience`: JWT 的接收者（通常是服务本身）。
*   `GitHub`, `Gitee`: 第三方登录（GitHub, Gitee）的配置。
    *   `Enable`: 是否启用该第三方登录。
    *   `ClientId`, `ClientSecret`: 在第三方平台申请的应用 ID 和密钥。

### `MailKitOptions` (邮件发送)

*   `Host`, `Port`, `EnableSsl`: SMTP 服务器的主机、端口和是否启用 SSL。
*   `UserName`, `Password`: 发送邮件所用的邮箱账号和密码（或授权码）。
*   `Domain`: （可选）邮件域。

### `AuditValue` (敏感词审计)

*   `Enable`: 是否启用 FreeSql 的 Aop.AuditValue 功能进行敏感词过滤（注意：相关库可能已过期）。

### `CAP` (分布式事务)

*   `DefaultStorage`: 默认使用的事件存储方式 (对应 `Storage` 中的数字)。
*   `DefaultMessageQueue`: 默认使用的消息队列 (对应 `MessageQueue` 中的数字)。
*   `Storage`: 定义支持的事件存储类型及其标识。
*   `MessageQueue`: 定义支持的消息队列类型及其标识。
*   `RabbitMQ`: RabbitMQ 的连接配置。

### `RecaptchaSettings` (Google reCAPTCHA)

*   `Enabled`: 是否启用 Google reCAPTCHA 验证。
*   `Version`: 使用的 reCAPTCHA 版本 (如 `reCAPTCHA_V3`)。
*   `HeaderKey`: 前端请求中包含 reCAPTCHA Token 的 Header 名称。
*   `MinimumScore`: V3 版本所需的最低分数阈值。
*   `SiteKey`, `SiteSecret`: Google reCAPTCHA 的站点密钥和私钥。
*   `VerifyBaseUrl`: reCAPTCHA 验证接口的基础 URL。

### `Serilog` (日志记录框架)

*   配置 Serilog 的日志输出目标 (Sinks)、最低级别和格式化等。

## `RateLimitConfig.json` (接口限流配置)

此文件使用 `AspNetCoreRateLimit` 库来配置 API 接口的访问频率限制。

*   `EnableEndpointRateLimiting`: 是否对每个具体的 Endpoint 进行限流（而不是全局）。
*   `StackBlockedRequests`: 被阻止的请求是否计入统计。
*   `RealIpHeader`, `ClientIdHeader`: 用于识别真实客户端 IP 和客户端 ID 的请求头。
*   `HttpStatusCode`, `QuotaExceededResponse`: 超过限流阈值时的响应状态码和响应体内容。
*   `IpWhitelist`, `EndpointWhitelist`, `ClientWhitelist`: IP、Endpoint、客户端 ID 的白名单。
*   `GeneralRules`: 通用限流规则，应用于所有（或匹配 `Endpoint` 模式）的请求。
    *   `Endpoint`: 规则应用的 Endpoint 模式 (`*` 表示所有)。
    *   `Period`: 时间窗口 (如 `1s`, `5m`, `1h`)。
    *   `Limit`: 在该时间窗口内允许的最大请求次数。
*   `IpRateLimitPolicies`: 针对特定 IP 的限流策略。

## 其他配置源

*   **环境变量**: 可以覆盖 `appsettings.json` 中的设置。
*   **命令行参数**: 也可以覆盖之前的配置。
*   **`appsettings.{Environment}.json`**: 特定环境（如 `Development`, `Production`）的配置文件，会覆盖 `appsettings.json` 中的同名设置。

请根据您的部署环境和需求，仔细检查并修改这些配置项。
