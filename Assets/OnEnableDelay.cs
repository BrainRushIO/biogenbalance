using UnityEngine;
using System.Collections;

public class OnEnableDelay : MonoBehaviour {


	float timer = 0;
	float showDuration = 2f;

	void OnEnable () {
		timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer > showDuration) {
			gameObject.SetActive (false);
		}
	}
}
