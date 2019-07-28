using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using Xunit;

namespace LinCms.Test
{
    public class ExpandObjTest
    {
        [Fact]
        public void ExpandObjToDictionary()
        {
            var perExpandObject = new ExpandoObject() as IDictionary<string,object>;

            perExpandObject.TryAdd("Name", "1");
        }
    }
}
