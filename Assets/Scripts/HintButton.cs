using UnityEngine;
using System.Collections;

public class HintButton : MonoBehaviour {

	public void ClickedHintButton() {
		if( ApplicationManager.s_instance == null )
			return;

		if( ApplicationManager.s_instance.currentApplicationMode == ApplicationManager.ApplicationMode.Practice && PracticeManager.s_instance != null ) {
			PracticeManager.s_instance.PressedHintButton();
		}
	}
}
