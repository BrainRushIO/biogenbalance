using UnityEngine;
using System.Collections;

public class PracticeCalibrateBalanceManager : BasePracticeSubmodule {

	public GameObject weightOutside, weightInside;
	public Animator leftGlassDoor, rightGlassDoor;
	public Transform defaultPivotPos, defaultCamPos;

	private enum PCToggles { WeightOutside, WeightInside, BalanceOn, BalanceCalibrated, FocusedOnBalanceFace, CalibrationModeOn, LDoorOpen, RDoorOpen }
	// Used to check if door is open or closed
	private int rightDoorOpenState, rightDoorClosedState, leftDoorOpenState, leftDoorClosedState;

	private void Start() {
		rightDoorOpenState = Animator.StringToHash( "Base Layer.SMB_RightGlass_Open" );
		rightDoorClosedState = Animator.StringToHash( "Base Layer.SMB_RightGlass_Closed" );
		leftDoorOpenState = Animator.StringToHash( "Base Layer.SMB_LeftGlass_Open" );
		leftDoorClosedState = Animator.StringToHash( "Base Layer.SMB_LeftGlass_Closed" );
	}

	void Update() {
		CheckInputs();
	}

	public override void UpdateSceneContents( int stepIndex ) {
		currentStep = stepIndex;

		// Get init data from step at given index. execute logic depending on data.
		toggles = moduleSteps[stepIndex].GetToggles();
		inputs = moduleSteps[stepIndex].GetInputs();

		// Have steps execute specific step logic if they have it
		moduleSteps[stepIndex].ExecuteStepLogic();
	}

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
		SelectableObject.SelectableObjectType clickedObjectType = clickedOnObject.objectType;

		switch( clickedObjectType ) {
		case SelectableObject.SelectableObjectType.CalibrationWeight:
			// If we aren't holding an object when we click the weight, make it our selected object.
			if( PracticeCalibrateBalanceManager.s_instance.selectedObject == SelectableObject.SelectableObjectType.None ) {
				PracticeCalibrateBalanceManager.s_instance.selectedObject = SelectableObject.SelectableObjectType.CalibrationWeight;
				// Toggle on highlights
				if( toggles[(int)PCToggles.WeightInside] ) {
					weightInside.GetComponent<Renderer>().materials[1].SetFloat( "_Thickness", 3.5f );
				} else if ( toggles[(int)PCToggles.WeightOutside] ) {
					weightOutside.GetComponent<Renderer>().materials[1].SetFloat( "_Thickness", 3.5f );
				}
			}
			break;

		case SelectableObject.SelectableObjectType.LeftDoor:
			// Check what state the animation is in and toggle on the opposite since we're going to transition animations after.
			if( leftGlassDoor.GetCurrentAnimatorStateInfo(0).fullPathHash == leftDoorOpenState ) {
				toggles[(int)PCToggles.LDoorOpen] = false;
			} else if ( leftGlassDoor.GetCurrentAnimatorStateInfo(0).fullPathHash == leftDoorClosedState ) {
				toggles[(int)PCToggles.LDoorOpen] = true;
			}
			leftGlassDoor.GetComponent<Animator>().SetTrigger( "Clicked" );
			break;

		case SelectableObject.SelectableObjectType.TareButton:
			if( toggles[(int)PCToggles.FocusedOnBalanceFace] && !toggles[(int)PCToggles.BalanceCalibrated] ) {
				ReadoutDisplay.s_instance.PlayCalibrationModeAnimation();
			}
			break;

		case SelectableObject.SelectableObjectType.OnButton:
			if( toggles[(int)PCToggles.FocusedOnBalanceFace] ) {
				toggles[(int)PCToggles.BalanceOn] = true;
				ReadoutDisplay.s_instance.TurnBalanceOn();
			}
			break;

		case SelectableObject.SelectableObjectType.RightDoor:
			// Check what state the animation is in and toggle on the opposite since we're going to transition animations after.
			if( leftGlassDoor.GetCurrentAnimatorStateInfo(0).fullPathHash == rightDoorOpenState ) {
				toggles[(int)PCToggles.RDoorOpen] = false;
			} else if ( leftGlassDoor.GetCurrentAnimatorStateInfo(0).fullPathHash == rightDoorClosedState ) {
				toggles[(int)PCToggles.RDoorOpen] = true;
			}
			rightGlassDoor.GetComponent<Animator>().SetTrigger( "Clicked" );
			break;

		case SelectableObject.SelectableObjectType.WeighPan:
			// If we're holding the calibration weight when clicking the weigh pan, place the weight.
			if( PracticeCalibrateBalanceManager.s_instance.selectedObject == SelectableObject.SelectableObjectType.CalibrationWeight ) {
				toggles[(int)PCToggles.WeightInside] = true;
				weightInside.SetActive( true );
				toggles[(int)PCToggles.WeightOutside] = false;
				weightOutside.SetActive( false );
			}
			break;
		}
	}

	public void ToggleBalanceCalibrationMode( bool toggle ) {
		toggles[(int)PCToggles.CalibrationModeOn] = toggle;
	}

	public void ClickedOnFocusOnBalanceButton() {
		toggles[(int)PCToggles.FocusedOnBalanceFace] = true;
	}

	public void ClickedOnLeaveFaceFocusButton() {
		toggles[(int)PCToggles.FocusedOnBalanceFace] = false;
	}
}