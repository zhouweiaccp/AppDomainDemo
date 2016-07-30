using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLibrary1
{
    [Serializable]
    public class Class1 : ClassLibrary2.IClass1
    {
        public string test() { return "System.";}
    }
}
