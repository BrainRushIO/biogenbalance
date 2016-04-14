using UnityEngine;
using System.Collections;

public class OrbitCamera : MonoBehaviour {

	public Transform pivotParent;
	public float dragSpeed = 1;
	private float panSpeed = 0.15f;
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

		switch( ApplicationManager.s_instance.currentApplicationMode )
		{
		case ApplicationManager.ApplicationMode.Familiarize:
			if( !canRotate || FamiliarizeManager.s_instance == null )
				break;
			// Handle draggin camera
			if ( FamiliarizeManager.s_instance.isDragging == false && Input.GetMouseButton (0) && Mathf.Abs((Input.mousePosition-lastMousePos).magnitude) > 2f ) {
				FamiliarizeManager.s_instance.isDragging = true;
			}
			if( FamiliarizeManager.s_instance.isDragging ) {
				pivotParent.transform.RotateAround (pivotParent.position, pivotParent.up, Input.GetAxis ("Mouse X") * dragSpeed);
				pivotParent.transform.RotateAround (pivotParent.position, pivotParent.right, Input.GetAxis ("Mouse Y") * dragSpeed);
				transform.LookAt (pivotParent);
			}
			break;
		case ApplicationManager.ApplicationMode.Practice:
			if( PracticeManager.s_instance == null )
				break;

			if ( PracticeManager.s_instance.isDragging == false && Input.GetMouseButton (0) && Mathf.Abs((Input.mousePosition-lastMousePos).magnitude) > 2f )
				PracticeManager.s_instance.isDragging = true;

			switch( ApplicationManager.s_instance.currentMouseMode )
			{
			case ApplicationManager.MouseMode.Rotate:
				if( !canRotate )
					break;
				if( PracticeManager.s_instance.isDragging ) {
					pivotParent.transform.RotateAround (pivotParent.position, pivotParent.up, Input.GetAxis ("Mouse X") * dragSpeed);
					pivotParent.transform.RotateAround (pivotParent.position, pivotParent.right, Input.GetAxis ("Mouse Y") * dragSpeed);
					transform.LookAt (pivotParent);
				}
				break;
			case ApplicationManager.MouseMode.Pan:
				if( !canPan )
					break;
				if( PracticeManager.s_instance.isDragging ) {
					pivotParent.transform.Translate( Vector3.down * Input.GetAxis( "Mouse Y" ) * panSpeed );
					pivotParent.transform.Translate( Vector3.right * Input.GetAxis( "Mouse X" ) * panSpeed );
					transform.LookAt (pivotParent);
				}
				break;
			}
			break;
		}

		// Handle zooming in and out
		if( canZoom ) {
			if( ApplicationManager.s_instance.currentApplicationMode == ApplicationManager.ApplicationMode.Familiarize ) {
				float mouseWheelValue = Input.GetAxis ("Mouse ScrollWheel");
				if ( mouseWheelValue > 0f && transform.localPosition.z > minDistance ) {
					transform.localPosition += -(transform.localPosition).normalized * Input.GetAxis ("Mouse ScrollWheel") * scrollSpeed;
					transform.LookAt (pivotParent);
				} else if ( mouseWheelValue < 0f && transform.localPosition.z < maxDistance ) {
					transform.localPosition += -(transform.localPosition).normalized * Input.GetAxis ("Mouse ScrollWheel") * scrollSpeed;
					transform.LookAt (pivotParent);
				}
			}
		}

		lastMousePos = Input.mousePosition;
	}
}
