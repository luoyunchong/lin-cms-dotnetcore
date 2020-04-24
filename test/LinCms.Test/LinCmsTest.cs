using FreeSql;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
namespace LinCms.Test
{
    public class LinCmsTest : BaseLinCmsTest
    {
        ITestOutputHelper testOutputHelper;
        private readonly IFreeSql freeSql;
        public LinCmsTest(ITestOutputHelper testOut)
        {
            testOutputHelper = testOut;
            freeSql = GetService<IFreeSql>();
        }
        [Fact]
        public void OutputTest()
        {
            testOutputHelper.WriteLine("BaseLinCmsTest ConfigureServices");
        }

        /// <summary>
        /// 主要负责工作单元的创建
        /// </summary>
        [Fact]
        public void CreateUnitOfWorkTest()
        {
            using (IUnitOfWork uow = freeSql.CreateUnitOfWork())
            {
                uow.GetOrBeginTransaction();

                using (IUnitOfWork uow2 = freeSql.CreateUnitOfWork())
                {
                    uow2.GetOrBeginTransaction();

                    uow2.Commit();
                }
                uow.Commit();
            }
        }
    }
}
