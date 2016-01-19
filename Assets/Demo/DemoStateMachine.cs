using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public partial class DemoStateMachine : AppStateMachine<DemoStateMachine>
{

	public Text loadingText;
	public Image background;

	protected override void Start()
	{
		base.Start();

		PlayState<DemoLoadingState> ();
	}

}
