using UnityEngine;
using System.Collections;

public class PracticeChooseBalanceManager : BasePracticeSubmodule {

	public GameObject step1, step2;

	public bool selectedMicrobalance = false, selectedSemiMicroBalance = false;

	public override void UpdateSceneContents( int stepIndex ) {
		switch( stepIndex )
		{
		case 0:
			step1.SetActive( true );
			step2.SetActive( false );
			ApplicationManager.s_instance.ChangeMouseMode( (int)ApplicationManager.MouseMode.Rotate );
			UIManager.s_instance.ToggleSidePanel( true, false );
			UIManager.s_instance.ToggleToolsActive( false, true, false, false );
			UIManager.s_instance.nextButton.gameObject.SetActive( true );
			break;
		case 1:
			step1.SetActive( false );
			step2.SetActive( true );
			ApplicationManager.s_instance.ChangeMouseMode( (int)ApplicationManager.MouseMode.Pointer );
			UIManager.s_instance.ToggleToolsActive( true, true, false, false );
			UIManager.s_instance.nextButton.gameObject.SetActive( false );
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

	/// <summary>
	/// Clicked the semi microbalance button in the UI.
	/// </summary>
	public void ClickedSemiMicrobalance() {
		selectedMicrobalance = false;
		selectedSemiMicroBalance = true;
		CheckAnswer();
	}

	/// <summary>
	/// Clicked the microbalance button in the UI.
	/// </summary>
	public void ClickedMicrobalance() {
		selectedMicrobalance = true;
		selectedSemiMicroBalance = false;
		CheckAnswer();
	}
}
