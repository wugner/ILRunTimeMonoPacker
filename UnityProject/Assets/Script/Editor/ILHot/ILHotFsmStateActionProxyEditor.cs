using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMakerEditor;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Reflection;
using HutongGames.PlayMaker;
using System.Collections.Generic;
using System.Linq;
using ReflectInfoGenerator;
using Newtonsoft.Json;
using ILHotAttribute;

namespace ILHot
{
	[CustomActionEditor(typeof(ILHotFsmStateActionProxy))]
	public class ILHotFsmStateActionProxyEditor : CustomActionEditor
	{
        class ILHotFsmProxyStruct
        {
            public Dictionary<string, NamedVariable>    PreviousVariableValueMapping;
            public Dictionary<string, NamedVariable>    NewVariableValueMapping;

            public Dictionary<string, FsmEvent>         PreviousEventValueMapping;
            public Dictionary<string, FsmEvent>         NewEventValueMapping;

            public Dictionary<string, NamedVariable> PreviousExpandValues;
            public Dictionary<string, NamedVariable> NewExpandValues;

            public ILHotFsmProxyStruct(
                Dictionary<string, NamedVariable> previousVariableValueMapping,
                Dictionary<string, NamedVariable> newVariableValueMapping,
                Dictionary<string, FsmEvent> previousEventValueMapping,
                Dictionary<string, FsmEvent> newEventValueMapping,
                Dictionary<string, NamedVariable> previousExpandValues,
                Dictionary<string, NamedVariable> newExpandValues
                )
            {
                PreviousVariableValueMapping    = previousVariableValueMapping;
                NewVariableValueMapping         = newVariableValueMapping;
                PreviousEventValueMapping       = previousEventValueMapping;
                NewEventValueMapping            = newEventValueMapping;
                PreviousExpandValues            = previousExpandValues;
                NewExpandValues                 = newExpandValues;
            }
        }

		class VariableInfo
		{
			public Type VariableType;
			public string FieldName;
			public int Index;
			public object RawValue;
		}
        
		public override bool OnGUI()
        {
            if (ILReflectJsonImport.FsmClassInfoList == null)
            {
                return false;
            }

            var action = target as ILHotFsmStateActionProxy;
            int selected = 0;
			if(!string.IsNullOrEmpty(action._runtimeClassName))
				selected = ILReflectJsonImport.FsmClassTypes.IndexOf(action._runtimeClassName);
			
			selected = EditorGUILayout.Popup(selected, ILReflectJsonImport.FsmDisplayTypes);
			
			if (selected <= 0)
            {
                return false;
			}

			if (action._runtimeClassName != ILReflectJsonImport.FsmClassTypes[selected])
				GUI.changed = true;
			
			string className = action._runtimeClassName = ILReflectJsonImport.FsmClassTypes[selected];
			if (className == "null" || string.IsNullOrEmpty(className))
			{
				FsmEditorGUILayout.ReadonlyTextField("Class name is empty");
				return GUI.changed;
			}

            var classInfo = ILReflectJsonImport.FsmClassInfoList.Find(info => info.ClassName == className);
            if (classInfo == null)
			{
				FsmEditorGUILayout.ReadonlyTextField(string.Format("Class {0} is not exist", className));
				return GUI.changed;
			}


            Dictionary<string, NamedVariable> previousVariableValueMapping = new Dictionary<string, NamedVariable>();
            Dictionary<string, NamedVariable> newVariableValueMapping = new Dictionary<string, NamedVariable>();

            Dictionary<string, FsmEvent> previousEventValueMapping = new Dictionary<string, FsmEvent>();
            Dictionary<string, FsmEvent> newEventValueMapping = new Dictionary<string, FsmEvent>();

            Dictionary<string, NamedVariable> previousExpandValues = new Dictionary<string, NamedVariable>();
            Dictionary<string, NamedVariable> newExpandValues = new Dictionary<string, NamedVariable>();
			if (action.EventFieldNames != null)
			{
				for (var i = 0; i < action.StringFieldNames.Length; i++)
				{
					if (i < action.StringValues.Length)
					{
						var v = action.StringValues[i];
						previousVariableValueMapping.Add(action.StringFieldNames[i], v);
					}
				}
				for (var i = 0; i < action.FloatFieldNames.Length; i++)
				{
					if(i < action.FloatValues.Length)
					{
						var v = action.FloatValues[i];
						previousVariableValueMapping.Add(action.FloatFieldNames[i], v);
					}
				}
				for (var i = 0; i < action.BoolFieldNames.Length; i++)
				{
					if (i < action.BoolValues.Length)
					{
						var v = action.BoolValues[i];
						previousVariableValueMapping.Add(action.BoolFieldNames[i], v);
					}
				}
				for (var i = 0; i < action.ObjectFieldNames.Length; i++)
				{
					if (i < action.ObjectValues.Length)
					{
						var v = action.ObjectValues[i];
						previousVariableValueMapping.Add(action.ObjectFieldNames[i], v);
					}
				}
                for (var i = 0; i < action.ExpandNames.Length; i++)
                {
                    if (i < action.ExpandValues.Length)
                    {
                        var v = action.ExpandValues[i];
                        previousExpandValues.Add(action.ExpandNames[i], v);
                    }
                }
                for (var i = 0; i < action.EventFieldNames.Length; i++)
                {
                    var ev = action.EventValues[i];
                    previousEventValueMapping.Add(action.EventFieldNames[i], ev);
                }
			}

            DrawExpand(new ILHotFsmProxyStruct(
                previousVariableValueMapping,
                newVariableValueMapping,
                previousEventValueMapping,
                newEventValueMapping,
                previousExpandValues,
                newExpandValues
                ), classInfo, "", true);

			var evNames = newEventValueMapping.Keys.ToArray();
			action.EventFieldNames = evNames;
			action.EventValues = evNames.Select(n => newEventValueMapping[n]).ToArray();

			ConvertKeyValuesByType(typeof(FsmString), newVariableValueMapping,
				(keys, values) => { action.StringFieldNames = keys; action.StringValues = values.Cast<FsmString>().ToArray(); });

			ConvertKeyValuesByType(typeof(FsmFloat), newVariableValueMapping,
				(keys, values) => { action.FloatFieldNames = keys; action.FloatValues = values.Cast<FsmFloat>().ToArray(); });

			ConvertKeyValuesByType(typeof(FsmBool), newVariableValueMapping,
				(keys, values) => { action.BoolFieldNames = keys; action.BoolValues = values.Cast<FsmBool>().ToArray(); });

			ConvertKeyValuesByType(typeof(FsmObject), newVariableValueMapping,
				(keys, values) => { action.ObjectFieldNames = keys; action.ObjectValues = values.Cast<FsmObject>().ToArray(); });
            
            ConvertKeyValuesByType(typeof(FsmBool), newExpandValues, (keys, values) => { action.ExpandNames = keys; action.ExpandValues = values.Cast<FsmBool>().ToArray(); });
            
			return GUI.changed;
        }

        void DrawExpand(ILHotFsmProxyStruct fsmStruct, IrpClassInfo classInfo, string baseName, bool draw)
        {
            foreach (var field in classInfo.Fields)
            {
                var fieldKey    = string.Concat(baseName, field.FieldName);
                if (field.FieldType == typeof(FsmEvent))
                {
                    DrawEvents(fieldKey, field.FieldName, fsmStruct.PreviousEventValueMapping, fsmStruct.NewEventValueMapping, draw);
                }
                else if (field.FieldType == typeof(FsmString)
                    || field.FieldType == typeof(FsmFloat)
                    || field.FieldType == typeof(FsmBool)
                    || field.FieldType == typeof(FsmObject))
                {
                    DrawNamedVariable(fieldKey, field.FieldName, field.FieldType, fsmStruct.PreviousVariableValueMapping, fsmStruct.NewVariableValueMapping, draw);
                }
                else if (field.FieldType.GetCustomAttributes(typeof(ILHotFsmSerilizableAttribute), false).Length != 0)
                {
                    var drawClass = field.ClassInfo;

                    FsmBool currentValue = true;
                    if (((FsmBool)(fsmStruct.PreviousExpandValues.ContainsKey(fieldKey))).Value)
                        currentValue = (FsmBool)(fsmStruct.PreviousExpandValues[fieldKey]);

                    currentValue.Value = FsmEditorGUILayout.BoldFoldout(currentValue.Value, new GUIContent(field.FieldName));
                    if (currentValue.Value)
                    {
                        EditorGUI.indentLevel += 1;
                        DrawExpand(fsmStruct, drawClass, string.Concat(fieldKey, "."), true);
                        EditorGUI.indentLevel -= 1;
                    }
                    else
                    {
                        DrawExpand(fsmStruct, drawClass, string.Concat(fieldKey, "."), false);
                    }

                    fsmStruct.NewExpandValues.Add(fieldKey, currentValue);
                }
            }

        }

		void ConvertKeyValuesByType(Type valueType, Dictionary<string, NamedVariable> newVariableValueMapping,
			Action<string[], NamedVariable[]> SetFieldCallback)
		{
			var keys = newVariableValueMapping
				.Where(kv => kv.Value != null && kv.Value.GetType() == valueType).Select(kv => kv.Key).ToArray();

			var values = keys.Select(k => newVariableValueMapping[k]).ToArray();

			SetFieldCallback(keys, values);
		}
		
		void DrawEvents(string fieldKey, string fieldName, Dictionary<string, FsmEvent> currentValueMapping, Dictionary<string, FsmEvent> newValueMapping, bool draw)
		{
			FsmEvent ev;
            if (!currentValueMapping.TryGetValue(fieldKey, out ev))
			{
				ev = null;
			}

            if (draw)
                ev = FsmEditorGUILayout.EventPopup(new GUIContent(fieldName), target.Fsm.Events.ToList(), ev);
            newValueMapping.Add(fieldKey, ev);
		}

		void DrawNamedVariable(string fieldKey, string fieldName, Type valueType,
			Dictionary<string, NamedVariable> currentValueMapping, Dictionary<string, NamedVariable> newValueMapping, bool draw)
		{
			NamedVariable value;
            if (!currentValueMapping.TryGetValue(fieldKey, out value))
			{
				value = null;
			}

            if (draw)
                value = FsmEditor.ActionEditor.EditField(fieldName, valueType, value, null) as NamedVariable;
            newValueMapping.Add(fieldKey, value);
		}
	}
}
