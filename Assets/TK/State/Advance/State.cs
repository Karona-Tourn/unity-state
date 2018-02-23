using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TK.State.Advance
{

	[CreateAssetMenu (menuName = "StatePro/State")]
	public class State : ScriptableObject
	{
		// Array of actions to do something
		[SerializeField]
		private Action[] actions = new Action[0];

		// Array of transitions to transit to other states
		[SerializeField]
		private Transition[] transitions = new Transition[0];

		private void CheckTransitions (StateController controller)
		{
			int count = transitions.Length;

			for (int i = 0; i < count; i++)
			{
				Transition t = transitions[i];
				bool isTrue = t.decision.Decide (controller);
				if (isTrue)
				{
					if (t.trueState != null)
					{
						controller.TransitToState (t.trueState);
						break;
					}
				}
				else
				{
					if (t.falseState != null)
					{
						controller.TransitToState (t.falseState);
						break;
					}
				}
			}
		}

		private void DoActions (StateController controller)
		{
			int actionCount = actions.Length;

			for (int i = 0; i < actionCount; i++)
			{
				actions[i].Act (controller);
			}

			CheckTransitions (controller);
		}

		public void Enter (StateController controller)
		{
			int actionCount = actions.Length;

			for (int i = 0; i < actionCount; i++)
			{
				actions[i].Enter (controller);
			}
		}

		public void UpdateState (StateController controller)
		{
			DoActions (controller);
			CheckTransitions (controller);
		}

		public void Exit (StateController controller)
		{
			int actionCount = actions.Length;

			for (int i = 0; i < actionCount; i++)
			{
				actions[i].Exit (controller);
			}
		}
	}

}