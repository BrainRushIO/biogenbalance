using UnityEngine;
using System.Collections;

public class FamiliarizeObject : MonoBehaviour {

	/// <summary>
	/// Key used when adding itself to the objects dictionary.
	/// </summary>
	public string dictionaryKey;
	public Transform cameraPivot, cameraStartPosition;
	public GameObject popupBubble;

	private Vector3 defautPivotPos, defaultStartPos;

	void Start() {
		defautPivotPos = cameraPivot.position;
		defaultStartPos = cameraStartPosition.position;
	}

	public void Select() {
		//popupBubble.SetActive( true );
	}

	public void Deselect() {
		//popupBubble.SetActive( false );
		cameraPivot.position = defautPivotPos;
		cameraStartPosition.position = defaultStartPos;
	}
}
