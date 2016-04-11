using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml;

public struct StepsListEntry {
	public bool isSectionParent;
	public int context;
	public ListViewDescriptionViewTextPair uiText;
}

public class AcquireManager : MonoBehaviour {
	public static AcquireManager s_instance;

	public enum AcquireModule { Choose, Prepare, Calibrate, Use }
	/// <summary>
	/// The type of the module. Set publicly in inspector.
	/// </summary>
	public AcquireModule moduleType = AcquireModule.Choose;

	public List<StepsListEntry> acquireStepList;
	public TextAsset acquireContentXML;

	/// <summary>
	/// The current step in the module.
	/// </summary>
	private int currentStepIndex = 0;
	private bool showListViewIndex = true;

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
		ApplicationManager.s_instance.currentMouseMode = ApplicationManager.MouseMode.Pointer;
		UIManager.s_instance.ToggleToolsActive( false, false, false, false );
		//GoToStep( 1 );
		currentStepIndex = -1;
		GoToNextStep();
		UIManager.s_instance.nextButton.gameObject.SetActive( true );
	}

	void Update () {
		switch( currentStepIndex )
		{
		default:
			break;
		}

	}

	private void UpdateNextButton() {
		if( currentStepIndex >= acquireStepList.Count-1 )
			UIManager.s_instance.nextButton.gameObject.SetActive( false );
		else 
			UIManager.s_instance.nextButton.gameObject.SetActive( true );
	}

	public void GoToNextStep() {
		currentStepIndex++;
		while ( currentStepIndex < acquireStepList.Count && acquireStepList[currentStepIndex].isSectionParent )
			currentStepIndex++;

		if( currentStepIndex >= acquireStepList.Count ) {
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

		// Update UI Text
		UIManager.s_instance.UpdateDescriptionViewText( acquireStepList[stepIndex].uiText.descriptionViewText );
		// Get our new selected list view button, change its text white, set it interactable, and check its checkbox.
		ListViewButton newListViewButtonSelection = UIManager.s_instance.listViewContentParent.GetChild(currentStepIndex).GetComponent<ListViewButton>();
		newListViewButtonSelection.GetComponent<Button>().interactable = true;
		newListViewButtonSelection.childText.color = Color.white;
		newListViewButtonSelection.checkBox.isOn = true;

		// Highlight button and check if the next button should appear or disappear.
		ToggleListViewButtonHighLight( currentStepIndex, true );
		UpdateNextButton();
	}

	private void ResetInputsAndObjects() {
		for( int i = 0; i < UIManager.s_instance.listViewContentParent.childCount; i++ )
			ToggleListViewButtonHighLight( i, false );
		UIManager.s_instance.UpdateDescriptionViewText( "" );
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
		//TODO Make this algorithm call the PopulateListFromNewParent method
		int currentContext = 0;
		int currentIndex = 1;
		PouplateListFromNewParent( parentNode, ref currentContext, ref currentIndex );
		Debug.Log( "Created Acquire Step List." );
	}

	private void PouplateListFromNewParent( XmlNode parentNode, ref int context, ref int currentIndex ) {
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
				newEntry.context = context;
				newEntry.uiText.listViewText = shownStepIndex + item.SelectSingleNode( "listText" ).InnerText;
				newEntry.uiText.descriptionViewText = item.SelectSingleNode( "descriptionText" ).InnerText;
				acquireStepList.Add( newEntry );
				currentIndex++;
				break;
			case "section":
				newEntry.isSectionParent = true;
				newEntry.context = context;
				newEntry.uiText.listViewText = item.SelectSingleNode( "listText" ).InnerText;
				acquireStepList.Add( newEntry );

				context++;
				PouplateListFromNewParent( item, ref context, ref currentIndex );
				context--;
				break;
			case "moduleTitle":
				newEntry.isSectionParent = true;
				newEntry.context = 0;
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

			string contextIndentation = "";
			for( int j = 1; j < temp.context; j++ )
				contextIndentation += ( temp.isSectionParent ) ? "\t   " : "\t";
			
			if( temp.isSectionParent ) {
				ListViewButton newListViewSectionTitle = Instantiate( defaultListViewSectionTitle ).GetComponent<ListViewButton>();
				newListViewSectionTitle.listIndex = i;
				newListViewSectionTitle.transform.SetParent(listViewVerticalLayoutGroup, false );
				newListViewSectionTitle.childText.text = /*contextIndentation + contextIndentation +*/ temp.uiText.listViewText;
			} else {
				ListViewButton newListViewButton = Instantiate( UIManager.s_instance.defaultListViewButton ).GetComponent<ListViewButton>();
				newListViewButton.listIndex = i;
				newListViewButton.transform.SetParent(listViewVerticalLayoutGroup, false );
				newListViewButton.childText.text = contextIndentation + temp.uiText.listViewText;
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
}
