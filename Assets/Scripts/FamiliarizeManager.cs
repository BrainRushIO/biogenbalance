using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml;

public struct FamiliarizeDictionaryEntry {
	public string listViewText, descriptionViewText;
	public ListViewButton button;
	public FamiliarizeObject obj;
}

public class FamiliarizeManager : MonoBehaviour {
	public static FamiliarizeManager s_instance;

	public enum FamiliarizeModule { MicroBalance, SemiMicroBalance }
	public FamiliarizeModule moduleType = FamiliarizeModule.MicroBalance;

	public Camera sceneCamera;
	public Transform defaultCameraPivot, defaultCameraStartTransform;
	public FamiliarizeObject selectedObject;
	[System.NonSerialized]
	public bool isDragging = false;
	public TextAsset familiarizeContentXML;

	[Header("UI")]
	public Button defaultListViewButton;

	public Dictionary<string, FamiliarizeDictionaryEntry> familiarizeDictionary;

	/// <summary>
	/// Checks if user has clicked down on mouse item. Used to differentiate between a click and drag
	/// </summary>
	private bool hasClickedDownOnItem = false;
	private bool isLerpingToNewPosition = false;
	private bool isCameraRotLerping = false;
	private OrbitCamera orbitCam;
	private Transform currentCameraPivot, currentCameraStartPos;
	private string defaultDescViewText = "Click an item in the Outliner to learn more about it.";

	void Awake() {
		if( s_instance == null ) {
			s_instance = this;
			InitializeFamiliarizeDictionary();
			InitializeListView();
		} else {
			Debug.LogWarning( "Destroying duplicate Familiarize Manager" );
			DestroyImmediate( gameObject );
		}
	}

	void Start() {
		UIManager.s_instance.ToggleToolsActive( false, false, false, false );
		orbitCam = sceneCamera.GetComponent<OrbitCamera>();
		UIManager.s_instance.descriptionViewText.text = defaultDescViewText;
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
						}
					}
				} else if( !isDragging && !ApplicationManager.s_instance.userIsInteractingWithUI ) {
					ClearSelectedFamiliarizeObject( true );
				}

				hasClickedDownOnItem = false;
				isDragging = false;
			}

			if( ApplicationManager.s_instance.userIsInteractingWithUI )
				return;
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

	void InitializeFamiliarizeDictionary() {
		familiarizeDictionary = new Dictionary<string, FamiliarizeDictionaryEntry>();
		XmlDocument xmlFamiliarizeContent = new XmlDocument();
		xmlFamiliarizeContent.LoadXml( familiarizeContentXML.text );
		//xmlFamiliarizeContent.Load( Application.dataPath + "/Resources/familiarize_content.xml" );

		string path = "";
		switch( moduleType ) 
		{
		case FamiliarizeModule.MicroBalance:
			path = "microBalance";
			break;
		case FamiliarizeModule.SemiMicroBalance:
			path = "semiMicroBalance";
			break;
		}

		XmlNodeList itemList = xmlFamiliarizeContent.SelectNodes( "/content/"+ path +"/item" );

		foreach( XmlNode item in itemList ) {
			FamiliarizeDictionaryEntry newEntry = new FamiliarizeDictionaryEntry();
			newEntry.listViewText = item.SelectSingleNode( "listText" ).InnerText;
			newEntry.descriptionViewText = item.SelectSingleNode( "descriptionText" ).InnerText;
			newEntry.button = null;
			newEntry.obj = null;

			familiarizeDictionary.Add( item.SelectSingleNode("key").InnerText, newEntry );
		}
		Debug.Log( "Created dictionary." );
	}

	private void InitializeListView() {
		int familiarizeDictionaryCount = familiarizeDictionary.Count;

		// Setting the height of the list view to match the amount of buttons i will add to it.
		RectTransform listViewVerticalLayoutGroup = UIManager.s_instance.listViewContentParent;
		Vector2 newWidthHeight = listViewVerticalLayoutGroup.sizeDelta;
		newWidthHeight.y = defaultListViewButton.GetComponent<RectTransform>().sizeDelta.y * familiarizeDictionaryCount;
		listViewVerticalLayoutGroup.sizeDelta = newWidthHeight;

		// Creating new buttons out the dictionary and stuffing them in the vertical layout group
		foreach(  KeyValuePair<string, FamiliarizeDictionaryEntry> entry in familiarizeDictionary ) {
			KeyValuePair<string, FamiliarizeDictionaryEntry> appendedEntry = entry;
			ListViewButton newListViewButton = Instantiate( defaultListViewButton ).GetComponent<ListViewButton>();
			newListViewButton.childText.text = entry.Value.listViewText;
			newListViewButton.key = appendedEntry.Key;
			FamiliarizeDictionaryEntry appendedValue = appendedEntry.Value;
			appendedValue.button = newListViewButton;
			newListViewButton.transform.SetParent(listViewVerticalLayoutGroup, false );

			// Setting the individual keys is called on the Start function of the list items.
		}
	}

	public void SelectObjectOfKey( string searchKey ) {
		if( isLerpingToNewPosition )
			return;

		FamiliarizeDictionaryEntry temp;
		if( familiarizeDictionary.TryGetValue(searchKey, out temp) ) {
			if( temp.obj != null ) {
				SelectFamiliarizeObject( temp.obj );
			}
			else
				Debug.LogError( "Dictionary lookup was successful but obj reference is null" );
		} else {
			Debug.LogError( "Could not find dictionary entry for key: "+ searchKey );
		}
	}

	private void SelectFamiliarizeObject( FamiliarizeObject newSelection ) {
		ClearSelectedFamiliarizeObject( false );

		// Highlight Button in List View
		ToggleListViewButtonHighLight( newSelection.dictionaryKey, true );
		familiarizeDictionary[newSelection.dictionaryKey].button.checkBox.enabled = true;

		// Update Description View text
		UIManager.s_instance.descriptionViewText.text = familiarizeDictionary[newSelection.dictionaryKey].descriptionViewText;

		// Selecting of new object and starting camera transition
		selectedObject = newSelection;
		currentCameraPivot = selectedObject.cameraPivot;
		currentCameraStartPos = selectedObject.cameraStartPosition;
		selectedObject.Select();
		StartCameraTransition();
	}
		
	private void ClearSelectedFamiliarizeObject( bool slerpToDefaultPos ) {
		if( selectedObject == null )
			return;

		ToggleListViewButtonHighLight( selectedObject.dictionaryKey, false );
		selectedObject.Deselect();
		selectedObject = null;
		currentCameraPivot = defaultCameraPivot;
		currentCameraStartPos = defaultCameraStartTransform;

		if( slerpToDefaultPos ) {
			UIManager.s_instance.descriptionViewText.text = defaultDescViewText;
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
//			if( (elapsedTime/slerpTime) >= 0.99f ) {
//				break;
//			}
			sceneCamera.transform.position = Vector3.Lerp( startPos, currentCameraStartPos.position, elapsedTime/slerpTime );
			if( !isCameraRotLerping )
				sceneCamera.transform.LookAt( currentCameraPivot );
			yield return null;
			elapsedTime = Time.time-startTime;
		}
		orbitCam.pivotParent.LookAt( orbitCam.transform );
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
		float slerpTime = 1;
		float startTime = Time.time;
		Quaternion startRot = sceneCamera.transform.rotation;

		while( elapsedTime < slerpTime ) {
//			if( (elapsedTime/slerpTime) >= 0.99f ) {
//				break;
//			}
			Quaternion targetRot = Quaternion.LookRotation( (currentCameraPivot.position-sceneCamera.transform.position).normalized );
			sceneCamera.transform.rotation = Quaternion.Lerp( startRot, targetRot, elapsedTime/slerpTime );
			yield return null;
			elapsedTime = Time.time-startTime;
		}
		sceneCamera.transform.LookAt( currentCameraPivot );
		isCameraRotLerping = false;
	}

	private void ToggleListViewButtonHighLight( string key, bool toggleOn ) {
		Button listViewButton = familiarizeDictionary[key].button.GetComponent<Button>();

		if( listViewButton == null ) {
			Debug.LogError( "Couldn't find button with key: "+ key );
			return;
		}

		ColorBlock tempBlock = listViewButton.colors;
		if( toggleOn ) {
			tempBlock.normalColor = UIManager.s_instance.listViewButtonHighlightColor;
		} else {
			tempBlock.normalColor = UIManager.s_instance.listViewButtonNormalColor;
		}
		listViewButton.colors = tempBlock;
	}
}