**English** | [简体中文](README.zh-CN.md)

﻿
<h1  align="center">
  <a href="http://doc.cms.7yue.pro/">
    <img width="200" src="https://consumerminiaclprd01.blob.core.chinacloudapi.cn/miniappbackground/sfgmember/lin/left-logo.png">
  </a>
  <h1>
  Lin CMS By .NET 
  </h1>
</h1>

😃 A simple and practical CMS implemented by [ASP.NET Core](https://docs.microsoft.com/zh-cn/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-6.0)

<a name="项目简介"></a>

## Project overview

![Publish](https://github.com/luoyunchong/lin-cms-dotnetcore/workflows/Publish/badge.svg)
[![.NET IDE Rider](https://img.shields.io/static/v1?style=float&logo=rider&label=Rider&message=jetbrains&color=red)](https://www.jetbrains.com/rider/)
![](https://img.shields.io/badge/.net8.0.0-3963bc.svg)
![](https://img.shields.io/badge/license-MIT-3963bc.svg)
![](https://img.shields.io/badge/developer-IGeekFan-3963bc.svg)

This project began as a personal interest after seeing Python and Node.js versions of Lin CMS and the community's [lin-cms-tp5](https://github.com/ChenJinchuang/lin-cms-tp5).

This is an [ASP.NET Core](https://docs.microsoft.com/zh-cn/dotnet/core/introduction) implementation of the Lin CMS backend, paired with a Vue.js frontend. It includes permissions, dictionary items, articles, comments and likes, notifications, tags, and other Juejin-style features.

<a name="什么是-lin-cms"></a>

## What is Lin CMS?

Lin CMS is a **content management system framework** distilled from the Lin team’s project experience. It helps developers build CMS applications more efficiently. For the frontend, visit the [**frontend repository**](https://github.com/luoyunchong/lin-cms-vue).

<a name="开源地址"></a>

## Source repositories
- Backend API on Gitee [https://gitee.com/igeekfan/lin-cms-dotnetcore](https://gitee.com/igeekfan/lin-cms-dotnetcore)
- Backend API on GitHub [https://github.com/luoyunchong/lin-cms-dotnetcore](https://github.com/luoyunchong/lin-cms-dotnetcore)
- Admin UI [https://github.com/luoyunchong/lin-cms-vue](https://github.com/luoyunchong/lin-cms-vue)
- User-facing UI [https://github.com/luoyunchong/lin-cms-vvlog](https://github.com/luoyunchong/lin-cms-vvlog)


<a name="线上文档地址完善中"></a>

## Online documentation (in progress)

- [Legacy deployment and third-party sign-in notes](./docs/README.en.md)


- [https://igeekfan.cn/dotnetcore/lin-cms/](https://igeekfan.cn/dotnetcore/lin-cms/)

<a name="线上-demo"></a>

## Online demos

- [API Swagger](https://api.igeekfan.cn/swagger/index.html)
- [User-facing lin-cms-vvlog](https://vvlog.igeekfan.cn)
- [Administrator interface: lin-cms-vue](https://cms.igeekfan.cn/)

Demo sign-in details are listed in the [Chinese README](README.zh-CN.md#线上-demo).

<a name="前端"></a>

## Frontend
- The original [open-source frontend](https://github.com/TaleLin/lin-cms-vue) is extended with articles, tags, comments, user and tag following, and technology channels. A plugin mechanism was planned but is not yet available. See the [module design](https://blog.igeekfan.cn/2019/11/24/lin-cms-dotnetcore/design/) for details.

<a name="lin-cms-的特点"></a>

### Lin CMS features

Lin CMS has several distinctive design goals:

<a name="lin-cms-是一个前后端分离的-cms-解决方案"></a>

### Lin CMS separates the frontend and backend

Lin provides both backend services and a corresponding frontend.

Traditional websites often render pages on the server using template technologies such as Razor.

Server-side rendering can support SEO well, but SEO is less important for an internal CMS administration system.

A practical question is who should implement server-rendered pages: frontend developers may not know server-side template languages, while backend developers may not specialize in page development. Separating the layers lets frontend developers use Vue, JavaScript, and CSS, while backend developers focus on the API.

The project also favors the experience provided by a single-page application.

<a name="框架本身已内置了-cms-常用的功能"></a>

### Common CMS features are built in

Lin includes common CMS capabilities such as user management, permissions, and logging, allowing developers to focus on their application’s business features.

For more information, see the [Lin CMS documentation](http://doc.cms.7yue.pro/).

<a name="所需基础"></a>

## Prerequisites

<a name="jwt-签名密钥必需"></a>

### JWT signing key (required)

In production, set `Authentication:JwtBearer:SecurityKey` in `appsettings.Production.json`, or provide `Authentication__JwtBearer__SecurityKey` through a protected environment variable or secret manager.
Use a unique cryptographically random key of at least 64 characters for each deployment; do not commit it to Git. If absent in development or testing, the application generates a key and writes it to `appsettings.json` in the working directory. In production it writes to `appsettings.Production.json` and reuses it on later starts. The application refuses to start if the key is too short or still uses the repository’s old default.
PowerShell can generate a key and set an environment variable for the current process:

```powershell
$jwtKeyBytes = New-Object byte[] 64
[System.Security.Cryptography.RandomNumberGenerator]::Fill($jwtKeyBytes)
$env:Authentication__JwtBearer__SecurityKey = [Convert]::ToBase64String($jwtKeyBytes)
```

Persist the generated value in `appsettings.Production.json` or a secret manager. All instances in one deployment must share the same key; do not regenerate it on every restart. If the configuration file is tracked by Git, revert the locally generated change or exclude it from deployment changes to avoid committing the key.
When using `run.sh`, provide `JWT_SIGNING_KEY` from a protected deployment environment; the script passes it to the container. If omitted, the app uses the mounted `appsettings.Production.json` and writes a local key file if configuration is still missing.
Deployments that used the publicly known default key must rotate it and restart every instance. Existing access tokens will become invalid and users must sign in again.
JWT validation also checks the user’s current status and loads current roles and groups from the database. Disabled or deleted accounts cannot continue using old tokens.

Because Lin separates frontend and backend, familiarity with C# and Vue is required.

<a name="后端-c"></a>

### C# backend
The backend is built on [.NET](https://docs.microsoft.com/zh-cn/dotnet/core/). Familiarity with MVC, Web API, filters, or .NET Framework MVC will help you get started.

<a name="相关技术"></a>

### Technologies
- Database：ORM:[FreeSql](https://github.com/dotnetcore/FreeSql)+DataBase:MySQL5.6/MsSqlServer/Oracle/Sqlite/PostgreSQL
- ASP.NET Core + WebAPI + RESTful
- Object mapping：[AutoMapper](https://automapper.org/)
- JSON Web Tokens:JWT
- API documentation：Swagger([Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore))
  - RapiDoc([RapiDoc](https://github.com/luoyunchong/IGeekFan.AspNetCore.RapiDoc))
  - Knife4jUI([Knife4jUI](https://github.com/luoyunchong/IGeekFan.AspNetCore.Knife4jUI))
- Serialization：Newtonsoft.Json
- Testing：Xunit
- Logging [Serilog](https://github.com/serilog/serilog-aspnetcore)
- Dependency injection[AutoFac](https://github.com/autofac/Autofac.Extensions.DependencyInjection)
- General extension methods Z.ExtensionMethods
- Cloud storage: Qiniu Cloud [MQiniu.Core](https://github.com/Hello-Mango/MQiniu.Core)
- Distributed transactions、EventBus：[DotNeteCore.CAP](https://github.com/dotnetcore/CAP)
- GitHub OAuth login[AspNet.Security.OAuth.GitHub](https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers/tree/dev/src/AspNet.Security.OAuth.GitHub)
- QQ OAuth login[AspNet.Security.OAuth.QQ](https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers/tree/dev/src/AspNet.Security.OAuth.QQ)
- Gitee OAuth login[AspNet.Security.OAuth.Gitee](https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers/tree/dev/src/AspNet.Security.OAuth.Gitee)
- [Docker](https://docs.docker.com/)
- [Azure DevOps](https://dev.azure.com/)
- [GitHub Action](https://help.github.com/en/actions)

<a name="实践"></a>

### Practices
- DDD(Domain-Driven Design)Domain-driven design 
- Logging (logging)
- DevOps(CI/CD,automated builds)
- Database Migrations（automated database migrations)
- Authentication(authentication)
- Authorization（authorization）
- Separation of Concerns（separation of concerns）
- Repository Pattern（repository pattern）
- SOLID Principles（single responsibility）
- Clean Code（clean code）

<a name="分层结构layers"></a>

### Layers
- src
  - LinCms.Web：API（ASP.NET Core)
  - LinCms.Application:application services
  - LinCms.Application.Contracts:DTO,data transfer objects，application service interfaces
  - LinCms.Infrastructure:infrastructure and persistence
  - LinCms.Core:application core: entities, common operations, AOP extensions, pagination, dependency interfaces, time extensions, current-user information, exceptions, and value objects
- test

<a name="前端-1"></a>

### Frontend 
Frontend development requires familiarity with Vue, ES6, axios, Element UI, webpack, Vuex, Vue Router, and related tools.


<a name="讨论交流"></a>

## Community

<a name="qq-交流群"></a>

### QQ group

- .NET Core搬砖队(lin-cms-dotnetcore) QQ group number: 762828442

<img class="QR-img"  width="258" height="300" src="./docs/images/qq.png">

<a name="微信公众号"></a>

### WeChat public account

Search WeChat for: dotNET搬砖队

<img class="QR-img" src="./docs/images/wechat.png">

<a name="更新日志"></a>

## Changelog
https://github.com/luoyunchong/lin-cms-dotnetcore/wiki/Changelog-English
