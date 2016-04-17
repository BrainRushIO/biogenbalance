using UnityEngine;
using System.Collections;

public class ValidateManager : MonoBehaviour {
	public static ValidateManager s_instance;

	public Camera sceneCamera;
	[System.NonSerialized]
	public OrbitCamera orbitCam;
	public bool isDragging = false;
	public bool isInIntro = true;
	public CanvasGroup introPopup, completionPopup, failPopup;
	public bool hasFinishedModule = false;
	public bool hasFailedModule = false;

	/// <summary>
	/// The current step. Zero-indexed. The List View steps are One-indexed e.g. List Step 1 is currentStep 0.
	/// </summary>
	public int currentStep = -1;
	public SelectableObject.SelectableObjectType selectedObject;
	public ValidateStep[] moduleSteps;

	[SerializeField]
	private bool[] toggles;
	[SerializeField]
	private bool[] inputs;

	private Vector3 currentCameraPivot, currentCameraStartPos;
	private bool isCameraRotLerping = false;
	private bool isLerpingToNewPosition = false;
	private bool hasClickedDownOnItem = false;
	private int numMistakes = 0;
	private float moduleStartTime = 0f;

	#region PFC_VARS
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

	public enum VToggles { InLevelingPosition, BalanceIsLeveled,
		WeightOutside, WeightInside, BalanceOn, BalanceCalibrated, CalibrationModeOn,
		WeighContainerOutside, WeightContainerInside, LDoorOpen, RDoorOpen, FocusedOnBalanceFace, BalanceTared, WeighContainerFilled, ReadingStabilized }

	private int rightDoorOpenState, rightDoorClosedState, leftDoorOpenState, leftDoorClosedState;
	#endregion

	void Awake() {
		if( s_instance == null ) {
			s_instance = this;
			Init();
		} else {
			Debug.LogWarning( "Destroying duplicate Validate Manager." );
			DestroyImmediate( this.gameObject );
		}
	}

	void Start () {
		// Initialization
		ApplicationManager.s_instance.ChangeMouseMode( (int)ApplicationManager.MouseMode.Pointer );
		UIManager.s_instance.ToggleToolsActive( true, true, true, true );
		UIManager.s_instance.ToggleSidePanel( false, false );
		UIManager.s_instance.hintButton.gameObject.SetActive( false );
		orbitCam = sceneCamera.GetComponent<OrbitCamera>();

		// Init Toggles
		toggles = new bool[15];
		for( int i = 0; i < toggles.Length; i++ )
			toggles[i] = false;

		toggles[(int)VToggles.WeighContainerOutside] = true;
		toggles[(int)VToggles.WeightOutside] = true;

		rightDoorOpenState = Animator.StringToHash( "Base Layer.SMB_RightGlass_Open" );
		rightDoorClosedState = Animator.StringToHash( "Base Layer.SMB_RightGlass_Closed" );
		foreach( ValidateStep step in moduleSteps )
			step.InitToggles();

		// Start Module
		currentStep = -1;
		//GoToNextStep(); This is now being called by closing the intro popup window.
	}

	void Update() {
		if( isInIntro || hasFinishedModule || hasFailedModule )
			return;

		CheckHoverAndClicks();

		// Making exceptions for what layers to ignore depending on step. Shitty, I know but we need to design this better next time.
		switch( currentStep )
		{
		case 0:
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
			CheckInputs( new VToggles[2] { VToggles.LDoorOpen, VToggles.RDoorOpen } );
			break;
		case 9:
			CheckInputs(new VToggles[1] { VToggles.FocusedOnBalanceFace } );
			break;
		case 14:
			CheckInputs( new VToggles[2] {VToggles.FocusedOnBalanceFace, VToggles.RDoorOpen} );
			break;
		case 15:
		case 16:
			CheckInputs( new VToggles[1] {VToggles.FocusedOnBalanceFace} );
			break;
		default:
			CheckInputs();
			break;
		}

		if( ReadoutDisplay.s_instance.hasStableReading && !toggles[(int)VToggles.ReadingStabilized] ) {
			toggles[(int)VToggles.ReadingStabilized] = true;
		}

		UpdateDoorTogglesBasedOnAnimationState( false );

		//		This case will be whatever step is the bubble leveling step
		switch( currentStep ) {
		case 1:
			if( Mathf.Abs(bubble.localPosition.x) <= bubbleWinThreshold && Mathf.Abs(bubble.localPosition.y) <= bubbleWinThreshold ) {
				toggles[(int)VToggles.BalanceIsLeveled] = true;
				SoundtrackManager.s_instance.PlayAudioSource( SoundtrackManager.s_instance.buttonBeep );
				//ClickedOnLeaveBackFocusButton();
				screwsCanvas.SetActive( false );
			}
			break;
		}
	}

	private void CheckHoverAndClicks() {
		// Inupt is disbaled if camera is lerping
		if( isLerpingToNewPosition )
			return;

		Ray mouseHoverRay = sceneCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit mouseHoverHit;
		if ( Physics.Raycast(mouseHoverRay, out mouseHoverHit) ) {
			SelectableObject hoveredObject = mouseHoverHit.transform.GetComponent<SelectableObject>();
			HoveredOverObject( hoveredObject );
		} else {
			HoveredOverObject( null );
		}


		// Finishing click
		if ( Input.GetMouseButtonUp(0) ) {
			// If we started and finished a click on an item, then interact with it
			if( hasClickedDownOnItem && !isDragging && ( ApplicationManager.s_instance.currentMouseMode == ApplicationManager.MouseMode.Pointer || ApplicationManager.s_instance.currentMouseMode == ApplicationManager.MouseMode.Forceps ) ) {
				Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if ( Physics.Raycast(ray, out hit) ) {
					SelectableObject clickedObject = hit.transform.GetComponent<SelectableObject>();
					if( clickedObject != null ) {
						bool usedForceps = ( ApplicationManager.s_instance.currentMouseMode == ApplicationManager.MouseMode.Forceps ) ? true : false;
						ClickedOnObject( clickedObject, usedForceps );
					}
				}
			} else if( !isDragging && !ApplicationManager.s_instance.userIsInteractingWithUI ) { // If we clicked away from any objects in the 3D Scene View, clear selection
				ClearSelectedObject();
			}

			hasClickedDownOnItem = false;
			isDragging = false;
		}

		// Pointer clicking is disabled if user is hovering over GUI
		if( ApplicationManager.s_instance.userIsInteractingWithUI )
			return;
		// Starting a click
		if ( Input.GetMouseButtonDown(0) && ( ApplicationManager.s_instance.currentMouseMode == ApplicationManager.MouseMode.Pointer || ApplicationManager.s_instance.currentMouseMode == ApplicationManager.MouseMode.Forceps ) ){
			Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if ( Physics.Raycast(ray, out hit) ){
				if ( hit.transform.gameObject.tag == "Animatable" || hit.transform.gameObject.tag == "Selectable" ) {
					hasClickedDownOnItem = true;
				}
			}
		}
	}

	public void GoToNextStep() {
		if( hasFinishedModule || hasFailedModule )
			return;

		currentStep++;

		// Win condition
		if( currentStep >= moduleSteps.Length ) {
			CompleteModule();
			return;
		}

		// Update scene
		UpdateSceneContents( currentStep );
		Debug.Log( "Now on step: " + currentStep );
	}

	private IEnumerator LerpCameraLookAt() {
		isCameraRotLerping = true;
		float elapsedTime = 0f;
		float slerpTime = 1;
		float startTime = Time.time;
		Quaternion startRot = sceneCamera.transform.rotation;

		while( elapsedTime < slerpTime ) {
			//			if( (elapsedTime/slerpTime) >= 0.99f ) {
			//				break;
			//			}
			Quaternion targetRot = Quaternion.LookRotation( (currentCameraPivot-sceneCamera.transform.position).normalized );
			sceneCamera.transform.rotation = Quaternion.Lerp( startRot, targetRot, elapsedTime/slerpTime );
			yield return null;
			elapsedTime = Time.time-startTime;
		}
		sceneCamera.transform.LookAt( currentCameraPivot );
		isCameraRotLerping = false;
	}

	private IEnumerator SlerpToNewCamPos() {
		isLerpingToNewPosition = true;
		orbitCam.transform.parent = null;
		orbitCam.pivotParent.position = currentCameraPivot;
		float elapsedTime = 0f;
		float slerpTime = 1f;
		float startTime = Time.time;
		Vector3 startPos = sceneCamera.transform.position;

		while( elapsedTime < slerpTime ) {
			//			if( (elapsedTime/slerpTime) >= 0.99f ) {
			//				break;
			//			}
			sceneCamera.transform.position = Vector3.Lerp( startPos, currentCameraStartPos, elapsedTime/slerpTime );
			if( !isCameraRotLerping )
				sceneCamera.transform.LookAt( currentCameraPivot );
			yield return null;
			elapsedTime = Time.time-startTime;
		}
		orbitCam.pivotParent.LookAt( orbitCam.transform );
		orbitCam.transform.parent = orbitCam.pivotParent;

		sceneCamera.transform.position = currentCameraStartPos;
		sceneCamera.transform.LookAt( currentCameraPivot );
		isLerpingToNewPosition = false;
		orbitCam.canRotate = true;
		orbitCam.canZoom = true;
	}

	public void StartNewCameraSlerp( Transform newPivot, Transform newCamPos ) {
		currentCameraPivot = newPivot.position;
		currentCameraStartPos = newCamPos.position;

		StartCoroutine( LerpCameraLookAt() );
		StartCoroutine( SlerpToNewCamPos() );
	}

	protected virtual void Init () {
		moduleSteps = GetComponentsInChildren<ValidateStep>();
		selectedObject = SelectableObject.SelectableObjectType.None;

		if( moduleSteps == null || moduleSteps.Length == 0) {
			Debug.LogWarning( "Could not find any children under BaseAcquireModule named: " + gameObject.name );
		}
	}

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
		GoToNextStep();
		return true;
	}

	#region PFC
	public void CheckInputs( VToggles[] ignoreToggles ) {
		for( int i = 0; i < toggles.Length; i++ ) {
			bool ignoreInput = false;

			foreach( VToggles toggle in ignoreToggles ){
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
		GoToNextStep();
	}

	public void UpdateSceneContents( int stepIndex ) {
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

	public void HoveredOverObject( SelectableObject obj ) {
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
			if( !toggles[(int)VToggles.FocusedOnBalanceFace] )
				break;

			if( ApplicationManager.s_instance.currentMouseMode == ApplicationManager.MouseMode.Pointer && selectedObject == SelectableObject.SelectableObjectType.None ) {
				ApplicationManager.s_instance.SetSpecialMouseMode( (int)ApplicationManager.SpecialCursorMode.PointingHand );
			}
			break;
		}
	}

	public void ClickedOnObject( SelectableObject clickedOnObject, bool usedForceps ) {
		SelectableObject.SelectableObjectType clickedObjectType = clickedOnObject.objectType;

		switch( clickedObjectType )
		{
		case SelectableObject.SelectableObjectType.TareButton:
			if( usedForceps || !toggles[(int)VToggles.FocusedOnBalanceFace] )
				return;
			if( toggles[(int)VToggles.FocusedOnBalanceFace] && !toggles[(int)VToggles.BalanceCalibrated] && toggles[(int)VToggles.BalanceOn] ) {
				ReadoutDisplay.s_instance.PlayCalibrationModeAnimation();
			} else if( toggles[(int)VToggles.WeightContainerInside] && !toggles[(int)VToggles.RDoorOpen] ) {
				ReadoutDisplay.s_instance.ZeroOut();
				toggles[(int)VToggles.BalanceTared] = true;
				SoundtrackManager.s_instance.PlayAudioSource( SoundtrackManager.s_instance.buttonBeep );
			} else {
				Fail();
			}
			break;

		case SelectableObject.SelectableObjectType.RiceContainer:
			if( usedForceps || toggles[(int)VToggles.WeighContainerFilled] || !toggles[(int)VToggles.WeightContainerInside] )
				return;
			// If we aren't holding an object when we click the weight, make it our selected object.
			if( selectedObject == SelectableObject.SelectableObjectType.None ) {
				SelectObject( SelectableObject.SelectableObjectType.RiceContainer );
			} else {
				Fail();
			}
			break;

		case SelectableObject.SelectableObjectType.RightDoor:
			if( usedForceps )
				return;
			// Check what state the animation is in and toggle on the opposite since we're going to transition animations after.
			UpdateDoorTogglesBasedOnAnimationState( true );

			ReadoutDisplay.s_instance.doorsAreOpen = toggles[(int)VToggles.RDoorOpen];// newDoorOpenValue;
			rightGlassDoor.GetComponent<Animator>().SetTrigger( "Clicked" );
			SoundtrackManager.s_instance.PlayAudioSource( SoundtrackManager.s_instance.slidingDoor );
			break;

		case SelectableObject.SelectableObjectType.WeighContainer:
			if( toggles[(int)VToggles.WeightContainerInside] && selectedObject == SelectableObject.SelectableObjectType.RiceContainer ) {
				if( toggles[(int)VToggles.WeighContainerFilled])
					return;

				StartCoroutine( PourRice() );
			} 
			// It can only be selected if balance has been calibrated
			else if( toggles[(int)VToggles.WeighContainerOutside] && selectedObject == SelectableObject.SelectableObjectType.None && toggles[(int)VToggles.BalanceCalibrated] ) {
				SelectObject( SelectableObject.SelectableObjectType.WeighContainer );
			} else {
				Fail();
			}
			break;

		case SelectableObject.SelectableObjectType.WeighPan:
			// If we're holding the calibration weight when clicking the weigh pan, place the weight.
			if( selectedObject == SelectableObject.SelectableObjectType.CalibrationWeight ) {
				toggles[(int)VToggles.WeightInside] = true;
				weightInside.SetActive( true );
				toggles[(int)VToggles.WeightOutside] = false;
				weightOutside.SetActive( false );
				ClearSelectedObject();
			} else if( !toggles[(int)VToggles.WeightContainerInside] && selectedObject == SelectableObject.SelectableObjectType.WeighContainer) {
				weighContainerOutside.SetActive( false );
				toggles[(int)VToggles.WeighContainerOutside] = false;
				weighContainerInside.SetActive( true );
				toggles[(int)VToggles.WeightContainerInside] = true;
				ClearSelectedObject();
				ReadoutDisplay.s_instance.readoutNumberText.text = "9.7306";
			} else {
				Fail();
			}
			break;

			//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		case SelectableObject.SelectableObjectType.CalibrationWeight:
			if( !usedForceps )
				return;
			// If we aren't holding an object when we click the weight, make it our selected object.
			if( selectedObject == SelectableObject.SelectableObjectType.None  ) {
				// Toggle on highlights
				if( toggles[(int)VToggles.WeightInside] ) {
					toggles[(int)VToggles.WeightInside] = false;
					weightInside.SetActive( false );
					toggles[(int)VToggles.WeightOutside] = true;
					weightOutside.SetActive( true );
				} else if ( toggles[(int)VToggles.WeightOutside] || toggles[(int)VToggles.CalibrationModeOn] ) {
					SelectObject( SelectableObject.SelectableObjectType.CalibrationWeight );
				}
			} else {
				Fail();
			}
			break;

		case SelectableObject.SelectableObjectType.OnButton:
			if( usedForceps || !toggles[(int)VToggles.FocusedOnBalanceFace] )
				return;

			if( toggles[(int)VToggles.FocusedOnBalanceFace] && toggles[(int)VToggles.BalanceIsLeveled] ) {
				toggles[(int)VToggles.BalanceOn] = true;
				ReadoutDisplay.s_instance.TurnBalanceOn();
			} else {
				Fail();
			}
			break;
		}
	}

	private void Fail() {
		Debug.LogWarning( "Fail" );
		hasFailedModule = true;
		StartCoroutine(ToggleOnFailPopup() );
	}

	private void UpdateDoorTogglesBasedOnAnimationState( bool inverse ) {
		if( rightGlassDoor.GetCurrentAnimatorStateInfo(0).fullPathHash == rightDoorOpenState ) {
			toggles[(int)VToggles.RDoorOpen] = ( inverse ) ? false : true;
		} else if ( rightGlassDoor.GetCurrentAnimatorStateInfo(0).fullPathHash == rightDoorClosedState ) {
			toggles[(int)VToggles.RDoorOpen] = ( inverse ) ? true : false;
		}
	}

	// Prepare
	public void ClickedOnPositioningButton() {
		toggles[(int)VToggles.InLevelingPosition] = true;
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
		toggles[(int)VToggles.CalibrationModeOn] = toggle;
	}

	public void ClickedOnFocusOnBackButton() {
		toggles[(int)VToggles.InLevelingPosition] = true;
		StartNewCameraSlerp( backPivot, backCamPos );
	}

	public void ClickedOnLeaveBackFocusButton() {
		if( toggles[(int)VToggles.BalanceIsLeveled] ) {
			accessBehindViewCanvas.SetActive( false );
		}
		else
			accessBehindViewCanvas.SetActive( true );
		screwsCanvas.SetActive( false );
		bubbleCanvas.SetActive( false );
		returnFromBackCanvas.SetActive( false );
		toggles[(int)VToggles.InLevelingPosition] = false;
		StartNewCameraSlerp( defaultPivotPos, backDefaultCamPos );
	}


	// Use
	public void ClickedOnFocusOnBalanceButton() {
		toggles[(int)VToggles.FocusedOnBalanceFace] = true;
		StartNewCameraSlerp( facePivotPos, faceCamPos );
	}

	public void ClickedOnLeaveFaceFocusButton() {
		toggles[(int)VToggles.FocusedOnBalanceFace] = false;
		StartNewCameraSlerp( defaultPivotPos, defaultCamPos );
	}

	private IEnumerator ToggleBalancedCalibrationOn() {
		yield return new WaitForSeconds( 5f );
		toggles[(int)VToggles.BalanceCalibrated] = true;
		toggles[(int)VToggles.CalibrationModeOn] = false;
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
		toggles[(int)VToggles.WeighContainerFilled] = true;
		riceContainerInside.SetActive( false );
		ClearSelectedObject();
		//selectedObject = SelectableObject.SelectableObjectType.None;
		riceContainerOutside.SetActive( true );
		Debug.Log( "Ended Pouring Rice." );
	}
	#endregion

	public void ClickedCloseIntroPopupButton() {
		StartCoroutine( CloseIntroPopup() );
	}

	private IEnumerator CloseIntroPopup() {
		float startTime = Time.time;
		float lerpDuration = 0.15f;

		while( lerpDuration > Time.time - startTime ) {
			introPopup.alpha = Mathf.Lerp( 1f, 0f, (Time.time-startTime)/lerpDuration );
			yield return null;
		}

		introPopup.transform.parent.gameObject.SetActive( false );

		isInIntro = false;
		GoToNextStep();
	}

	public void CompleteModule() {
		hasFinishedModule = true;
		ApplicationManager.s_instance.playerData.validate = true;
		ApplicationManager.s_instance.playerData.completionTime = Time.time-moduleStartTime;
		ApplicationManager.s_instance.Save();

		StartCoroutine( ToggleOnCompletionPopup() );
	}

	private IEnumerator ToggleOnCompletionPopup() {
		float startTime = Time.time;
		float lerpDuration = 0.15f;
		completionPopup.interactable = true;
		completionPopup.blocksRaycasts = true;

		while( lerpDuration >= Time.time - startTime ) {
			completionPopup.alpha = Mathf.Lerp( 0f, 1f, (Time.time-startTime)/lerpDuration );
			yield return null;
		}

		completionPopup.alpha = 1f;
	}

	private IEnumerator ToggleOnFailPopup() {
		float startTime = Time.time;
		float lerpDuration = 0.15f;
		failPopup.interactable = true;
		failPopup.blocksRaycasts = true;

		while( lerpDuration >= Time.time - startTime ) {
			failPopup.alpha = Mathf.Lerp( 0f, 1f, (Time.time-startTime)/lerpDuration );
			yield return null;
		}

		failPopup.alpha = 1f;
	}

	public void PressedTryAgainButton() {
		ApplicationManager.s_instance.ForceLoadScene( "V1" );
	}
}