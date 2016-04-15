using UnityEngine;
using System.Collections;

public class PracticeUseBalanceManager : BasePracticeSubmodule {

	public override void UpdateSceneContents( int stepIndex ) {
		//TODO Get init data from step at given index. execute logic depending on data.

		// Have steps execute specific step logic if they have it
		moduleSteps[stepIndex].ExecuteStepLogic();
	}

	/// <summary>
	/// Resets the scene objects to their default position.
	/// </summary>
	public override void ResetScene() {

	}

	protected override void SelectObject( SelectableObject newSelection ) {
		ClearSelectedObject( false );
	}

	public override void ClearSelectedObject( bool slerpToDefaultPos ) {
		if( selectedObject == SelectableObject.SelectableObjectType.None )
			return;

		//TODO Turn off highlights and shit
		selectedObject = SelectableObject.SelectableObjectType.None;
	}

	public override void ClickedOnObject( SelectableObject clickedOnObject, bool usedForceps ) {
		SelectableObject.SelectableObjectType clickedObjectType = clickedOnObject.objectType;

		switch( clickedObjectType ) {
		case SelectableObject.SelectableObjectType.CalibrationWeight:
			break;
		case SelectableObject.SelectableObjectType.LeftDoor:
			break;
		case SelectableObject.SelectableObjectType.TareButton:
			break;
		case SelectableObject.SelectableObjectType.OnButton:
			break;
		case SelectableObject.SelectableObjectType.RiceContainer:
			break;
		case SelectableObject.SelectableObjectType.RightDoor:
			break;
		case SelectableObject.SelectableObjectType.WeighContainer:
			break;
		case SelectableObject.SelectableObjectType.WeighPan:
			break;
		}
	}

	public void ToggleBalanceCalibrationMode( bool toggle ) {
		//TODO
	}
}
