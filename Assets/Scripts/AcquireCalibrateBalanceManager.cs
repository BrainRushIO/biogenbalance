using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AcquireCalibrateBalanceManager : BaseAcquireSubmodule {
	public Animator rightGlass;
	public Text plusTextObject, readoutTextObject, unitTextObject;
	protected override void Init() {
		base.Init();
	}

	public override void UpdateSceneContents( int stepIndex ) {
		//TODO Get init data from step at given index. execute logic depending on data.

		// Have steps execute specific step logic if they have it
		moduleSteps[stepIndex].ExecuteStepLogic();

		switch (stepIndex) {

		case 0:
			StartCoroutine ( TurnOnBalance() );
			break;

		case 2:
			
			ReadoutDisplay.s_instance.PlayCalibrationModeAnimation ();
			StartCoroutine (CalibrationMode ());
			break;
		}
	}

	private IEnumerator TurnOnBalance(){
		SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.buttonBeep);
		readoutTextObject.enabled = true;
		readoutTextObject.text = "888.8888";
		plusTextObject.enabled = true;
		unitTextObject.enabled = true;
		yield return new WaitForSeconds (2f);
		readoutTextObject.enabled = false;
		unitTextObject.enabled = false;
		yield return new WaitForSeconds (2f);
		readoutTextObject.enabled = true;
		readoutTextObject.text = "0.0000";
		unitTextObject.enabled = true;

	}
	private IEnumerator CalibrationMode(){
		SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.buttonBeep);

	}

	public override void ResetScene() {
	}
}
