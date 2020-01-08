using LinCms.Core.Common;
using Xunit;

namespace LinCms.Test.Common
{
   public class LinCmsUtilsTest
    {
        [Fact]
        public void test()
        {
            var d= LinCmsUtils.IpQueryCity("117.83.181.123");
        }
    }
}
