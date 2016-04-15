using UnityEngine;
using System.Collections;

public class PracticePrepareBalanceManager : BasePracticeSubmodule {

	public RectTransform bubble;

	private float currentBubbleX, currentBubbleY;
	private float bubbleWinThreshold = 2.5f;
	private float bubbleMaxDiameter = 66.6f;

	void Start() {
		toggles = new bool[2];
		inputs = new bool[2];

		if( moduleSteps.Length > 0 ) {
			toggles = moduleSteps[0].GetToggles();
			inputs = moduleSteps[0].GetInputs();
		}
	}

	void Update() {
		CheckInputs();

		switch( currentStep ) {
		case 0:
			break;
		case 1:
			if( Mathf.Abs(bubble.position.x) <= bubbleWinThreshold && Mathf.Abs(bubble.position.y) <= bubbleWinThreshold ) {
				Debug.Log( "Yay you win!" );
			}
			break;
		}
	}

	public override void UpdateSceneContents( int stepIndex ) {
		currentStep = stepIndex;

		// Get init data from step at given index. execute logic depending on data.
		toggles = moduleSteps[stepIndex].GetToggles();
		inputs = moduleSteps[stepIndex].GetInputs();

		// Have steps execute specific step logic if they have it
		moduleSteps[stepIndex].ExecuteStepLogic();
	}

	/// <summary>
	/// Resets the scene objects to their default position.
	/// </summary>
	public override void ResetScene() {
	}

	protected override void SelectObject( SelectableObject newSelection ) {
		ClearSelectedObject( false );
	}

	public override void ClearSelectedObject( bool lerpToDefaultPos ) {
		if( selectedObject == SelectableObject.SelectableObjectType.None )
			return;

		//TODO Turn off highlights and shit
		selectedObject = SelectableObject.SelectableObjectType.None;
	}

	public override void ClickedOnObject( SelectableObject clickedOnObject ) {
		SelectableObject.SelectableObjectType clickedObjectType = clickedOnObject.objectType;

		switch( clickedObjectType ) 
		{
		case SelectableObject.SelectableObjectType.LevelingScrews:
			break;
		}
	}

	public void ClickedOnPositioningButton() {
		// Toggles button clicked
		toggles[0] = true;
	}

	public void ClickedLeftScrewUp() {
		Vector3 bubblePos = bubble.localPosition;
		bubblePos.x += bubbleMaxDiameter*0.1f;
		bubblePos.y += bubbleMaxDiameter*0.1f;
		bubble.localPosition = bubblePos;

		NormalizeBubblePos();
	}

	public void ClickedLeftScrewDown() {
		Vector3 bubblePos = bubble.position;
		bubblePos.x -= bubbleMaxDiameter*0.1f;
		bubblePos.y -= bubbleMaxDiameter*0.1f;
		bubble.position = bubblePos;

		NormalizeBubblePos();
	}

	public void ClickedRightScrewUp() {
		Vector3 bubblePos = bubble.position;
		bubblePos.x -= bubbleMaxDiameter*0.1f;
		bubblePos.y += bubbleMaxDiameter*0.1f;
		bubble.position = bubblePos;

		NormalizeBubblePos();
	}

	public void ClickedRightScrewDown() {
		Vector3 bubblePos = bubble.position;
		bubblePos.x += bubbleMaxDiameter*0.1f;
		bubblePos.y -= bubbleMaxDiameter*0.1f;
		bubble.position = bubblePos;

		NormalizeBubblePos();
	}

	void NormalizeBubblePos() {
		Vector2 bubblePos = new Vector2(bubble.localPosition.x, bubble.localPosition.y );

		bubblePos.x = Mathf.Clamp( bubble.localPosition.x, -bubbleMaxDiameter, bubbleMaxDiameter );
		bubblePos.y = Mathf.Clamp( bubble.localPosition.y, -bubbleMaxDiameter, bubbleMaxDiameter );
		Vector2 temp = new Vector2();
		temp.x = Mathf.Cos( bubblePos.x );
		temp.y = Mathf.Sin( bubblePos.y );

		Vector3 newBubblePos = new Vector3( temp.x, temp.y, bubble.localPosition.z );

		bubble.localPosition = newBubblePos;
	}
}
