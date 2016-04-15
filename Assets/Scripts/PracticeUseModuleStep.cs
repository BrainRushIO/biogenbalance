using UnityEngine;
using System.Collections;

public class PracticeUseModuleStep : BasePracticeModuleStep {

	[Header("Inputs")]
	public bool weighContainerOutside;
	public bool weightContainerInside;
	public bool lDoorOpen;
	public bool rDoorOpen;
	public bool focusedOnBalanceFace;
	public bool balanceTared;
	public bool weighContainerFilled;
	public bool readingStabilized;

	void Awake() {
		inputs = new bool[8];
		inputs[0] = weighContainerOutside;
		inputs[1] = weightContainerInside;
		inputs[2] = lDoorOpen;
		inputs[3] = rDoorOpen;
		inputs[4] = focusedOnBalanceFace;
		inputs[5] = balanceTared;
		inputs[6] = weighContainerFilled;
		inputs[7] = readingStabilized;
	}

	void Start() {
		int sI = transform.GetSiblingIndex();
		if( sI > 0 )
			objectToggles = PracticeUseBalanceManager.s_instance.moduleSteps[sI-1].GetInputs();
		else {
			objectToggles = new bool[7];
			objectToggles[0] = true;
			for( int i = 1; i < objectToggles.Length; i++ )
				objectToggles[i] = false;
		}		
	}

	/// <summary>
	/// Executes the step logic. This is called from the Submodule Manager. Any logic that can't be expressed via simple bool toggles goes here. The index is the sibling index of this object.
	/// </summary>
	public override void ExecuteStepLogic() {
		int index = transform.GetSiblingIndex();
		switch( index )
		{
		default:
//			Debug.Log( "No step logic for this index." );
			break;
		}

//		Debug.LogError( "Cannot execute step logic for index "+ index +". Index out of range." );
	}
}
