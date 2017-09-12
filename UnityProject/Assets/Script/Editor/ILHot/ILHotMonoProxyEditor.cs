using ILHotAttribute;
using Newtonsoft.Json;
using ReflectInfoGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ILHot
{
    [CustomEditor(typeof(ILHotMonoProxy), true)]
    public class ILHotMonoProxyEditor : Editor
    {
        private class ILHotMonoProxyStruct
        {
            public Dictionary<string, string> StringValues;
            public Dictionary<string, string> NewStringValues;

            public Dictionary<string, float> FloatValues;
            public Dictionary<string, float> NewFloatValues;

            public Dictionary<string, bool> BoolValues;
            public Dictionary<string, bool> NewBoolValues;

            public Dictionary<string, UnityEngine.Object> ObjectValues;
            public Dictionary<string, UnityEngine.Object> NewObjectValues;

            public Dictionary<string, bool> ExpandValues;
            public Dictionary<string, bool> NewExpandValues;

            public ILHotMonoProxyStruct(
                Dictionary<string, string> stringValues,
                Dictionary<string, string> newStringValues,
                Dictionary<string, float> floatValues,
                Dictionary<string, float> newFloatValues,
                Dictionary<string, bool> boolValues,
                Dictionary<string, bool> newBoolValues,
                Dictionary<string, UnityEngine.Object> objectValues,
                Dictionary<string, UnityEngine.Object> newObjectValues,
                Dictionary<string, bool> expandValues,
                Dictionary<string, bool> newExpandValues
                )
            {
                StringValues    = stringValues;
                NewStringValues = newStringValues;
                FloatValues     = floatValues;
                NewFloatValues  = newFloatValues;
                BoolValues      = boolValues;
                NewBoolValues   = newBoolValues;
                ObjectValues    = objectValues;
                NewObjectValues = newObjectValues;
                ExpandValues    = expandValues;
                NewExpandValues = newExpandValues;
            }
        }
        
        public override void OnInspectorGUI()
        {
            if (ILReflectJsonImport.MonoClassInfoList == null)
                ILReflectJsonImport.LoadJson();

            if (ILReflectJsonImport.MonoClassInfoList == null)
            {
                EditorGUILayout.HelpBox("Can not find ILHot ", MessageType.Error);
                return;
            }

            var classNameProperty = serializedObject.FindProperty("_runtimeClassName");
            
            var selected = 0;
            if (!string.IsNullOrEmpty(classNameProperty.stringValue))
                selected = ILReflectJsonImport.MonoClassTypes.FindIndex(t => t == classNameProperty.stringValue);

            selected = EditorGUILayout.Popup("Class Name", selected, ILReflectJsonImport.MonoDisplayTypes);

            if (selected <= 0)
            {
                if (string.IsNullOrEmpty(classNameProperty.stringValue))
                {
                    EditorGUILayout.HelpBox("Please specify a type ", MessageType.Error);
                }
                else
                {
                    EditorGUILayout.HelpBox(string.Format("Type [{0}] is not exsit. Maybe deleted or name changed",
                        classNameProperty.stringValue), MessageType.Error);
                }
                return;
            }

            classNameProperty.stringValue = ILReflectJsonImport.MonoClassTypes[selected];
            
            var classInfo = ILReflectJsonImport.MonoClassInfoList.Find(info => info.ClassName == classNameProperty.stringValue);
            if (classInfo == null)
            {
                EditorGUILayout.HelpBox("Can not find type " + classNameProperty.stringValue, MessageType.Error);
                return;
            }

            var instance = (ILHotMonoProxy)target;
            Dictionary<string, string> stringValues = GetValues(instance.StringNames, instance.StringValues);
            Dictionary<string, string> newStringValues = new Dictionary<string, string>();

            Dictionary<string, float> floatValues = GetValues(instance.FloatNames, instance.FloatValues);
            Dictionary<string, float> newFloatValues = new Dictionary<string, float>();

            Dictionary<string, bool> boolValues = GetValues(instance.BoolNames, instance.BoolValues);
            Dictionary<string, bool> newBoolValues = new Dictionary<string, bool>();

            Dictionary<string, UnityEngine.Object> objectValues = GetValues(instance.ObjectNames, instance.ObjectValues);
            Dictionary<string, UnityEngine.Object> newObjectValues = new Dictionary<string, UnityEngine.Object>();
            
            Dictionary<string, bool> expandValues = GetValues(instance.ExpandNames, instance.ExpandValues);
            Dictionary<string, bool> newExpandValues = new Dictionary<string, bool>();

            DrawExpand(new ILHotMonoProxyStruct(
                stringValues,
                newStringValues,
                floatValues,
                newFloatValues,
                boolValues,
                newBoolValues,
                objectValues,
                newObjectValues,
                expandValues,
                newExpandValues), classInfo, "", classNameProperty, true);
            
            SetKeysAndValues(newStringValues, "_stringNames", "_stringValues", (p, v) => p.stringValue = v);
            SetKeysAndValues(newFloatValues, "_floatNames", "_floatValues", (p, v) => p.floatValue = v);
            SetKeysAndValues(newBoolValues, "_boolNames", "_boolValues", (p, v) => p.boolValue = v);
            SetKeysAndValues(newObjectValues, "_objectNames", "_objectValues", (p, v) => p.objectReferenceValue = v);
            SetKeysAndValues(newExpandValues, "_expandNames", "_expandValues", (p, v) => p.boolValue = v);

            serializedObject.ApplyModifiedProperties();
        }

        void DrawExpand(ILHotMonoProxyStruct monoStruct, IrpClassInfo classInfo, string baseName, SerializedProperty serializedProperty, bool draw)
        {
            foreach (var field in classInfo.Fields)
            {
                var fieldKey = string.Concat(baseName, field.FieldName);
                if (field.FieldType == typeof(string))
                {
                    DrawText(field.FieldName, fieldKey, monoStruct.StringValues, monoStruct.NewStringValues, draw);
                }
                else if (field.FieldType == typeof(float))
                {
                    DrawFloat(field.FieldName, fieldKey, monoStruct.FloatValues, monoStruct.NewFloatValues, draw);
                }
                else if (field.FieldType == typeof(bool))
                {
                    DrawBool(field.FieldName, fieldKey, monoStruct.BoolValues, monoStruct.NewBoolValues, draw);
                }
                else if (field.FieldType == typeof(UnityEngine.Object) || field.FieldType.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    DrawObject(field.FieldName, fieldKey, field.FieldType, monoStruct.ObjectValues, monoStruct.NewObjectValues, draw);
                }
                else if (field.FieldType.GetCustomAttributes(typeof(ILHotMonoSerilizableAttribute), false).Length != 0)
                {
                    var drawClass = field.ClassInfo;

                    var currentValue = true;
                    if (monoStruct.ExpandValues.ContainsKey(fieldKey))
                        currentValue = monoStruct.ExpandValues[fieldKey];

                    currentValue    = EditorGUILayout.Foldout(currentValue, field.FieldName);
                    if (currentValue)
                    {
                        EditorGUI.indentLevel += 1;
                        DrawExpand(monoStruct, drawClass, string.Concat(fieldKey, "."), serializedProperty, true);
                        EditorGUI.indentLevel -= 1;
                    }
                    else
                    {
                        DrawExpand(monoStruct, drawClass, string.Concat(fieldKey, "."), serializedProperty, false);
                    }
                    monoStruct.NewExpandValues.Add(fieldKey, currentValue);
                }
            }
        }

        void SetKeysAndValues<T>(Dictionary<string, T> dict,
            string namePropName, string valuePropName, Action<SerializedProperty, T> setValueCallback)
        {
            var nameProp = serializedObject.FindProperty(namePropName);
            SetSerializedPropertArraySize(nameProp, dict.Count);
            var valueProp = serializedObject.FindProperty(valuePropName);
            SetSerializedPropertArraySize(valueProp, dict.Count);

            var keys = dict.Keys.ToList();
            for (var i = 0; i < keys.Count; i++)
            {
                var k = keys[i];
                var v = dict[k];
                nameProp.GetArrayElementAtIndex(i).stringValue = k;
                setValueCallback(valueProp.GetArrayElementAtIndex(i), v);
            }
        }

        void SetSerializedPropertArraySize(SerializedProperty p, int size)
        {
            for (var i = p.arraySize; i < size; i++)
            {
                p.InsertArrayElementAtIndex(i);
            }
            while (p.arraySize > size)
            {
                p.DeleteArrayElementAtIndex(p.arraySize - 1);
            }
        }

        void DrawText(string fieldName, string fieldKey, Dictionary<string, string> preValues, Dictionary<string, string> newValues, bool draw)
        {
            var currentValue = "";
            if (preValues.ContainsKey(fieldKey))
                currentValue = preValues[fieldKey];

            if (draw)
                currentValue = EditorGUILayout.TextField(fieldName, currentValue);
            newValues.Add(fieldKey, currentValue);
        }

        void DrawFloat(string fieldName, string fieldKey, Dictionary<string, float> pregValues, Dictionary<string, float> newValues, bool draw)
        {
            float currentValue = 0;
            if (pregValues.ContainsKey(fieldKey))
                currentValue = pregValues[fieldKey];

            if (draw)
                currentValue = EditorGUILayout.FloatField(fieldName, currentValue);
            newValues.Add(fieldKey, currentValue);
        }
        void DrawBool(string fieldName, string fieldKey, Dictionary<string, bool> pregValues, Dictionary<string, bool> newValues, bool draw)
        {
            bool currentValue = false;
            if (pregValues.ContainsKey(fieldKey))
                currentValue = pregValues[fieldKey];

            if (draw)
                currentValue = EditorGUILayout.Toggle(fieldName, currentValue);
            newValues.Add(fieldKey, currentValue);
        }
        void DrawObject(string fieldName, string fieldKey, Type objType, Dictionary<string, UnityEngine.Object> pregValues, Dictionary<string, UnityEngine.Object> newValues, bool draw)
        {
            UnityEngine.Object currentValue = null;
            if (pregValues.ContainsKey(fieldKey))
                currentValue = pregValues[fieldKey];

            if (draw)
                currentValue = EditorGUILayout.ObjectField(fieldName, currentValue, objType, true);
            newValues.Add(fieldKey, currentValue);
        }

        Dictionary<string, T> GetValues<T>(List<string> names, List<T> values)
        {
            var ret = new Dictionary<string, T>();
            if (names != null && values != null)
            {
                for (var i = 0; i < names.Count; i++)
                {
                    ret.Add(names[i], values[i]);
                }
            }
            return ret;
        }
    }
}

