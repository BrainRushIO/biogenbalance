using UnityEngine;
using System.Collections;

public class PracticeCalibrateModuleStep : BasePracticeModuleStep {

	[Header("Object Toggles")]
	public bool toggleWeightOutside;
	public bool toggleWeightInside;
	public bool toggleBalanceOn;
	public bool toggleBalanceCalibrated;
	public bool toggleFocusedOnBalanceFace;
	public bool toggleCalibrationModeOn;
	public bool toggleLDoorOpen;
	public bool toggleRDoorOpen;

	[Header("Inputs")]
	public bool inputWeightOutside;
	public bool inputWeightInside;
	public bool inputBalanceOn;
	public bool inputBalanceCalibrated;
	public bool inputFocusedOnBalanceFace;
	public bool inputCalibrationModeOn;
	public bool inputLDoorOpen;
	public bool inputRDoorOpen;

	void Awake () {
		int sI = transform.GetSiblingIndex();
		if( sI < 0 )
			objectToggles = PracticeManager.s_instance.submoduleManager.moduleSteps[sI-1].GetInputs();
		objectToggles = new bool[8];
		objectToggles[0] = toggleWeightOutside;
		objectToggles[1] = toggleWeightInside;
		objectToggles[2] = toggleBalanceOn;
		objectToggles[3] = toggleBalanceCalibrated;
		objectToggles[4] = toggleFocusedOnBalanceFace;
		objectToggles[5] = toggleCalibrationModeOn;
		objectToggles[6] = toggleLDoorOpen;
		objectToggles[7] = toggleRDoorOpen;

		inputs = new bool[8];
		inputs[0] = inputWeightOutside;
		inputs[1] = inputWeightInside;
		inputs[2] = inputBalanceOn;
		inputs[3] = inputBalanceCalibrated;
		inputs[4] = inputFocusedOnBalanceFace;
		inputs[5] = inputCalibrationModeOn;
		inputs[6] = inputLDoorOpen;
		inputs[7] = inputRDoorOpen;
	}

//	void Start() {
//		int sI = transform.GetSiblingIndex();
//		if( sI > 0 )
//			objectToggles = PracticeManager.s_instance.submoduleManager.moduleSteps[sI-1].GetInputs();
//	}

	/// <summary>
	/// Executes the step logic. This is called from the Submodule Manager. Any logic that can't be expressed via simple bool toggles goes here. The index is the sibling index of this object.
	/// </summary>
	public override void ExecuteStepLogic() {
		int index = transform.GetSiblingIndex();
		switch( index )
		{
		case 0:
			break;
		case 1:
			break;
		case 2:
			break;
		case 3:
			break;
		case 4:
			break;
		case 5:
			break;
		case 6:
			break;
		case 7:
			break;
		default:
			Debug.LogWarning( "No step logic for this index." );
			break;
		}
	}
}
