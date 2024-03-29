﻿using UnityEngine;
using System.Collections;

public class PracticePrepareBalanceManager : BasePracticeSubmodule {

	public RectTransform bubble;
	public Canvas screwsCanvas, bubbleCanvas;

	private float currentBubbleX, currentBubbleY;
	private float bubbleWinThreshold = 2.5f;
	private float bubbleMaxRadius = 32f;

	void Start() {
		toggles = new bool[2];
		inputs = new bool[2];

		if( moduleSteps.Length > 0 ) {
			toggles = moduleSteps[0].GetToggles();
			inputs = moduleSteps[0].GetInputs();
		}
	}

	void Update() {
		if( PracticeManager.s_instance.isInIntro || PracticeManager.s_instance.hasFinishedModule )
			return;

		CheckInputs();

		switch( currentStep ) {
		case 0:
			UIManager.s_instance.ToggleSidePanel (true, false);
			break;
		case 1:
			if( Mathf.Abs(bubble.localPosition.x) <= bubbleWinThreshold && Mathf.Abs(bubble.localPosition.y) <= bubbleWinThreshold ) {
				screwsCanvas.gameObject.SetActive( false );
				bubbleCanvas.gameObject.SetActive( false );
				PracticeManager.s_instance.CompleteModule();
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

	public override void ClickedOnObject( SelectableObject clickedOnObject, bool usedForceps ) {
	}

	public void ClickedOnPositioningButton() {
		// Toggles button clicked
		toggles[0] = true;
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
}
