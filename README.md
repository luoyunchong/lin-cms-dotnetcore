
<h1  align="center">
  <a href="http://doc.cms.7yue.pro/">
    <img width="200" src="https://consumerminiaclprd01.blob.core.chinacloudapi.cn/miniappbackground/sfgmember/lin/left-logo.png">
  </a>
  <h1>
  Lin CMS .NET Core
  </h1>
</h1>
😃 A simple and practical CMS implememted by .NET Core 2.2

## 项目简介
<!-- ![Total visitor](https://visitor-count-badge.herokuapp.com/total.svg?repo_id=https://github.com/luoyunchong/lin-cms-dotnetcore)
![Visitors in today](https://visitor-count-badge.herokuapp.com/today.svg?repo_id=https://github.com/luoyunchong/lin-cms-dotnetcore) -->

<!-- ![](https://img.shields.io/badge/版本-0.0.1.beta.2-3963bc.svg) -->

![Build Status](https://travis-ci.org/luoyunchong/lin-cms-dotnetcore.svg?branch=master)
![](https://img.shields.io/badge/.NETCore-2.2.0-3963bc.svg)
![](https://img.shields.io/badge/license-MIT-3963bc.svg)
![](https://img.shields.io/badge/developer-IGeekFan-3963bc.svg)


本项目是完全出于个人喜爱，看到Lin-cms有了python,node.js,社区也有人出了[lin-cms-tp5](https://github.com/ChenJinchuang/lin-cms-tp5)的版本

本项目是 Lin CMS 后端的 [.NET Core 2.2](https://docs.microsoft.com/zh-cn/dotnet/core/)的 实现，

## 什么是 Lin CMS？

 Lin-CMS 是林间有风团队经过大量项目实践所提炼出的一套**内容管理系统框架**。Lin-CMS 可以有效的帮助开发者提高 CMS 的开发效率, 需要前端？请访问[**前端仓库**](https://github.com/TaleLin/lin-cms-vue)。官方团队产品了解请访问[**TaleLin**](https://github.com/TaleLin)

## 线上文档地址(完善中)

[https://luoyunchong.github.io/vuepress-docs/dotnetcore/lin-cms/](https://luoyunchong.github.io/vuepress-docs/dotnetcore/lin-cms/)

## 线上 Demo

- 官方地址 [http://face.cms.7yue.pro/#/login](http://face.cms.7yue.pro/#/login) 
  - 用户名： super
  - 密码： 123456
- 本项目swagger地址 [http://47.106.80.39:5001/swagger/index.html](http://47.106.80.39:5001/swagger/index.html)

## 前端
- 在原[开源项目](https://github.com/TaleLin/lin-cms-vue)中增加了博客文章、回复、留言板、标签、文章分组管理、插件式功能（还没有）
- [https://github.com/luoyunchong/lin-cms-vue](https://github.com/luoyunchong/lin-cms-vue)

### Lin CMS 的特点

Lin CMS 的构筑思想是有其自身特点的。下面我们阐述一些 Lin 的主要特点。

### Lin CMS 是一个前后端分离的 CMS 解决方案

这意味着，Lin 既提供后台的支撑，也有一套对应的前端系统，

首先，传统的网站开发更多的是采用服务端渲染的方式，需用使用一种模板语言在服务端完成页面渲染：比如 Razor等模板技术。

服务端渲染的好处在于可以比较好的支持 SEO，但作为内部使用的 CMS 管理系统，SEO 并不重要。

但一个不可忽视的事实是，服务器渲染的页面到底是由前端开发者来完成，还是由服务器开发者来完成？其实都不太合适。现在已经没有多少前端开发者是了解这些服务端模板语言的，而服务器开发者本身是不太擅长开发页面的。那还是分开吧，前端用最熟悉的 Vue 写 JS 和 CSS，而服务器只关注自己的 API 即可。

其次，单页面应用程序的体验本身就要好于传统网站。

### 框架本身已内置了 CMS 常用的功能

Lin 已经内置了 CMS 中最为常见的需求：用户管理、权限管理、日志系统等。开发者只需要集中精力开发自己的 CMS 业务即可

更多关于Lin CMS的介绍请访问[Lin CMS线上文档](http://doc.cms.7yue.pro/)

## 所需基础

由于 Lin 采用的是前后端分离的架构，所以你至少需要熟悉 C# 和 Vue。

### 后端 C#
该项目的Lin 的服务端框架是基于[.NET Core 2.2](https://docs.microsoft.com/zh-cn/dotnet/core/)构建的，所以如果你比较熟悉Mvc、WebAPI、过滤器等概念，或者是 有.NET Framework中Mvc开发经验，相信你一定很容易写出代码。

### 后端主要技术
- 数据库：FreeSql+MySQL5.6
- ASP.NET Core2.2+MVC
- 简化对象映射：AutoMapper
- 身份认证框架：IdentityServer4
- Json Web令牌:JWT
- 文档：Swagger
- 序列化：Newtonsoft.Json
- 测试框架：Xunit
- 日志 NLog
- 简化注入服务：Scrutor
- 通用扩展方法 Z.ExtensionMethods

### 前端 
前端需要开发者比较熟悉 Vue 的，另外需要了解 ES6,axios,ElementUi、webpack、Vuex、Vue-Router等等等

## 讨论交流

### QQ 交流群

- 林间有风（lin-cms-vue） QQ 群号：643205479
- .NET Core搬砖队(lin-cms-dotnetcore) QQ群号：762828442

<img class="QR-img" width="258" height="300" src="https://ae01.alicdn.com/kf/Hed659970c86c4004b42480fe7d7f97acW.jpg">

<img class="QR-img" style="margin-left:10px" width="258" height="300" src="https://ae01.alicdn.com/kf/H6c1668c7987a436caae6b19ee6b86af5J.jpg">

### 微信公众号

微信搜索：林间有风

<img class="QR-img" src="https://ae01.alicdn.com/kf/H4e69faac4a834b8a82f54ea05d2dd53av.jpg">


## 下个版本开发计划
1. 迁移Mock.Luo项目至lin-cms-dotnetcore,
2. 实现模块化开发，支持基础组件安装与卸载。
3. 实现abp vnext的文档的功能 [https://docs.abp.io/en/abp/latest](https://docs.abp.io/en/abp/latest)
4. 写文档。
- [x] 创建时间、创建人、修改时间、修改人、删除人、删除时间、是否删除(软删除)
- [x] 部署 
- [ ] 系统访问日志、错误日志可视化
- [ ] 完善文档
- [ ] 重构核心库结构
- [ ] 基于lin-cms-vue的基础,将之前Mock.luo项目中的博客迁移过来
  - [x] 博客随笔发布/编辑/删除
  - [ ] 留言板
  - [ ] 评论回复、审核、点赞等
  - [ ] 前端展示博客效果
  - [x] 基础资料  BaseItem
  - [x] 类别管理  BaseType

##  开源地址
* Gitee 链接 [https://gitee.com/igeekfan/lin-cms-dotnetcore](https://gitee.com/igeekfan/lin-cms-dotnetcore)
* GitHub 链接 [https://github.com/luoyunchong/lin-cms-dotnetcore](https://github.com/luoyunchong/lin-cms-dotnetcore)
