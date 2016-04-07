using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml;

public struct AcquireStepListEntry {
	public bool isSectionParent;
	public int context;
	public ListViewDescriptionViewTextPair uiText;
}

public class AcquireManager : MonoBehaviour {
	public static AcquireManager s_instance;

	public enum AcquireModule { Choose, Calibrate, Prepare, Use }
	/// <summary>
	/// The type of the module. Set publicly in inspector.
	/// </summary>
	public AcquireModule moduleType = AcquireModule.Choose;

	public List<AcquireStepListEntry> acquireStepList;
	public TextAsset acquireContentXML;

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
	
	}

	private void InitializeAcquireStepList() {
		acquireStepList = new List<AcquireStepListEntry>();
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

		XmlNodeList stepList = xmlFamiliarizeContent.SelectSingleNode( "/content/"+ path  ).ChildNodes;

		//TODO Include content pulled from the <popup> node.
		//TODO Make this algorithm call the PopulateListFromNewParent method
		int currentContext = 0;
		foreach( XmlNode item in stepList ) {
			AcquireStepListEntry newEntry = new AcquireStepListEntry();

			switch( item.Name )
			{
			case "step":
				newEntry.isSectionParent = false;
				newEntry.context = currentContext;
				newEntry.uiText.listViewText = item.SelectSingleNode( "listText" ).InnerText;
				newEntry.uiText.descriptionViewText = item.SelectSingleNode( "descriptionText" ).InnerText;
				acquireStepList.Add( newEntry );
				break;
			case "section":
				newEntry.isSectionParent = true;
				newEntry.uiText.listViewText = item.SelectSingleNode( "listText" ).InnerText;
				acquireStepList.Add( newEntry );
				PouplateListFromNewParent( item, ref currentContext );
				break;
			case "popupWindow":
				break;
			default:
				if( item.Name != "listText" )
					Debug.LogError( "Unrecognized XmlNode value. Program doesn't support value of :" + item.Name );
				break;
			}
		}
		Debug.Log( "Created Acquire Step List." );
	}

	private void PouplateListFromNewParent( XmlNode parentNode, ref int context ) {
		context++;
		XmlNodeList stepList = parentNode.ChildNodes;
		foreach( XmlNode item in stepList ) {
			AcquireStepListEntry newEntry = new AcquireStepListEntry();

			switch( item.Name )
			{
			case "step":
				newEntry.isSectionParent = false;
				newEntry.context = context;
				newEntry.uiText.listViewText = item.SelectSingleNode( "listText" ).InnerText;
				newEntry.uiText.descriptionViewText = item.SelectSingleNode( "descriptionText" ).InnerText;
				acquireStepList.Add( newEntry );
				break;
			case "section":
				newEntry.isSectionParent = true;
				newEntry.context = context;
				newEntry.uiText.listViewText = item.SelectSingleNode( "listText" ).InnerText;
				acquireStepList.Add( newEntry );
				PouplateListFromNewParent( item, ref context );
				break;
			default:
				if( item.Name != "listText" )
					Debug.LogError( "Unrecognized XmlNode value. Program doesn't support value of :" + item.Name );
				break;
			}
		}
		context--;
	}

	private void InitializeAcquireListView() {
		int acquireListCount = acquireStepList.Count;

		// Setting the height of the list view to match the amount of buttons I will add to it.
		RectTransform listViewVerticalLayoutGroup = UIManager.s_instance.listViewContentParent;
		Vector2 newWidthHeight = listViewVerticalLayoutGroup.sizeDelta;
		newWidthHeight.x = defaultListViewButton.GetComponent<RectTransform>().sizeDelta.x;
		newWidthHeight.y = defaultListViewButton.GetComponent<RectTransform>().sizeDelta.y * acquireListCount;
		listViewVerticalLayoutGroup.sizeDelta = newWidthHeight;

		// Creating new buttons out the dictionary and stuffing them in the vertical layout group
		for( int i = 0; i < acquireListCount; i++ ) {
			AcquireStepListEntry temp = acquireStepList[i];

			string contextIndentation = "";
			for( int j = 0; j < temp.context; j++ )
				contextIndentation += "\t\t";

			Debug.Log( temp.context );
			
			if( temp.isSectionParent ) {
				ListViewButton newListViewSectionTitle = Instantiate( defaultListViewSectionTitle ).GetComponent<ListViewButton>();
				newListViewSectionTitle.transform.SetParent(listViewVerticalLayoutGroup, false );
				newListViewSectionTitle.childText.text = contextIndentation + temp.uiText.listViewText;
			} else {
				ListViewButton newListViewButton = Instantiate( defaultListViewButton ).GetComponent<ListViewButton>();
				newListViewButton.transform.SetParent(listViewVerticalLayoutGroup, false );
				newListViewButton.childText.text = contextIndentation + temp.uiText.listViewText;
			}
		}
	}
}
