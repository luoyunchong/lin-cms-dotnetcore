**English** | [简体中文](configuration.md)

# Configuration reference

This guide describes the key settings in the LinCms.Core project's main configuration files, including `appsettings.json`.

## `appsettings.json`

The main ASP.NET Core configuration file, containing settings shared across environments.

### `ConnectionStrings` (Database and cache connections)

*   `DefaultDB`: Default database type, corresponding to a number in `DataType` (for example, 0 means MySQL).
*   `DataType`: Supported database types and their numeric identifiers.
*   `MySql`, `SqlServer`, `PostgreSQL`, `Oracle`, `Sqlite`: Connection strings for each database. Supply the correct connection details for the selected database.
*   `Redis`: Redis cache connection string. The typical format is `host:port,password=yourpassword,defaultDatabase=0,...`。

### `Basic` (Swagger Basic authentication)

*   `Enable`: Whether Swagger UI Basic authentication is enabled.
*   `ProtectPaths`: Paths protected by Basic authentication, such as the Swagger JSON path.
*   `Realm`: Basic authentication realm.
*   `UserName`, `UserPassword`: Username and credential for Basic authentication.

### `Cache` (AOP caching)

*   `Enable`: Whether AOP caching based on `CacheableAttribute` is enabled.
*   `ExpireSeconds`: Default cache expiration in seconds.

### `Logging` (Logging)

*   `LogLevel`: Minimum log level for each source, such as `Default` or `Microsoft`.

### `AllowedHosts` (Allowed hosts)

*   Allowed hostnames; `*` permits all hosts.

### `LoginCaptcha` (Login CAPTCHA)

*   `Enabled`: Whether an image CAPTCHA is required at login.
*   `Salt`: Salt used to generate the CAPTCHA tag.

### `FileStorage` (File storage)

*   `MaxFileSize`: Maximum size of one uploaded file, in bytes.
*   `NumLimit`: Maximum number of files in one upload.
*   `Include`, `Exclude`: Allowed and excluded file extensions.
*   `ServiceName`: Selected file-storage service (`LocalFileService` or `QiniuService`).
*   `LocalFile`: Local file-storage configuration.
    *   `PrefixPath`: Relative path prefix for stored files.
    *   `Host`: Base URL for accessing locally stored files.
*   `Qiniu`: Qiniu Cloud object-storage configuration.
    *   `AK`, `SK`: Qiniu Cloud access and secret keys.
    *   `Bucket`: Qiniu Cloud bucket name.
    *   `PrefixPath`: Path prefix for files in the bucket.
    *   `Host`: Access domain for Qiniu Cloud storage.
    *   `UseHttps`: Whether to use HTTPS.

### `Site` (Site information)

*   `VVLogDomain`, `CMSDomain`, `ApiDomain`: Domains for different parts of the project, such as the frontend and API.
*   `Email`, `BlogUrl`, `DocUrl`: Contact email, blog URL, and documentation URL.

### `WithOrigins` (CORS configuration)

*   Origins allowed to make cross-origin requests.

### `Service` (Service configuration)

*   `Name`: Name of the current service.
*   `UseHttps`: Whether HTTPS is required.

### `Authentication` (Authentication configuration)

*   `JwtBearer`: Core JWT (JSON Web Token) authentication settings.
    *   `SecurityKey`: Key used to sign and validate JWTs; **it must be sufficiently long and kept secret**.
    *   `Issuer`: JWT issuer.
    *   `Audience`: JWT audience, usually the service itself.
*   `GitHub`, `Gitee`: Third-party sign-in settings for GitHub and Gitee.
    *   `Enable`: Whether the third-party sign-in provider is enabled.
    *   `ClientId`, `ClientSecret`: Application ID and secret obtained from the third-party platform.

### `MailKitOptions` (Email)

*   `Host`, `Port`, `EnableSsl`: SMTP host, port, and whether SSL is enabled.
*   `UserName`, `Password`: Email account and credential (or authorization code) used to send mail.
*   `Domain`: Optional mail domain.

### `AuditValue` (Prohibited-word auditing)

*   `Enable`: Whether to use FreeSql Aop.AuditValue for prohibited-word filtering; the related library may be outdated.

### `CAP` (Distributed transactions)

*   `DefaultStorage`: Default event storage type, corresponding to a number in `Storage`.
*   `DefaultMessageQueue`: Default message queue, corresponding to a number in `MessageQueue`.
*   `Storage`: Supported event-storage types and their identifiers.
*   `MessageQueue`: Supported message-queue types and their identifiers.
*   `RabbitMQ`: RabbitMQ connection settings.

### `RecaptchaSettings` (Google reCAPTCHA)

*   `Enabled`: Whether Google reCAPTCHA is enabled.
*   `Version`: reCAPTCHA version, such as `reCAPTCHA_V3`.
*   `HeaderKey`: Name of the request header carrying the reCAPTCHA token.
*   `MinimumScore`: Minimum score threshold for v3.
*   `SiteKey`, `SiteSecret`: Google reCAPTCHA site key and secret.
*   `VerifyBaseUrl`: Base URL for the reCAPTCHA verification endpoint.

### `Serilog` (Logging framework)

*   Serilog sinks, minimum levels, formatting, and related settings.

## `RateLimitConfig.json` (API rate limiting)

This file uses `AspNetCoreRateLimit` to configure API request-rate limits.

*   `EnableEndpointRateLimiting`: Whether limits apply to individual endpoints rather than globally.
*   `StackBlockedRequests`: Whether blocked requests count toward the limit.
*   `RealIpHeader`, `ClientIdHeader`: Request headers used to identify the real client IP and client ID.
*   `HttpStatusCode`, `QuotaExceededResponse`: Response status code and body when the limit is exceeded.
*   `IpWhitelist`, `EndpointWhitelist`, `ClientWhitelist`: Allow lists for IPs, endpoints, and client IDs.
*   `GeneralRules`: General rate-limit rules for all requests or those matching an `Endpoint` pattern.
    *   `Endpoint`: Endpoint pattern for the rule; `*` matches all.
    *   `Period`: Time window, such as `1s`, `5m`, or `1h`.
    *   `Limit`: Maximum number of requests allowed in the window.
*   `IpRateLimitPolicies`: Rate-limit policies for specific IPs.

## Other configuration sources

*   **Environment variables** can override settings in `appsettings.json`.
*   **Command-line arguments** can also override earlier configuration.
*   **`appsettings.{Environment}.json`**: Environment-specific configuration, such as `Development` or `Production`, overrides matching settings in `appsettings.json`.

Review and adjust these settings for your deployment environment and requirements.
