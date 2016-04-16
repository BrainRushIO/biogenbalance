using UnityEngine;
using System.Collections;

public class PracticeFullCourseManager : BasePracticeSubmodule {

	// Prepare
	public RectTransform bubble;
	public Transform backPivot, backCamPos, backDefaultCamPos;
	public GameObject accessBehindViewCanvas, screwsCanvas, bubbleCanvas, returnFromBackCanvas;

	private float currentBubbleX, currentBubbleY;
	private float bubbleWinThreshold = 2.5f;
	private float bubbleMaxRadius = 32f;

	// Calibrate
	public GameObject weightOutside, weightInside;

	// Use
	public GameObject weighContainerOutside, weighContainerInside, riceContainerOutside, riceContainerInside;
	public SkinnedMeshRenderer riceSkinnedMeshRenderer;
	public Animator leftGlassDoor, rightGlassDoor;
	public Transform defaultPivotPos, defaultCamPos, facePivotPos, faceCamPos;

	public enum PFCToggles { InLevelingPosition, BalanceIsLeveled,
								WeightOutside, WeightInside, BalanceOn, BalanceCalibrated, CalibrationModeOn,
								WeighContainerOutside, WeightContainerInside, LDoorOpen, RDoorOpen, FocusedOnBalanceFace, BalanceTared, WeighContainerFilled, ReadingStabilized }

	private int rightDoorOpenState, rightDoorClosedState, leftDoorOpenState, leftDoorClosedState;

	void Start() {
		toggles = new bool[15];
		for( int i = 0; i < toggles.Length; i++ )
			toggles[i] = false;

		toggles[(int)PFCToggles.WeighContainerOutside] = true;
		toggles[(int)PFCToggles.WeightOutside] = true;

		rightDoorOpenState = Animator.StringToHash( "Base Layer.SMB_RightGlass_Open" );
		rightDoorClosedState = Animator.StringToHash( "Base Layer.SMB_RightGlass_Closed" );
		leftDoorOpenState = Animator.StringToHash( "Base Layer.SMB_LeftGlass_Open" );
		leftDoorClosedState = Animator.StringToHash( "Base Layer.SMB_LeftGlass_Closed" );
	}

	void Update() {
		// Making exceptions for what layers to ignore depending on step. Shitty, I know but we need to design this better next time.
		switch( currentStep )
		{
		case 0:
		case 1:
		case 2:
		case 3:
		case 4:
		case 14:
			CheckInputs( new PFCToggles[2] { PFCToggles.LDoorOpen, PFCToggles.RDoorOpen } );
			break;
		default:
			CheckInputs();
			break;
		}

		if( ReadoutDisplay.s_instance.hasStableReading && !toggles[(int)PFCToggles.ReadingStabilized] ) {
			toggles[(int)PFCToggles.ReadingStabilized] = true;
		}
		
		UpdateDoorTogglesBasedOnAnimationState( false );

//		This case will be whatever step is the bubble leveling step
		switch( currentStep ) {
		case 1:
			if( Mathf.Abs(bubble.localPosition.x) <= bubbleWinThreshold && Mathf.Abs(bubble.localPosition.y) <= bubbleWinThreshold ) {
				toggles[(int)PFCToggles.BalanceIsLeveled] = true;
				SoundtrackManager.s_instance.PlayAudioSource( SoundtrackManager.s_instance.buttonBeep );
				ClickedOnLeaveBackFocusButton();
			}
			break;
		}
	}

	public void CheckInputs( PFCToggles[] ignoreToggles ) {
		for( int i = 0; i < toggles.Length; i++ ) {
			bool ignoreInput = false;

			foreach( PFCToggles toggle in ignoreToggles ){
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
		case 8:
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
		case SelectableObject.SelectableObjectType.OnButton:
		case SelectableObject.SelectableObjectType.TareButton:
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
			if( usedForceps )
				return;
			if( toggles[(int)PFCToggles.FocusedOnBalanceFace] && !toggles[(int)PFCToggles.BalanceCalibrated] && toggles[(int)PFCToggles.BalanceOn] ) {
				ReadoutDisplay.s_instance.PlayCalibrationModeAnimation();
			} else if( toggles[(int)PFCToggles.WeightContainerInside] && !toggles[(int)PFCToggles.RDoorOpen] ) {
				ReadoutDisplay.s_instance.ZeroOut();
				toggles[(int)PFCToggles.BalanceTared] = true;
				SoundtrackManager.s_instance.PlayAudioSource( SoundtrackManager.s_instance.buttonBeep );
			} else {
				PracticeManager.s_instance.PressedHintButton();
			}
			break;

		case SelectableObject.SelectableObjectType.RiceContainer:
			if( usedForceps || toggles[(int)PFCToggles.WeighContainerFilled] || !toggles[(int)PFCToggles.WeightContainerInside] )
				return;
			// If we aren't holding an object when we click the weight, make it our selected object.
			if( selectedObject == SelectableObject.SelectableObjectType.None ) {
				SelectObject( SelectableObject.SelectableObjectType.RiceContainer );
			} else {
				PracticeManager.s_instance.PressedHintButton();
			}
			break;

		case SelectableObject.SelectableObjectType.RightDoor:
			if( usedForceps )
				return;
			// Check what state the animation is in and toggle on the opposite since we're going to transition animations after.
			UpdateDoorTogglesBasedOnAnimationState( true );

			ReadoutDisplay.s_instance.doorsAreOpen = toggles[(int)PFCToggles.RDoorOpen];// newDoorOpenValue;
			rightGlassDoor.GetComponent<Animator>().SetTrigger( "Clicked" );
			SoundtrackManager.s_instance.PlayAudioSource( SoundtrackManager.s_instance.slidingDoor );
			break;

		case SelectableObject.SelectableObjectType.WeighContainer:
			if( toggles[(int)PFCToggles.WeightContainerInside] && PracticeUseBalanceManager.s_instance.selectedObject == SelectableObject.SelectableObjectType.RiceContainer ) {
				if( toggles[(int)PFCToggles.WeighContainerFilled])
					return;

				StartCoroutine( PourRice() );
			} 
			// It can only be selected if balance has been calibrated
			else if( toggles[(int)PFCToggles.WeighContainerOutside] && selectedObject == SelectableObject.SelectableObjectType.None && toggles[(int)PFCToggles.BalanceCalibrated] ) {
				SelectObject( SelectableObject.SelectableObjectType.WeighContainer );
			} else {
				PracticeManager.s_instance.PressedHintButton();
			}
			break;

		case SelectableObject.SelectableObjectType.WeighPan:
			// If we're holding the calibration weight when clicking the weigh pan, place the weight.
			if( selectedObject == SelectableObject.SelectableObjectType.CalibrationWeight ) {
				toggles[(int)PFCToggles.WeightInside] = true;
				weightInside.SetActive( true );
				toggles[(int)PFCToggles.WeightOutside] = false;
				weightOutside.SetActive( false );
				ClearSelectedObject();
			} else if( !toggles[(int)PFCToggles.WeightContainerInside] && selectedObject == SelectableObject.SelectableObjectType.WeighContainer) {
				weighContainerOutside.SetActive( false );
				toggles[(int)PFCToggles.WeighContainerOutside] = false;
				weighContainerInside.SetActive( true );
				toggles[(int)PFCToggles.WeightContainerInside] = true;
				ClearSelectedObject();
				ReadoutDisplay.s_instance.readoutNumberText.text = "9.7306";
			} else {
				PracticeManager.s_instance.PressedHintButton();
			}
			break;

			//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		case SelectableObject.SelectableObjectType.CalibrationWeight:
			if( !usedForceps )
				return;
			// If we aren't holding an object when we click the weight, make it our selected object.
			if( PracticeCalibrateBalanceManager.s_instance.selectedObject == SelectableObject.SelectableObjectType.None  ) {
				// Toggle on highlights
				if( toggles[(int)PFCToggles.WeightInside] ) {
					toggles[(int)PFCToggles.WeightInside] = false;
					weightInside.SetActive( false );
					toggles[(int)PFCToggles.WeightOutside] = true;
					weightOutside.SetActive( true );
				} else if ( toggles[(int)PFCToggles.WeightOutside] || toggles[(int)PFCToggles.CalibrationModeOn] ) {
					SelectObject( SelectableObject.SelectableObjectType.CalibrationWeight );
				}
			} else {
				PracticeManager.s_instance.PressedHintButton();
			}
			break;

		case SelectableObject.SelectableObjectType.OnButton:
			if( usedForceps )
				return;

			if( toggles[(int)PFCToggles.FocusedOnBalanceFace] && toggles[(int)PFCToggles.BalanceIsLeveled] ) {
				toggles[(int)PFCToggles.BalanceOn] = true;
				ReadoutDisplay.s_instance.TurnBalanceOn();
			} else {
				PracticeManager.s_instance.PressedHintButton();
			}
			break;
		}
	}

	private void UpdateDoorTogglesBasedOnAnimationState( bool inverse ) {
		if( rightGlassDoor.GetCurrentAnimatorStateInfo(0).fullPathHash == rightDoorOpenState ) {
			toggles[(int)PFCToggles.RDoorOpen] = ( inverse ) ? false : true;
		} else if ( rightGlassDoor.GetCurrentAnimatorStateInfo(0).fullPathHash == rightDoorClosedState ) {
			toggles[(int)PFCToggles.RDoorOpen] = ( inverse ) ? true : false;
		}
	}

	// Prepare
	public void ClickedOnPositioningButton() {
		toggles[(int)PFCToggles.InLevelingPosition] = true;
	}

	public void ClickedLeftScrewUp() {
		Vector3 bubblePos = bubble.localPosition;
		bubblePos.x += bubbleMaxRadius*0.1f;
		bubblePos.y += bubbleMaxRadius*0.05f;
		bubble.localPosition = bubblePos;

		NormalizeBubblePos();
	}

	public void ClickedLeftScrewDown() {
		Vector3 bubblePos = bubble.localPosition;
		bubblePos.x -= bubbleMaxRadius*0.1f;
		bubblePos.y -= bubbleMaxRadius*0.05f;
		bubble.localPosition = bubblePos;

		NormalizeBubblePos();
	}

	public void ClickedRightScrewUp() {
		Vector3 bubblePos = bubble.localPosition;
		bubblePos.x -= bubbleMaxRadius*0.1f;
		bubblePos.y += bubbleMaxRadius*0.05f;
		bubble.localPosition = bubblePos;

		NormalizeBubblePos();
	}

	public void ClickedRightScrewDown() {
		Vector3 bubblePos = bubble.localPosition;
		bubblePos.x += bubbleMaxRadius*0.1f;
		bubblePos.y -= bubbleMaxRadius*0.05f;
		bubble.localPosition = bubblePos;

		NormalizeBubblePos();
	}

	void NormalizeBubblePos() {
		Vector2 bubblePos = new Vector2(bubble.localPosition.x, bubble.localPosition.y );

		//bubblePos.x = Mathf.Clamp( bubble.localPosition.x, -bubbleMaxRadius, bubbleMaxRadius );
		//bubblePos.y = Mathf.Clamp( bubble.localPosition.y, -bubbleMaxRadius, bubbleMaxRadius );
		Vector2 tempPos = new Vector2();
		tempPos = Vector2.ClampMagnitude (bubblePos, bubbleMaxRadius);

		Vector3 newBubblePos = new Vector3( tempPos.x , tempPos.y , bubble.localPosition.z );

		bubble.localPosition = newBubblePos;
	}

	// Calibration
	public void ToggleBalanceCalibrationMode( bool toggle ) {
		toggles[(int)PFCToggles.CalibrationModeOn] = toggle;
	}

	public void ClickedOnFocusOnBackButton() {
		toggles[(int)PFCToggles.InLevelingPosition] = true;
		PracticeManager.s_instance.StartNewCameraSlerp( backPivot, backCamPos );
	}

	public void ClickedOnLeaveBackFocusButton() {
		if( toggles[(int)PFCToggles.BalanceIsLeveled] ) {
			accessBehindViewCanvas.SetActive( false );
		}
		else
			accessBehindViewCanvas.SetActive( true );
		screwsCanvas.SetActive( false );
		bubbleCanvas.SetActive( false );
		returnFromBackCanvas.SetActive( false );
		toggles[(int)PFCToggles.InLevelingPosition] = false;
		PracticeManager.s_instance.StartNewCameraSlerp( defaultPivotPos, backDefaultCamPos );
	}


	// Use
	public void ClickedOnFocusOnBalanceButton() {
		toggles[(int)PFCToggles.FocusedOnBalanceFace] = true;
		PracticeManager.s_instance.StartNewCameraSlerp( facePivotPos, faceCamPos );
	}

	public void ClickedOnLeaveFaceFocusButton() {
		toggles[(int)PFCToggles.FocusedOnBalanceFace] = false;
		PracticeManager.s_instance.StartNewCameraSlerp( defaultPivotPos, defaultCamPos );
	}

	private IEnumerator ToggleBalancedCalibrationOn() {
		yield return new WaitForSeconds( 5f );
		toggles[(int)PFCToggles.BalanceCalibrated] = true;
		toggles[(int)PFCToggles.CalibrationModeOn] = false;
		SoundtrackManager.s_instance.PlayAudioSource( SoundtrackManager.s_instance.buttonBeep );
		ReadoutDisplay.s_instance.ToggleDisplay( true, true, false );
		ReadoutDisplay.s_instance.ZeroOut();
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
		toggles[(int)PFCToggles.WeighContainerFilled] = true;
		riceContainerInside.SetActive( false );
		ClearSelectedObject();
		//selectedObject = SelectableObject.SelectableObjectType.None;
		riceContainerOutside.SetActive( true );
		Debug.Log( "Ended Pouring Rice." );
	}
}
