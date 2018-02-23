using UnityEngine;
using UnityEngine.Events;

using System;
using System.Collections.Generic;

namespace TK.State.Intermediate
{
	[Flags]
	public enum StateMachineSetting
	{
		BindNormalState = 1 << 0,
		BindAnimCallbacks =	1 << 1,
		BindAnimEventState = 1 << 2,
	}

	public partial class StateMachine<KeyType, SubClassType>
		where SubClassType : StateMachine<KeyType, SubClassType>
	{
		public interface IStateMachine
		{
			SubClassType GetMachine ();

			void OnAnimEvent ();

			void OnAnimEvent (int v);

			void OnAnimEvent (float v);

			void OnAnimEvent (string v);

			void OnAnimEvent (UnityEngine.Object v);
		}

		public abstract class StateMachineMono : MonoBehaviour, IStateMachine
		{
			private SubClassType machine = null;

			public void OnAnimEvent ()
			{
			}

			public void OnAnimEvent (int v)
			{
			}

			public void OnAnimEvent (float v)
			{
			}

			public void OnAnimEvent (string v)
			{
			}

			public void OnAnimEvent (UnityEngine.Object v)
			{
			}

			public abstract StateMachineSetting MachineSetting{ get; }

			public abstract bool UseStateCallbackFromThisClass{ get; }

			public SubClassType GetMachine ()
			{
				if (machine == null)
				{
					machine = (SubClassType)System.Activator.CreateInstance (typeof(SubClassType), UseStateCallbackFromThisClass ? this : null, MachineSetting);
				}
				return machine;
			}
		}

		public class AnimStateMachine : StateMachineBehaviour
		{
			// Current state key
			[SerializeField]
			private KeyType startState = default(KeyType);

			// Tell whether use the start state or not
			[SerializeField]
			private bool useStartState = false;

			private SubClassType machine = null;

			public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
			{
				// Start the starting state that is set
				if (useStartState)
				{
					machine = animator.gameObject.GetComponent<IStateMachine> ().GetMachine ();
					machine.SetCurrentState (startState);
				}

				if (machine == null ||
				    machine.currentState.state.OnAnimStateEnter == null)
				{
					return;
				}
					
				machine.currentState.state.OnAnimStateEnter (animator, stateInfo, layerIndex);
			}

			public override void OnStateMove (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
			{
				if (machine == null ||
				    machine.currentState.state.OnAnimStateMove == null)
				{
					return;
				}

				machine.currentState.state.OnAnimStateMove (animator, stateInfo, layerIndex);
			}

			public override void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
			{
				if (machine == null ||
				    machine.currentState.state.OnAnimStateUpdate == null)
				{
					return;
				}

				machine.currentState.state.OnAnimStateUpdate (animator, stateInfo, layerIndex);
			}

			public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
			{
				if (machine == null ||
				    machine.currentState.state.OnAnimStateExit == null)
				{
					return;
				}

				machine.currentState.state.OnAnimStateExit (animator, stateInfo, layerIndex);
			}
		}

		private class StateCallbacks
		{
			// Callback for enter state
			public UnityAction OnEnter = null;

			// Callback for enter state
			public UnityAction<object[]> OnEnterWithParam = null;

			// Callback for update state
			public UnityAction OnUpdate = null;

			// Callback for fixe update state
			public UnityAction OnFixedUpdate = null;

			// Callback for late update state
			public UnityAction OnLateUpdate = null;

			// Callback for exit state
			public UnityAction OnExit = null;

			// Event invoked in animation frame
			public UnityAction animEvent = null;
			public UnityAction<string> stringAnimEvent = null;
			public UnityAction<UnityEngine.Object> objectAnimEvent = null;
			public UnityAction<float> floatAnimEvent = null;
			public UnityAction<int> intAnimEvent = null;

			// Event invoked by animation state machines
			public UnityAction<Animator, AnimatorStateInfo, int> OnAnimStateEnter = null;
			public UnityAction<Animator, AnimatorStateInfo, int> OnAnimStateMove = null;
			public UnityAction<Animator, AnimatorStateInfo, int> OnAnimStateUpdate = null;
			public UnityAction<Animator, AnimatorStateInfo, int> OnAnimStateExit = null;
		}

		private class State
		{
			public StateCallbacks state = null;
			public KeyType currentKey = default (KeyType);
			public KeyType previousKey = default (KeyType);
		}

		// Setting for this state machine binding
		private StateMachineSetting settings = StateMachineSetting.BindNormalState;

		// Cache all used states
		private SortedDictionary<KeyType, StateCallbacks> caches = new SortedDictionary<KeyType, StateCallbacks> ();

		private State currentState = null;

		private object main = null;

		public object Main
		{
			get { return main; }
		}

		/// <summary>
		/// Gets the state of the previous.
		/// </summary>
		/// <value>The state of the previous.</value>
		public KeyType PreviousState
		{
			get { return currentState.previousKey; }
		}

		public KeyType CurrentState
		{
			get { return currentState.currentKey; }
		}

		public StateMachine (object main, StateMachineSetting setting)
		{
			currentState = new State () { 
				state = new StateCallbacks () {
					OnEnter = DoNothing,
					OnExit = DoNothing,
					OnFixedUpdate = DoNothing,
					OnLateUpdate = DoNothing,
					OnUpdate = DoNothing
				}
			};

			this.main = main;
			settings = setting;

			if (this.main == null)
			{
				this.main = this;
			}
		}

		public void SetCurrentState (KeyType state, object[] args = null)
		{
			currentState.currentKey = state;
			ChangeState (state, args);
		}

		private bool CanBind (StateMachineSetting setting)
		{
			return (settings & setting) != 0;
		}

		private void ChangeState (KeyType key, object[] args)
		{
			if (currentState.state != null && currentState.state.OnExit != null)
			{
				currentState.state.OnExit ();
			}

			if (!caches.TryGetValue (key, out currentState.state))
			{
				StateCallbacks newState = new StateCallbacks ();

				if (CanBind (StateMachineSetting.BindNormalState))
				{
					newState.OnEnter = BindMethod (key + "_Enter");
					newState.OnEnterWithParam = BindMethod<object[]> (key + "_Enter");
					newState.OnExit = BindMethod (key + "_Exit");
					newState.OnUpdate = BindMethod (key + "_Update");
					newState.OnFixedUpdate = BindMethod (key + "_FixedUpdate");
					newState.OnLateUpdate = BindMethod (key + "_LateUpdate");
				}
				else
				{
					newState.OnEnter = DoNothing;
					newState.OnExit = DoNothing;
					newState.OnUpdate = DoNothing;
					newState.OnFixedUpdate = DoNothing;
					newState.OnLateUpdate = DoNothing;
				}

				if (CanBind (StateMachineSetting.BindAnimCallbacks))
				{
					newState.OnAnimStateEnter = BindAnimMethod (key + "_AnimEnter");
					newState.OnAnimStateMove = BindAnimMethod (key + "_AnimMove");
					newState.OnAnimStateUpdate = BindAnimMethod (key + "_AnimUpdate");
					newState.OnAnimStateExit = BindAnimMethod (key + "_AnimExit");
				}
				else
				{
					newState.OnAnimStateEnter = DoNothing;
					newState.OnAnimStateMove = DoNothing;
					newState.OnAnimStateUpdate = DoNothing;
					newState.OnAnimStateExit = DoNothing;
				}

				if (CanBind (StateMachineSetting.BindAnimEventState))
				{
					string animEventMethod = key + "_AnimEvent";
					newState.animEvent = BindMethod (animEventMethod);
					newState.intAnimEvent = BindMethod<int> (animEventMethod);
					newState.floatAnimEvent = BindMethod<float> (animEventMethod);
					newState.stringAnimEvent = BindMethod<string> (animEventMethod);
					newState.objectAnimEvent = BindMethod<UnityEngine.Object> (animEventMethod);
				}
				else
				{
					newState.animEvent = DoNothing;
					newState.intAnimEvent = DoNothing;
					newState.floatAnimEvent = DoNothing;
					newState.stringAnimEvent = DoNothing;
					newState.objectAnimEvent = DoNothing;
				}

				caches.Add (key, newState);

				currentState.state = newState;
				currentState.previousKey = currentState.currentKey;
				currentState.currentKey = key;
			}

			if (args == null || args.Length == 0)
			{
				if (currentState.state.OnEnter != null)
				{
					currentState.state.OnEnter ();
				}
			}
			else
			{
				if (currentState.state.OnEnterWithParam != null)
				{
					currentState.state.OnEnterWithParam (args);
				}
			}
		}

		private UnityAction BindMethod (string name)
		{
			UnityAction a = (UnityAction)Delegate.CreateDelegate (typeof(UnityAction), main, name, false, false);
			return (a == null ? DoNothing : a);
		}

		private UnityAction<T> BindMethod<T> (string name)
		{
			UnityAction<T> a = (UnityAction<T>)Delegate.CreateDelegate (typeof(UnityAction<T>), main, name, false, false);
			return (a == null ? DoNothing<T> : a);
		}

		private UnityAction<Animator, AnimatorStateInfo, int> BindAnimMethod (string name)
		{
			UnityAction<Animator, AnimatorStateInfo, int> a = (UnityAction<Animator, AnimatorStateInfo, int>)Delegate.CreateDelegate (typeof(UnityAction<Animator, AnimatorStateInfo, int>), main, name, false, false);
			return (a == null ? DoNothing : a);
		}

		public virtual void ExecuteUpdate ()
		{
			if (currentState.state.OnUpdate == null)
			{
				return;
			}

			currentState.state.OnUpdate ();
		}

		public virtual void ExecuteFixedUpdate ()
		{
			if (currentState.state.OnFixedUpdate == null)
			{
				return;
			}

			currentState.state.OnFixedUpdate ();
		}

		public virtual void ExecuteLateUpdate ()
		{
			if (currentState.state.OnLateUpdate == null)
			{
				return;
			}

			currentState.state.OnLateUpdate ();
		}

		public void InvokeAnimEvent ()
		{
			if (currentState.state.animEvent == null)
			{
				return;
			}

			currentState.state.animEvent ();
		}

		public void InvokeAnimEvent (float v)
		{
			if (currentState.state.floatAnimEvent == null)
			{
				return;
			}

			currentState.state.floatAnimEvent (v);
		}

		public void InvokeAnimEvent (int v)
		{
			if (currentState.state.intAnimEvent == null)
			{
				return;
			}

			currentState.state.intAnimEvent (v);
		}

		public void InvokeAnimEvent (string v)
		{
			if (currentState.state.stringAnimEvent == null)
			{
				return;
			}

			currentState.state.stringAnimEvent (v);
		}

		public void InvokeAnimEvent (UnityEngine.Object v)
		{
			if (currentState.state.objectAnimEvent == null)
			{
				return;
			}

			currentState.state.objectAnimEvent (v);
		}

		private static void DoNothing ()
		{
		}

		private static void DoNothing<T> (T v)
		{
		}

		private static void DoNothing (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
		}
	}
}