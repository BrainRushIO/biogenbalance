using UnityEngine;
using System.Collections;

public class PracticeChooseBalanceManager : BasePracticeSubmodule {

	public Camera balanceCamera;

	public bool selectedMicrobalance = false, selectedSemiMicroBalance = false;

	public override void UpdateSceneContents( int stepIndex ) {
		switch( stepIndex )
		{
		case 0:
			balanceCamera.gameObject.SetActive( true );
			break;
		case 1:
			balanceCamera.gameObject.SetActive( false );
			break;
		}
	}

	public void CheckAnswer() {
		if( selectedSemiMicroBalance && !selectedMicrobalance ) {
			Debug.LogWarning( "YAY YOU WIN." );
		} else {
			PracticeManager.s_instance.PressedHintButton();
		}
	}

	public void SelectBalance( int balance ) {
		// SemiMicrobalance
		if( balance == 0 ) {
			selectedSemiMicroBalance = true; 
			selectedMicrobalance = false;
		}
		// MicroBalance
		else {
			selectedSemiMicroBalance = false; 
			selectedMicrobalance = true;
		}
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
