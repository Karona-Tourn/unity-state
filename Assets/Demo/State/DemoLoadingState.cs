using UnityEngine;
using System.Collections;

public partial class DemoStateMachine
{
	public class DemoLoadingState : AppState
	{
		private float loadingTime = 2;
		private const float LOADING_INTERVAL = 0.2f;
		private float animTime;
		private string dot = "";

		public override void Enter ()
		{
			base.Enter ();
			animTime = Time.realtimeSinceStartup + LOADING_INTERVAL;
		}

		public override void Exit ()
		{
			base.Exit ();

			StateMachine.loadingText.gameObject.SetActive (false);
		}

		public override void Update ()
		{
			base.Update ();

			float realTime = Time.realtimeSinceStartup;

			if (realTime >= animTime)
			{
				dot += '.';
				if (dot.Length > 3)
				{
					dot = "";
				}
				StateMachine.loadingText.text = "Loading" + dot;
				animTime = realTime + LOADING_INTERVAL;
			}

			loadingTime -= Time.deltaTime;

			if (loadingTime <= 0)
			{
				StateMachine.PlayState<DemoFadeInState> ();
			}
		}

	}
}
