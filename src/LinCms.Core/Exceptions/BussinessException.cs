using System;
using System.Collections.Generic;
using System.Text;

namespace LinCms.Exceptions
{
    public class BussinessException : ApplicationException
    {
        public BussinessException() : base("服务器繁忙，请稍后再试!")
        {
        }
    }
}
