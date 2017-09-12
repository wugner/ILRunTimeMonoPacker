using UnityEngine;
using UnityEditor;
using System.Linq;
using ReflectInfoGenerator;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ILHot
{
    public class ILReflectJsonImport : AssetPostprocessor
    {
        static List<IrpClassInfo> _fsmClassInfoList;
        public static List<IrpClassInfo> FsmClassInfoList { get { return _fsmClassInfoList; } }
        static List<string> _fsmClassTypes;
        public static List<string> FsmClassTypes { get { return _fsmClassTypes; } }
        static string[] _fsmDisplayTypes;
        public static string[] FsmDisplayTypes { get { return _fsmDisplayTypes; } }

        static List<IrpClassInfo> _monoClassInfoList;
        public static List<IrpClassInfo> MonoClassInfoList { get { return _monoClassInfoList; } }
        static List<string> _monoClassTypes;
        public static List<string> MonoClassTypes { get { return _monoClassTypes; } }
        static string[] _monoDisplayTypes;
        public static string[] MonoDisplayTypes { get { return _monoDisplayTypes; } }

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (importedAssets.ToList().FindAll(ass => ass.Contains("ReflectInfoFiles")).Count != 0)
            {
                LoadJson();
            }
        }

        public static void LoadJson()
        {
            var preference = Resources.Load<ILHotPreference>("ILHotPreference");
            var fsmJsonPath = Constant.AppDataPath + "\\" + (string.IsNullOrEmpty(preference.FsmJsonPath) ? "" : preference.FsmJsonPath);
            var monoJsonPath = Constant.AppDataPath + "\\" + (string.IsNullOrEmpty(preference.MonoJsonPath) ? "" : preference.MonoJsonPath);
            if (File.Exists(fsmJsonPath) && string.IsNullOrEmpty(preference.FsmJsonPath) == false)
            {
                StreamReader sr = new StreamReader(fsmJsonPath);
                _fsmClassInfoList = JsonConvert.DeserializeObject<List<IrpClassInfo>>(sr.ReadToEnd());

                _fsmClassTypes = _fsmClassInfoList.Select(linker => linker.ClassName).ToList();
                _fsmClassTypes.Sort();
                _fsmClassTypes.Insert(0, "null");

                _fsmDisplayTypes = _fsmClassTypes.Select(s =>
                {
                    if (s == "null")
                        return s;

                    var index = s.LastIndexOf('.');
                    return string.Concat(s.Substring(0, index), "/", s.Substring(index + 1));
                }).ToArray();

            }

            if (File.Exists(monoJsonPath) && string.IsNullOrEmpty(preference.MonoJsonPath) == false)
            {
                StreamReader sr = new StreamReader(monoJsonPath);
                _monoClassInfoList = JsonConvert.DeserializeObject<List<IrpClassInfo>>(sr.ReadToEnd());

                _monoClassTypes = _monoClassInfoList.Select(linker => linker.ClassName).ToList();
                _monoClassTypes.Sort();
                _monoClassTypes.Insert(0, "null");

                _monoDisplayTypes = _monoClassTypes.Select(s =>
                {
                    if (s == "null")
                        return s;

                    var index = s.LastIndexOf('.');
                    return string.Concat(s.Substring(0, index), "/", s.Substring(index + 1));
                }).ToArray();
            }
        }
    }
}
