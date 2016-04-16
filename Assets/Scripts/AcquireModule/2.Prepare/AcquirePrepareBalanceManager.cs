using UnityEngine;
using System.Collections;

public class AcquirePrepareBalanceManager : BaseAcquireSubmodule {
	public Animator rightGlass;
	protected override void Init() {
		base.Init();
	}

	public override void UpdateSceneContents( int stepIndex ) {
		//TODO Get init data from step at given index. execute logic depending on data.

		// Have steps execute specific step logic if they have it
		moduleSteps[stepIndex].ExecuteStepLogic();

		switch (stepIndex) {
		case 4:
			rightGlass.GetComponent<Animator> ().SetTrigger ("Clicked");
			SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.slidingDoor);
			break;
		case 6:
			rightGlass.GetComponent<Animator> ().SetTrigger ("Clicked");
			SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.slidingDoor);
			break;

		}

	}

	public override void ResetScene() {
	}
}
