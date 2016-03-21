using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuBarButton : MonoBehaviour {
	public GameObject dropDownMenuObject;
	[System.NonSerialized]
	public bool isSelected;

	public void WasClicked() {
		if (isSelected) {
			if (dropDownMenuObject != null)
				dropDownMenuObject.SetActive (false);
		} else {
			UIManager.s_instance.DeselectTopBarButtons ();
			UIManager.s_instance.CloseDropDowns ();

			if (dropDownMenuObject != null)
				dropDownMenuObject.SetActive (true);
		}
		isSelected = !isSelected;
	}

	public void Deselect() {
		isSelected = false;
		if( dropDownMenuObject != null )
			dropDownMenuObject.SetActive (false);
		//TODO button color stuff
	}
}
