using IGeekFan.Localization.FreeSql.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Localization.Demo
{
    public class Test : ITest
    {
        public void Insert<T1>(List<T1> source) where T1 : class
        {
            throw new NotImplementedException();
        }

        public void Insert<T1>(IEnumerable<T1> source) where T1 : class
        {
            throw new NotImplementedException();
        }

        public void Insert<T1>(T1 t) where T1 : class, new()
        {
            throw new NotImplementedException();
        }
    }
}
