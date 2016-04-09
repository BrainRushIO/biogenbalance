﻿using UnityEngine;
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
	public Text descriptionViewText;
	public RectTransform defaultListViewModuleTitle;
	public Color listViewButtonNormalColor, listViewButtonHighlightColor;

	[Header("Tools")]
	public Button pointerToolButton;
	public Button rotateToolButton;
	public Button panToolButton;
	public Button forcepsToolButton;

	public Texture2D pointerCursor,
				rotateCursor,
				panCursor,
				forcepsCursor,
				selectableItemCursor,
				holdingItemCursor,
				placeItemCursor;

	void Awake() {
		if( s_instance == null ) {
			s_instance = this;
			DontDestroyOnLoad( gameObject );
		} else {
			DestroyImmediate( gameObject );
		}
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
		listViewContentParent.parent.parent.GetComponent<ScrollRect>().verticalScrollbar.value = 1f;
		listViewContentParent.parent.parent.GetComponent<ScrollRect>().horizontalScrollbar.value = 0f;;

		// Remove children
		listViewContentParent.DetachChildren();
	}

	/// <summary>
	/// Toggles the side panel on.
	/// </summary>
	/// <param name="toggleOn">If set to <c>true</c> toggle Side Panel on.</param>
	public void ToggleSidePanelOn( bool toggleOn ) {
		
	}

	public void UpdateDescriptionViewText( string newText ) {
		if( newText != null ) {
			descriptionViewText.transform.parent.parent.parent.GetComponent<ScrollRect>().verticalScrollbar.value = 1f;
			descriptionViewText.text = newText;
		} else {
			Debug.LogWarning( "UpdateDescriptionViewText() received a null string." );
		}
	}

	//TODO create a method that sets the values of the list and description views scrollbars
}
