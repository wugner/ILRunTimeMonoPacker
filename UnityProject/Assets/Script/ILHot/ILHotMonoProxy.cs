using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ILHot
{
	public class ILHotMonoProxy : MonoBehaviour
	{
		protected ILType _ilType;
		protected Type _instanceType;
		protected object _instance;

		[SerializeField]
		protected string _runtimeClassName;
		public string RuntimeClassName { get { return _runtimeClassName; } }
        
        [SerializeField]
		List<string> _stringNames;
		public List<string> StringNames { set { _stringNames = value; } get { return _stringNames; } }
		[SerializeField]
		List<string> _stringValues;
		public List<string> StringValues { set { _stringValues = value; } get { return _stringValues; } }

		[SerializeField]
		List<string> _floatNames;
		public List<string> FloatNames { set { _floatNames = value; } get { return _floatNames; } }
		[SerializeField]
		List<float> _floatValues;
		public List<float> FloatValues { set { _floatValues = value; } get { return _floatValues; } }

		[SerializeField]
		List<string> _boolNames;
		public List<string> BoolNames { set { _boolNames = value; } get { return _boolNames; } }
		[SerializeField]
		List<bool> _boolValues;
		public List<bool> BoolValues { set { _boolValues = value; } get { return _boolValues; } }

		[SerializeField]
		List<string> _objectNames;
		public List<string> ObjectNames { set { _objectNames = value; } get { return _objectNames; } }
		[SerializeField]
		List<UnityEngine.Object> _objectValues;
		public List<UnityEngine.Object> ObjectValues { set { _objectValues = value; } get { return _objectValues; } }

        [SerializeField]
        List<string> _expandNames;
        public List<string> ExpandNames { set { _expandNames = value; } get { return _expandNames; } }
        [SerializeField]
        List<bool> _expandValues;
        public List<bool> ExpandValues { set { _expandValues = value; } get { return _expandValues; } }

        [SerializeField]
        List<string> _ilObjectNames;
        public List<string> ILObjectNames { set { _ilObjectNames = value; } get { return _ilObjectNames; } }
        [SerializeField]
        List<Component> _ilObjectValues;
        public List<Component> ILObjectValues { set { _ilObjectValues = value; } get { return _ilObjectValues; } }

        public List<string> EventNames;
		public List<UnityEngine.Events.UnityEvent> EventValues;

		private ILRuntime.Runtime.Enviorment.AppDomain _appdomain;

		private void Awake()
		{
			_appdomain = ILRunTimeSingleTon.Domain;
			if (_appdomain != null)
			{
				_ilType = (ILType)ILRunTimeSingleTon.Domain.GetType(_runtimeClassName);
				_instanceType = _ilType.ReflectionType;
				_instance = GetInstance(_runtimeClassName);
			}
			else
			{
				Assembly assembly1 = Assembly.Load("SampleHotProject1");
                Assembly assembly2 = Assembly.Load("SampleHotProject2");

                _instanceType = assembly1.GetType(_runtimeClassName);
                if (_instanceType == null)
                    _instanceType = assembly2.GetType(_runtimeClassName);

				_instance = GetInstance(_runtimeClassName);
			}

            Call("RuntimeMonoInit", false, this);

			SetFieldValueFromArray(_floatNames, _floatValues);
			SetFieldValueFromArray(_stringNames, _stringValues);
			SetFieldValueFromArray(_boolNames, _boolValues);
			SetFieldValueFromArray(_objectNames, _objectValues);

			CallMethod("Awake");
		}

        object GetInstance(string runtimeClassName)
        {
            object instance;

            if (_appdomain != null)
            {
                Debug.Log("runtimeClassName " + runtimeClassName);
                instance = _appdomain.Instantiate(runtimeClassName);
            }
            else
            {
                Assembly assembly1 = Assembly.Load("SampleHotProject1");
                Assembly assembly2 = Assembly.Load("SampleHotProject2");
                var instanceType = assembly1.GetType(runtimeClassName);
                if (instanceType == null)
                    instanceType = assembly2.GetType(runtimeClassName);

                instance = Activator.CreateInstance(instanceType);
            }

            return instance;
        }

		void SetFieldValueFromArray(List<string> names, IList values)
		{
			if (values.Count == 0)
				return;
			for (var i = 0; i < names.Count; i++)
			{
				var n = names[i];
				var v = values[i];
				SetFieldValue(n, v);
			}
		}

		void SetFieldValue(string fieldName, object value)
		{
            Debug.Log("fieldName " + fieldName);
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
                            tmpInstance = GetInstance(type.FullName);

                        field.SetValue(instance, tmpInstance);

                        instance = tmpInstance;
                    }
                }
            }
            else
				field.SetValue(instance, value);
        }
        
        private void Start()
		{
			CallMethod("Start");
            CallMethod("PrintTest");
        }

		private void OnEnable()
		{
			CallMethod("OnEnable");
		}
        
        private void OnDisable()
		{
			CallMethod("OnDisable");
		}

		private void OnDestroy()
		{
			CallMethod("OnDestroy");
		}
        
		public void CallMethod(string methodName, bool requirReceiver = false)
		{
			Call(methodName, requirReceiver);
		}

		public void CallMethodWithArgs(string methodName, bool requirReceiver = false, params object[] args)
		{
			Call(methodName, requirReceiver, args);
		}
        
		protected void Call(string methodName, bool requireReceiver, params object[] args)
		{
			if (_appdomain != null)
			{
				var method = _ilType.GetMethod(methodName);
				if (method != null)
					_appdomain.Invoke(method, _instance, args);
				else if (requireReceiver)
					throw new Exception(string.Format("Can not get method {0} for runtime mono type {1}", methodName, _runtimeClassName));
			}
			else
			{
				var method = _instanceType.GetMethod(methodName);
				if (method != null)
					method.Invoke(_instance, args);
				else if (requireReceiver)
					throw new Exception(string.Format("Can not get method {0} for runtime mono type {1}", methodName, _runtimeClassName));
			}
		}
	}
}
