using UnityEngine;
using System.Collections;

public class OrbitCamera : MonoBehaviour {

	public Transform pivot;
	public Transform target;
	public float dragSpeed = 0.5f;
	public float scrollSpeed= 0.5f;
	public float minDistance = 1;
	public float maxDistance = 10;

	private Vector3 lastMousePos;

	void Start () {
		transform.LookAt (pivot);
	}

	void Update () {		
		if ( FamiliarizeManager.s_instance.isDragging == false && Input.GetMouseButton (0) && Mathf.Abs((Input.mousePosition-lastMousePos).magnitude) > 2f ) {
			FamiliarizeManager.s_instance.isDragging = true;
		}
		if( FamiliarizeManager.s_instance.isDragging ) {
			pivot.transform.RotateAround (pivot.position, pivot.up, Input.GetAxis ("Mouse X") * dragSpeed);
			pivot.transform.RotateAround (pivot.position, pivot.right, Input.GetAxis ("Mouse Y") * dragSpeed);
			transform.LookAt (pivot);
		}

		if ( Input.GetAxis ("Mouse ScrollWheel") > 0f && transform.localPosition.z > minDistance) {
			Vector3 differenceVector = (pivot.position - transform.position).normalized;
			differenceVector.z *= Input.GetAxis( "Mouse ScrollWheel" ) * scrollSpeed ;
			transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - Input.GetAxis ("Mouse ScrollWheel") * scrollSpeed );
			transform.LookAt (pivot);
		} else if ( Input.GetAxis ("Mouse ScrollWheel") < 0f && transform.localPosition.z < maxDistance) {
			Vector3 differenceVector = (pivot.position - transform.position).normalized;
			differenceVector.z *= Input.GetAxis( "Mouse ScrollWheel" ) * scrollSpeed ;
			transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - Input.GetAxis ("Mouse ScrollWheel") * scrollSpeed );
			transform.LookAt (pivot);
		}

		lastMousePos = Input.mousePosition;
	}
}
