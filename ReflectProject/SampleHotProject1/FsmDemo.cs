using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ILHotAttribute;

namespace SampleHotProject1
{
    [ILHotFsmSerilizable(DisplayName = "DemoTest", SubClass = true)]
    public class FsmTest : ILHotFsmStateAction
    {
        [SerializeField]
        public FsmString _ladyGaGa;

        [SerializeField]
        public FsmTestEx _Michele;
    }

    [ILHotFsmSerilizable(DisplayName = "DemoTest", SubClass = true)]
    public class FsmTestEx : ILHotFsmStateAction
    {
        [SerializeField]
        public FsmString _Michele;

        [SerializeField]
        public FsmFloat _age;
    }

    [ILHotFsmSerilizable(DisplayName = "DemoTest")]
    public class FsmDemo : ILHotFsmStateAction
    {
        [SerializeField]
        FsmObject _fx;

        [SerializeField]
        FsmTest _fxTest;

        List<string> _inAreaUids = new List<string>();

        [SerializeField]
        FsmString _uids;

        [SerializeField]
        FsmFloat _radius;

        FsmGameObject go;

        public override void Init(FsmStateAction action)
        {
            base.Init(action);
        }

        public override void Awake()
        {
            base.Awake();
            Debug.Log("_uids " + _uids);
            Debug.Log("_fxTest._ladyGaGa " + _fxTest._ladyGaGa);
            Debug.Log("_fxTest._Michele._Michele " + _fxTest._Michele._Michele);
            Debug.Log("_fxTest._Michele._age " + _fxTest._Michele._age);
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
        }
    }
}
