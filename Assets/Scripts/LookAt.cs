using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LookAt : MonoBehaviour {

	/// <summary>
	/// The in-game/scene camera. Not the UI camera.
	/// </summary>
	public Transform cam;

	void Start() {
		if( cam == null && ApplicationManager.s_instance.currentApplicationMode == ApplicationManager.ApplicationMode.Familiarize ) {
			cam = FamiliarizeManager.s_instance.sceneCamera.transform;
		}
	}

	void Update () {
		//transform.RotateAround (target.position, Vector3.up, Input.GetAxis ("Mouse X") * dragSpeed);
		transform.LookAt(transform.position + cam.rotation * Vector3.forward, cam.rotation * Vector3.up);
	}
}
