using System;

using UnityEngine;
using System.Collections.Generic;

public partial class AppStateMachine<StateMachineType> : MonoBehaviour
	where StateMachineType : AppStateMachine<StateMachineType>
{
	// Store cached states
	private Dictionary<string, AppState> cachedStates = new Dictionary<string, AppState>();

	private AppState currentState;
	private Type prevState;

	#region Core Method

	protected virtual void Awake()
	{
	}

	// Use this for initialization
	protected virtual void Start()
	{
	}

	// Update is called once per frame
	protected virtual void Update()
	{
		if (currentState != null)
		{
			currentState.Update();
		}
	}

	protected virtual void FixedUpdate()
	{
		if (currentState != null)
		{
			currentState.FixedUpdate();
		}
	}

	protected virtual void LateUpdate()
	{
		if (currentState != null)
		{
			currentState.LateUpdate();
		}
	}

	#endregion

	public void ClearStateCache()
	{
		cachedStates.Clear();
	}

	private void PlayState(AppState state)
	{
		// Exit previous state
		if (currentState != null)
		{
			currentState.Exit();
			prevState = currentState.GetType();
		}

		currentState = state;

		if (currentState != null)
		{
			currentState.Enter();
		}
	}

	private void PlayState(Type stateType, bool cached)
	{
	}

	public void PlayPrevState()
	{
		if (prevState != null)
		{
			AppState state = Activator.CreateInstance(prevState) as AppState;
			PlayState(state);
		}
	}

	/// <summary>
	/// Start to execute state
	/// </summary>
	/// <typeparam name="StateType">Type of state</typeparam>
	/// <param name="cached">Tell whether the state is cached or not</param>
	public void PlayState<StateType>(bool cached = false) where StateType : AppState, new()
	{
		AppState newState = null;
		string stateName = typeof(StateType).Name;

		// Exit previous state
		if (currentState != null)
		{
			currentState.Exit();
			prevState = currentState.GetType();
		}

		if (!cachedStates.TryGetValue(stateName, out newState))
		{
			newState = new StateType();
			newState.StateMachine = this as StateMachineType;

			// Cache state
			if (cached)
			{
				cachedStates.Add(stateName, newState);
			}
		}

		currentState = newState;

		// Enter current state
		currentState.Enter();
	}

	/// <summary>
	/// Stop state
	/// </summary>
	public void StopState()
	{
		PlayState(null);
	}
}
