using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ILHot
{
    public class ILHotTool : EditorWindow
    {
        static string _iLDLLMonoJsonPath;
        static string _iLDLLFsmJsonPath;
        
        [MenuItem("Tools/ILHot/DLLPath")]
        static void DLLPathWindows()
        {
            Selection.activeObject = Resources.Load<ILHotPreference>("ILHotPreference");
        }

        private void OnGUI()
        {
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            _iLDLLMonoJsonPath  = EditorPrefs.GetString("ILDLLMonoJsonPath", "");
            _iLDLLMonoJsonPath  = EditorGUILayout.TextField("MonoJsonPath", _iLDLLMonoJsonPath);
            if (GUILayout.Button("Browse", GUILayout.MaxWidth(100)))
            {
                var path = EditorUtility.OpenFilePanel("选择MonoJson文件", _iLDLLMonoJsonPath, "json");
                _iLDLLMonoJsonPath = string.IsNullOrEmpty(path) ? _iLDLLMonoJsonPath : path;
            }

            EditorPrefs.SetString("ILDLLMonoJsonPath", _iLDLLMonoJsonPath);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            _iLDLLFsmJsonPath = EditorPrefs.GetString("ILDLLFsmJsonPath", "");
            _iLDLLFsmJsonPath = EditorGUILayout.TextField("FsmJsonPath", _iLDLLFsmJsonPath);
            if (GUILayout.Button("Browse", GUILayout.MaxWidth(100)))
            {
                var path = EditorUtility.OpenFilePanel("选择FsmJson文件", _iLDLLFsmJsonPath, "json");
                _iLDLLFsmJsonPath = string.IsNullOrEmpty(path) ? _iLDLLFsmJsonPath : path;
            }

            EditorPrefs.SetString("ILDLLFsmJsonPath", _iLDLLFsmJsonPath);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);
        }
    }
}
