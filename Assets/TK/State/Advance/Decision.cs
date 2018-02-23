using UnityEngine;

namespace TK.State.Advance
{

	public abstract class Decision : ScriptableObject
	{
		public abstract bool Decide (StateController controller);
	}

}