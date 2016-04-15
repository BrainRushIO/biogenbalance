using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class AcquireUseBalanceManager : BaseAcquireSubmodule {
	public GameObject rice, insideWeighContainer, outsideWeighContainer, outsideRiceContainer, insideRiceContainer;
	public Animator riceContainer, rightGlass;
	public Text readoutText, unitText, plusText;
	protected override void Init() {
		base.Init();
	}

	public override void UpdateSceneContents( int stepIndex ) {
		//TODO Get init data from step at given index. execute logic depending on data.

		// Have steps execute specific step logic if they have it
		moduleSteps[stepIndex].ExecuteStepLogic();

		switch (stepIndex) {
		case 0:
			rightGlass.GetComponent<Animator> ().SetTrigger ("Clicked");
			SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.slidingDoor);
			break;
		case 1:
			insideWeighContainer.SetActive (true);
			rightGlass.GetComponent<Animator> ().SetTrigger ("Clicked");
			SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.slidingDoor);
			outsideWeighContainer.SetActive (false);
			readoutText.text = "9.7306";
			break;
		case 2:
			readoutText.text = "0.0000";
			SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.buttonBeep);
			break;
		case 3:
			rightGlass.GetComponent<Animator> ().SetTrigger ("Clicked");
			SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.slidingDoor);
			break;
		case 4:
			insideRiceContainer.SetActive (true);
			rice.SetActive (true);
			SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.rice2);
			rice.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, 100f);
			outsideRiceContainer.SetActive (false);
			readoutText.text = "50.2452";
			break;
		case 5:
			insideRiceContainer.SetActive (false);
			rightGlass.GetComponent<Animator> ().SetTrigger ("Clicked");
			SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.slidingDoor);
			outsideRiceContainer.SetActive (true);
			break;
		case 6:
			break;

		}
	}

	public override void ResetScene() {
	}
}
