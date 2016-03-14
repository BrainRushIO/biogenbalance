using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LookAt : MonoBehaviour {

	public Transform cam;



	// Use this for initialization
	void Awake(){
		//transform.LookAt (cam);
	}

	void Start () {

		//transform.LookAt (cam);

	}
	
	// Update is called once per frame
	void Update () {
		//transform.RotateAround (target.position, Vector3.up, Input.GetAxis ("Mouse X") * dragSpeed);
		transform.LookAt(transform.position + cam.rotation * Vector3.forward, cam.rotation * Vector3.up);
	}
}
