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
	public bool isSlerpingToNewPosition = false;
	public Vector3 defaultCameraStartPos;
	public OrbitCamera orbitCam;
	public Transform currentCameraPivot, currentCameraStartPos;

	void Awake() {
		if( s_instance == null ) {
			s_instance = this;
		} else {
			DestroyImmediate( gameObject );
		}
	}

	void Start() {
		UIManager.s_instance.ToggleToolsActive( false, false, false, false );
		defaultCameraStartPos = defaultCameraStartTransform.position;
		orbitCam = sceneCamera.GetComponent<OrbitCamera>();
	}

	void Update () {
		#region Input
		if( !isSlerpingToNewPosition ) {
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
		StartCoroutine( SlerpToNewCamPos() );
	}
		
	private void ClearSelectedFamiliarizeObject( bool slerpToDefaultPos ) {
		if( selectedObject != null ) {
			selectedObject.Deselect();
			selectedObject = null;
			currentCameraPivot = defaultCameraPivot;
			currentCameraStartPos = defaultCameraStartTransform;

			if( slerpToDefaultPos ) {
				StartCoroutine( SlerpToNewCamPos() );
			}
		}
	}

	private IEnumerator SlerpToNewCamPos() {
		isSlerpingToNewPosition = true;
		float elapsedTime = 0f;
		float slerpTime = 1f;
		float startTime = Time.time;
		Vector3 startPos = sceneCamera.transform.position;

		while( elapsedTime < slerpTime) {
			if( (elapsedTime/slerpTime) >= 0.98f ) {
				sceneCamera.transform.position = currentCameraStartPos.position;
				break;
			}
			sceneCamera.transform.position = Vector3.Slerp( startPos, currentCameraStartPos.position, elapsedTime/slerpTime );
			sceneCamera.transform.LookAt( currentCameraPivot );
			yield return null;
			elapsedTime = Time.time-startTime;
		}
		isSlerpingToNewPosition = false;
		orbitCam.pivot.position = currentCameraPivot.position;
		sceneCamera.transform.LookAt( currentCameraPivot.position );
	}
}
