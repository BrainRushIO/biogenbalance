using UnityEngine;
using System.Collections;

public class NextButton : MonoBehaviour {

	public void PressedNextButton() {
		if( ApplicationManager.s_instance != null ) {
			switch( ApplicationManager.s_instance.currentApplicationMode )
			{
			case ApplicationManager.ApplicationMode.Acquire:
				AcquireManager.s_instance.GoToNextStep();
				break;
			case ApplicationManager.ApplicationMode.Practice:
				PracticeManager.s_instance.GoToNextStep();
				break;
			}
		}
	}
}
