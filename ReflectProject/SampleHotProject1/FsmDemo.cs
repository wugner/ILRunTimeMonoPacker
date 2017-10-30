using HutongGames.PlayMaker;
using ILHot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SampleHotProject1
{
    [ILHotComponentMenu(MenuPath = "FsmTestComponent")]
    public class FsmTest : ILHotFsmStateAction
    {
        [ILHotSerializeField]
        public FsmString _ladyGaGa;

        [ILHotSerializeField]
        public FsmTestEx _Michele;
    }

    [ILHotComponentMenu(MenuPath = "FsmTestExComponent")]
    public class FsmTestEx : ILHotFsmStateAction
    {
        [ILHotSerializeField]
        public FsmString _Michele;

        [ILHotSerializeField]
        public FsmFloat _age;
    }

    [ILHotComponentMenu(MenuPath = "FsmDemoComponent")]
    [ILHotFsmProxy]
    public class FsmDemo : ILHotFsmStateAction
    {
        [ILHotSerializeField]
        FsmObject _fx;

        [ILHotSerializeField]
        FsmTest _fxTest;

        List<string> _inAreaUids = new List<string>();

        [ILHotSerializeField]
        FsmString _uids;

        [ILHotSerializeField]
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
