using UnityEngine;
using System.Collections;

public class ClickedOn : MonoBehaviour {


	void Update(){
		if (Input.GetMouseButtonDown(0)){ // if left button pressed...
			Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
				if (Physics.Raycast(ray, out hit)){
				if (hit.transform.gameObject.tag == "Animatable") {
					hit.transform.gameObject.GetComponent<Animator> ().SetTrigger ("Clicked");
					}
			// the object identified by hit.transform was clicked
			// do whatever you want
				}
	}

}
}
