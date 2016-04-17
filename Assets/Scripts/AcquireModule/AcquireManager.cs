using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml;

public struct StepsListEntry {
	public bool isSectionParent;
	public int stepIndex;
	public ListViewDescriptionViewTextPair uiText;
}

public class AcquireManager : MonoBehaviour {
	public static AcquireManager s_instance;

	public enum AcquireModule { Choose, Prepare, Calibrate, Use }
	/// <summary>
	/// The type of the module. Set publicly in inspector.
	/// </summary>
	public AcquireModule moduleType = AcquireModule.Choose;

	public Camera sceneCamera;
	public bool isInIntro = true;
	public CanvasGroup introPopup, completionPopup;
	public List<StepsListEntry> acquireStepList;
	public TextAsset acquireContentXML;

	/// <summary>
	/// The submodule manager. Submodules control the logic for different module types: e.g. Choose, Prepare, Calibrate, Use
	/// </summary>
	private BaseAcquireSubmodule submoduleManager;
	/// <summary>
	/// The current step in the module. This differs from the stepIndex in StepsListEntry because it accounts for Section elements in the ListView. This is more like the sibling index for the ListViewContentParent's children.
	/// </summary>
	private int currentStepIndex = 0;
	private bool showListViewIndex = true;
	private bool isLerpingToNewPosition = false;
	[SerializeField]
	private bool hasFinishedModule = false;

	[Header("UI")]
	public RectTransform defaultListViewSectionTitle;

	void Awake() {
		if( s_instance == null ) {
			s_instance = this;
			InitializeAcquireStepList();
			InitializeAcquireListView();
		} else {
			Debug.LogWarning( "Destroying duplicate Acquire Manager." );
			DestroyImmediate( this.gameObject );
		}
	}

	void Start () {
		ApplicationManager.s_instance.ChangeMouseMode( 0 );
		UIManager.s_instance.ToggleToolsActive( false, false, false, false );
		UIManager.s_instance.ToggleSidePanel( true, false );
		UIManager.s_instance.nextButton.gameObject.SetActive( false );
		submoduleManager = BaseAcquireSubmodule.s_instance;

		currentStepIndex = -1;
	}

	private void UpdateNextButton() {
		// Sets the next button off only if we are on the last step.
//		if( currentStepIndex >= acquireStepList.Count-1 )
//			UIManager.s_instance.nextButton.gameObject.SetActive( false );
//		else 
			UIManager.s_instance.nextButton.gameObject.SetActive( true );
	}

	public void GoToNextStep() {
		if( hasFinishedModule )
			return;
		
		currentStepIndex++;
		while ( currentStepIndex < acquireStepList.Count && acquireStepList[currentStepIndex].isSectionParent )
			currentStepIndex++;

		if( currentStepIndex >= acquireStepList.Count ) {
			CompleteModule();
			return;
		}

		GoToStep(currentStepIndex);
	}

	public void GoToStep( int newStepIndex ) {
		if( hasFinishedModule )
			return;

		ResetSceneObjects();
		currentStepIndex = newStepIndex;
		
		// Update UI Text
		UIManager.s_instance.UpdateDescriptionViewText( acquireStepList[currentStepIndex].uiText.descriptionViewText );

		// Update scene
		StartCoroutine( InitializeNextStep() );

		// Get our new selected list view button, change its text white, set it interactable, and check its checkbox.
		ListViewButton newListViewButtonSelection = UIManager.s_instance.listViewContentParent.GetChild(currentStepIndex).GetComponent<ListViewButton>();
		newListViewButtonSelection.GetComponent<Button>().interactable = true;
		newListViewButtonSelection.childText.color = Color.white;
		newListViewButtonSelection.checkBox.isOn = true;

		// Highlight button and check if the "Next" button should appear or disappear.
		ToggleListViewButtonHighLight( currentStepIndex, true );
		UpdateNextButton();
	}

	private void ResetSceneObjects() {
		for( int i = 0; i < UIManager.s_instance.listViewContentParent.childCount; i++ )
			ToggleListViewButtonHighLight( i, false );
		UIManager.s_instance.UpdateDescriptionViewText( "" );

		//TODO Remove this if 
		if( submoduleManager != null )
			submoduleManager.ResetScene();
	}

	private void InitializeAcquireStepList() {
		acquireStepList = new List<StepsListEntry>();
		XmlDocument xmlFamiliarizeContent = new XmlDocument();
		xmlFamiliarizeContent.LoadXml( acquireContentXML.text );

		string path = "";
		switch( moduleType ) 
		{
		case AcquireModule.Choose:
			path = "chooseBalance";
			break;
		case AcquireModule.Calibrate:
			path = "calibrateBalance";
			break;
		case AcquireModule.Prepare:
			path = "prepareBalance";
			break;
		case AcquireModule.Use:
			path = "useBalance";
			break;
		}
			
		XmlNode parentNode = xmlFamiliarizeContent.SelectSingleNode( "/content/"+ path  );

		//TODO Include content pulled from the <popup> node.
		int currentContext = 0;
		int currentIndex = 1;
		PopulateListFromNewParent( parentNode, ref currentContext, ref currentIndex );
		Debug.Log( "Created Acquire Step List." );
	}

	/// <summary>
	/// Pouplates the list from new parent node.
	/// </summary>
	/// <param name="parentNode">Parent node.</param>
	/// <param name="newStepIndex">Step index stored in the Entry struct. This is used to find the corresponding scene in the AcquireModuleManager.</param>
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
				acquireStepList.Add( newEntry );
				currentIndex++;
				newStepIndex++;
				break;
			case "section":
				newEntry.isSectionParent = true;
				newEntry.stepIndex = -1;
				newEntry.uiText.listViewText = item.SelectSingleNode( "listText" ).InnerText;
				acquireStepList.Add( newEntry );

				PopulateListFromNewParent( item, ref newStepIndex, ref currentIndex );
				break;
			case "moduleTitle":
				newEntry.isSectionParent = true;
				newEntry.stepIndex = -1;
				newEntry.uiText.listViewText = item.InnerText;
				acquireStepList.Add( newEntry );
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
		int acquireListCount = acquireStepList.Count;

		// Setting the height of the list view to match the amount of buttons I will add to it.
		RectTransform listViewVerticalLayoutGroup = UIManager.s_instance.listViewContentParent;
		Vector2 newWidthHeight = listViewVerticalLayoutGroup.sizeDelta;
		//newWidthHeight.x = defaultListViewButton.GetComponent<RectTransform>().sizeDelta.x;
		newWidthHeight.y = UIManager.s_instance.defaultListViewButton.GetComponent<RectTransform>().sizeDelta.y * acquireListCount;
		listViewVerticalLayoutGroup.sizeDelta = newWidthHeight;

		// Creating new buttons out the dictionary and stuffing them in the vertical layout group
		for( int i = 0; i < acquireListCount; i++ ) {
			// Index 0 is special because it is the title for our list view
			if( i == 0 ) {
				ListViewButton newListViewSectionTitle = Instantiate( UIManager.s_instance.defaultListViewModuleTitle ).GetComponent<ListViewButton>();
				newListViewSectionTitle.listIndex = 0;
				newListViewSectionTitle.transform.SetParent(listViewVerticalLayoutGroup, false );
				newListViewSectionTitle.childText.text = acquireStepList[0].uiText.listViewText;
				continue;
			}

			StepsListEntry temp = acquireStepList[i];
			
			if( temp.isSectionParent ) {
				ListViewButton newListViewSectionTitle = Instantiate( defaultListViewSectionTitle ).GetComponent<ListViewButton>();
				newListViewSectionTitle.listIndex = i;
				newListViewSectionTitle.transform.SetParent(listViewVerticalLayoutGroup, false );
				newListViewSectionTitle.childText.text = /*contextIndentation + contextIndentation +*/ temp.uiText.listViewText;
			} else {
				ListViewButton newListViewButton = Instantiate( UIManager.s_instance.defaultListViewButton ).GetComponent<ListViewButton>();
				newListViewButton.listIndex = i;
				newListViewButton.transform.SetParent(listViewVerticalLayoutGroup, false );
				newListViewButton.childText.text = temp.uiText.listViewText;
				newListViewButton.childText.color = Color.grey;
				newListViewButton.GetComponent<Button>().interactable = false;
			}
		}
	}

	private void ToggleListViewButtonHighLight( int index, bool toggleOn ) {
		if( acquireStepList[index].isSectionParent )
			return;

		Button listViewButton =	UIManager.s_instance.listViewContentParent.GetChild(index).GetComponent<Button>();

		if( listViewButton == null ) {
			//TODO This error is called when the first item is clicked because it is trying to toggleOff a selected object that doesn't exist. We're going to set the first click and selection anyways so don't mind.
			Debug.LogError( "Couldn't find button with key: "+ index );
			return;
		}

		ColorBlock tempBlock = listViewButton.colors;
		if( toggleOn ) {
			tempBlock.normalColor = UIManager.s_instance.listViewButtonHighlightColor;
		} else {
			tempBlock.normalColor = UIManager.s_instance.listViewButtonNormalColor;
		}
		listViewButton.colors = tempBlock;

		if( toggleOn ) { 
			if( index > 3 ) {	
				UIManager.s_instance.UpdateListViewVerticalScrollbarValue( (index+1) / (float)UIManager.s_instance.listViewContentParent.childCount );
			} else {
				UIManager.s_instance.UpdateListViewVerticalScrollbarValue( 0f );
			}

			UIManager.s_instance.UpdateListViewHorizontalScrollbarValue( 0f );
		}
	}

	public void ClickedNextButton() {
		if( isLerpingToNewPosition )
			return;
		
		GoToNextStep();
	}

	private IEnumerator InitializeNextStep() {
		if( submoduleManager == null ) {
			Debug.LogError( "Submodule manager is null" );
			yield break;
		}

		yield return LerpToNewCamPos( submoduleManager.GetStepCameraTransform(acquireStepList[currentStepIndex].stepIndex ) );
		submoduleManager.UpdateSceneContents( acquireStepList[currentStepIndex].stepIndex );
	}

	private IEnumerator LerpToNewCamPos( Transform targetTransform ) {
		isLerpingToNewPosition = true;
		float elapsedTime = 0f;
		float lerpTime = 0.8f;
		float startTime = Time.time;
		Vector3 startPos = sceneCamera.transform.position;
		Quaternion startRot = sceneCamera.transform.rotation;

		while( elapsedTime < lerpTime ) {
			sceneCamera.transform.position = Vector3.Lerp( startPos, targetTransform.position, elapsedTime/lerpTime );
			sceneCamera.transform.rotation = Quaternion.Lerp( startRot, targetTransform.rotation, elapsedTime/lerpTime );
			yield return null;
			elapsedTime = Time.time-startTime;
		}
		sceneCamera.transform.position = targetTransform.position;
		sceneCamera.transform.rotation = targetTransform.rotation;
		isLerpingToNewPosition = false;
	}

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
		UIManager.s_instance.nextButton.gameObject.SetActive( true );
		isInIntro = false;
		GoToNextStep();
	}

	private void CompleteModule() {
		hasFinishedModule = true;
		switch( moduleType )
		{
		case AcquireModule.Choose:
			ApplicationManager.s_instance.playerData.a_choose = true;
			break;
		case AcquireModule.Prepare:
			ApplicationManager.s_instance.playerData.a_prepare = true;
			break;
		case AcquireModule.Calibrate:
			ApplicationManager.s_instance.playerData.a_calibrate = true;
			break;
		case AcquireModule.Use:
			ApplicationManager.s_instance.playerData.a_use = true;
			break;
		}
		ApplicationManager.s_instance.Save();
		UIManager.s_instance.nextButton.gameObject.SetActive( false );

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

	public void ClickedGoToPracticeModule() {
		switch( moduleType )
		{
		case AcquireModule.Choose:
			ApplicationManager.s_instance.LoadScene( "P1" );
			break;
		case AcquireModule.Prepare:
			ApplicationManager.s_instance.LoadScene( "P2" );
			break;
		case AcquireModule.Calibrate:
			ApplicationManager.s_instance.LoadScene( "P3" );
			break;
		case AcquireModule.Use:
			ApplicationManager.s_instance.LoadScene( "P4" );
			break;
		}
	}

	public void ClickedGoToNextAcquireModule() {
		switch( moduleType )
		{
		case AcquireModule.Choose:
			ApplicationManager.s_instance.LoadScene( "A2" );
			break;
		case AcquireModule.Prepare:
			ApplicationManager.s_instance.LoadScene( "A3" );
			break;
		case AcquireModule.Calibrate:
			ApplicationManager.s_instance.LoadScene( "A4" );
			break;
		}
	}
}
