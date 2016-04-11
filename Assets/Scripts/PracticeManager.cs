using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml;

public class PracticeManager : MonoBehaviour {
	public static PracticeManager s_instance;

	public ListViewButton defaultListViewText;

	private List<StepsListEntry> practiceStepList;
	private int currentStepIndex = 0;
	private bool showListViewIndex = true;

	void Awake() {
		if( s_instance == null ) {
			s_instance = this;
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

		switch( stepIndex )
		{
		default:
			break;
		}

		UIManager.s_instance.UpdateDescriptionViewText( practiceStepList[stepIndex].uiText.descriptionViewText);
	}

	public void ResetInputsAndObjects() {
		
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
				practiceStepList.Add( newEntry );
				currentIndex++;
				break;
			case "section":
				newEntry.isSectionParent = true;
				newEntry.context = context;
				newEntry.uiText.listViewText = item.SelectSingleNode( "listText" ).InnerText;
				practiceStepList.Add( newEntry );

				context++;
				PouplateListFromNewParent( item, ref context, ref currentIndex );
				context--;
				break;
			case "moduleTitle":
				newEntry.isSectionParent = true;
				newEntry.context = 0;
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

	private void InitializePracticeListView() {
		int acquireListCount = practiceStepList.Count;

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
				newListViewSectionTitle.childText.text = practiceStepList[0].uiText.listViewText;
				continue;
			}

			StepsListEntry temp = practiceStepList[i];

			string contextIndentation = "";
			for( int j = 1; j < temp.context; j++ )
				contextIndentation += ( temp.isSectionParent ) ? "\t   " : "\t";

			if( temp.isSectionParent ) {
				ListViewButton newListViewSectionTitle = Instantiate( UIManager.s_instance.defaultListViewSectionTitle ).GetComponent<ListViewButton>();
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
		if( practiceStepList[index].isSectionParent )
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
