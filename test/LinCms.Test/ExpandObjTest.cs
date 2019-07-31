using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using LinCms.Zero.Data.Enums;
using Xunit;

namespace LinCms.Test
{
    public class ExpandObjTest
    {
        [Fact]
        public void ExpandObjToDictionary()
        {
            var perExpandObject = new ExpandoObject() as IDictionary<string, object>;

            perExpandObject.TryAdd("Name", "1");
        }

        [Fact]
        public void enumToDict()
        {

            var _errCodes = Enum.GetValues(typeof(ErrorCode))
                   .Cast<ErrorCode>()
                   .ToDictionary(t => (int)t, t => t.ToString());
        }
    }
}
