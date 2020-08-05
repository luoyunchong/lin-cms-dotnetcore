using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Sample.RabbitMQ.MySql.Models;

namespace Sample.RabbitMQ.MySql.Controllers
{
    public class ValuesController : Controller
    {
        private readonly ICapPublisher _capBus;
        private readonly IFreeSql _freeSql;
        public ValuesController(ICapPublisher capPublisher, IFreeSql freeSql)
        {
            _capBus = capPublisher;
            _freeSql = freeSql;
        }

        [Route("~/without/transaction")]
        public async Task<IActionResult> WithoutTransaction()
        {
            await _capBus.PublishAsync("sample.rabbitmq.mysql", new Person()
            {
                Id = 123,
                Name = "Bar"
            });

            return Ok();
        }

        [Route("~/adonet/transaction")]
        public IActionResult AdonetWithTransaction()
        {
            using (var connection = new MySqlConnection(AppDbContext.ConnectionString))
            {
                using (var transaction = connection.BeginTransaction(_capBus, true))
                {
                    //your business code
                    connection.Execute("insert into test(name) values('test')", transaction: (IDbTransaction)transaction.DbTransaction);

                    //for (int i = 0; i < 5; i++)
                    //{
                    _capBus.Publish("sample.rabbitmq.mysql", DateTime.Now);
                    //}
                }
            }

            return Ok();
        }

        [NonAction]
        [CapSubscribe("sample.rabbitmq.mysql")]
        public void Subscriber(DateTime p)
        {
            Console.WriteLine($@"{DateTime.Now} Subscriber invoked, Info: {p}");
        }

        [NonAction]
        [CapSubscribe("sample.rabbitmq.mysql", Group = "group.test2")]
        public void Subscriber2(DateTime p, [FromCap]CapHeader header)
        {
            Console.WriteLine($@"{DateTime.Now} Subscriber invoked, Info: {p}");
        }

        [HttpGet("~/freesql/transaction")]
        public DateTime FreeSqlWithTransaction()
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
    }
}
