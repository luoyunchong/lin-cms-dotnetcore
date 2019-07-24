using System;
using System.Collections.Generic;
using System.Text;
using LinCms.Zero.Common;
using Xunit;

namespace LinCms.Test
{
    public class Md5CommonTest
    {
        [Fact]
        public void Get32Md5One()
        {
            string result = Utils.Get32Md5("123qwe");
        }
    }
}
