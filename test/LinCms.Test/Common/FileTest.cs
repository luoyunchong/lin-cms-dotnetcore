using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace LinCms.Test.Common
{
    public class FileTest
    {
        [Fact]
        public void test()
        {
            string url = "https://github.com/fighting41love/funNLP/raw/master/data/%E6%95%8F%E6%84%9F%E8%AF%8D%E5%BA%93/%E5%B9%BF%E5%91%8A.txt";
            string lastWriteTime = new FileInfo(url).LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
            ;
        }
    }
}
