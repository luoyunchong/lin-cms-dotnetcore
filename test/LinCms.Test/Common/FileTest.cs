using System.IO;
using Xunit;

namespace LinCms.Test.Common
{
    public class FileTest
    {
        [Fact]
        public void test()
        {
            string url = "https://gitee.com/kongren/funNLP/raw/master/data/%E6%95%8F%E6%84%9F%E8%AF%8D%E5%BA%93/%E6%95%8F%E6%84%9F%E8%AF%8D.txt";
            string lastWriteTime = new FileInfo(url).LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
