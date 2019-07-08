using System;
using System.Collections.Generic;
using System.Text;

namespace LinCms.Domain.Interfaces
{
    public interface ILinLogRepository
    {
        List<LinLog> GetLogUsers();

    }
}
