using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Xml;

public struct AcquireStepText {
	public string listViewText, descriptionViewText;
}

public class AcquireManager : MonoBehaviour {
	public static AcquireManager s_instance;

	public enum AcquireModule { Choose, Calibrate, Prepare, Use }
	/// <summary>
	/// The type of the module. Set publicly in inspector.
	/// </summary>
	public AcquireModule moduleType = AcquireModule.Choose;

	public TextAsset acquireContentXML;

	/// <summary>
	/// The current step in the module.
	/// </summary>
	private int currentStepIndex = 0;


	void Awake() {
		if( s_instance == null ) {
			s_instance = this;
		} else {
			Debug.LogWarning( "Destroying duplicate Acquire Manager." );
			DestroyImmediate( this.gameObject );
		}
	}

	void Start () {
	
	}

	void Update () {
	
	}

	void InitializeAcquireDictionary() {
//		familiarizeDictionary = new Dictionary<string, FamiliarizeDictionaryEntry>();
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

		XmlNodeList itemList = xmlFamiliarizeContent.SelectNodes( "/content/"+ path +"/step" );

		foreach( XmlNode item in itemList ) {
			AcquireStepText newEntry = new AcquireStepText();
			newEntry.listViewText = item.SelectSingleNode( "listText" ).InnerText;
			newEntry.descriptionViewText = item.SelectSingleNode( "descriptionText" ).InnerText;

//			familiarizeDictionary.Add( item.SelectSingleNode("key").InnerText, newEntry );
		}
		Debug.Log( "Created dictionary." );
	}
}
