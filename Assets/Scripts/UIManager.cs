using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public struct ListViewDescriptionViewTextPair {
	public string listViewText, descriptionViewText;
}

public class UIManager : MonoBehaviour {
	public static UIManager s_instance;

	[Header("General")]
	public MessageWindow messageWindow;

	[Header("Top Bar")]
	public Button familiarizeButton;
	public Button acquireButton;
	public Button practiceButton;
	public Button validateButton;
	public Button progressButton;
	public Button settingsButton;
	public Button nextButton;
	public RectTransform familiarizeDropdownItems,
				acquireDropdownItems,
				practiceDropdownItems;
	public Color menuButtonsBaseColor, menuButtonsHighlightColor;

	[Header("Side Bar")]
	public RectTransform sidePanel;
	public RectTransform listViewContentParent;
	public Scrollbar listViewVerticalScrollbar, listViewHorizontalScrollbar, descriptionViewScrollbar;
	public Text descriptionViewText;
	public RectTransform defaultListViewModuleTitle, defaultListViewButton, defaultListViewSectionTitle;
	public Color listViewButtonNormalColor, listViewButtonHighlightColor;
	public Button hintButton;

	[Header("Tools")]
	public Button pointerToolButton;
	public Button rotateToolButton;
	public Button panToolButton;
	public Button forcepsToolButton;
	private Sprite pointerButtonNormalSprite, pointerButtonHighlightedSprite,
					rotateButtonNormalSprite, rotateButtonHighlightedSprite,
					panButtonNormalSprite, panButtonHighlightedSprite,
					forcepsButtonNormalSprite, forcepsButtonHighlightedSprite;

	[Header("Cursors")]
	public Texture2D pointerCursor;
	public Texture2D rotateCursor,
				panCursor,
				forcepsCursor,
				selectableItemCursor,
				holdingItemCursor,
				placeItemCursor;

	void Awake() {
		if( s_instance == null ) {
			s_instance = this;
			DontDestroyOnLoad( gameObject );

			Init();
		} else {
			DestroyImmediate( gameObject );
		}
	}

	void Init() {
		pointerButtonNormalSprite = pointerToolButton.image.sprite;
		pointerButtonHighlightedSprite = pointerToolButton.spriteState.highlightedSprite;

		rotateButtonNormalSprite = rotateToolButton.image.sprite;
		rotateButtonHighlightedSprite = rotateToolButton.spriteState.highlightedSprite;

		panButtonNormalSprite = panToolButton.image.sprite;
		panButtonHighlightedSprite = panToolButton.spriteState.highlightedSprite;

		forcepsButtonNormalSprite = forcepsToolButton.image.sprite;
		forcepsButtonHighlightedSprite = forcepsToolButton.spriteState.highlightedSprite;
	}

	public void CloseDropDowns() {
		familiarizeDropdownItems.gameObject.SetActive (false);
		acquireDropdownItems.gameObject.SetActive (false);
		practiceDropdownItems.gameObject.SetActive (false);
	}

	public void DeselectTopBarButtons() {
		familiarizeButton.GetComponent<MenuBarButton> ().Deselect();
		acquireButton.GetComponent<MenuBarButton> ().Deselect();
		practiceButton.GetComponent<MenuBarButton> ().Deselect();
		validateButton.GetComponent<MenuBarButton> ().Deselect();
		progressButton.GetComponent<MenuBarButton> ().Deselect();
		settingsButton.GetComponent<MenuBarButton> ().Deselect();
	}

	/// <summary>
	/// Highlights the menu button.
	/// </summary>
	/// <param name="moduleInitial">Module initial. e.g. Familiarize = "F"</param>
	public void HighlightMenuButton( int optionNumber ) {
		familiarizeButton.GetComponent<MenuBarButton> ().ToggleHighlight( false );
		acquireButton.GetComponent<MenuBarButton> ().ToggleHighlight( false );
		practiceButton.GetComponent<MenuBarButton> ().ToggleHighlight( false );
		validateButton.GetComponent<MenuBarButton> ().ToggleHighlight( false );
		progressButton.GetComponent<MenuBarButton> ().ToggleHighlight( false );
		settingsButton.GetComponent<MenuBarButton> ().ToggleHighlight( false );

		switch (optionNumber) 
		{
		case 0:
			familiarizeButton.GetComponent<MenuBarButton> ().ToggleHighlight( true );
			break;
		case 1:
			acquireButton.GetComponent<MenuBarButton> ().ToggleHighlight( true );
			break;
		case 2:
			practiceButton.GetComponent<MenuBarButton> ().ToggleHighlight( true );
			break;
		case 3:
			validateButton.GetComponent<MenuBarButton> ().ToggleHighlight( true );
			break;
		case 4:
			progressButton.GetComponent<MenuBarButton> ().ToggleHighlight( true );
			break;
		case 5:
			settingsButton.GetComponent<MenuBarButton> ().ToggleHighlight( true );
			break;
		}
	}

	public void UpdateMouseCursor() {
		if( ApplicationManager.s_instance.currentSpecialCursorMode == ApplicationManager.SpecialCursorMode.None ) {
			switch (ApplicationManager.s_instance.currentMouseMode) 
			{
			case ApplicationManager.MouseMode.Pointer:
				Cursor.SetCursor( pointerCursor, Vector2.zero, CursorMode.ForceSoftware );
				break;
			case ApplicationManager.MouseMode.Rotate:
				Cursor.SetCursor( rotateCursor, Vector2.zero, CursorMode.ForceSoftware );
				break;
			case ApplicationManager.MouseMode.Pan:
				Cursor.SetCursor( panCursor, Vector2.zero, CursorMode.ForceSoftware );
				break;
			case ApplicationManager.MouseMode.Forceps:
				Cursor.SetCursor( forcepsCursor, Vector2.zero, CursorMode.ForceSoftware );
				break;
			}
		} else {
			switch( ApplicationManager.s_instance.currentSpecialCursorMode )
			{
			case ApplicationManager.SpecialCursorMode.OpenHand:
				Cursor.SetCursor( selectableItemCursor, Vector2.zero, CursorMode.ForceSoftware );
				break;
			case ApplicationManager.SpecialCursorMode.ClosedHand:
				Cursor.SetCursor( holdingItemCursor, Vector2.zero, CursorMode.ForceSoftware );
				break;
			case ApplicationManager.SpecialCursorMode.PointingHand:
				Cursor.SetCursor( placeItemCursor, Vector2.zero, CursorMode.ForceSoftware );
				break;
			}
		}
	}

	/// <summary>
	/// Toggles the tools active or innactive.
	/// </summary>
	/// <param name="pointer">If set to <c>true</c> pointer is toggled active.</param>
	/// <param name="rotate">If set to <c>true</c> rotate is toggled active.</param>
	/// <param name="pan">If set to <c>true</c> pan is toggled active.</param>
	/// <param name="forceps">If set to <c>true</c> forceps is toggled active.</param>
	public void ToggleToolsActive( bool pointer, bool rotate, bool pan, bool forceps ) {
		pointerToolButton.interactable = pointer;
		pointerToolButton.transform.GetChild(1).gameObject.SetActive( !pointer );

		rotateToolButton.interactable = rotate;
		rotateToolButton.transform.GetChild(1).gameObject.SetActive( !rotate );

		panToolButton.interactable = pan;
		panToolButton.transform.GetChild(1).gameObject.SetActive( !pan );

		forcepsToolButton.interactable = forceps;
		forcepsToolButton.transform.GetChild(1).gameObject.SetActive( !forceps );
	}

	public void ClearListView() {
		// Reset the scroll bars to 0. The scroll bar component is found in the grandfather of the listViewContentParent
		UpdateListViewVerticalScrollbarValue( 0f );
		UpdateListViewHorizontalScrollbarValue( 0f );

		// Remove children
		listViewContentParent.DetachChildren();
	}

	public void UpdateDescriptionViewText( string newText ) {
		if( newText != null ) {
			UpdateDescriptionViewScrollbarValue( 0f );
			descriptionViewText.text = newText;
		} else {
			Debug.LogWarning( "UpdateDescriptionViewText() received a null string." );
		}
	}

	public void UpdateListViewVerticalScrollbarValue( float percentage ) {
		listViewVerticalScrollbar.value = 1f - percentage;
	}

	public void UpdateListViewHorizontalScrollbarValue( float percentage ) {
		listViewHorizontalScrollbar.value = percentage;
	}

	public void UpdateDescriptionViewScrollbarValue( float percentage ) {
		descriptionViewScrollbar.value = 1f - percentage;
	}

	/// <summary>
	/// Toggles the side panel to appear or disappear.
	/// </summary>
	/// <param name="toggleOn">If set to <c>true</c> toggle on.</param>
	public void ToggleSidePanel( bool toggleOn, bool lerpTransition ) {
		CanvasGroup cG = sidePanel.GetComponent<CanvasGroup>();
		if( cG.alpha == 1f && toggleOn == true )
			return;
		else if( cG.alpha == 0f && toggleOn == false )
			return;

		if( lerpTransition ) 
			StartCoroutine( LerpSidePanelAlpha( toggleOn, 0.15f, cG ) );
		else
			cG.alpha = ( toggleOn ) ? 1f : 0f;
		
		cG.interactable = toggleOn;
		cG.blocksRaycasts = toggleOn;

		sidePanel.GetComponent<UIBoundsTrigger>().active = toggleOn;
	}

	private IEnumerator LerpSidePanelAlpha( bool zeroToOne, float duration, CanvasGroup cG ) {
		float startVal = ( zeroToOne ) ? 0f : 1f;
		float endVal = ( zeroToOne ) ? 1f : 0f;
		float lerpTime = 0f;

		while( lerpTime < duration ) {
			cG.alpha = Mathf.Lerp( startVal, endVal, lerpTime/duration );

			yield return null;
			lerpTime += Time.deltaTime;
		}
		cG.alpha = endVal;
	}

	public void ToggleToolHighlight( int newHighlightedTool ) {
		pointerToolButton.image.sprite = pointerButtonNormalSprite;
		rotateToolButton.image.sprite = rotateButtonNormalSprite;
		panToolButton.image.sprite = panButtonNormalSprite;
		forcepsToolButton.image.sprite = forcepsButtonNormalSprite;

		ApplicationManager.MouseMode toolToHighlight = (ApplicationManager.MouseMode)newHighlightedTool;
		switch( toolToHighlight )
		{
		case ApplicationManager.MouseMode.Pointer:
			pointerToolButton.image.sprite = pointerButtonHighlightedSprite;
			break;
		case ApplicationManager.MouseMode.Rotate:
			rotateToolButton.image.sprite = rotateButtonHighlightedSprite;
			break;
		case ApplicationManager.MouseMode.Pan:
			panToolButton.image.sprite = panButtonHighlightedSprite;
			break;
		case ApplicationManager.MouseMode.Forceps:
			forcepsToolButton.image.sprite = forcepsButtonHighlightedSprite;
			break;
		}
	}
}
