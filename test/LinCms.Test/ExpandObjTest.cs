using System.Collections.Generic;
using System.Dynamic;
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

    }
}
