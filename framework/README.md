# DotNetCore.CAP.Provider
DotNetCore.CAP  为其增加FreeSql中的统一事务提交

## Getting Started

### NuGet 

你可以运行以下下命令在你的项目中安装 CAP。

```
PM> Install-Package DotNetCore.CAP.MySql.Provider
```

安装FreeSql
```
PM> Install-Package FreeSql
PM> Install-Package FreeSql.Provider.MySqlConnector
```
### Configuration

首先配置CAP到 Startup.cs 文件中，如下：

```c#
    public IFreeSql Fsql { get; }
    public IConfiguration Configuration { get; }
    private string connectionString = @"Data Source=localhost;Port=3306;User ID=root;Password=123456;Initial Catalog=captest;Charset=utf8mb4;SslMode=none;Max pool size=10";
    
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;

        Fsql = new FreeSqlBuilder()
            .UseConnectionString(DataType.MySql, connectionString)
            .UseAutoSyncStructure(true)
            .Build();
    }
    
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IFreeSql>(Fsql);
        services.AddCap(x =>
        {
            x.UseMySql(connectionString);
            x.UseRabbitMQ("localhost");
            x.UseDashboard();
            x.FailedRetryCount = 5;
            x.FailedThresholdCallback = (type, msg) =>
            {
                Console.WriteLine(
                    $@"A message of type {type} failed after executing {x.FailedRetryCount} several times, requiring manual troubleshooting. Message name: {msg.GetName()}");
            };
        });

        services.AddControllers();
    }
```


在控制器中得到
```
private readonly IFreeSql _freeSql;
private readonly ICapPublisher _capBus;
public TestController(IFreeSql freeSql, ICapPublisher capBus)
{
    _freeSql = freeSql;
    _capBus = capBus;
}

[HttpGet("~/freesql/transaction")]
public DateTime GetTime3()
{
    DateTime now = DateTime.Now;
    using (var uow = _freeSql.CreateUnitOfWork())
    {
        using ICapTransaction trans = uow.BeginTransaction(_capBus, false);
        var repo = uow.GetRepository<WeatherForecast>();

        repo.Insert(new WeatherForecast()
        {
            Date = now,
            Summary = "summary",
            TemperatureC = 100
        });

        repo.Insert(new WeatherForecast()
        {
            Date = now,
            Summary = "11222",
            TemperatureC = 200
        });
        _capBus.Publish("freesql.time", now);
        trans.Commit();
    }

    return now;
}

[CapSubscribe("freesql.time")]
public void GetTime(DateTime time)
{
    Console.WriteLine($"time:{time}");
}
```

