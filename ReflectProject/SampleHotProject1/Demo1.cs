using ILHotAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SampleHotProject1
{
    [ILHotMonoSerilizable(DisplayName = "TestObj", SubClass = false)]
    public class DemoTestObj : UnityEngine.Object
    {
        [SerializeField]
        public string DemoStr;
    }

    [ILHotMonoSerilizable(DisplayName = "Test", SubClass = true)]
    public class DemoTest
    {
        [SerializeField]
        public string DemoStr;
        [SerializeField]
        public bool DemoBool;
        [SerializeField]
        public float DemoFloat;
        [SerializeField]
        public DemoTestEx TestEx;
    }

    [ILHotMonoSerilizable(DisplayName = "Test", SubClass = true)]
    public class DemoTestEx
    {
        [SerializeField]
        public string LadyGaGa;
    }

    [ILHotMonoSerilizable(DisplayName = "DemoTest")]
    public class Demo1
    {
        [SerializeField]
        public string TempStr;

        [SerializeField]
        public UnityEngine.Object Obj;

        [SerializeField]
        public DemoTest Demo;

        [SerializeField]
        public DemoTestObj DemoObj;

        public void PrintTest()
        {
            Debug.Log("PrintTest Demo.DemoStr " + Demo.DemoStr);
            Debug.Log("PrintTest Demo.DemoBool " + Demo.DemoBool);
            Debug.Log("PrintTest Demo.DemoFloat " + Demo.DemoFloat);
            Debug.Log("PrintTest Demo.TestEx.LadyGaGa " + Demo.TestEx.LadyGaGa);
        }

        public void Start()
        {
            Debug.Log("SampleHotProject1.Demo1.Start");
        }

        public void OnEnable()
        {
            Debug.Log("SampleHotProject1.Demo1.OnEnable");
        }

        public virtual string VirtualTest(string nameStr)
        {
            return string.Concat("IL1.VirtualTest:" + nameStr);
        }

        [ILHotSerilizableMethod]
        public static string Test1(string tmpStr)
        {
            return string.Concat("IL1.Test1:" + tmpStr);
        }

        public static MyVector2 GetVector()
        {
            return new MyVector2()
            {
                x = 1,
                y = 2
            };
        }
    }

    [ILHotMonoSerilizable(DisplayName = "DemoTest")]
    public class Demo1Test
    {
        [SerializeField]
        public string TempStr;

        //  [SerializeField]
        //  public UnityEngine.Object Obj;
        
        public void Start()
        {
            Debug.Log("SampleHotProject1.Demo1.Start");
        }

        public void OnEnable()
        {
            Debug.Log("SampleHotProject1.Demo1.OnEnable");
        }

        public virtual string VirtualTest(string nameStr)
        {
            return string.Concat("IL1.VirtualTest:" + nameStr);
        }

        [ILHotSerilizableMethod]
        public static string Test1(string tmpStr)
        {
            return string.Concat("IL1.Test1:" + tmpStr);
        }

        public static string GetVector()
        {
            return "vector";
            //  return new MyVector2()
            //  {
            //      x = 1,
            //      y = 2
            //  };
        }
    }

    public class MyVector2
    {
        public int x;
        public int y;

        public string GetString()
        {
            return string.Format("{0} : {1}", x, y);
        }
    }
}
