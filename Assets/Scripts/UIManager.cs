using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	public static UIManager s_instance;

	[Header("Top Bar")]
	public Button familiarizeButton,
				acquireButton,
				practiceButton,
				validateButton,
				progressButton;
	public RectTransform familiarizeDropdownItems,
				acquireDropdownItems,
				practiceDropdownItems;

	[Header("Side Bar")]
	public RectTransform sidepanel;
	public Text listViewText,
				descriptionViewText;

	[Header("Tools")]
	public Button pointerToolButton,
				rotateToolButton,
				panToolButton,
				forcepsToolButton;

	void Awake() {
		if( s_instance == null ) {
			s_instance = this;
			DontDestroyOnLoad( gameObject );
		} else {
			DestroyImmediate( gameObject );
		}
	}

	void Start () {
	
	}

	void Update () {
	
	}
}
