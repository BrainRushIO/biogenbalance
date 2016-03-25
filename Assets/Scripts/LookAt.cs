using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LookAt : MonoBehaviour {

	/// <summary>
	/// The in-game/scene camera. Not the UI camera.
	/// </summary>
	public Transform cam;

	void Update () {
		//transform.RotateAround (target.position, Vector3.up, Input.GetAxis ("Mouse X") * dragSpeed);
		transform.LookAt(transform.position + cam.rotation * Vector3.forward, cam.rotation * Vector3.up);
	}
}
