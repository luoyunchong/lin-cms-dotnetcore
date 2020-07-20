using System;
using System.Collections.Generic;
using System.Linq;
using LinCms.Data.Enums;
using Xunit;

namespace LinCms.Test
{
    public class EnumTest
    {
        [Fact]
        public void ErrorCodeTest()
        {
           Dictionary<int, string> errCodes = Enum.GetValues(typeof(ErrorCode))
                .Cast<ErrorCode>()
                .ToDictionary(t => (int)t, t => t.ToString());
           
        }

        [Fact]
        public void Test()
        {
            string path = "https://lc-gold-cdn.xitu.io/bac28828a49181c34110.png";

            string name = path.Substring(path.LastIndexOf("/") + 1, path.Length - path.LastIndexOf("/") - 1);

            Assert.Equal("bac28828a49181c34110.png", name);
        }
    }
}
