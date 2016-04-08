using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PracticeManager : MonoBehaviour {
	public static PracticeManager s_instance;

	public ListViewButton defaultListViewText;

	private List<StepsListEntry> practiceStepList;
	private int currentStepIndex = 0;

	void Awake() {
		if( s_instance == null ) {
			s_instance = this;
		} else {
			Debug.LogWarning( "Destroying duplicate Practice Manager." );
			DestroyImmediate( this.gameObject );
		}
	}

	void Start () {
	
	}

	void Update () {
		switch( currentStepIndex )
		{
		default:
			break;
		}
	}

	public void PressedHintButton() {
		UIManager.s_instance.ToggleSidePanelOn( true );
	}

	public void GoToNextStep() {
		currentStepIndex++;
		while ( currentStepIndex < practiceStepList.Count && practiceStepList[currentStepIndex].isSectionParent )
			currentStepIndex++;

		if( currentStepIndex >= practiceStepList.Count ) {
			Debug.LogWarning( "Current step index outside of list bounds." );
			return;
		}

		GoToStep(currentStepIndex);
	}

	public void GoToStep( int stepIndex ) {
		ResetInputsAndObjects();
		currentStepIndex = stepIndex;

		switch( stepIndex )
		{
		default:
			break;
		}

		UIManager.s_instance.UpdateDescriptionViewText( practiceStepList[stepIndex].uiText.descriptionViewText);
	}

	public void ResetInputsAndObjects() {
		
	}
}
