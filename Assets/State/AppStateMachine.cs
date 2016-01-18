// Microsoft name space
using System;
using System.Collections.Generic;

// Unity name space
using UnityEngine;

/// <summary>
/// Control state
/// </summary>
/// <typeparam name="StateMachineType">Type of state machine class inherited</typeparam>
public partial class AppStateMachine<StateMachineType> : MonoBehaviour
	where StateMachineType : AppStateMachine<StateMachineType>
{
	// Store cached states
	private Dictionary<string, AppState> cachedStates = new Dictionary<string, AppState>();

	private AppState currentState;
	private Type previousState;

	public bool IsCurrentState<StateType>() where StateType : AppState
	{
		return currentState is StateType;
	}

	public bool IsPreviousState<StateType>() where StateType : AppState
	{
		return previousState == typeof(StateType);
	}

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
			previousState = currentState.GetType();
		}

		currentState = state;

		// Enter current state
		if (currentState != null)
		{
			currentState.Enter();
		}
	}

	private void PlayState(Type stateType, bool cached)
	{
		AppState newState = null;
		string stateName = stateType.Name;

		if (!cachedStates.TryGetValue(stateName, out newState))
		{
			newState = Activator.CreateInstance(stateType) as AppState;
			newState.StateMachine = this as StateMachineType;

			// Cache state
			if (cached)
			{
				cachedStates.Add(stateName, newState);
			}
		}

		PlayState(newState);
	}

	public void PlayPreviousState()
	{
		if (previousState != null)
		{
			PlayState(previousState, false);
		}
	}

	/// <summary>
	/// Start to execute state
	/// </summary>
	/// <typeparam name="StateType">Type of state</typeparam>
	/// <param name="cached">Tell whether the state is cached or not</param>
	public void PlayState<StateType>(bool cached = false) where StateType : AppState, new()
	{
		PlayState(typeof(StateType), cached);
	}

	/// <summary>
	/// Stop state
	/// </summary>
	public void StopState()
	{
		PlayState(null);
	}
}
