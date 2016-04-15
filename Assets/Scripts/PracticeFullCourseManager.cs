﻿using UnityEngine;
using System.Collections;

public class PracticeFullCourseManager : BasePracticeSubmodule {

	// Prepare
	public RectTransform bubble;

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

	private enum PFCToggles { InLevelingPosition, BalanceIsLeveled,
								WeightOutside, WeightInside, BalanceOn, BalanceCalibrated, CalibrationModeOn,
								WeighContainerOutside, WeightContainerInside, LDoorOpen, RDoorOpen, FocusedOnBalanceFace, BalanceTared, WeighContainerFilled, ReadingStabilized }

	void Update() {
		//FIXME remove this comment when done
		//CheckInputs();

		if( ReadoutDisplay.s_instance.hasStableReading && !toggles[(int)PFCToggles.ReadingStabilized] ) {
			toggles[(int)PFCToggles.ReadingStabilized] = true;
		}

//		TODO this case will be whatever step is the bubble leveling step
		switch( currentStep ) {
		case 1:
			if( Mathf.Abs(bubble.localPosition.x) <= bubbleWinThreshold && Mathf.Abs(bubble.localPosition.y) <= bubbleWinThreshold ) {
				toggles[(int)PFCToggles.BalanceIsLeveled] = true;
			}
			break;
		}
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

	/// <summary>
	/// Resets the scene objects to their default position.
	/// </summary>
	public override void ResetScene() {

	}

	public override void ClearSelectedObject( bool slerpToDefaultPos ) {
		if( selectedObject == SelectableObject.SelectableObjectType.None )
			return;

		//TODO Turn off highlights and shit
		selectedObject = SelectableObject.SelectableObjectType.None;
	}

	public override void ClickedOnObject( SelectableObject clickedOnObject, bool usedForceps ) {
		SelectableObject.SelectableObjectType clickedObjectType = clickedOnObject.objectType;

		switch( clickedObjectType )
		{
		case SelectableObject.SelectableObjectType.TareButton:
			if( usedForceps )
				return;
			if( toggles[(int)PFCToggles.FocusedOnBalanceFace] && !toggles[(int)PFCToggles.BalanceCalibrated] ) {
				ReadoutDisplay.s_instance.PlayCalibrationModeAnimation();
			} else if( toggles[(int)PFCToggles.WeightContainerInside] ) {
				ReadoutDisplay.s_instance.ZeroOut();
				toggles[(int)PFCToggles.BalanceTared] = true;
				SoundtrackManager.s_instance.PlayAudioSource( SoundtrackManager.s_instance.buttonBeep );
			}
			break;

		case SelectableObject.SelectableObjectType.RiceContainer:
			if( usedForceps || toggles[(int)PFCToggles.WeighContainerFilled] )
				return;
			// If we aren't holding an object when we click the weight, make it our selected object.
			if( PracticeCalibrateBalanceManager.s_instance.selectedObject == SelectableObject.SelectableObjectType.None ) {
				PracticeCalibrateBalanceManager.s_instance.selectedObject = SelectableObject.SelectableObjectType.RiceContainer;
				riceContainerOutside.GetComponent<Renderer>().materials[1].SetFloat( "_Thickness", 3.5f );
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
			bool newDoorOpenValue = !toggles[(int)PFCToggles.RDoorOpen];
			toggles[(int)PFCToggles.RDoorOpen] = newDoorOpenValue;
			ReadoutDisplay.s_instance.doorsAreOpen = newDoorOpenValue;
			rightGlassDoor.GetComponent<Animator>().SetTrigger( "Clicked" );
			SoundtrackManager.s_instance.PlayAudioSource( SoundtrackManager.s_instance.slidingDoor );
			break;

		case SelectableObject.SelectableObjectType.WeighContainer:
			if( toggles[(int)PFCToggles.WeightContainerInside] && PracticeUseBalanceManager.s_instance.selectedObject == SelectableObject.SelectableObjectType.RiceContainer ) {
				if( toggles[(int)PFCToggles.WeighContainerFilled])
					return;

				riceContainerOutside.GetComponent<Renderer>().materials[1].SetFloat( "_Thickness", 0f );
				StartCoroutine( PourRice() );
			} else if( toggles[(int)PFCToggles.WeighContainerOutside] && PracticeUseBalanceManager.s_instance.selectedObject == SelectableObject.SelectableObjectType.None ) {
				PracticeUseBalanceManager.s_instance.selectedObject = SelectableObject.SelectableObjectType.WeighContainer;
				weighContainerOutside.GetComponent<Renderer>().materials[1].SetFloat( "_Thickness", 3.5f );
			}
			break;

		case SelectableObject.SelectableObjectType.WeighPan:
			// If we're holding the calibration weight when clicking the weigh pan, place the weight.
			if( selectedObject == SelectableObject.SelectableObjectType.CalibrationWeight ) {
				toggles[(int)PFCToggles.WeightInside] = true;
				weightInside.SetActive( true );
				toggles[(int)PFCToggles.WeightOutside] = false;
				weightOutside.SetActive( false );
			} else if( !toggles[(int)PFCToggles.WeightContainerInside] && selectedObject == SelectableObject.SelectableObjectType.WeighContainer) {
				weighContainerOutside.SetActive( false );
				toggles[(int)PFCToggles.WeighContainerOutside] = false;
				weighContainerInside.SetActive( true );
				toggles[(int)PFCToggles.WeightContainerInside] = true;
				selectedObject = SelectableObject.SelectableObjectType.None;
				ReadoutDisplay.s_instance.readoutNumberText.text = "9.7306";
			}
			break;
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		case SelectableObject.SelectableObjectType.CalibrationWeight:
			if( !usedForceps )
				return;
			// If we aren't holding an object when we click the weight, make it our selected object.
			if( PracticeCalibrateBalanceManager.s_instance.selectedObject == SelectableObject.SelectableObjectType.None ) {
				// Toggle on highlights
				if( toggles[(int)PFCToggles.WeightInside] ) {
					toggles[(int)PFCToggles.WeightInside] = false;
					weightInside.SetActive( false );
					toggles[(int)PFCToggles.WeightInside] = true;
					weightOutside.SetActive( true );
					weightOutside.GetComponent<Renderer>().materials[1].SetFloat( "_Thickness", 0f );
				} else if ( toggles[(int)PFCToggles.WeightOutside] ) {
					PracticeCalibrateBalanceManager.s_instance.selectedObject = SelectableObject.SelectableObjectType.CalibrationWeight;
					weightOutside.GetComponent<Renderer>().materials[1].SetFloat( "_Thickness", 3.5f );
				}
			}
			break;

		case SelectableObject.SelectableObjectType.LeftDoor:
			if( usedForceps )
				return;
//			// Check what state the animation is in and toggle on the opposite since we're going to transition animations after.
//			if( leftGlassDoor.GetCurrentAnimatorStateInfo(0).fullPathHash == leftDoorOpenState ) {
//				toggles[(int)PCToggles.LDoorOpen] = false;
//			} else if ( leftGlassDoor.GetCurrentAnimatorStateInfo(0).fullPathHash == leftDoorClosedState ) {
//				toggles[(int)PCToggles.LDoorOpen] = true;
//			}
//			leftGlassDoor.GetComponent<Animator>().SetTrigger( "Clicked" );
			break;

		case SelectableObject.SelectableObjectType.OnButton:
			if( usedForceps )
				return;
			if( toggles[(int)PFCToggles.FocusedOnBalanceFace] ) {
				toggles[(int)PFCToggles.BalanceOn] = true;
				ReadoutDisplay.s_instance.TurnBalanceOn();
			}
			break;
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
		selectedObject = SelectableObject.SelectableObjectType.None;
		riceContainerOutside.SetActive( true );
		Debug.Log( "Ended Pouring Rice." );
	}
}
