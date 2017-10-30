using ILHot;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Simple : MonoBehaviour {
	void Start () {
        Debug.Log("Simple start");
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
        
        //  if (ILRunTimeSingleTon.Domain != null)
        //  {
        //      var ret1 = ILRunTimeSingleTon.Domain.Invoke("SampleHotProject1.Demo", "Test", null, "Tom");
        //      Debug.Log(string.Concat("Call dll 1:" + ret1));
        //  
        //      var ret2 = ILRunTimeSingleTon.Domain.Invoke("SampleHotProject2.Demo2", "SortTest", null);
        //      Debug.Log(string.Concat("Call dll 2:" + ret2));
        //  }

        var simpleObj = GameObject.Instantiate(Resources.Load("Simple"));
    }
	
	void Update () {
		
	}
}
