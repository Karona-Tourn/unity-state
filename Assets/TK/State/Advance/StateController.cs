using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TK.State.Advance
{

	public class StateController : MonoBehaviour
	{
		private State currentState = null;

		public void UpdateController ()
		{
			if (currentState != null)
				currentState.UpdateState (this);
		}

		public void TransitToState (State state)
		{
			if (currentState != null)
				currentState.Exit (this);

			currentState = state;

			if (currentState != null)
				currentState.Enter (this);
		}
	}

}