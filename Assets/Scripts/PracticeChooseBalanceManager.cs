using UnityEngine;
using System.Collections;

public class PracticeChooseBalanceManager : BasePracticeSubmodule {
	
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

	public override void ClickedOnObject( SelectableObject clickedOnObject ) {
	}
}
