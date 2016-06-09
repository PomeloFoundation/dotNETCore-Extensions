using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System
{
    public interface IConvertible<T>
    {
        T ToType();
    }
}
