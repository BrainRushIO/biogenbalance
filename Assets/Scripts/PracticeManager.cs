using UnityEngine;
using System.Collections;

public class PracticeManager : MonoBehaviour {
	public static PracticeManager s_instance;

	private int currentStepIndex = 0;

	void Awake() {
		if( s_instance == null ) {
			s_instance = this;
		} else {
			Debug.LogWarning( "Destroying duplicate Practice Manager." );
			DestroyImmediate( this.gameObject );
		}
	}

	void Start () {
	
	}

	void Update () {
		switch( currentStepIndex )
		{
		default:
			break;
		}
	}

	public void PressedHintButton() {
		UIManager.s_instance.ToggleSidePanelOn( true );
	}
}
