using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinCms.Zero.Data.Enums;
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
    }
}
