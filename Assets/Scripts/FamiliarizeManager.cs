using UnityEngine;
using System.Collections;

public class FamiliarizeManager : MonoBehaviour {

	public Camera sceneCamera;

	/// <summary>
	/// Checks if user has clicked down on mouse item. Used to differentiate between a click and drag
	/// </summary>
	private bool hasClickedDownOnItem = false;
	private bool isDragging = false;

	void Update () {
		#region Input
		if ( Input.GetMouseButtonUp(0) ) {
			if( hasClickedDownOnItem && !isDragging ) {
				Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if ( Physics.Raycast(ray, out hit) ){
					if ( hit.transform.gameObject.tag == "Animatable" ) {
						hit.transform.gameObject.GetComponent<Animator> ().SetTrigger ("Clicked");
					}
					//TODO Check if they have a clickable object on them and do something about it
				}
			}
		}
		if ( Input.GetMouseButtonDown(0) ){
			Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if ( Physics.Raycast(ray, out hit) ){
				if ( hit.transform.gameObject.tag == "Animatable" || hit.transform.gameObject.tag == "Clickable" ) {
					hasClickedDownOnItem = true;
				}
			}
		}
		#endregion
	}
}
