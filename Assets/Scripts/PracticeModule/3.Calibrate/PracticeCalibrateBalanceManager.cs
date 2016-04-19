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
		if( PracticeManager.s_instance.isInIntro || PracticeManager.s_instance.hasFinishedModule )
			return;

		// Making exceptions for what layers to ignore depending on step. Shitty, I know but we need to design this better next time.
		switch( currentStep )
		{
		case 0:
		case 1:
		case 2:
			CheckInputs( new PCToggles[1] { PCToggles.RDoorOpen } );
			break;

		default:
			if( CheckInputs() == true && currentStep > moduleSteps.Length-1 )
				PracticeManager.s_instance.CompleteModule();
			break;
		}

		UpdateDoorTogglesBasedOnAnimationState( false );
	}

	private void CheckInputs( PCToggles[] ignoreToggles ) {
		for( int i = 0; i < toggles.Length; i++ ) {
			bool ignoreInput = false;

			foreach( PCToggles toggle in ignoreToggles ){
				if( i == (int)toggle ) {
					ignoreInput = true;
					break;
				}
			}
			if( ignoreInput )
				continue;
			if( toggles[i] != inputs[i] )
				return;
		}
		PracticeManager.s_instance.GoToNextStep();
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

	public override void HoveredOverObject( SelectableObject obj ) {
		// We didn't hover over anything
		if( obj == null ) {
			// If we're still holding something then change to closed hand
			if( selectedObject != SelectableObject.SelectableObjectType.None ) {
				ApplicationManager.s_instance.SetSpecialMouseMode( (int)ApplicationManager.SpecialCursorMode.ClosedHand );
			} else {
				// Set the special cursor mode in the Application Manager to None if it isn't already that.
				if( ApplicationManager.s_instance.currentSpecialCursorMode != ApplicationManager.SpecialCursorMode.None )
					ApplicationManager.s_instance.SetSpecialMouseMode( (int)ApplicationManager.SpecialCursorMode.None );
			}
			return;
		}

		SelectableObject.SelectableObjectType hoveredObjectType = obj.objectType;

		switch( hoveredObjectType )
		{
		case SelectableObject.SelectableObjectType.CalibrationWeight:
			if( selectedObject == SelectableObject.SelectableObjectType.None && ApplicationManager.s_instance.currentMouseMode == ApplicationManager.MouseMode.Forceps ) {
				ApplicationManager.s_instance.SetSpecialMouseMode( (int)ApplicationManager.SpecialCursorMode.OpenHand );
			}
			break;
		case SelectableObject.SelectableObjectType.RiceContainer:
			if( selectedObject == SelectableObject.SelectableObjectType.None && ApplicationManager.s_instance.currentMouseMode == ApplicationManager.MouseMode.Pointer ) {
				ApplicationManager.s_instance.SetSpecialMouseMode( (int)ApplicationManager.SpecialCursorMode.OpenHand );
			}
			break;
		case SelectableObject.SelectableObjectType.WeighContainer:
			if( selectedObject == SelectableObject.SelectableObjectType.None && ApplicationManager.s_instance.currentMouseMode == ApplicationManager.MouseMode.Pointer ) {
				ApplicationManager.s_instance.SetSpecialMouseMode( (int)ApplicationManager.SpecialCursorMode.OpenHand );
			} else if( selectedObject == SelectableObject.SelectableObjectType.RiceContainer ) {
				ApplicationManager.s_instance.SetSpecialMouseMode( (int)ApplicationManager.SpecialCursorMode.PointingHand );
			}
			break;
		case SelectableObject.SelectableObjectType.WeighPan:
			if( (selectedObject == SelectableObject.SelectableObjectType.CalibrationWeight && ApplicationManager.s_instance.currentMouseMode == ApplicationManager.MouseMode.Forceps ) 
				|| selectedObject == SelectableObject.SelectableObjectType.WeighContainer && ApplicationManager.s_instance.currentMouseMode == ApplicationManager.MouseMode.Pointer ) {
				ApplicationManager.s_instance.SetSpecialMouseMode( (int)ApplicationManager.SpecialCursorMode.PointingHand );
			}
			break;
		case SelectableObject.SelectableObjectType.RightDoor:
			if( ApplicationManager.s_instance.currentMouseMode == ApplicationManager.MouseMode.Pointer && selectedObject == SelectableObject.SelectableObjectType.None ) {
				ApplicationManager.s_instance.SetSpecialMouseMode( (int)ApplicationManager.SpecialCursorMode.PointingHand );
			}
			break;
		case SelectableObject.SelectableObjectType.OnButton:
		case SelectableObject.SelectableObjectType.TareButton:
			if( !toggles[(int)PCToggles.FocusedOnBalanceFace] )
				break;

			if( ApplicationManager.s_instance.currentMouseMode == ApplicationManager.MouseMode.Pointer && selectedObject == SelectableObject.SelectableObjectType.None ) {
				ApplicationManager.s_instance.SetSpecialMouseMode( (int)ApplicationManager.SpecialCursorMode.PointingHand );
			}
			break;
		}
	}

	public override void ClickedOnObject( SelectableObject clickedOnObject, bool usedForceps ) {
		SelectableObject.SelectableObjectType clickedObjectType = clickedOnObject.objectType;

		switch( clickedObjectType ) {
		case SelectableObject.SelectableObjectType.CalibrationWeight:
			if( !usedForceps ) {
				PracticeManager.s_instance.PressedHintButton();
				PracticeManager.s_instance.numMistakes++;
				return;
			}
			// If we aren't holding an object when we click the weight, make it our selected object.
			if( PracticeCalibrateBalanceManager.s_instance.selectedObject == SelectableObject.SelectableObjectType.None ) {
				// Toggle on highlights
				if( toggles[(int)PCToggles.WeightInside] ) {
					toggles[(int)PCToggles.WeightInside] = false;
					weightInside.SetActive( false );
					toggles[(int)PCToggles.WeightOutside] = true;
					weightOutside.SetActive( true );
				} else if ( toggles[(int)PCToggles.WeightOutside] || toggles[(int)PCToggles.CalibrationModeOn] ) {
					SelectObject( SelectableObject.SelectableObjectType.CalibrationWeight );
				}
			}
			break;

		case SelectableObject.SelectableObjectType.TareButton:
			if( usedForceps || !toggles[(int)PCToggles.FocusedOnBalanceFace] )
				return;
			if( toggles[(int)PCToggles.FocusedOnBalanceFace] && !toggles[(int)PCToggles.BalanceCalibrated] ) {
				ReadoutDisplay.s_instance.PlayCalibrationModeAnimation();
			} else {
				PracticeManager.s_instance.PressedHintButton();
				PracticeManager.s_instance.numMistakes++;
			}
			break;

		case SelectableObject.SelectableObjectType.OnButton:
			if( usedForceps || !toggles[(int)PCToggles.FocusedOnBalanceFace] )
				return;
			if( toggles[(int)PCToggles.FocusedOnBalanceFace] ) {
				toggles[(int)PCToggles.BalanceOn] = true;
				ReadoutDisplay.s_instance.TurnBalanceOn();
			} else {
				PracticeManager.s_instance.PressedHintButton();
				PracticeManager.s_instance.numMistakes++;
			}
			break;

		case SelectableObject.SelectableObjectType.RightDoor:
			if( usedForceps )
				return;
			// Check what state the animation is in and toggle on the opposite since we're going to transition animations after.
			UpdateDoorTogglesBasedOnAnimationState( true );

			ReadoutDisplay.s_instance.doorsAreOpen = toggles[(int)PCToggles.RDoorOpen];// newDoorOpenValue;
			rightGlassDoor.GetComponent<Animator>().SetTrigger( "Clicked" );
			SoundtrackManager.s_instance.PlayAudioSource( SoundtrackManager.s_instance.slidingDoor );
			break;

		case SelectableObject.SelectableObjectType.WeighPan:
			// If we're holding the calibration weight when clicking the weigh pan, place the weight.
			if( PracticeCalibrateBalanceManager.s_instance.selectedObject == SelectableObject.SelectableObjectType.CalibrationWeight && toggles[(int)PCToggles.BalanceOn] && toggles[(int)PCToggles.CalibrationModeOn] ) {
				toggles[(int)PCToggles.WeightInside] = true;
				weightInside.SetActive( true );
				toggles[(int)PCToggles.WeightOutside] = false;
				weightOutside.SetActive( false );
				ClearSelectedObject();
			} else {
				PracticeManager.s_instance.PressedHintButton();
				PracticeManager.s_instance.numMistakes++;
			}
			break;
		}
	}

	private void UpdateDoorTogglesBasedOnAnimationState( bool inverse ) {
		if( rightGlassDoor.GetCurrentAnimatorStateInfo(0).fullPathHash == rightDoorOpenState ) {
			toggles[(int)PCToggles.RDoorOpen] = ( inverse ) ? false : true;
		} else if ( rightGlassDoor.GetCurrentAnimatorStateInfo(0).fullPathHash == rightDoorClosedState ) {
			toggles[(int)PCToggles.RDoorOpen] = ( inverse ) ? true : false;
		}
	}

	public void ToggleBalanceCalibrationMode( bool toggle ) {
		toggles[(int)PCToggles.CalibrationModeOn] = toggle;
	}

	public void ClickedOnFocusOnBalanceButton() {
		if( PracticeManager.s_instance.isInIntro )
			return;
		
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