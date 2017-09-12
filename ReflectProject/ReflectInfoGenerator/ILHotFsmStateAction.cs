using HutongGames.PlayMaker;
using UnityEngine;

namespace SampleHotProject1
{
	public class ILHotFsmStateAction
    {
		protected FsmStateAction _action;
		protected Fsm _fsm;

		public virtual void Init(FsmStateAction action)
		{
			_action = action;
			_fsm = action.Fsm;
		}

		public virtual void Awake()
		{
		}
		public virtual void OnEnter()
		{
		}
		public virtual void OnExit()
		{
		}
		public virtual void OnUpdate()
		{
		}

        public virtual void OnFixedUpdate()
        {
        }

        public virtual void OnLateUpdate()
        {
        }
        public virtual void OnPreprocess()
		{
		}

		public virtual void DoCollisionEnter(Collision collisionInfo)
		{
		}
		public virtual void DoCollisionStay(Collision collisionInfo)
		{
		}
		public virtual void DoCollisionExit(Collision collisionInfo)
		{
		}

		public virtual void DoTriggerEnter(Collider other)
		{
		}
		public virtual void DoTriggerExit(Collider other)
		{
		}
		public virtual void DoTriggerStay(Collider other)
		{
		}
	}
}