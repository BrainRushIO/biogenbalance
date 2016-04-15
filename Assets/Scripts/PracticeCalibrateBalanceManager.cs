using UnityEngine;
using System.Collections;

public class PracticeCalibrateBalanceManager : BasePracticeSubmodule {

	public GameObject weightOutside, weightInside;
	public Animator leftGlassDoor, rightGlassDoor;
	public Transform defaultPivotPos, defaultCamPos, facePivotPos, faceCamPos;

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
		if( CheckInputs() == true && currentStep >= moduleSteps.Length-1 )
			Debug.Log( "Yay you win!" );
	}

	public override void UpdateSceneContents( int stepIndex ) {
		currentStep = stepIndex;

		// Get init data from step at given index. execute logic depending on data.
		toggles = moduleSteps[currentStep].GetToggles();
		inputs = moduleSteps[currentStep].GetInputs();

		// Have steps execute specific step logic if they have it
		moduleSteps[currentStep].ExecuteStepLogic();

		// I'm sorry and this won't happen again but here's some loggic that should go in ExecuteStepLogic
		switch( currentStep ) {
		case 6:
			StartCoroutine( ToggleBalancedCalibrationOn() );
			break;
		}
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

	public override void ClickedOnObject( SelectableObject clickedOnObject, bool usedForceps ) {
		SelectableObject.SelectableObjectType clickedObjectType = clickedOnObject.objectType;

		switch( clickedObjectType ) {
		case SelectableObject.SelectableObjectType.CalibrationWeight:
			if( !usedForceps )
				return;
			// If we aren't holding an object when we click the weight, make it our selected object.
			if( PracticeCalibrateBalanceManager.s_instance.selectedObject == SelectableObject.SelectableObjectType.None ) {
				// Toggle on highlights
				if( toggles[(int)PCToggles.WeightInside] ) {
					toggles[(int)PCToggles.WeightInside] = false;
					weightInside.SetActive( false );
					toggles[(int)PCToggles.WeightOutside] = true;
					weightOutside.SetActive( true );
					weightOutside.GetComponent<Renderer>().materials[1].SetFloat( "_Thickness", 0f );
				} else if ( toggles[(int)PCToggles.WeightOutside] ) {
					PracticeCalibrateBalanceManager.s_instance.selectedObject = SelectableObject.SelectableObjectType.CalibrationWeight;
					weightOutside.GetComponent<Renderer>().materials[1].SetFloat( "_Thickness", 3.5f );
				}
			}
			break;

		case SelectableObject.SelectableObjectType.LeftDoor:
			if( usedForceps )
				return;
			// Check what state the animation is in and toggle on the opposite since we're going to transition animations after.
			if( leftGlassDoor.GetCurrentAnimatorStateInfo(0).fullPathHash == leftDoorOpenState ) {
				toggles[(int)PCToggles.LDoorOpen] = false;
			} else if ( leftGlassDoor.GetCurrentAnimatorStateInfo(0).fullPathHash == leftDoorClosedState ) {
				toggles[(int)PCToggles.LDoorOpen] = true;
			}
			leftGlassDoor.GetComponent<Animator>().SetTrigger( "Clicked" );
			break;

		case SelectableObject.SelectableObjectType.TareButton:
			if( usedForceps )
				return;
			if( toggles[(int)PCToggles.FocusedOnBalanceFace] && !toggles[(int)PCToggles.BalanceCalibrated] ) {
				ReadoutDisplay.s_instance.PlayCalibrationModeAnimation();
			}
			break;

		case SelectableObject.SelectableObjectType.OnButton:
			if( usedForceps )
				return;
			if( toggles[(int)PCToggles.FocusedOnBalanceFace] ) {
				toggles[(int)PCToggles.BalanceOn] = true;
				ReadoutDisplay.s_instance.TurnBalanceOn();
			}
			break;

		case SelectableObject.SelectableObjectType.RightDoor:
			if( usedForceps )
				return;
			// Check what state the animation is in and toggle on the opposite since we're going to transition animations after.
//			if( leftGlassDoor.GetCurrentAnimatorStateInfo(0).fullPathHash == rightDoorOpenState ) {
//				toggles[(int)PCToggles.RDoorOpen] = false;
//			} else if ( leftGlassDoor.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash("Base Layer.SMB_RightGlass_Closed")/*rightDoorClosedState*/ ) {
//				toggles[(int)PCToggles.RDoorOpen] = true;
//			}
			toggles[(int)PCToggles.RDoorOpen] = !toggles[(int)PCToggles.RDoorOpen];
			rightGlassDoor.GetComponent<Animator>().SetTrigger( "Clicked" );
			SoundtrackManager.s_instance.PlayAudioSource( SoundtrackManager.s_instance.slidingDoor );
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
		PracticeManager.s_instance.StartNewCameraSlerp( facePivotPos, faceCamPos );
	}

	public void ClickedOnLeaveFaceFocusButton() {
		toggles[(int)PCToggles.FocusedOnBalanceFace] = false;
		PracticeManager.s_instance.StartNewCameraSlerp( defaultPivotPos, defaultCamPos );
	}

	private IEnumerator ToggleBalancedCalibrationOn() {
		yield return new WaitForSeconds( 5f );
		toggles[(int)PCToggles.BalanceCalibrated] = true;
		toggles[(int)PCToggles.CalibrationModeOn] = false;
		SoundtrackManager.s_instance.PlayAudioSource( SoundtrackManager.s_instance.buttonBeep );
		ReadoutDisplay.s_instance.ToggleDisplay( true, true, false );
		ReadoutDisplay.s_instance.ZeroOut();
	}
}