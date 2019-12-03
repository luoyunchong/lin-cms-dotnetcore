using ToolGood.Words;
using Xunit;

namespace LinCms.Test.Startups
{
    public class StartupTest
    {
        [Fact]
        public void ff()
        {
            IllegalWordsSearch searchEx2 = new IllegalWordsSearch();

            string newVal = searchEx2.Replace(@"动乱");
        }
    }
}
