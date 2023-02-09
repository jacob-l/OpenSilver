using System;
using System.Collections.Generic;
using System.Text;

namespace Experimental
{
    internal class GenericType<T>
    {
        public T MyProperty { get; set; }
    }

    internal class TypeWithGenericParameter : GenericType<string>
    { }
}
