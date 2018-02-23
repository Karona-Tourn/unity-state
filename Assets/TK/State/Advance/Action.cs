using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TK.State.Advance
{
	public abstract class Action : ScriptableObject
	{
		public virtual void Enter (StateController controller)
		{
#if UNITY_EDITOR
		Debug.Log ("State: " + GetType ().Name);
#endif
		}

		public virtual void Exit (StateController controller)
		{
		}

		public abstract void Act (StateController controller);
	}
}
