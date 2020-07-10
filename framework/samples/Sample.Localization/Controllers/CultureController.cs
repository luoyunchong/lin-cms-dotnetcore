using System.Collections.Generic;
using System.Linq;
using IGeekFan.Localization.FreeSql.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Sample.Localization.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CultureController : ControllerBase
    {
        private readonly ILogger<CultureController> _logger;
        private readonly IFreeSql freeSql;
        private readonly IStringLocalizer stringLocalizer;

        public CultureController(ILogger<CultureController> logger, IFreeSql freeSql, IStringLocalizerFactory localizerFactory)
        {
            _logger = logger;
            this.freeSql = freeSql;
            this.stringLocalizer = localizerFactory.Create(null); ;
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return stringLocalizer["Request Localization"] + id;
        }

        [HttpGet]
        public IEnumerable<LocalCulture> Get()
        {
            var locals = new List<LocalCulture>()
                {
                    new LocalCulture("en-US","英文",new List<LocalResource>(){ new LocalResource("Hello","Hello")}),
                    new LocalCulture("zh-CN","中文",new List<LocalResource>(){ new LocalResource("Hello", "您好") }),
                    new LocalCulture("ja-JP","日文",new List<LocalResource>(){ new LocalResource("Hello", "こんにちは") }),
                    new LocalCulture("fr-FR","法语",new List<LocalResource>(){ new LocalResource("Hello", "Bonjour") }),
                };

            freeSql.Insert(new List<LocalResource>() { new LocalResource("Hello", "Hello") }).ExecuteAffrows();

            freeSql.Insert(locals[0].Resources.ToList()).ExecuteAffrows();

            freeSql.Insert<LocalResource>(locals[0].Resources).ExecuteAffrows();

            ICollection<LocalResource> resources = new List<LocalResource>() { new LocalResource("Hello", "Hello") };

            freeSql.Insert(resources).ExecuteAffrows();

            List<LocalCulture> cultures = freeSql.Select<LocalCulture>().ToList();
            if (cultures.Count == 0)
            {
                foreach (var item in locals)
                {
                    long id = freeSql.Insert(item).ExecuteIdentity();

                    item.Resources = item.Resources.ToList().Select(r =>
                    {
                        r.CultureId = id;
                        return r;
                    }).ToList();

                    freeSql.Insert<LocalResource>(item.Resources).ExecuteAffrows();
                }
                return locals;
            }
            return cultures;
        }
    }
}
