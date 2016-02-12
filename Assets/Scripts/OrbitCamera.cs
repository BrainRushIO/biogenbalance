using UnityEngine;
using System.Collections;

public class OrbitCamera : MonoBehaviour {

	public GameObject pivot;
	public Transform target;
	public float dragSpeed = 0.5f;
	public float scrollSpeed= 0.5f;
	public float minDistance = 1;
	public float maxDistance = 10;

	// Use this for initialization
	void Start () {
		transform.LookAt (target);

	}
	
	// Update is called once per frame
	void Update () {
		

		if (Input.GetMouseButton (0)) {
			
			pivot.transform.RotateAround (target.position, pivot.transform.up, Input.GetAxis ("Mouse X") * dragSpeed);
			pivot.transform.RotateAround (target.position, pivot.transform.right, Input.GetAxis ("Mouse Y") * dragSpeed);
			transform.LookAt (target);
		}

		if (Input.GetAxis ("Mouse ScrollWheel") > 0f && transform.localPosition.z > minDistance) {
		
			transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - Input.GetAxis ("Mouse ScrollWheel") * scrollSpeed);
			transform.LookAt (target);
		}

		if (Input.GetAxis ("Mouse ScrollWheel") < 0f && transform.localPosition.z < maxDistance) {

			transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - Input.GetAxis ("Mouse ScrollWheel") * scrollSpeed);
			transform.LookAt (target);
		}
			
	}
}
