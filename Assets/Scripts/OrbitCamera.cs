using UnityEngine;
using System.Collections;

public class OrbitCamera : MonoBehaviour {

	public Transform pivot;
	public Transform target;
	public float dragSpeed = 1;
	public float scrollSpeed= 1;
	public float minDistance = 3;
	public float maxDistance = 8;

	private Vector3 lastMousePos;

	void Start () {
		transform.LookAt (pivot);
	}

	void Update () {
		// Handle draggin camera
		if ( FamiliarizeManager.s_instance.isDragging == false && Input.GetMouseButton (0) && Mathf.Abs((Input.mousePosition-lastMousePos).magnitude) > 2f ) {
			FamiliarizeManager.s_instance.isDragging = true;
		}
		if( FamiliarizeManager.s_instance.isDragging ) {
			pivot.transform.RotateAround (pivot.position, pivot.up, Input.GetAxis ("Mouse X") * dragSpeed);
			pivot.transform.RotateAround (pivot.position, pivot.right, Input.GetAxis ("Mouse Y") * dragSpeed);
			transform.LookAt (pivot);
		}

		// Handle zooming in and out
		float mouseWheelValue = Input.GetAxis ("Mouse ScrollWheel");
		if ( mouseWheelValue > 0f && transform.localPosition.z > minDistance) {
			transform.localPosition += -(transform.localPosition).normalized * Input.GetAxis ("Mouse ScrollWheel") * scrollSpeed;
			transform.LookAt (pivot);
		} else if ( mouseWheelValue < 0f && transform.localPosition.z < maxDistance) {
			transform.localPosition += -(transform.localPosition).normalized * Input.GetAxis ("Mouse ScrollWheel") * scrollSpeed;
			transform.LookAt (pivot);
		}

		lastMousePos = Input.mousePosition;
	}
}
