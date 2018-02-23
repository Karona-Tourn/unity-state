using System;

namespace TK.State.Advance
{

	[Serializable]
	public class Transition
	{
		public Decision decision = null;
		public State trueState = null;
		public State falseState = null;
	}

}