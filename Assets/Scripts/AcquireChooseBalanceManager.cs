using UnityEngine;
using System.Collections;

public class AcquireChooseBalanceManager : BaseAcquireSubmodule {

	protected override void Init() {
		base.Init();
	}

	public override void UpdateSceneContents( int stepIndex ) {
		//TODO Get init data from step at given index. execute logic depending on data.

		// Have steps execute specific step logic if they have it
		moduleSteps[stepIndex].ExecuteStepLogic();
	}

	public override void ResetScene() {
	}
}
