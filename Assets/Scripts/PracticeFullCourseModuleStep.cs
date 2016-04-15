using UnityEngine;
using System.Collections;

public class PracticeFullCourseModuleStep : BasePracticeModuleStep {

	[Header("Prepare Inputs")]
	public bool inLevelingPosition = false;
	public bool balanceIsLeveled = false;

	[Header("Calibrate Inputs")]
	public bool weightOutside;
	public bool weightInside;
	public bool balanceOn;
	public bool balanceCalibrated;
	public bool calibrationModeOn;

	[Header("Use Submodule Inputs")]
	public bool weighContainerOutside;
	public bool weightContainerInside;
	public bool lDoorOpen;
	public bool rDoorOpen;
	public bool focusedOnBalanceFace;
	public bool balanceTared;
	public bool weighContainerFilled;
	public bool readingStabilized;

	void Awake() {
		inputs = new bool[15];
		inputs[0] = inLevelingPosition;
		inputs[1] = balanceIsLeveled;

		inputs[2] = weightOutside;
		inputs[3] = weightInside;
		inputs[4] = balanceOn;
		inputs[5] = balanceCalibrated;
		inputs[6] = calibrationModeOn;

		inputs[7] = weighContainerOutside;
		inputs[8] = weightContainerInside;
		inputs[9] = lDoorOpen;
		inputs[10] = rDoorOpen;
		inputs[11] = focusedOnBalanceFace;
		inputs[12] = balanceTared;
		inputs[13] = weighContainerFilled;
		inputs[14] = readingStabilized;
	}

	void Start() {
		int sI = transform.GetSiblingIndex();

		objectToggles = new bool[15];
		for( int i = 0; i < objectToggles.Length; i++ )
			objectToggles[i] = false;
	}

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
			ApplicationManager.s_instance.ChangeMouseMode( (int)ApplicationManager.MouseMode.Pointer );
			UIManager.s_instance.ToggleToolsActive( true, false, false, false );
			PracticeManager.s_instance.orbitCam.canZoom = true;
			break;
		default:
			Debug.LogWarning( "No step logic for this index." );
			break;
		}
	}
}
