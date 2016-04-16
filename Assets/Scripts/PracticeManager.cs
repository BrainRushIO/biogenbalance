using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml;

public class PracticeManager : MonoBehaviour {
	public static PracticeManager s_instance;

	public enum PracticeModule { Choose, Prepare, Calibrate, Use, FullCourse }
	/// <summary>
	/// The type of the module. Set publicly in inspector.
	/// </summary>
	public PracticeModule moduleType = PracticeModule.Choose;

	public Camera sceneCamera;
	[System.NonSerialized]
	public OrbitCamera orbitCam;
	public ListViewButton defaultListViewText;
	public TextAsset practiceContentXML;
	public bool isDragging = false;

	public BasePracticeSubmodule submoduleManager;
	private List<StepsListEntry> practiceStepList;
	private Vector3 currentCameraPivot, currentCameraStartPos;
	private bool isCameraRotLerping = false;
	private int currentStepIndex = 0;
	private bool showListViewIndex = true;
	private bool isLerpingToNewPosition = false;
	private bool hasClickedDownOnItem = false;

	void Awake() {
		if( s_instance == null ) {
			s_instance = this;
			InitializePracticeStepList();
			InitializeAcquireListView();
		} else {
			Debug.LogWarning( "Destroying duplicate Practice Manager." );
			DestroyImmediate( this.gameObject );
		}
	}

	void Start () {
		ApplicationManager.s_instance.ChangeMouseMode( (int)ApplicationManager.MouseMode.Pointer );
		UIManager.s_instance.ToggleToolsActive( true, true, true, true );
		UIManager.s_instance.ToggleSidePanel( false, false );
		//HACK remove this line
		//UIManager.s_instance.nextButton.gameObject.SetActive( true );
		submoduleManager = BasePracticeSubmodule.s_instance;
		orbitCam = sceneCamera.GetComponent<OrbitCamera>();

		currentStepIndex = -1;
		GoToNextStep();
	}

	void Update() {
		Ray mouseHoverRay = sceneCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit mouseHoverHit;
		if ( Physics.Raycast(mouseHoverRay, out mouseHoverHit) ) {
			SelectableObject hoveredObject = mouseHoverHit.transform.GetComponent<SelectableObject>();
			submoduleManager.HoveredOverObject( hoveredObject );
		} else {
			submoduleManager.HoveredOverObject( null );
		}

		#region PointerInput
		// Inupt is disbaled if camera is lerping
		if( isLerpingToNewPosition )
			return;
		
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
						submoduleManager.ClickedOnObject( clickedObject, usedForceps );
					}
				}
			} else if( !isDragging && !ApplicationManager.s_instance.userIsInteractingWithUI ) { // If we clicked away from any objects in the 3D Scene View, clear selection
				submoduleManager.ClearSelectedObject();
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
		#endregion
	}

	public void PressedHintButton() {
		UIManager.s_instance.ToggleSidePanel( true, true );
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

		// Update scene
		StartCoroutine( InitializeNextStep() );

		// Get our new selected list view button, change its text white, set it interactable, and check its checkbox.
		ListViewButton newListViewButtonSelection = UIManager.s_instance.listViewContentParent.GetChild(currentStepIndex).GetComponent<ListViewButton>();
		newListViewButtonSelection.childText.color = Color.white;
		newListViewButtonSelection.checkBox.isOn = true;

		UIManager.s_instance.UpdateDescriptionViewText( "Hint: " + practiceStepList[stepIndex].uiText.descriptionViewText );
		ToggleListViewItemHighLight( stepIndex, true );
	}

	private IEnumerator InitializeNextStep() {
		if( submoduleManager == null ) {
			Debug.LogError( "Submodule manager is null" );
			yield break;
		}
		submoduleManager.UpdateSceneContents( practiceStepList[currentStepIndex].stepIndex );

		// Check if next step requires lerping to using pivot system
		if( submoduleManager.moduleSteps.Length > 0 ) {
			if( submoduleManager.moduleSteps[practiceStepList[currentStepIndex].stepIndex].GetPivotPosition() != null ) {
				currentCameraStartPos = submoduleManager.moduleSteps[practiceStepList[currentStepIndex].stepIndex].GetCameraPosition().position;
				currentCameraPivot = submoduleManager.moduleSteps[practiceStepList[currentStepIndex].stepIndex].GetPivotPosition().position;

				StartCoroutine( LerpCameraLookAt() );
				StartCoroutine( SlerpToNewCamPos() );
			}
			// If we have a camera position for a transform then use the transform system
			else if ( submoduleManager.moduleSteps[practiceStepList[currentStepIndex].stepIndex].GetCameraPosition() != null ) {
				// TODO Copy new lerp coroutine from Acquire manager
				//yield return LerpToNewCamPos( submoduleManager.GetStepCameraTransform(acquireStepList[currentStepIndex].stepIndex ) );
			}
		}
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

	public void ResetInputsAndObjects() {
		for( int i = 0; i < UIManager.s_instance.listViewContentParent.childCount; i++ )
			ToggleListViewItemHighLight( i, false );
		UIManager.s_instance.UpdateDescriptionViewText( "" );
		UIManager.s_instance.ToggleSidePanel( false, false );

//		//TODO Remove this if 
//		if( submoduleManager != null )
//			submoduleManager.ResetScene();
	}
	
	private void InitializePracticeStepList() {
		practiceStepList = new List<StepsListEntry>();
		XmlDocument xmlFamiliarizeContent = new XmlDocument();
		xmlFamiliarizeContent.LoadXml( practiceContentXML.text );
		
		string path = "";
		switch( moduleType ) 
		{
		case PracticeModule.Choose:
			path = "chooseBalance";
			break;
		case PracticeModule.Calibrate:
			path = "calibrateBalance";
			break;
		case PracticeModule.Prepare:
			path = "prepareBalance";
			break;
		case PracticeModule.Use:
			path = "useBalance";
			break;
		case PracticeModule.FullCourse:
			path = "completeCourse";
			break;
		}
		
		XmlNode parentNode = xmlFamiliarizeContent.SelectSingleNode( "/content/"+ path  );
		
		//TODO Include content pulled from the <popup> node.
		int currentContext = 0;
		int currentIndex = 1;
		PopulateListFromNewParent( parentNode, ref currentContext, ref currentIndex );
		Debug.Log( "Created Practice Step List." );
	}

	/// <summary>
	/// Pouplates the list from new parent node.
	/// </summary>
	/// <param name="parentNode">Parent node.</param>
	/// <param name="newStepIndex">Step index stored in the Entry struct. This is used to find the corresponding scene in the PracticeModuleManager.</param>
	/// <param name="currentIndex">Number used solely to display the a number next to the name of the step in the List View.</param>
	private void PopulateListFromNewParent( XmlNode parentNode, ref int newStepIndex, ref int currentIndex ) {
		XmlNodeList stepList = parentNode.ChildNodes;
		foreach( XmlNode item in stepList ) {
			StepsListEntry newEntry = new StepsListEntry();

			string shownStepIndex = "";
			if( showListViewIndex )
				shownStepIndex = currentIndex.ToString() + ". ";

			switch( item.Name )
			{
			case "step":
				newEntry.isSectionParent = false;
				newEntry.stepIndex = newStepIndex;
				newEntry.uiText.listViewText = shownStepIndex + item.SelectSingleNode( "listText" ).InnerText;
				newEntry.uiText.descriptionViewText = item.SelectSingleNode( "descriptionText" ).InnerText;
				practiceStepList.Add( newEntry );
				currentIndex++;
				newStepIndex++;
				break;
			case "section":
				newEntry.isSectionParent = true;
				newEntry.stepIndex = -1;
				newEntry.uiText.listViewText = item.SelectSingleNode( "listText" ).InnerText;
				practiceStepList.Add( newEntry );
				break;
			case "moduleTitle":
				newEntry.isSectionParent = true;
				newEntry.stepIndex = -1;
				newEntry.uiText.listViewText = item.InnerText;
				practiceStepList.Add( newEntry );
				break;
			case "listText":
			case "popupWindow":
				break;
			default:
				Debug.LogError( "Unrecognized XmlNode value. Program doesn't support value of :" + item.Name );
				break;
			}
		}
	}

	private void InitializeAcquireListView() {
		int practiceListCount = practiceStepList.Count;

		// Setting the height of the list view to match the amount of buttons I will add to it.
		RectTransform listViewVerticalLayoutGroup = UIManager.s_instance.listViewContentParent;
		Vector2 newWidthHeight = listViewVerticalLayoutGroup.sizeDelta;
		//newWidthHeight.x = defaultListViewButton.GetComponent<RectTransform>().sizeDelta.x;
		newWidthHeight.y = UIManager.s_instance.defaultListViewButton.GetComponent<RectTransform>().sizeDelta.y * practiceListCount;
		listViewVerticalLayoutGroup.sizeDelta = newWidthHeight;

		// Creating new buttons out the dictionary and stuffing them in the vertical layout group
		for( int i = 0; i < practiceListCount; i++ ) {
			// Index 0 is special because it is the title for our list view
			if( i == 0 ) {
				ListViewButton newListViewSectionTitle = Instantiate( UIManager.s_instance.defaultListViewModuleTitle ).GetComponent<ListViewButton>();
				newListViewSectionTitle.listIndex = 0;
				newListViewSectionTitle.transform.SetParent(listViewVerticalLayoutGroup, false );
				newListViewSectionTitle.childText.text = practiceStepList[0].uiText.listViewText;
				continue;
			}

			StepsListEntry temp = practiceStepList[i];

			if( temp.isSectionParent ) {
				ListViewButton newListViewSectionTitle = Instantiate( UIManager.s_instance.defaultListViewSectionTitle ).GetComponent<ListViewButton>();
				newListViewSectionTitle.listIndex = i;
				newListViewSectionTitle.transform.SetParent(listViewVerticalLayoutGroup, false );
				newListViewSectionTitle.childText.text = /*contextIndentation + contextIndentation +*/ temp.uiText.listViewText;
			} else {
				ListViewButton newListViewButton = Instantiate( defaultListViewText ).GetComponent<ListViewButton>();
				newListViewButton.listIndex = i;
				newListViewButton.transform.SetParent(listViewVerticalLayoutGroup, false );
				newListViewButton.childText.text = temp.uiText.listViewText;
				newListViewButton.childText.color = new Color( 0f, 0f, 0f, 0f );
			}
		}
	}

	private void ToggleListViewItemHighLight( int index, bool toggleOn ) {
		// Section parents don't highlight. Only the items under them.
		if( practiceStepList[index].isSectionParent )
			return;

		Image listViewTextImage = UIManager.s_instance.listViewContentParent.GetChild(index).GetComponent<Image>();

		if( listViewTextImage == null ) {
			//TODO This error is called when the first item is clicked because it is trying to toggleOff a selected object that doesn't exist. We're going to set the first click and selection anyways so don't mind.
			Debug.LogError( "Couldn't find item with index: "+ index );
			return;
		}

		// Toggle highlight color if we are toggling on.
		listViewTextImage.color = ( toggleOn ) ? UIManager.s_instance.listViewButtonHighlightColor : UIManager.s_instance.listViewButtonNormalColor;

		if( toggleOn ) { 
			if( index > 3 ) {	
				UIManager.s_instance.UpdateListViewVerticalScrollbarValue( (index+1) / (float)UIManager.s_instance.listViewContentParent.childCount );
			} else {
				UIManager.s_instance.UpdateListViewVerticalScrollbarValue( 0f );
			}

			UIManager.s_instance.UpdateListViewHorizontalScrollbarValue( 0f );
		}
	}

	/// <summary>
	/// Called in the occurence that the "Next" UI button is clicked.
	/// </summary>
	public void ClickedNextButton() {
		GoToNextStep();
	}

	public void StartNewCameraSlerp( Transform newPivot, Transform newCamPos ) {
		currentCameraPivot = newPivot.position;
		currentCameraStartPos = newCamPos.position;

		StartCoroutine( LerpCameraLookAt() );
		StartCoroutine( SlerpToNewCamPos() );
	}
}