using UnityEngine;
using System.Collections;

public class PracticeUseBalanceManager : BasePracticeSubmodule {
	public GameObject weighContainerOutside, weighContainerInside, riceContainerOutside, riceContainerInside;
	public Animator leftGlassDoor, rightGlassDoor;
	public Transform defaultPivotPos, defaultCamPos, facePivotPos, faceCamPos;
	
	private enum PUToggles { WeighContainerOutside, WeightContainerInside, LDoorOpen, RDoorOpen, FocusedOnBalanceFace, BalanceTared, WeighContainerFilled }
	// Used to check if door is open or closed
	private int rightDoorOpenState, rightDoorClosedState, leftDoorOpenState, leftDoorClosedState;

	public override void UpdateSceneContents( int stepIndex ) {
		currentStep = stepIndex;

		// Get init data from step at given index. execute logic depending on data.
		toggles = moduleSteps[currentStep].GetToggles();
		inputs = moduleSteps[currentStep].GetInputs();

		// Have steps execute specific step logic if they have it
		moduleSteps[currentStep].ExecuteStepLogic();
	}

	public override void ClearSelectedObject( bool slerpToDefaultPos ) {
		if( selectedObject == SelectableObject.SelectableObjectType.None )
			return;

		weighContainerOutside.GetComponent<Renderer>().materials[1].SetFloat("Thickness", 0f );
		weighContainerInside.GetComponent<Renderer>().materials[1].SetFloat("Thickness", 0f );
		riceContainerOutside.GetComponent<Renderer>().materials[1].SetFloat("Thickness", 0f );
		riceContainerInside.GetComponent<Renderer>().materials[1].SetFloat("Thickness", 0f );
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
			}
			break;

		case SelectableObject.SelectableObjectType.RiceContainer:
			if( usedForceps )
				return;
			// If we aren't holding an object when we click the weight, make it our selected object.
			if( PracticeCalibrateBalanceManager.s_instance.selectedObject == SelectableObject.SelectableObjectType.None ) {
				PracticeCalibrateBalanceManager.s_instance.selectedObject = SelectableObject.SelectableObjectType.CalibrationWeight;
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
			toggles[(int)PUToggles.RDoorOpen] = !toggles[(int)PUToggles.RDoorOpen];
			rightGlassDoor.GetComponent<Animator>().SetTrigger( "Clicked" );
			SoundtrackManager.s_instance.PlayAudioSource( SoundtrackManager.s_instance.slidingDoor );
			break;

		case SelectableObject.SelectableObjectType.WeighContainer:
			break;

		case SelectableObject.SelectableObjectType.WeighPan:
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
}
