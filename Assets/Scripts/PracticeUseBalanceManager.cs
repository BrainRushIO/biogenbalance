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
	}

	void Update() {
		CheckInputs();

		if( ReadoutDisplay.s_instance.hasStableReading && !toggles[(int)PUToggles.ReadingStabilized] ) {
			toggles[(int)PUToggles.ReadingStabilized] = true;
		}
	}

	public override void UpdateSceneContents( int stepIndex ) {
		currentStep = stepIndex;

		// Get init data from step at given index. execute logic depending on data.
		toggles = moduleSteps[currentStep].GetToggles();
		inputs = moduleSteps[currentStep].GetInputs();

		// Have steps execute specific step logic if they have it
		moduleSteps[currentStep].ExecuteStepLogic();
	}

	public override void ClearSelectedObject( bool slerpToDefaultPos ) {
		// HACK remove this
		return;

		if( selectedObject == SelectableObject.SelectableObjectType.None )
			return;

		weighContainerOutside.GetComponent<Renderer>().materials[1].SetFloat("Thickness", 0f );
		riceContainerOutside.GetComponent<Renderer>().materials[1].SetFloat("Thickness", 0f );
		selectedObject = SelectableObject.SelectableObjectType.None;
	}

	public override void ClickedOnObject( SelectableObject clickedOnObject, bool usedForceps ) {
		SelectableObject.SelectableObjectType clickedObjectType = clickedOnObject.objectType;

		switch( clickedObjectType )
		{
		case SelectableObject.SelectableObjectType.TareButton:
			if( toggles[(int)PUToggles.WeightContainerInside] ) {
				ReadoutDisplay.s_instance.ZeroOut();
				toggles[(int)PUToggles.BalanceTared] = true;
				SoundtrackManager.s_instance.PlayAudioSource( SoundtrackManager.s_instance.buttonBeep );
			}
			break;

		case SelectableObject.SelectableObjectType.RiceContainer:
			if( usedForceps || toggles[(int)PUToggles.WeighContainerFilled] )
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
			bool newDoorOpenValue = !toggles[(int)PUToggles.RDoorOpen];
			toggles[(int)PUToggles.RDoorOpen] = newDoorOpenValue;
			ReadoutDisplay.s_instance.doorsAreOpen = newDoorOpenValue;
			rightGlassDoor.GetComponent<Animator>().SetTrigger( "Clicked" );
			SoundtrackManager.s_instance.PlayAudioSource( SoundtrackManager.s_instance.slidingDoor );
			break;

		case SelectableObject.SelectableObjectType.WeighContainer:
			if( toggles[(int)PUToggles.WeightContainerInside] && PracticeUseBalanceManager.s_instance.selectedObject == SelectableObject.SelectableObjectType.RiceContainer ) {
				if( toggles[(int)PUToggles.WeighContainerFilled])
					return;
				
				riceContainerOutside.GetComponent<Renderer>().materials[1].SetFloat( "_Thickness", 0f );
				StartCoroutine( PourRice() );
			} else if( toggles[(int)PUToggles.WeighContainerOutside] && PracticeUseBalanceManager.s_instance.selectedObject == SelectableObject.SelectableObjectType.None ) {
				PracticeUseBalanceManager.s_instance.selectedObject = SelectableObject.SelectableObjectType.WeighContainer;
				weighContainerOutside.GetComponent<Renderer>().materials[1].SetFloat( "_Thickness", 3.5f );
			}
			break;

		case SelectableObject.SelectableObjectType.WeighPan:
			if( !toggles[(int)PUToggles.WeightContainerInside] && selectedObject == SelectableObject.SelectableObjectType.WeighContainer) {
				weighContainerOutside.SetActive( false );
				toggles[(int)PUToggles.WeighContainerOutside] = false;
				weighContainerInside.SetActive( true );
				toggles[(int)PUToggles.WeightContainerInside] = true;
				selectedObject = SelectableObject.SelectableObjectType.None;
				ReadoutDisplay.s_instance.readoutNumberText.text = "9.7306";
			}
			break;
		}
	}

	public void ClickedOnFocusOnBalanceButton() {
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
}
