using UnityEngine;
using System.Collections;

public partial class AppStateMachine<StateMachineType>
{

	public class AppState
	{
		/// <summary>
		/// Get or set state machine object
		/// </summary>
		public StateMachineType StateMachine { get; set; }

		public virtual void Enter()
		{
		}

		public virtual void Exit()
		{
		}

		public virtual void Update()
		{
		}

		public virtual void FixedUpdate()
		{
		}

		public virtual void LateUpdate()
		{
		}
	}

}
