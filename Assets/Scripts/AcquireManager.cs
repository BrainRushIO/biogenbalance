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
	private bool DebugShowListViewIndex = false;

	/// <summary>
	/// The current step in the module.
	/// </summary>
	private int currentStepIndex = 0;

	[Header("UI")]
	public Button defaultListViewButton;
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
	}

	void Update () {
		switch( currentStepIndex )
		{
		default:
			break;
		}
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

		UIManager.s_instance.UpdateDescriptionViewText( acquireStepList[stepIndex].uiText.descriptionViewText);
	}

	private void ResetInputsAndObjects() {
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
		PouplateListFromNewParent( parentNode, ref currentContext );
		Debug.Log( "Created Acquire Step List." );
	}

	private void PouplateListFromNewParent( XmlNode parentNode, ref int context ) {
		XmlNodeList stepList = parentNode.ChildNodes;
		foreach( XmlNode item in stepList ) {
			StepsListEntry newEntry = new StepsListEntry();

			string debugIndex = "";
			if( DebugShowListViewIndex )
				debugIndex = acquireStepList.Count.ToString();

			switch( item.Name )
			{
			case "step":
				newEntry.isSectionParent = false;
				newEntry.context = context;
				newEntry.uiText.listViewText = debugIndex + item.SelectSingleNode( "listText" ).InnerText;
				newEntry.uiText.descriptionViewText = item.SelectSingleNode( "descriptionText" ).InnerText;
				acquireStepList.Add( newEntry );
				break;
			case "section":
				newEntry.isSectionParent = true;
				newEntry.context = context;
				newEntry.uiText.listViewText = debugIndex + item.SelectSingleNode( "listText" ).InnerText;
				acquireStepList.Add( newEntry );

				context++;
				PouplateListFromNewParent( item, ref context );
				context--;
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
		newWidthHeight.y = defaultListViewButton.GetComponent<RectTransform>().sizeDelta.y * acquireListCount;
		listViewVerticalLayoutGroup.sizeDelta = newWidthHeight;

		// Creating new buttons out the dictionary and stuffing them in the vertical layout group
		for( int i = 0; i < acquireListCount; i++ ) {
			StepsListEntry temp = acquireStepList[i];

			string contextIndentation = "";
			for( int j = 0; j < temp.context; j++ )
				contextIndentation += "\t";
			
			if( temp.isSectionParent ) {
				ListViewButton newListViewSectionTitle = Instantiate( defaultListViewSectionTitle ).GetComponent<ListViewButton>();
				newListViewSectionTitle.listIndex = i;
				newListViewSectionTitle.transform.SetParent(listViewVerticalLayoutGroup, false );
				newListViewSectionTitle.childText.text = contextIndentation + temp.uiText.listViewText;
			} else {
				ListViewButton newListViewButton = Instantiate( defaultListViewButton ).GetComponent<ListViewButton>();
				newListViewButton.listIndex = i;
				newListViewButton.transform.SetParent(listViewVerticalLayoutGroup, false );
				newListViewButton.childText.text = contextIndentation + temp.uiText.listViewText;
			}
		}
	}
}
