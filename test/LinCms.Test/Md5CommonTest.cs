using LinCms.Core.Common;
using Xunit;
using Xunit.Abstractions;

namespace LinCms.Test
{
    public class Md5CommonTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Md5CommonTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Get32Md5One()
        {
            string result = LinCmsUtils.Get32Md5("123qwe");
            _testOutputHelper.WriteLine(result);
        }
    }
}
