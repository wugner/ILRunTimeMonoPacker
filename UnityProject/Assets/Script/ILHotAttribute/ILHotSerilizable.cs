using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ILHotAttribute
{
    public class ILHotMonoSerilizableAttribute : Attribute
    {
        public string DisplayName;
        public bool SubClass;
        public ILHotMonoSerilizableAttribute() { }
    }

    public class ILHotFsmSerilizableAttribute : Attribute
    {
        public string DisplayName;
        public bool SubClass;
        public ILHotFsmSerilizableAttribute() { }
    }
}
