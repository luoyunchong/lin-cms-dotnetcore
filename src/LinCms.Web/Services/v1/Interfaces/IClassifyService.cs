using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinCms.Web.Services.v1.Interfaces
{
    public interface IClassifyService
    {
        void UpdateArticleCount(Guid? id, int inCreaseCount);
    }
}
