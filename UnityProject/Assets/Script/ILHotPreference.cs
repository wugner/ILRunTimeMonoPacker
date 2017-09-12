using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILHot
{
    [CreateAssetMenu]
    public class ILHotPreference : ScriptableObject
    {
        [SerializeField]
        List<string> _ilHotDllList;
        public List<string> ILHotDLLList { get { return _ilHotDllList; } }

        [SerializeField]
        public string _monoJsonPath;
        public string MonoJsonPath { get { return _monoJsonPath; } }

        [SerializeField]
        public string _fsmJsonPath;
        public string FsmJsonPath { get { return _fsmJsonPath; } }
    }
}
