using ToolGood.Words;
using Xunit;

namespace LinCms.Test.Utils
{
    public class ToolWordTest
    {
        [Fact]
        public void IssuesTest_17()
        {
            var illegalWordsSearch = new IllegalWordsSearch();
            string s = "中国|zg人|abc";
            illegalWordsSearch.SetKeywords(s.Split('|'));
            var str = illegalWordsSearch.Replace("我是中美国人厉害中国完美ａｂｃddb好的", '*');

            Assert.Equal("我是中美国人厉害**完美***ddb好的", str);
        }
    }
}
