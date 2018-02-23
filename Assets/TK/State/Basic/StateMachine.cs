// Microsoft name space
using System;
using System.Collections.Generic;

namespace TK.State.Basic
{

	/// <summary>
	/// Control state
	/// </summary>
	/// <typeparam name="SubClassType">Type of state machine class inherited</typeparam>
	public partial class StateMachine<SubClassType>
		where SubClassType : StateMachine<SubClassType>
	{

		// Store cached states
		private SortedDictionary<string, State> cachedStates = new SortedDictionary<string, State> ();

		// Instance of current running state
		private State currentState = null;

		// Previous state type that was already run
		private Type previousState = null;

		public State CurrentState
		{
			get { return currentState; }
		}

		private bool HasActiveState
		{
			get { return currentState != null; }
		}

		#region Public Functions

		// Update is called once per frame
		public virtual void Update ()
		{
			if (!HasActiveState)
			{
				return;
			}

			currentState.Update ();
		}

		public virtual void FixedUpdate ()
		{
			if (!HasActiveState)
			{
				return;
			}

			currentState.FixedUpdate ();
		}

		public virtual void LateUpdate ()
		{
			if (!HasActiveState)
			{
				return;
			}

			currentState.LateUpdate ();
		}

		public bool IsCurrentState<StateType> () where StateType : State
		{
			return currentState is StateType;
		}

		public bool IsPreviousState<StateType> () where StateType : State
		{
			return previousState == typeof (StateType);
		}

		public void ClearCachedStates ()
		{
			cachedStates.Clear ();
		}

		/// <summary>
		/// Play state
		/// </summary>
		/// <param name="state">State to be played</param>
		/// <param name="cache">Cache the played state</param>
		public void PlayState (State state, bool cache = false, params object[] args)
		{
			// Exit function if the state is null
			if (state == null)
			{
				return;
			}

			if (cache)
			{
				string stateName = state.GetType ().Name;

				if (!cachedStates.ContainsKey (stateName))
				{
					// Cache state
					cachedStates.Add (stateName, state);
				}
			}

			// Exit previous state
			if (HasActiveState)
			{
				currentState.Exit ();
				previousState = currentState.GetType ();
			}

			currentState = state;

			// Enter current state
			if (HasActiveState)
			{
				if (currentState.StateMachine == null)
				{
					currentState.StateMachine = this as SubClassType;
				}
				currentState.Enter (args);
			}
		}

		/// <summary>
		/// Start to execute state
		/// </summary>
		/// <typeparam name="StateType">Type of state</typeparam>
		/// <param name="cache">Tell whether the state is cached or not</param>
		public void PlayState<StateType> (bool cache = false, params object[] args) where StateType : State
		{
			State state = null;
			Type type = typeof(StateType);

			if (!cachedStates.TryGetValue (type.Name, out state))
			{
				state = Activator.CreateInstance (type) as State;
			}

			PlayState (state, cache, args);
		}

		/// <summary>
		/// Stop state
		/// </summary>
		public void StopState ()
		{
			PlayState (null);
		}

		/// <summary>
		/// Get debugging status
		/// </summary>
		public string GetDebugStatus ()
		{
			string status = String.Empty;
			status = HasActiveState ? currentState.GetType ().Name : "No State";
			return status;
		}

		#endregion
	}

}
