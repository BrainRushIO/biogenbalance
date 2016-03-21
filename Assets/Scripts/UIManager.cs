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
}
