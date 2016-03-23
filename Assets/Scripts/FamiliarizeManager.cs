using UnityEngine;
using System.Collections;

public class FamiliarizeManager : MonoBehaviour {
	public static FamiliarizeManager s_instance;

	public Camera sceneCamera;
	public Transform defaultCameraPivot, defaultCameraStartTransform;
	public FamiliarizeObject selectedObject;
	[System.NonSerialized]
	public bool isDragging = false;

	/// <summary>
	/// Checks if user has clicked down on mouse item. Used to differentiate between a click and drag
	/// </summary>
	private bool hasClickedDownOnItem = false;
	private bool isLerpingToNewPosition = false;
	private bool isCameraRotLerping = false;
	private OrbitCamera orbitCam;
	private Transform currentCameraPivot, currentCameraStartPos;

	void Awake() {
		if( s_instance == null ) {
			s_instance = this;
		} else {
			DestroyImmediate( gameObject );
		}
	}

	void Start() {
		UIManager.s_instance.ToggleToolsActive( false, false, false, false );
		orbitCam = sceneCamera.GetComponent<OrbitCamera>();
	}

	void Update () {
		#region Input
		if( !isLerpingToNewPosition ) {
			if ( Input.GetMouseButtonUp(0) ) {
				if( hasClickedDownOnItem && !isDragging ) {
					Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit;
					if ( Physics.Raycast(ray, out hit) ) {
						if ( hit.transform.gameObject.tag == "Animatable" ) {
							hit.transform.gameObject.GetComponent<Animator> ().SetTrigger ("Clicked");
						}
						FamiliarizeObject clickedObject = hit.transform.GetComponent<FamiliarizeObject>();
						if( clickedObject != null ) {
							SelectFamiliarizeObject( clickedObject );
						} else {
							ClearSelectedFamiliarizeObject( true );
						}
					}
				}

				hasClickedDownOnItem = false;
				isDragging = false;
			}
			if ( Input.GetMouseButtonDown(0) ){
				Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if ( Physics.Raycast(ray, out hit) ){
					if ( hit.transform.gameObject.tag == "Animatable" || hit.transform.gameObject.tag == "Selectable" ) {
						hasClickedDownOnItem = true;
					}
				}
			}
		}
		#endregion
	}

	private void SelectFamiliarizeObject( FamiliarizeObject newSelection ) {
		ClearSelectedFamiliarizeObject( false );

		selectedObject = newSelection;
		currentCameraPivot = selectedObject.cameraPivot;
		currentCameraStartPos = selectedObject.cameraStartPosition;
		selectedObject.Select();
		StartCameraTransition();
	}
		
	private void ClearSelectedFamiliarizeObject( bool slerpToDefaultPos ) {
		if( selectedObject == null )
			return;
		
		selectedObject.Deselect();
		selectedObject = null;
		currentCameraPivot = defaultCameraPivot;
		currentCameraStartPos = defaultCameraStartTransform;

		if( slerpToDefaultPos ) {
			StartCameraTransition();
		}
	}

	private void StartCameraTransition() {
		orbitCam.canRotate = false;
		orbitCam.canZoom = false;
		StartCoroutine( LerpCameraLookAt() );
		StartCoroutine( SlerpToNewCamPos() );
	}

	private IEnumerator SlerpToNewCamPos() {
		isLerpingToNewPosition = true;
		orbitCam.transform.parent = null;
		orbitCam.pivotParent.position = currentCameraPivot.position;
		float elapsedTime = 0f;
		float slerpTime = 1f;
		float startTime = Time.time;
		Vector3 startPos = sceneCamera.transform.position;

		while( elapsedTime < slerpTime ) {
			if( (elapsedTime/slerpTime) >= 0.98f ) {
				break;
			}
			sceneCamera.transform.position = Vector3.Lerp( startPos, currentCameraStartPos.position, elapsedTime/slerpTime );
			if( !isCameraRotLerping )
				sceneCamera.transform.LookAt( currentCameraPivot );
			yield return null;
			elapsedTime = Time.time-startTime;
		}
		orbitCam.pivotParent.rotation = Quaternion.identity;
		orbitCam.transform.parent = orbitCam.pivotParent;

		sceneCamera.transform.position = currentCameraStartPos.position;
		sceneCamera.transform.LookAt( currentCameraPivot );
		isLerpingToNewPosition = false;
		orbitCam.canRotate = true;
		orbitCam.canZoom = true;
	}

	private IEnumerator LerpCameraLookAt() {
		isCameraRotLerping = true;
		float elapsedTime = 0f;
		float slerpTime = 0.5f;
		float startTime = Time.time;
		Quaternion startRot = sceneCamera.transform.rotation;

		while( elapsedTime < slerpTime ) {
			if( (elapsedTime/slerpTime) >= 0.98f ) {
				break;
			}
			Quaternion targetRot = Quaternion.LookRotation( (currentCameraPivot.position-sceneCamera.transform.position).normalized );
			sceneCamera.transform.rotation = Quaternion.Lerp( startRot, targetRot, elapsedTime/slerpTime );
			yield return null;
			elapsedTime = Time.time-startTime;
		}
		sceneCamera.transform.LookAt( currentCameraPivot );
		isCameraRotLerping = false;
	}
}
