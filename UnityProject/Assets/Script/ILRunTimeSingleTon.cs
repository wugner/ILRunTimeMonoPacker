using ILRuntime.Runtime.Intepreter;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ILRunTimeSingleTon : MonoBehaviour {
    static ILRunTimeSingleTon _instance;

    public static ILRunTimeSingleTon Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ILRunTimeSingleTon>();
                if (_instance == null)
                {
                    var go = new GameObject("ILRunTimeSingleTom");
                    _instance = go.AddComponent<ILRunTimeSingleTon>();
                }
                DontDestroyOnLoad(_instance);
            }
            return _instance;
        }
    }

    private ILRuntime.Runtime.Enviorment.AppDomain _domain;
    public static ILRuntime.Runtime.Enviorment.AppDomain Domain { get { return Instance._domain; } }

    public void LoadHotDLL(Dictionary<string, byte[]> hotDllList)
    {
        AnalyzeDll(hotDllList["dll"], hotDllList.ContainsKey("pdb") ? hotDllList["pdb"] : null);
    }
    
    public void LoadHotDLL(byte[] dllBytes)
    {
        AnalyzeDll(dllBytes);
    }

    void AnalyzeDll(byte[] dll, byte[] pdb = null)
    {
        if (_domain == null)
            _domain = new ILRuntime.Runtime.Enviorment.AppDomain();
        
        using (MemoryStream ds = new MemoryStream(dll))
        {
            if (pdb != null)
            {
                using (MemoryStream ps = new MemoryStream(pdb))
                {
                    _domain.LoadAssembly(ds, ps, new Mono.Cecil.Pdb.PdbReaderProvider());
                }
            }
            else
            {
                _domain.LoadAssembly(ds);
            }
        }

        RegistDelegate();
    }

    private void RegistDelegate()
    {
        _domain.DelegateManager.RegisterMethodDelegate<string>();
        _domain.DelegateManager.RegisterMethodDelegate<object>();
        _domain.DelegateManager.RegisterMethodDelegate<ILTypeInstance>();
        
        _domain.DelegateManager.RegisterFunctionDelegate<object, int>();
        _domain.DelegateManager.RegisterFunctionDelegate<object, object, int>();
        _domain.DelegateManager.RegisterFunctionDelegate<System.Reflection.Assembly, System.Boolean>();
        _domain.DelegateManager.RegisterFunctionDelegate<System.Reflection.Assembly, System.Type>();
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
