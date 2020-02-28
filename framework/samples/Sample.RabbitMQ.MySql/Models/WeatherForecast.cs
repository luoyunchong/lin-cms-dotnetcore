using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSql.DataAnnotations;

namespace Sample.RabbitMQ.MySql.Models
{
    public class WeatherForecast
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        [Column(DbType = "varchar(20)")]
        public string Summary { get; set; }
    }
}
