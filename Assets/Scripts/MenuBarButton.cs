using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuBarButton : MonoBehaviour {
	public GameObject dropDownMenuObject;
	[System.NonSerialized]
	public bool isSelected;

	private bool isHoveringOverDropdown = false;

	public void WasClicked() {
		if (!isSelected) {
			UIManager.s_instance.DeselectTopBarButtons ();
			UIManager.s_instance.CloseDropDowns ();

			if (dropDownMenuObject != null)
				dropDownMenuObject.SetActive (true);
		
		} else  {
			if (dropDownMenuObject != null)
				dropDownMenuObject.SetActive (false);
		}
		isSelected = !isSelected;
	}

	public void Deselect() {
		isSelected = false;
		if( dropDownMenuObject != null && !isHoveringOverDropdown )
			dropDownMenuObject.SetActive (false);
		//TODO button color stuff
	}

	/// <summary>
	/// Sets the is hovering over dropdown bool. This is called from the Event Trigger on the Dropdown objects in the scene.
	/// </summary>
	/// <param name="val">If set to <c>true</c> value.</param>
	public void SetIsHoveringOverDropdown( bool val ) {
		isHoveringOverDropdown = val;
	}
}
