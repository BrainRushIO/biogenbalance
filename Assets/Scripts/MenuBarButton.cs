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
	}

	/// <summary>
	/// Sets the is hovering over dropdown bool. This is called from the Event Trigger on the Dropdown objects in the scene.
	/// </summary>
	/// <param name="val">If set to <c>true</c> value.</param>
	public void SetIsHoveringOverDropdown( bool val ) {
		isHoveringOverDropdown = val;
	}

	/// <summary>
	/// Changes the normal color of the button to highlighted and back.
	/// </summary>
	/// <param name="toggleHighlightOn">If set to <c>true</c> toggle highlight on.</param>
	public void ToggleHighlight( bool toggleHighlightOn ) {
		Button thisButton = GetComponent<Button> ();
		ColorBlock newBlock = thisButton.colors;
		newBlock.normalColor = (toggleHighlightOn) ? UIManager.s_instance.menuButtonsHighlightColor : UIManager.s_instance.menuButtonsBaseColor;
		thisButton.colors = newBlock;
	}
}
