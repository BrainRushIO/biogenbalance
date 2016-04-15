using UnityEngine;
using System.Collections;

public abstract class BasePracticeSubmodule : MonoBehaviour {
	public static BasePracticeSubmodule s_instance;

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

	/// <summary>
	/// Resets the scene objects to their default position.
	/// </summary>
	public virtual void ResetScene() {}

	protected virtual void SelectObject( SelectableObject newSelection ) {
		ClearSelectedObject( false );
	}

	/// <summary>
	/// Checks the inputs to see if we have met the requirements to go to the next step.
	/// </summary>
	public void CheckInputs() {
		for( int i = 0; i < toggles.Length; i++ ) {
			if( toggles[i] != inputs[i] )
				return;
		}
		PracticeManager.s_instance.GoToNextStep();
	}

	public abstract void ClearSelectedObject( bool slerpToDefaultPos );/* {
		if( selectedObject == SelectableObject.SelectableObjectType.None )
			return;

		//TODO Turn off highlights and shit
		selectedObject = SelectableObject.SelectableObjectType.None;
	}*/

	public abstract void ClickedOnObject( SelectableObject clickedOnObject );/* {
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
