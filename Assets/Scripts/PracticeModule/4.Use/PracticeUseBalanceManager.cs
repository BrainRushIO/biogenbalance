using UnityEngine;
using System.Collections;

public class PracticeUseBalanceManager : BasePracticeSubmodule {
	public GameObject weighContainerOutside, weighContainerInside, riceContainerOutside, riceContainerInside;
	public SkinnedMeshRenderer riceSkinnedMeshRenderer;
	public Animator leftGlassDoor, rightGlassDoor;
	public Transform defaultPivotPos, defaultCamPos, facePivotPos, faceCamPos;
	
	private enum PUToggles { WeighContainerOutside, WeightContainerInside, LDoorOpen, RDoorOpen, FocusedOnBalanceFace, BalanceTared, WeighContainerFilled, ReadingStabilized }
	// Used to check if door is open or closed
	private int rightDoorOpenState, rightDoorClosedState, leftDoorOpenState, leftDoorClosedState;

	void Start() {
		toggles = new bool[8];
		// Just saying that the weight container is outside
		toggles[0] = true;
		// Set the rest to false
		for( int i = 1; i < toggles.Length; i++ )
			toggles[i] = false;

		ReadoutDisplay readOutDisplay = ReadoutDisplay.s_instance;
		readOutDisplay.balanceOn = true;
		readOutDisplay.balanceCalibrated = true;
		readOutDisplay.ToggleDisplay( true, true, false );
		readOutDisplay.readoutNumberText.text = "0.0000";

		rightDoorOpenState = Animator.StringToHash( "Base Layer.SMB_RightGlass_Open" );
		rightDoorClosedState = Animator.StringToHash( "Base Layer.SMB_RightGlass_Closed" );
		leftDoorOpenState = Animator.StringToHash( "Base Layer.SMB_LeftGlass_Open" );
		leftDoorClosedState = Animator.StringToHash( "Base Layer.SMB_LeftGlass_Closed" );
	}

	void Update() {
		if( PracticeManager.s_instance.isInIntro )
			return;

		// Making exceptions for what layers to ignore depending on step. Shitty, I know but we need to design this better next time.
		switch( currentStep )
		{
//		case 0:
//			UIManager.s_instance.ToggleSidePanel (true, false);
//			break;
		case 4:
			CheckInputs( new PUToggles[2] { PUToggles.RDoorOpen, PUToggles.FocusedOnBalanceFace }  );
			break;
		case 6:
			CheckInputs( new PUToggles[1] { PUToggles.FocusedOnBalanceFace }  );
			break;
		default:
			CheckInputs();
			break;
		}

		UpdateDoorTogglesBasedOnAnimationState( false );

		if( ReadoutDisplay.s_instance.hasStableReading && !toggles[(int)PUToggles.ReadingStabilized] ) {
			toggles[(int)PUToggles.ReadingStabilized] = true;
		}
	}

	private void CheckInputs( PUToggles[] ignoreToggles ) {
		for( int i = 0; i < toggles.Length; i++ ) {
			bool ignoreInput = false;

			foreach( PUToggles toggle in ignoreToggles ){
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
			if( !toggles[(int)PUToggles.FocusedOnBalanceFace] )
				break;

			if( ApplicationManager.s_instance.currentMouseMode == ApplicationManager.MouseMode.Pointer && selectedObject == SelectableObject.SelectableObjectType.None ) {
				ApplicationManager.s_instance.SetSpecialMouseMode( (int)ApplicationManager.SpecialCursorMode.PointingHand );
			}
			break;
		}
	}

	public override void ClickedOnObject( SelectableObject clickedOnObject, bool usedForceps ) {
		SelectableObject.SelectableObjectType clickedObjectType = clickedOnObject.objectType;

		switch( clickedObjectType )
		{
		case SelectableObject.SelectableObjectType.TareButton:
			if( usedForceps || !toggles[(int)PUToggles.FocusedOnBalanceFace] )
				return;
			if( toggles[(int)PUToggles.WeightContainerInside] && !toggles[(int)PUToggles.RDoorOpen] ) {
				ReadoutDisplay.s_instance.ZeroOut();
				toggles[(int)PUToggles.BalanceTared] = true;
				SoundtrackManager.s_instance.PlayAudioSource( SoundtrackManager.s_instance.buttonBeep );
			} else {
				PracticeManager.s_instance.PressedHintButton();
				PracticeManager.s_instance.numMistakes++;
			}
			break;

		case SelectableObject.SelectableObjectType.RiceContainer:
			if( usedForceps || toggles[(int)PUToggles.WeighContainerFilled] || !toggles[(int)PUToggles.WeightContainerInside] )
				return;
			// If we aren't holding an object when we click the weight, make it our selected object.
			if( selectedObject == SelectableObject.SelectableObjectType.None ) {
				SelectObject( SelectableObject.SelectableObjectType.RiceContainer );
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

			ReadoutDisplay.s_instance.doorsAreOpen = toggles[(int)PUToggles.RDoorOpen];// newDoorOpenValue;
			rightGlassDoor.GetComponent<Animator>().SetTrigger( "Clicked" );
			SoundtrackManager.s_instance.PlayAudioSource( SoundtrackManager.s_instance.slidingDoor );
			break;

		case SelectableObject.SelectableObjectType.WeighContainer:
			if( toggles[(int)PUToggles.WeightContainerInside] && PracticeUseBalanceManager.s_instance.selectedObject == SelectableObject.SelectableObjectType.RiceContainer ) {
				if( toggles[(int)PUToggles.WeighContainerFilled])
					return;

				StartCoroutine( PourRice() );
			} 
			// It can only be selected if balance has been calibrated
			else if( toggles[(int)PUToggles.WeighContainerOutside] && selectedObject == SelectableObject.SelectableObjectType.None ) {
				SelectObject( SelectableObject.SelectableObjectType.WeighContainer );
			} else {
				PracticeManager.s_instance.PressedHintButton();
				PracticeManager.s_instance.numMistakes++;
			}
			break;

		case SelectableObject.SelectableObjectType.WeighPan:
			// If we're holding the calibration weight when clicking the weigh pan, place the weight.
			if( !toggles[(int)PUToggles.WeightContainerInside] && selectedObject == SelectableObject.SelectableObjectType.WeighContainer) {
				weighContainerOutside.SetActive( false );
				toggles[(int)PUToggles.WeighContainerOutside] = false;
				weighContainerInside.SetActive( true );
				toggles[(int)PUToggles.WeightContainerInside] = true;
				ClearSelectedObject();
				ReadoutDisplay.s_instance.readoutNumberText.text = "9.7306";
			} else {
				PracticeManager.s_instance.PressedHintButton();
				PracticeManager.s_instance.numMistakes++;
			}
			break;
		}
	}

	public void ClickedOnFocusOnBalanceButton() {
		if( PracticeManager.s_instance.isInIntro )
			return;

		toggles[(int)PUToggles.FocusedOnBalanceFace] = true;
		PracticeManager.s_instance.StartNewCameraSlerp( facePivotPos, faceCamPos );
	}

	public void ClickedOnLeaveFaceFocusButton() {
		toggles[(int)PUToggles.FocusedOnBalanceFace] = false;
		PracticeManager.s_instance.StartNewCameraSlerp( defaultPivotPos, defaultCamPos );
	}

	public IEnumerator PourRice() {
		Debug.Log( "Pouring Rice" );
		riceContainerOutside.SetActive( false );
		riceContainerInside.SetActive( true );
		riceContainerInside.GetComponent<Animator>().SetTrigger( "Activate" );
		SoundtrackManager.s_instance.PlayAudioSource( SoundtrackManager.s_instance.rice2 );
		ReadoutDisplay.s_instance.WeighObject( 50.0244f );

		float startTime = Time.time;
		float lerpDuration = 2f;
		while( lerpDuration > Time.time-startTime ) {
			riceSkinnedMeshRenderer.SetBlendShapeWeight( 0, 100f * ((Time.time-startTime)/lerpDuration) );
			yield return null;
		}
		riceSkinnedMeshRenderer.SetBlendShapeWeight( 0, 100f );
		toggles[(int)PUToggles.WeighContainerFilled] = true;
		riceContainerInside.SetActive( false );
		selectedObject = SelectableObject.SelectableObjectType.None;
		riceContainerOutside.SetActive( true );
		Debug.Log( "Ended Pouring Rice." );
	}

	private void UpdateDoorTogglesBasedOnAnimationState( bool inverse ) {
		if( rightGlassDoor.GetCurrentAnimatorStateInfo(0).fullPathHash == rightDoorOpenState ) {
			toggles[(int)PUToggles.RDoorOpen] = ( inverse ) ? false : true;
		} else if ( rightGlassDoor.GetCurrentAnimatorStateInfo(0).fullPathHash == rightDoorClosedState ) {
			toggles[(int)PUToggles.RDoorOpen] = ( inverse ) ? true : false;
		}
	}
}
