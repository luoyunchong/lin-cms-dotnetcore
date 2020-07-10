using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Localization.Demo
{
    public interface ITest
    {
        void Insert<T1>(IEnumerable<T1> source) where T1 : class;
        void Insert<T1>(T1 t) where T1 : class;
    }
}
