**English** | [简体中文](README.md)

### Restart the supervisor

```bash
sudo /etc/init.d/supervisor restart
```

### Project overview

### Local development

- CAP dashboard: [https://localhost:5001/cap](https://localhost:5001/cap)

### Third-party sign-in

- https://localhost:5001/cms/oauth2/signin?provider=Gitee&redirectUrl=https://vvlog.igeekfan.cn/
- https://localhost:5001/cms/oauth2/signin?provider=GitHub&redirectUrl=https://vvlog.igeekfan.cn/
- https://localhost:5001/cms/oauth2/signin?provider=QQ&redirectUrl=https://vvlog.igeekfan.cn/

### Linking third-party accounts

See the [Chinese guide](README.md) for the example account-linking URLs.

GitHub quick sign-in uses this MyGet feed:

[https://www.myget.org/feed/aspnet-contrib/package/nuget/AspNet.Security.OAuth.GitHub](https://www.myget.org/feed/aspnet-contrib/package/nuget/AspNet.Security.OAuth.GitHub)

//https://github.com/login/oauth/authorize?client_id=0be6b05fc717bfc4fb67&state=github&redirect_uri=https://localhost:5001/signin-github
