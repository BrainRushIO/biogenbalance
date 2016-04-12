using UnityEngine;
using System.Collections;

public class OrbitCamera : MonoBehaviour {

	public Transform pivotParent;
	public float dragSpeed = 1;
	public float scrollSpeed= 1;
	public float minDistance = 1;
	public float maxDistance = 8;
	public bool canRotate = true, canZoom = true, canPan = true;

	private Vector3 lastMousePos;

	void Start () {
		transform.LookAt (pivotParent);
	}

	void Update () {
		if( ApplicationManager.s_instance.userIsInteractingWithUI )
			return;

		// Handle draggin camera
		if( canRotate ) {
			if( ApplicationManager.s_instance.currentApplicationMode == ApplicationManager.ApplicationMode.Familiarize ) {
				if ( FamiliarizeManager.s_instance.isDragging == false && Input.GetMouseButton (0) && Mathf.Abs((Input.mousePosition-lastMousePos).magnitude) > 2f ) {
					FamiliarizeManager.s_instance.isDragging = true;
				}
				if( FamiliarizeManager.s_instance.isDragging ) {
					pivotParent.transform.RotateAround (pivotParent.position, pivotParent.up, Input.GetAxis ("Mouse X") * dragSpeed);
					pivotParent.transform.RotateAround (pivotParent.position, pivotParent.right, Input.GetAxis ("Mouse Y") * dragSpeed);
					transform.LookAt (pivotParent);
				}
			}
		}

		// Handle zooming in and out
		if( canZoom ) {
			float mouseWheelValue = Input.GetAxis ("Mouse ScrollWheel");
			if ( mouseWheelValue > 0f && transform.localPosition.z > minDistance ) {
				transform.localPosition += -(transform.localPosition).normalized * Input.GetAxis ("Mouse ScrollWheel") * scrollSpeed;
				transform.LookAt (pivotParent);
			} else if ( mouseWheelValue < 0f && transform.localPosition.z < maxDistance ) {
				transform.localPosition += -(transform.localPosition).normalized * Input.GetAxis ("Mouse ScrollWheel") * scrollSpeed;
				transform.LookAt (pivotParent);
			}
		}

		if( canPan ) {
			
		}

		lastMousePos = Input.mousePosition;
	}
}
