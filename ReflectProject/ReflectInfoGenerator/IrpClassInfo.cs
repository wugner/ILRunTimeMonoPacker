using System;
using System.Collections.Generic;

namespace ReflectInfoGenerator
{
    public class IrpClassInfo
    {
        public string ClassName;
        public string DisplayName;
        public bool IsSubClass;
        public bool IsObject;
        public List<IrpFieldInfo> Fields;
    }

    public class IrpFieldInfo
    {
        public string FieldName;
        public string FieldType;
        public List<string> Attributes;
        public IrpClassInfo ClassInfo;
    }
}
