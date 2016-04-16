using UnityEngine;
using System.Collections;

public abstract class BasePracticeSubmodule : MonoBehaviour {
	public static BasePracticeSubmodule s_instance;

	/// <summary>
	/// The current step. Zero-indexed. The List View steps are One-indexed e.g. List Step 1 is currentStep 0.
	/// </summary>
	public int currentStep = 0;
	public SelectableObject.SelectableObjectType selectedObject;
	public BasePracticeModuleStep[] moduleSteps;

	[SerializeField]
	protected bool[] toggles;
	[SerializeField]
	protected bool[] inputs;

	void Awake() {
		if( s_instance == null ) {
			s_instance = this;
			Init();
		} else {
			Debug.LogWarning( "Deleting duplicate BaseAcquireModule named " + gameObject.name );
		}
	}

	protected virtual void Init () {
		moduleSteps = GetComponentsInChildren<BasePracticeModuleStep>();
		selectedObject = SelectableObject.SelectableObjectType.None;

		if( moduleSteps == null || moduleSteps.Length == 0) {
			Debug.LogWarning( "Could not find any children under BaseAcquireModule named: " + gameObject.name );
		}
	}

	public abstract void UpdateSceneContents( int stepIndex );

	protected virtual void SelectObject( SelectableObject newSelection ) {
		ClearSelectedObject();

		selectedObject = newSelection.objectType;
		ApplicationManager.s_instance.SetSpecialMouseMode( (int)ApplicationManager.SpecialCursorMode.ClosedHand );
	}

	protected virtual void SelectObject( SelectableObject.SelectableObjectType newSelection ) {
		ClearSelectedObject();

		selectedObject = newSelection;
		ApplicationManager.s_instance.SetSpecialMouseMode( (int)ApplicationManager.SpecialCursorMode.ClosedHand );
	}

	public virtual void ClearSelectedObject( ) {
		if( selectedObject == SelectableObject.SelectableObjectType.None )
			return;

		selectedObject = SelectableObject.SelectableObjectType.None;
		ApplicationManager.s_instance.SetSpecialMouseMode( (int)ApplicationManager.SpecialCursorMode.None );
	}

	/// <summary>
	/// Checks the inputs to see if we have met the requirements to go to the next step.
	/// </summary>
	public bool CheckInputs() {
		for( int i = 0; i < toggles.Length; i++ ) {
			if( toggles[i] != inputs[i] )
				return false;
		}
		PracticeManager.s_instance.GoToNextStep();
		return true;
	}

	public virtual void HoveredOverObject( SelectableObject obj ) {}

	public abstract void ClickedOnObject( SelectableObject clickedOnObject, bool usedForceps );/* {
		SelectableObject.SelectableObjectType clickedObjectType = clickedOnObject.objectType;

		switch( clickedObjectType ) {
		case SelectableObject.SelectableObjectType.CalibrationWeight:
			break;
		case SelectableObject.SelectableObjectType.LeftDoor:
			break;
		case SelectableObject.SelectableObjectType.TareButton:
			break;
		case SelectableObject.SelectableObjectType.LevelingScrews:
			break;
		case SelectableObject.SelectableObjectType.OnButton:
			break;
		case SelectableObject.SelectableObjectType.RiceContainer:
			break;
		case SelectableObject.SelectableObjectType.RightDoor:
			break;
		case SelectableObject.SelectableObjectType.WeighContainer:
			break;
		case SelectableObject.SelectableObjectType.WeighPan:
			break;
		}
	}*/
}
