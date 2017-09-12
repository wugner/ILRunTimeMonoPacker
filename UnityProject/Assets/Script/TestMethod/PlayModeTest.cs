using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PartialTest;
using System.Reflection;
using System;
using ILHotAttribute;
using ILHot;
using System.Linq;

public class PlayModeTest{
    [RuntimeInitializeOnLoadMethod]
    public static void PreLoadDLl()
    {
        var preference = Resources.Load<ILHotPreference>("ILHotPreference");
        var dllList = preference.ILHotDLLList.Select(ilPath =>
        {
            return Constant.AppDataPath + "\\" + ilPath;
        }).ToList();

        foreach (var dllPath in dllList)
        {
            byte[] dllBytes = File.ReadAllBytes(dllPath);
            ILRunTimeSingleTon.Instance.LoadHotDLL(dllBytes);
        }
    }

	[Test]
	public void CallILMethodTestSimplePasses()
    {
        if (ILRunTimeSingleTon.Domain != null)
        {
            var ret1 = ILRunTimeSingleTon.Domain.Invoke("SampleHotProject1.Demo1", "Test1", null, "Tom");
            Debug.Log(string.Concat("[CallILMethodTestSimplePasses] Call dll 1:" + ret1));

            var ret2 = ILRunTimeSingleTon.Domain.Invoke("SampleHotProject2.Demo2", "Test2", null, "Timy");
            Debug.Log(string.Concat("[CallILMethodTestSimplePasses] Call dll 2:" + ret2));
        }
    }

    [Test]
    public void IL2CallIL1MethodTestSimplePasses()
    {
        if (ILRunTimeSingleTon.Domain != null)
        {
            var ret = ILRunTimeSingleTon.Domain.Invoke("SampleHotProject2.Demo2", "CallIL1Method", null, "Timy");
            Debug.Log(string.Concat("[IL2CallIL1MethodTestSimplePasses] Call dll 2:" + ret));
        }
    }

    [Test]
    public void CycCallMethodTestSimplePasses()
    {
        if (ILRunTimeSingleTon.Domain != null)
        {
            var ret = ILRunTimeSingleTon.Domain.Invoke("SampleHotProject2.Demo2", "CycCallFunc", null);
            Debug.Log(string.Concat("[CycCallMethodTestSimplePasses] Call dll 2:", ret));
        }
    }

    public static string GetTempStr()
    {
        return "Test str";
    }
    
    public static object GetTestVector()
    {
        Debug.Log("GetTestVector");
        // var obj = ILRunTimeSingleTon.Domain.Invoke("SampleHotProject1.Demo1", "GetVector", null);
        var obj = "hello";
        return obj;
    }

    [Test]
    public void UnityInterfaceMethodTestSimplePasses()
    {
        if (ILRunTimeSingleTon.Domain != null)
        {
            var ret = ILRunTimeSingleTon.Domain.Invoke("SampleHotProject2.Demo2", "UnityInterfaceTest", null);
            Debug.Log(string.Concat("[UnityInterfaceMethodTestSimplePasses] Call dll 2:", ret));
        }
    }

    [Test]
    public void ILInterfaceMethodTestSimplePasses()
    {
        if (ILRunTimeSingleTon.Domain != null)
        {
            var ret = ILRunTimeSingleTon.Domain.Invoke("SampleHotProject2.Demo2", "ILInterfaceTest", null);
            Debug.Log(string.Concat("[ILInterfaceMethodTestSimplePasses] Call dll 2:", ret));
        }
    }

    [Test]
    public void ILExtraFuncMethodTestSimplePasses()
    {
        if (ILRunTimeSingleTon.Domain != null)
        {
            var ret = ILRunTimeSingleTon.Domain.Invoke("SampleHotProject2.Demo2", "ILExtraFuncTest", null);
            Debug.Log(string.Concat("[ILExtraFuncMethodTestSimplePasses] Call dll 2:", ret));
        }
    }
    
    [Test]
    public void UnityExtraFuncMethodTestSimplePasses()
    {
        if (ILRunTimeSingleTon.Domain != null)
        {
            var ret = ILRunTimeSingleTon.Domain.Invoke("SampleHotProject2.Demo2", "UnityExtraFuncTest", null);
            Debug.Log(string.Concat("[UnityExtraFuncMethodTestSimplePasses] Call dll 2:", ret));
        }
    }

    [Test]
    public void SortTestMethodTestSimplePasses()
    {
        if (ILRunTimeSingleTon.Domain != null)
        {
            var ret = (bool)ILRunTimeSingleTon.Domain.Invoke("SampleHotProject2.Demo2", "SortTest", null);
            if (ret == false)
                throw new System.Exception("il sort error");
            Debug.Log(string.Concat("[SortTestMethodTestSimplePasses] Call dll 2:", ret));
        }
    }

    [Test]
    public void SerializeDLLAttributeMethodTestSimplePasses()
    {
        var preference = Resources.Load<ILHotPreference>("ILHotPreference");
        var dllList = preference.ILHotDLLList.Select(ilPath =>
        {
            return Constant.AppDataPath + "\\" + ilPath;
        }).ToList();
        dllList.Add(Constant.AppDataPath + "\\..\\Library\\ScriptAssemblies\\Assembly-CSharp.dll");

        ReflectInfoGenerator.Reflect.LoadDLL(dllList);
        ReflectInfoGenerator.Reflect.SetOutPutPath(@"D:\ILRunTimeMonoPacker\UnityProject\Assets\Import\ILHot\ReflectInfoFiles", "AttILDLL.json");
        var result = ReflectInfoGenerator.Reflect.FormatByAttribute("ILHotAttribute.ILHotMonoSerilizableAttribute");
        if (result != "Succeed")
            throw new System.Exception(result);
        Debug.Log(string.Concat("[SerializeDLLAttributeMethodTestSimplePasses] Call FormatByAttribute:", result));
    }

    [Test]
    public void SerializeDLLClassMethodTestSimplePasses()
    {
        var preference = Resources.Load<ILHotPreference>("ILHotPreference");
        var dllList = preference.ILHotDLLList.Select(ilPath =>
        {
            return Constant.AppDataPath + "\\" + ilPath;
        }).ToList();
        dllList.Add(Constant.AppDataPath + "\\..\\Library\\ScriptAssemblies\\Assembly-CSharp.dll");

        ReflectInfoGenerator.Reflect.LoadDLL(dllList);
        ReflectInfoGenerator.Reflect.SetOutPutPath(@"D:\ILRunTimeMonoPacker\UnityProject\Assets\Import\ILHot\ReflectInfoFiles", "BassClassILDLL.json");
        var result = ReflectInfoGenerator.Reflect.FormatByBaseClass("SampleHotProject1.Demo1");
        if (result != "Succeed")
            throw new System.Exception(result);
        Debug.Log(string.Concat("[SerializeDLLClassMethodTestSimplePasses] Call FormatByBaseClass:", result));
    }

    [Test]
    public void SerializeDLLInterfaceMethodTestSimplePasses()
    {
        var preference = Resources.Load<ILHotPreference>("ILHotPreference");
        var dllList = preference.ILHotDLLList.Select(ilPath =>
        {
            return Constant.AppDataPath + "\\" + ilPath;
        }).ToList();
        dllList.Add(Constant.AppDataPath + "\\..\\Library\\ScriptAssemblies\\Assembly-CSharp.dll");

        ReflectInfoGenerator.Reflect.LoadDLL(dllList);
        ReflectInfoGenerator.Reflect.SetOutPutPath(@"D:\ILRunTimeMonoPacker\UnityProject\Assets\Import\ILHot\ReflectInfoFiles", "InterfaceILDLL.json");
        var result = ReflectInfoGenerator.Reflect.FormatByInterface("SampleHotProject2.ILBaseInterface");
        if (result != "Succeed")
            throw new System.Exception(result);
        Debug.Log(string.Concat("[SerializeDLLInterfaceMethodTestSimplePasses] Call FormatByInterface:", result));
    }
    
    [Test]
    public void SerializeDLLMonoInterfaceMethodTestSimplePasses()
    {
        var preference = Resources.Load<ILHotPreference>("ILHotPreference");
        var dllList = preference.ILHotDLLList.Select(ilPath =>
        {
            return Constant.AppDataPath + "\\" + ilPath;
        }).ToList();
        dllList.Add(Constant.AppDataPath + "\\..\\Library\\ScriptAssemblies\\Assembly-CSharp.dll");

        ReflectInfoGenerator.Reflect.LoadDLL(dllList);
        ReflectInfoGenerator.Reflect.SetOutPutPath(@"D:\ILRunTimeMonoPacker\UnityProject\Assets\Import\ILHot\ReflectInfoFiles", "MonoReflectInfo.json");
        var result = ReflectInfoGenerator.Reflect.FormatMono();
        if (result != "Succeed")
            throw new System.Exception(result);
        Debug.Log(string.Concat("[SerializeDLLMonoInterfaceMethodTestSimplePasses] Call FormatMono: ", result));
    }

    [Test]
    public void SerializeDLLPlayMakerInterfaceMethodTestSimplePasses()
    {
        var preference = Resources.Load<ILHotPreference>("ILHotPreference");
        var dllList = preference.ILHotDLLList.Select(ilPath =>
        {
            return Constant.AppDataPath + "\\" + ilPath;
        }).ToList();
        dllList.Add(Constant.AppDataPath + "\\..\\Library\\ScriptAssemblies\\Assembly-CSharp.dll");
        
        ReflectInfoGenerator.Reflect.LoadDLL(dllList);
        ReflectInfoGenerator.Reflect.SetOutPutPath(@"D:\ILRunTimeMonoPacker\UnityProject\Assets\Import\ILHot\ReflectInfoFiles", "FsmReflectInfo.json");
        var result = ReflectInfoGenerator.Reflect.FormatPlayMaker();
        if (result != "Succeed")
            throw new System.Exception(result);
        Debug.Log(string.Concat("[SerializeDLLPlayMakerInterfaceMethodTestSimplePasses] Call FormatPlayMaker: ", result));
    }

    public static UnityInterface GetInterfaceClass()
    {
        return new UnityInterface() {
           Name     = "Unity",
           Score    = 100
        };
    }

    public static object GetInterfaceObj()
    {
        return new UnityInterface();
    }
    
    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
	public IEnumerator PlayModeTestWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return null;
	}
}