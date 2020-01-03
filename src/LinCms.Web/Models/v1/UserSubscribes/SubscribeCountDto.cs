using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinCms.Web.Models.v1.UserSubscribes
{
    public class SubscribeCountDto
    {
        public long SubscribeCount { get; set; }
        public long FansCount { get; set; }

        public long TagCount { get; set; }
    }
}
