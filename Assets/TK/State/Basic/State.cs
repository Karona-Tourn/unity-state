namespace TK.State.Basic
{
	public partial class StateMachine<SubClassType>
	{
		public partial class State
		{
			/// <summary>
			/// Get or set state machine object
			/// </summary>
			public SubClassType StateMachine { get; set; }

			/// <summary>
			/// Enter state
			/// </summary>
			public virtual void Enter (params object[] args)
			{
			}

			/// <summary>
			/// Exit stat
			/// </summary>
			public virtual void Exit ()
			{
			}

			public virtual void Update ()
			{
			}

			public virtual void FixedUpdate ()
			{
			}

			public virtual void LateUpdate ()
			{
			}
		}
	}
}
