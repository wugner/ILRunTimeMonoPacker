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

        public static string FormatByAttribute(string attName)
        {
            var attTypeList = (from assembly in AssemblyList
                               where assembly.GetType(attName) != null
                               select assembly.GetType(attName)).ToList();

            if (attTypeList.Count > 0)
            {
                var subClassField = attTypeList[0].GetField("SubClass", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                var displayNameField = attTypeList[0].GetField("DisplayName", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                string result = FormatDLLByManual(type =>
                {
                    var attList = type.GetCustomAttributes(attTypeList[0], false);
                    if (attList.Length == 0)
                        return new PredicateInfo() { Result = false };

                    var subClass = subClassField.GetValue(attList[0]);
                    var displayName = displayNameField.GetValue(attList[0]);
                    return new PredicateInfo() {
                        Result = (bool)subClass == false,
                        DisplayName = (string)displayName
                    };
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
                            Result = classTypeList[0].IsAssignableFrom(type)
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
                            Result = classTypeList[0].IsAssignableFrom(type)
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
            var result = FormatByAttribute("ILHotAttribute.ILHotMonoSerilizableAttribute");
            return result;
        }

        public static string FormatPlayMaker()
        {
            var baseClassName = "SampleHotProject1.ILHotFsmStateAction";
            var classTypeList = (from assembly in AssemblyList
                                 where assembly.GetType(baseClassName) != null
                                 select assembly.GetType(baseClassName)).ToList();

            var attName = "ILHotAttribute.ILHotFsmSerilizableAttribute";
            var attTypeList = (from assembly in AssemblyList
                               where assembly.GetType(attName) != null
                               select assembly.GetType(attName)).ToList();

            if (classTypeList.Count > 0 && attTypeList.Count > 0)
            {
                var subClassField = attTypeList[0].GetField("SubClass", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                var displayNameField = attTypeList[0].GetField("DisplayName", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                var result = FormatDLLByManual(type => {

                    var attList = type.GetCustomAttributes(attTypeList[0], false);
                    if (attList.Length == 0)
                        return new PredicateInfo() { Result = false };

                    var subClass = subClassField.GetValue(attList[0]);
                    var displayName = displayNameField.GetValue(attList[0]);
                    return new PredicateInfo()
                    {
                        Result = classTypeList[0].IsAssignableFrom(type) && classTypeList[0].FullName != type.FullName && (bool)subClass == false,
                        DisplayName = (string)displayName
                    };
                },
                field =>
                {
                    return (field.GetCustomAttributes(false).ToList().FindAll(att => att.GetType().FullName == "UnityEngine.SerializeField").Count != 0)
                        && (field.FieldType.IsSubclassOf(typeof(NamedVariable)) || field.FieldType == typeof(FsmEvent) || classTypeList[0].IsAssignableFrom(field.FieldType));
                });
                return result;
            }

            return "Faild:Dll or attribute is invalid!";
        }

        public class PredicateInfo
        {
            public bool Result;
            public string DisplayName;
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
                        classList.Add(classItem);
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
            if ((ignorePredicate || predicate.Result) && type.FullName.Contains("+") == false)
            {
                IrpClassInfo classItem = new IrpClassInfo();
                classItem.Fields = new List<IrpFieldInfo>();
                classItem.ClassName = type.FullName;
                classItem.DisplayName = predicate.DisplayName;
                var fieldList = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (var field in fieldList)
                {
                    if (filePredicate(field) == true)
                    {
                        var fieldInfo = new IrpFieldInfo();
                        fieldInfo.FieldName = field.Name;
                        fieldInfo.FieldType = field.FieldType;
                        var attributes = field.FieldType.GetCustomAttributes(false);
                        if (attributes.ToList().FindAll(att => att.GetType().FullName == "ILHotAttribute.ILHotMonoSerilizableAttribute" || att.GetType().FullName == "ILHotAttribute.ILHotFsmSerilizableAttribute").Count() > 0)
                            fieldInfo.ClassInfo = GetTypeInfo(field.FieldType, TypePredicate, filePredicate, true);
                        classItem.Fields.Add(fieldInfo);
                    }
                }

                return classItem;
            }

            return null;
        }
    }
}
