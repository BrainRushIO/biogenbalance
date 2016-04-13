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

	public ListViewButton defaultListViewText;
	public TextAsset practiceContentXML;

	private List<StepsListEntry> practiceStepList;
	private int currentStepIndex = 0;
	private bool showListViewIndex = true;

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

		// Get our new selected list view button, change its text white, set it interactable, and check its checkbox.
		ListViewButton newListViewButtonSelection = UIManager.s_instance.listViewContentParent.GetChild(currentStepIndex).GetComponent<ListViewButton>();
		newListViewButtonSelection.childText.color = Color.white;
		newListViewButtonSelection.checkBox.isOn = true;

		UIManager.s_instance.UpdateDescriptionViewText( "Hint: " + practiceStepList[stepIndex].uiText.descriptionViewText );
		ToggleListViewItemHighLight( stepIndex, true );
	}

	public void ResetInputsAndObjects() {
		for( int i = 0; i < UIManager.s_instance.listViewContentParent.childCount; i++ )
			ToggleListViewItemHighLight( i, false );
		UIManager.s_instance.UpdateDescriptionViewText( "" );

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
}
