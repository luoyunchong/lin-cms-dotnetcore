
### 重启守护进程

```bash
sudo /etc/init.d/supervisor restart 
```

### 项目介绍 

### 本地
- cap Dashboard [https://localhost:5001/cap](https://localhost:5001/cap)

### 第三方账号登录
- https://localhost:5001/cms/oauth2/signin?provider=Gitee&redirectUrl=https://vvlog.igeekfan.cn/
- https://localhost:5001/cms/oauth2/signin?provider=GitHub&redirectUrl=https://vvlog.igeekfan.cn/
- https://localhost:5001/cms/oauth2/signin?provider=QQ&redirectUrl=https://vvlog.igeekfan.cn/

### 第三方账号绑定
- https://localhost:5001/cms/oauth2/signin-bind?provider=GitHub&redirectUrl=https://vvlog.igeekfan.cn&token=
- https://localhost:5001/cms/oauth2/signin-bind?provider=QQ&redirectUrl=http://localhost:8081&token=
github快速登录，使用的myget的源。

[https://www.myget.org/feed/aspnet-contrib/package/nuget/AspNet.Security.OAuth.GitHub](https://www.myget.org/feed/aspnet-contrib/package/nuget/AspNet.Security.OAuth.GitHub)


//https://github.com/login/oauth/authorize?client_id=0be6b05fc717bfc4fb67&state=github&redirect_uri=https://localhost:5001/signin-github