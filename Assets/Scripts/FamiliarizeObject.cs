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
