using ILHot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SampleHotProject1
{
    [ILHotMonoProxy]
    [ILHotComponentMenu(MenuPath = "DemoTestObjComponent")]
    public class DemoTestObj : MonoBehaviour
    {
        [ILHotSerializeField]
        public string DemoStr;
    }
    
    public class DemoTest
    {
        [ILHotSerializeField]
        public string DemoStr;
        [ILHotSerializeField]
        public bool DemoBool;
        [ILHotSerializeField]
        public float DemoFloat;
        [ILHotSerializeField]
        public DemoTestEx TestEx;
    }
    
    public class DemoTestEx
    {
        [ILHotSerializeField]
        public string LadyGaGa;
    }
    
    [ILHotMonoProxy]
    [ILHotComponentMenu(MenuPath = "DemoTestComponent")]
    public class Demo1
    {
        [ILHotSerializeField]
        public string TempStr;

        [ILHotSerializeField]
        public UnityEngine.Object Obj;

        [ILHotSerializeField]
        public DemoTest Demo;

        [ILHotSerializeField]
        public DemoTestObj DemoObj;

        public void PrintTest()
        {
            Debug.Log("PrintTest Demo.DemoStr " + Demo.DemoStr);
            Debug.Log("PrintTest Demo.DemoBool " + Demo.DemoBool);
            Debug.Log("PrintTest Demo.DemoFloat " + Demo.DemoFloat);
            Debug.Log("PrintTest DemoObj.DemoStr " + DemoObj.DemoStr);
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
    
    [ILHotComponentMenu(MenuPath = "Demo1TestComponent")]
    public class Demo1Test
    {
        [ILHotSerializeField]
        public string TempStr;
        
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
        
        public static string Test1(string tmpStr)
        {
            return string.Concat("IL1.Test1:" + tmpStr);
        }

        public static string GetVector()
        {
            return "vector";
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
