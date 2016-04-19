using UnityEngine;
using System.Collections;

public class FamiliarizeObject : MonoBehaviour {

	/// <summary>
	/// Key used when adding itself to the objects dictionary.
	/// </summary>
	public string dictionaryKey;
	public Transform cameraPivot, cameraStartPosition;
	public GameObject popupBubble;
	public UnityEngine.UI.Text bubbleText;

	void Start() {
		if( gameObject.tag != "Selectable" )
			Debug.LogWarning( gameObject.name +"'s tag is not set to \"Selectable\"");

		if( FamiliarizeManager.s_instance.familiarizeDictionary.ContainsKey( dictionaryKey ) ) {
			FamiliarizeDictionaryEntry appendedEntry =  FamiliarizeManager.s_instance.familiarizeDictionary[dictionaryKey];
			appendedEntry.obj = this;
			FamiliarizeManager.s_instance.familiarizeDictionary[dictionaryKey] = appendedEntry;

			bubbleText.text = FamiliarizeManager.s_instance.familiarizeDictionary[dictionaryKey].uiText.listViewText;
		} else {
			Debug.LogWarning( gameObject.name +"'s FamiliarizeObject couldn't find its key in the Dictionary. Key:" +dictionaryKey );
		}
	}

	public void Select() {
		if( popupBubble != null )
			popupBubble.SetActive( true );
	}

	public void Deselect() {
		if( popupBubble != null )
			popupBubble.SetActive( false );
		else
			Debug.Log( transform.name +"'s FamiliarizeObject  is missing a reference to a popup bubble." );
	}
}
