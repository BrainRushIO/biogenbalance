using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AcquireCalibrateBalanceManager : BaseAcquireSubmodule {
	public Animator rightGlass;
	public GameObject outsideWeight, insideWeight;
	public Text readoutText, plusText, unitText;
	protected override void Init() {
		base.Init();
	}

	public override void UpdateSceneContents( int stepIndex ) {
		//TODO Get init data from step at given index. execute logic depending on data.

		// Have steps execute specific step logic if they have it
		moduleSteps[stepIndex].ExecuteStepLogic();

		switch (stepIndex) {

		case 0: 	
			ReadoutDisplay.s_instance.TurnBalanceOn ();
			break;

		case 2:
			ReadoutDisplay.s_instance.balanceOn = true;
			ReadoutDisplay.s_instance.PlayCalibrationModeAnimation ();
			break;

		case 4:
			rightGlass.GetComponent<Animator> ().SetTrigger ("Clicked");
			SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.slidingDoor);
			break;

		case 5:
			outsideWeight.SetActive(false);
			insideWeight.SetActive(true);
			break;
		case 6:
			rightGlass.GetComponent<Animator> ().SetTrigger ("Clicked");
			SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.slidingDoor);
			break;
		case 7:
			readoutText.text = "200.0000";
			SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.buttonBeep);
			break;
		case 8:
			rightGlass.GetComponent<Animator> ().SetTrigger ("Clicked");
			SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.slidingDoor);
			insideWeight.SetActive (false);
			outsideWeight.SetActive (true);
			break;
		case 9:
			rightGlass.GetComponent<Animator> ().SetTrigger ("Clicked");
			SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.slidingDoor);
			break;
		}
	}

	public override void ResetScene() {
	}
}
