using UnityEngine;
using System.Collections;

public partial class DemoStateMachine
{
	public class DemoFadeInState : AppState
	{
		private float fadeTime;

		public override void Enter ()
		{
			base.Enter ();

			fadeTime = Time.realtimeSinceStartup + 2;
		}

		public override void Exit ()
		{
			base.Exit ();
		}

		public override void Update ()
		{
			base.Update ();

			float percent = Mathf.Clamp01 (Time.realtimeSinceStartup / fadeTime);

			Color col = StateMachine.background.color;
			col.a = Mathf.Lerp (1, 0, percent);
			StateMachine.background.color = col;

			if (percent == 1)
			{
				StateMachine.PlayState<DemoMainState> ();
			}
		}

	}
}
