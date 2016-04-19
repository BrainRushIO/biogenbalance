using UnityEngine;
using System.Collections;

public class ValidateStep : MonoBehaviour {

	private bool[] objectToggles;
	private bool[] inputs;

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

	public void InitToggles() {
		int sI = transform.GetSiblingIndex();
		if( sI > 0 )
			objectToggles = ValidateManager.s_instance.moduleSteps[sI-1].GetInputs();
		else {
			objectToggles = new bool[15];
			for( int i = 1; i < objectToggles.Length; i++ )
				objectToggles[i] = false;

			objectToggles[(int)ValidateManager.VToggles.WeightOutside] = true;
			objectToggles[(int)ValidateManager.VToggles.WeighContainerOutside] = true;
		}
	}
		
	/// <summary>
	/// Executes the step logic. This is called from the Submodule Manager. Any logic that can't be expressed via simple bool toggles goes here. The index is the sibling index of this object.
	/// </summary>
	public void ExecuteStepLogic() {
		int index = transform.GetSiblingIndex();
		switch( index )
		{
		default:
			Debug.LogWarning( "No step logic for this index." );
			break;
		}
	}

	/// <summary>
	/// Returns the dictionary containing the bools for toggling on and off objects in the scene.
	/// </summary>
	/// <returns>The step init data.</returns>
	public bool[] GetToggles() {
		return objectToggles;
	}
	
	public bool[] GetInputs() {
		return inputs;
	}
}
