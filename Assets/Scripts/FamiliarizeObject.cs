using UnityEngine;
using System.Collections;

public class FamiliarizeObject : MonoBehaviour {

	/// <summary>
	/// Key used when adding itself to the objects dictionary.
	/// </summary>
	public string dictionaryKey;
	public Transform cameraPivot, cameraStartPosition;
	public GameObject popupBubble;

	public void Select() {
		popupBubble.SetActive( true );
	}

	public void Deselect() {
		popupBubble.SetActive( false );
	}
}
