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


	void Awake() {
		if( s_instance == null ) {
			s_instance = this;
			InitializeAcquireStepList();
		} else {
			Debug.LogWarning( "Destroying duplicate Acquire Manager." );
			DestroyImmediate( this.gameObject );
		}
	}

	void Start () {
	
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
			path = "choose";
			break;
		case AcquireModule.Calibrate:
			path = "calibrate";
			break;
		case AcquireModule.Prepare:
			path = "prepare";
			break;
		case AcquireModule.Use:
			path = "use";
			break;
		}

		XmlNodeList stepList = xmlFamiliarizeContent.SelectSingleNode( "/content/"+ path  ).ChildNodes;

		int currentContext = 0;
		foreach( XmlNode item in stepList ) {
			AcquireStepListEntry newEntry = new AcquireStepListEntry();

			switch( item.Value )
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
			default:
				Debug.LogError( "Unrecognized XmlNode value. Program doesn't support value of :" + item.Value );
				break;
			}
		}
		Debug.Log( "Created Acquire Step List." );
	}

	private void PouplateListFromNewParent( XmlNode parentNode, ref int context ) {
		context++;
		XmlNodeList stepList = parentNode.ChildNodes;
		foreach( XmlNode item in stepList ) {
			
		}
		context--;
	}
}
