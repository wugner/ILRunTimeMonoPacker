using PartialTest;
using SampleHotProject1;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SampleHotProject2
{
    public class Demo2 : SampleHotProject1.Demo1
    {
        public static string Test2(string tmpStr)
        {
            return string.Concat("IL1.Test1:" + tmpStr);
        }

        public static string CallIL1Method(string tmpStr)
        {
            return Test1(tmpStr);
        }

        public static string CycCallFunc()
        {
            try
            {
                var obj = PlayModeTest.GetTestVector();
                if (obj == null)
                    return "obj = null";
                // var vector = (MyVector2)obj;
                return string.Concat("IL2.CycCallFunc:" + obj);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static string UnityInterfaceTest()
        {
            var interfaceClass = PlayModeTest.GetInterfaceClass();
            var interfaceObj = PlayModeTest.GetInterfaceObj();
            return string.Concat("IL2.UnityInterfaceTest:", interfaceClass is UnityBaseInterface, "   ", interfaceObj is UnityBaseInterface);
        }

        public static string ILInterfaceTest()
        {
            var interfaceClass = new ILInterface();
            object interfaceObj = new ILInterface();
            return string.Concat("IL2.ILInterfaceTest:", interfaceClass is ILInterface, "   ", interfaceObj is ILInterface);
        }

        public static string ILExtraFuncTest()
        {
            var alpha = new ILAI();
            var beta = new ILAI() { Name = "Beta" };
            return string.Concat("IL2.ExtraFuncTest:" + alpha.GetILAIName() + "   " + beta.GetILAIName());
        }

        public static string UnityExtraFuncTest()
        {
            var nan = new UnityAI();
            var jeck = new UnityAI() { Name = "Jeck" };
            return string.Concat("IL2.UnityExtraFuncTest:" + nan.GetUnityAIName() + "   " + jeck.GetUnityAIName());
        }
        
        public static bool SortTest()
        {
            List<int> intList = new List<int>() { 3, 2, 5, 1};
            intList.ILSort((arg1, arg2) => ((int)arg1 - (int)arg2));

            foreach (var v in intList)
                Debug.Log("intList " + v);
            return intList[0] == 1 && intList[1] == 2 && intList[2] == 3 && intList[3] == 5;
        }
    }
}
