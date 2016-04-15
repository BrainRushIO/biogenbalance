using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MessageWindow : MonoBehaviour {

	public Text windowTitleText, bodyText;
	public Button closeButton;

	public void CloseWinow() {
		ApplicationManager.s_instance.CloseMessageWindow();
	}
}
