using HutongGames.PlayMaker;
using ILRuntime.CLR.TypeSystem;
using System;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using ReflectInfoGenerator;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using System.Linq;

namespace ILHot
{
	[ActionCategory("ILHot_Common")]
	public class ILHotFsmProxy : FsmStateAction
	{
		public FsmEvent TestEv1;
		public FsmEvent TestEv2;

		ILType _iltype;
		object _instance;
		Type _instanceType;

		public string _runtimeClassName;

		public string[] EventFieldNames;
		public FsmEvent[] EventValues;

		public string[] StringFieldNames;
		public FsmString[] StringValues;

		public string[] FloatFieldNames;
		public FsmFloat[] FloatValues;

		public string[] BoolFieldNames;
		public FsmBool[] BoolValues;

		public string[] ObjectFieldNames;
		public FsmObject[] ObjectValues;

        public string[] ExpandNames;
        public FsmBool[] ExpandValues;

		public override void Awake()
		{
            Debug.Log("ILHotFsmStateActionProxy Awake");
            base.Awake();
			if (!Application.isPlaying)
				return;

			if (ILRunTimeSingleTon.Domain != null)
			{
				_iltype = (ILType)ILRunTimeSingleTon.Domain.GetType(_runtimeClassName);
                if(_iltype != null)
				    _instanceType = _iltype.ReflectionType;
				_instance = ILRunTimeSingleTon.Domain.Instantiate(_runtimeClassName);
			}
			else
			{
                Assembly assembly1 = Assembly.Load("SampleHotProject1");
                Assembly assembly2 = Assembly.Load("SampleHotProject2");

                _instanceType = assembly1.GetType(_runtimeClassName);
                if (_instanceType == null)
                    _instanceType = assembly2.GetType(_runtimeClassName);

                _instance = Activator.CreateInstance(_instanceType);
			}

			if (EventFieldNames != null && EventFieldNames.Length != 0)
			{
				for (var i = 0; i < EventFieldNames.Length; i++)
				{
					var fname = EventFieldNames[i];
					var ev = EventValues[i];
					SetFieldValue(fname, ev);
				}
			}

			if (StringFieldNames != null && StringFieldNames.Length != 0)
			{
				for (var i = 0; i < StringFieldNames.Length; i++)
				{
					SetFieldValue(StringFieldNames[i], StringValues[i]);
				}
			}

			if (FloatFieldNames != null && FloatFieldNames.Length != 0)
			{
				for (var i = 0; i < FloatFieldNames.Length; i++)
				{
					SetFieldValue(FloatFieldNames[i], FloatValues[i]);
				}
			}

			if (BoolFieldNames != null && BoolFieldNames.Length != 0)
			{
				for (var i = 0; i < BoolFieldNames.Length; i++)
				{
					SetFieldValue(BoolFieldNames[i], BoolValues[i]);
				}
			}

			if (ObjectFieldNames != null && ObjectFieldNames.Length != 0)
			{
				for (var i = 0; i < ObjectFieldNames.Length; i++)
				{
					SetFieldValue(ObjectFieldNames[i], ObjectValues[i]);
				}
			}
            
			CallMethod("Init", this);
			CallMethod("Awake");
		}

		void SetFieldValue(string fieldName, object value)
		{
			var field = _instanceType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var instance = _instance;
            if (field == null)
            {
                var nameList = fieldName.Split('.');
                var type = _instanceType;
                for (var index = 0; index < nameList.Length; index++)
                {
                    field = type.GetField(nameList[index], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    type = field.FieldType;

                    if (index == nameList.Length - 1)
                        field.SetValue(instance, value);
                    else
                    {
                        var tmpInstance = field.GetValue(instance);
                        if (tmpInstance == null)
                            tmpInstance = Activator.CreateInstance(type);

                        field.SetValue(instance, tmpInstance);

                        instance = tmpInstance;
                    }
                }
            }
            else
			    field.SetValue(_instance, value);
		}

		public override void OnEnter()
		{
			base.OnEnter();
			CallMethod("OnEnter");
		}
		public override void OnExit()
		{
			base.OnExit();
			CallMethod("OnExit");
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			CallMethod("OnUpdate");
		}

		public override void OnLateUpdate()
		{
			base.OnLateUpdate();
			CallMethod("OnLateUpdate");
		}

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            CallMethod("OnFixedUpdate");
        }
        
        public override void DoCollisionEnter(Collision collisionInfo)
		{
			base.DoCollisionEnter(collisionInfo);
			CallMethod("DoCollisionEnter", collisionInfo);
		}
		public override void DoCollisionStay(Collision collisionInfo)
		{
			base.DoCollisionStay(collisionInfo);
			CallMethod("DoCollisionStay", collisionInfo);
		}
		public override void DoCollisionExit(Collision collisionInfo)
		{
			base.DoCollisionExit(collisionInfo);
			CallMethod("DoCollisionExit", collisionInfo);
		}

		public override void DoTriggerEnter(Collider other)
		{
			base.DoTriggerEnter(other);
			CallMethod("DoTriggerEnter", other);
		}
		public override void DoTriggerExit(Collider other)
		{
			base.DoTriggerExit(other);
			CallMethod("DoTriggerExit", other);
		}
		public override void DoTriggerStay(Collider other)
		{
			base.DoTriggerStay(other);
			CallMethod("DoTriggerStay", other);
		}
		
		void CallMethod(string methodName, params object[] args)
		{
			if (ILRunTimeSingleTon.Domain != null)
			{
                if (_iltype != null)
                {
                    var method = _iltype.GetMethod(methodName);
                    if (method != null)
                        ILRunTimeSingleTon.Domain.Invoke(_runtimeClassName, methodName, _instance, args);
                }

			}
			else
			{
				_instanceType.GetMethod(methodName).Invoke(_instance, args);
			}
		}

		public bool OnReceivePhotonMessage(string methodName, params object[] args)
		{
			if(ILRunTimeSingleTon.Domain != null)
			{
				var method = _iltype.GetMethod(methodName);
				if (method == null)
					return false;

				ILRunTimeSingleTon.Domain.Invoke(method, _instance, args);
				return true;
			}
			else
			{
				var method = _instanceType.GetMethod(methodName);
				if (method == null)
					return false;

				method.Invoke(_instanceType, args);
				return true;
			}
		}
		
		public override string ErrorCheck()
		{
            if (string.IsNullOrEmpty(_runtimeClassName))
                return "Please select a type!";
            else
            {
                List<IrpClassInfo> _classInfoList = new List<IrpClassInfo>();
                var ilDLLFsmJsonPath = EditorPrefs.GetString("ILDLLFsmJsonPath", "");

                if (File.Exists(ilDLLFsmJsonPath))
                {
                    StreamReader sr = new StreamReader(ilDLLFsmJsonPath);
                    _classInfoList = JsonConvert.DeserializeObject<List<IrpClassInfo>>(sr.ReadToEnd());
                    if (_classInfoList.FindAll(info => info.ClassName == _runtimeClassName).Count() == 0)
                    {
                        return _runtimeClassName + " is deleted!";
                    }
                }
            }
			return null;
		}
	}
}