using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	public static UIManager s_instance;

	[Header("Top Bar")]
	public Button familiarizeButton;
	public Button acquireButton;
	public Button practiceButton;
	public Button validateButton;
	public Button progressButton;
	public Button settingsButton;
	public RectTransform familiarizeDropdownItems,
				acquireDropdownItems,
				practiceDropdownItems;
	public Color menuButtonsBaseColor, menuButtonsHighlightColor;

	[Header("Side Bar")]
	public RectTransform sidePanel;
	public Text listViewText,
				descriptionViewText;

	[Header("Tools")]
	public Button pointerToolButton;
	public Button rotateToolButton;
	public Button panToolButton;
	public Button forcepsToolButton;

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
}
