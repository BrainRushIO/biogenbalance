using UnityEngine;
using System.Collections;

/// <summary>
/// User interface bounds trigger. Used in conjunction with OnPointerEnter and OnPointerExit. Tells the manager that the user is interacting with the UI so in-game input doesn't get computed.
/// </summary>
public class UIBoundsTrigger : MonoBehaviour {

	public bool active = true;

	/// <summary>
	/// Sets the user interface trigger in Application manager to the set value.
	/// </summary>
	/// <param name="value">If set to <c>true</c> value.</param>
	public void SetUITrigger( bool value ) {
		if( !active )
			return;

		ApplicationManager.s_instance.userIsInteractingWithUI = value;
	}
}
