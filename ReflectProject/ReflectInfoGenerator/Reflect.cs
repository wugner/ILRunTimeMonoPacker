using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HutongGames.PlayMaker;

namespace ReflectInfoGenerator
{
    public class Reflect
    {
        static List<Assembly> AssemblyList = new List<Assembly>();
        static string OutPutPath = string.Empty;
        static string JsonFileName = string.Empty;

        public static string Test(string name)
        {
            return "Hello " + name;
        }

        public static string LoadDLL(List<string> dllList, List<string> dependenceList = null)
        {
            AssemblyList.Clear();

            try
            {
                if (dependenceList != null)
                {
                    foreach (var path in dependenceList)
                    {
                        Assembly.LoadFrom(path);
                    }
                }

                foreach (var path in dllList)
                {
                    AssemblyList.Add(Assembly.LoadFrom(path));
                }
            }
            catch (Exception e)
            {
                return string.Concat("Faild:", e.Message);
            }
            
            return "Succeed";
        }

        public static string SetOutPutPath(string path, string name)
        {
            try
            {
                OutPutPath = path;
                JsonFileName = name;
            }
            catch (Exception e)
            {
                return "Faild:Path or filename is invalid!";
            }

            return string.Concat("Succeed:", path, "\\", name);
        }

        static string tmpStr = "";
        public static string FormatByAttribute(string attName)
        {
            var componentName = "ILHot.ILHotComponentMenu";
            var componentType = GetType(componentName);

            var monoProxyType = GetType(attName);
            
            var serializeName = "ILHot.ILHotSerializeField";
            var serializeType = GetType(serializeName);

            var objectClassName = "ILHot.ILMonoBehaviour";
            var objectClassType = GetType(objectClassName);
            
            if (componentType != null && monoProxyType != null && serializeType != null && objectClassType != null)
            {
                var displayNameField = componentType.GetField("MenuPath", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                string result = FormatDLLByManual(type =>
                {
                    var componentAttribute  = type.GetCustomAttributes(componentType, false).ToList();
                    var monoAttribute       = type.GetCustomAttributes(monoProxyType, false).ToList();

                    var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).ToList();
                    var serializeField = fields.Find(field => field.GetCustomAttributes(serializeType, false).Length != 0);
                    
                    var predicateInfo = new PredicateInfo();
                    predicateInfo.IsProxy       = monoAttribute.Count > 0;
                    predicateInfo.IsBaseType    = serializeField != null;
                    predicateInfo.IsSubClass    = monoAttribute.Count == 0;
                    predicateInfo.IsObject      = objectClassType.IsAssignableFrom(type);
                    predicateInfo.DisplayName   = componentAttribute.Count == 0 ? "" : (string)displayNameField.GetValue(componentAttribute[0]);

                    if (predicateInfo.DisplayName == "DemoTestObjComponent")
                    {
                        tmpStr += "\n" + JsonConvert.SerializeObject(predicateInfo);
                    }

                    return predicateInfo;
                },
                field =>
                {
                    return (field.IsPublic || field.GetCustomAttributes(typeof(SerializableAttribute), false).Length != 0);
                });
                return result;
            }

            return "Faild:Attribute is invalid!";
        }

        public static string FormatByBaseClass(string baseClassName)
        {
            var classTypeList = (from assembly in AssemblyList
                                 where assembly.GetType(baseClassName) != null
                                 select assembly.GetType(baseClassName)).ToList();

            if (classTypeList.Count > 0)
            {
                var result = FormatDLLByManual(
                    type =>
                    {
                        return new PredicateInfo()
                        {
                            IsProxy = classTypeList[0].IsAssignableFrom(type)
                        };
                    },
                    field =>
                    {
                        return (field.IsPublic || field.GetCustomAttributes(typeof(SerializableAttribute), false).Length != 0);
                    });
                return result;
            }

            return "Faild:Baseclass is invalid!";
        }

        public static string FormatByInterface(string baseClassName)
        {
            var classTypeList = (from assembly in AssemblyList
                                 where assembly.GetType(baseClassName) != null
                                 select assembly.GetType(baseClassName)).ToList();

            if (classTypeList.Count > 0)
            {
                var result = FormatDLLByManual(
                    type =>
                    {
                        return new PredicateInfo()
                        {
                            IsProxy = classTypeList[0].IsAssignableFrom(type)
                        };
                    },
                    field =>
                    {
                        return (field.IsPublic || field.GetCustomAttributes(typeof(SerializableAttribute), false).Length != 0);
                    });
                return result;
            }
            return "Faild:Interface is invalid!";
        }

        public static string FormatMono()
        {
            var result = FormatByAttribute("ILHot.ILHotMonoProxyAttribute");
            return result;
        }

        static string tempStr = "";

        public static Type GetType(string className)
        {
            var classList = (from assembly in AssemblyList
                             where assembly.GetType(className) != null
                             select assembly.GetType(className)).ToList();
            return classList.Count > 0 ? classList[0] : null;
        }

        public static string FormatPlayMaker()
        {
            var componentName   = "ILHot.ILHotComponentMenu";
            var componentType   = GetType(componentName);
            
            var fsmProxyName    = "ILHot.ILHotFsmProxyAttribute";
            var fsmProxyType    = GetType(fsmProxyName);

            var serializeName   = "ILHot.ILHotSerializeField";
            var serializeType   = GetType(serializeName);

            var objectClassName = "ILHot.ILProxyBehabiour";
            var objectClassType = GetType(objectClassName);

            if (componentType != null && fsmProxyType != null && serializeType != null)
            {
                var displayNameField = componentType.GetField("MenuPath", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                var result = FormatDLLByManual(type => {
                    var componentAttribute  = type.GetCustomAttributes(componentType, false).ToList();
                    var fsmAttribute        = type.GetCustomAttributes(fsmProxyType, false).ToList();

                    var fields              = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).ToList();
                    var serializeField      = fields.Find(field => field.GetCustomAttributes(serializeType, false).Length != 0);
                    
                    var predicateInfo = new PredicateInfo();
                    predicateInfo.IsProxy       = fsmAttribute.Count > 0;
                    predicateInfo.IsBaseType    = serializeField != null;
                    predicateInfo.IsSubClass    = fsmAttribute.Count == 0;
                    predicateInfo.IsObject      = objectClassType.IsAssignableFrom(type);
                    predicateInfo.DisplayName   = componentAttribute.Count == 0 ? "" : (string)displayNameField.GetValue(componentAttribute[0]);
                    return predicateInfo;
                },
                field =>
                {
                    return field.GetCustomAttributes(serializeType, false).Length > 0;
                });
                return result;
            }

            return "Faild:Dll or attribute is invalid!";
        }
        
        public class PredicateInfo
        {
            public bool     IsProxy;
            public bool     IsBaseType;
            public bool     IsSubClass;
            public bool     IsObject;
            public string   DisplayName;
        }

        public static string FormatDLLByManual(Func<Type, PredicateInfo> TypePredicate, Func<FieldInfo, bool> filePredicate)
        {
            List<IrpClassInfo> classList = new List<IrpClassInfo>();
            foreach (var assembly in AssemblyList)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    var classItem = GetTypeInfo(type, TypePredicate, filePredicate);
                    if (classItem != null)
                    {
                        classList.Add(classItem);
                        var fieldList = GetFieldClasses(classItem);
                        var assClassList = fieldList.FindAll(field => classList.FindIndex(c => c.ClassName == field) == -1);

                        tempStr += "\n" + classItem.ClassName;

                        tempStr += "\n   fieldList " + JsonConvert.SerializeObject(fieldList);
                        tempStr += "\n   assClassList " + JsonConvert.SerializeObject(assClassList);

                        foreach (var className in assClassList)
                        {
                            var classType = GetType(className);
                            if (classType != null)
                            {
                                var subClassItem = GetTypeInfo(classType, TypePredicate, filePredicate, true);
                                if (subClassItem != null)
                                    classList.Add(subClassItem);
                            }
                        }
                    }
                }
            }

            string serialize = JsonConvert.SerializeObject(classList, Formatting.Indented);
            if (Directory.Exists(OutPutPath))
            {
                string filePath = OutPutPath + "\\" + JsonFileName;
                FileStream file = new FileStream(filePath, FileMode.Create);
                byte[] data = System.Text.Encoding.Default.GetBytes(serialize);
                file.Write(data, 0, data.Length);
                file.Flush();
                file.Close();
                return "Succeed";
            }

            return "Faild:Jsonfile directory is invalid!";
        }

        public static IrpClassInfo GetTypeInfo(Type type, Func<Type, PredicateInfo> TypePredicate, Func<FieldInfo, bool> filePredicate, bool ignorePredicate = false)
        {
            var predicate = TypePredicate(type);
            if ((ignorePredicate || predicate.IsProxy) && predicate.IsBaseType == true && type.FullName.Contains("+") == false)
            {
                IrpClassInfo classItem = new IrpClassInfo();
                classItem.Fields = new List<IrpFieldInfo>();
                classItem.ClassName = type.FullName;
                classItem.DisplayName = predicate.DisplayName;
                classItem.IsSubClass = predicate.IsSubClass;
                classItem.IsObject = predicate.IsObject;
                var fieldList = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (var field in fieldList)
                {
                    if (filePredicate(field) == true)
                    {
                        var fieldInfo = new IrpFieldInfo();
                        fieldInfo.FieldName = field.Name;
                        fieldInfo.FieldType = field.FieldType.ToString();
                        fieldInfo.ClassInfo = GetTypeInfo(field.FieldType, TypePredicate, filePredicate, true);
                        classItem.Fields.Add(fieldInfo);
                    }
                }

                return classItem;
            }

            return null;
        }

        public static List<string> GetFieldClasses(IrpClassInfo classInfo)
        {
            var classList = new List<string>();
            foreach (var field in classInfo.Fields)
            {
                if (field.ClassInfo != null)
                {
                    classList.Add(field.FieldType);
                    classList = classList.Union(GetFieldClasses(field.ClassInfo)).ToList();
                }
            }
            return classList;
        }
    }
}
