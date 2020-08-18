using FreeSql;
using LinCms.Entities.Base;
using LinCms.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace LinCms.Test.Service.Base
{
    public class BaseTypeServiceTest : BaseLinCmsTest
    {
        private readonly IAuditBaseRepository<BaseType> baseRepository;
        private readonly UnitOfWorkManager _unitOfWorkManager;
        private readonly IFreeSql freeSql;

        public BaseTypeServiceTest() : base()
        {
            baseRepository = GetService<IAuditBaseRepository<BaseType>>();
            _unitOfWorkManager = GetService<UnitOfWorkManager>();
            freeSql = GetService<IFreeSql>();
        }


        [Fact]
        public void Test()
        {
            var d = freeSql.Select<BaseType, BaseItem>()
                .LeftJoin((a, b) => a.Id == (freeSql.Select<BaseItem>().As("b2").Where(b2 => b2.BaseTypeId == b.BaseTypeId).First(b2 => b2.Id)))
                 .ToList((a, b) => new { a });

        }

    }
}
