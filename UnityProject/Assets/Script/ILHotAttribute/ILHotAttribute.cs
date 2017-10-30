using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ILHot
{
    public class ILHotMonoProxyAttribute : Attribute
    {
        public ILHotMonoProxyAttribute() { }
    }

    public class ILHotFsmProxyAttribute : Attribute
    {
        public ILHotFsmProxyAttribute() { }
    }

    public class ILHotSerializeField : Attribute
    {
        public ILHotSerializeField() { }
    }

    public class ILHotComponentMenu : Attribute
    {
        public string MenuPath;
        public ILHotComponentMenu() { }
    }
}
