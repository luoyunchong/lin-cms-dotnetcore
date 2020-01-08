using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using LinCms.Core.Data.Enums;
using Xunit;

namespace LinCms.Test
{
    public class ExpandObjTest
    {
        [Fact]
        public void ExpandObjToDictionary()
        {
            IDictionary<string, object> perExpandObject = new ExpandoObject();

            perExpandObject.TryAdd("Name", "1");
        }

        [Fact]
        public void EnumToDict()
        {

            Dictionary<int,string> errCodes = Enum.GetValues(typeof(ErrorCode))
                   .Cast<ErrorCode>()
                   .ToDictionary(t => (int)t, t => t.ToString());
        }
    }
}
