using System.Linq;
using FreeSql;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Entities.Base;
using Xunit;

namespace LinCms.Test.Service.Base
{
    public class BaseTypeServiceTest
    {
        private readonly IAuditBaseRepository<BaseType> baseRepository;
        private readonly UnitOfWorkManager _unitOfWorkManager;
        private readonly IFreeSql freeSql;

        public BaseTypeServiceTest(IAuditBaseRepository<BaseType> baseRepository, UnitOfWorkManager unitOfWorkManager, IFreeSql freeSql) : base()
        {
            this.baseRepository = baseRepository;
            _unitOfWorkManager = unitOfWorkManager;
            this.freeSql = freeSql;
        }

        /*
        SELECT
            a.*
        FROM
            `base_type` a
            LEFT JOIN `base_item` b ON b.`id` = (
        SELECT
            b2.`id` 
        FROM
            `base_item` b2
        WHERE
             a.`id` = b2.`base_type_id`  
            LIMIT 1 
        )*/
        [Fact]
        public void LeftJoinOne()
        {
            var d = freeSql.Select<BaseType, BaseItem>()
                .LeftJoin((a, b) => b.Id == (freeSql.Select<BaseItem>().As("b2").Where(b2 => b2.BaseTypeId == a.Id).First(b2 => b2.Id)))
                .ToSql((a, b) => new { a, b });
        }



        [Fact]
        public void TestInclude()
        {
            var result = freeSql.Select<BaseType>()
                 .IncludeMany(r => r.BaseItems.Take(1)).ToList();

        }

        [Fact]
        public void LeftJoin()
        {
            var result = freeSql.Select<BaseType>()
                 .IncludeMany(r => r.BaseItems.Take(1)).ToList();

        }


    }
}
