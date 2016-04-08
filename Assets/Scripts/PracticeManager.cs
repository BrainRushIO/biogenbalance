using UnityEngine;
using System.Collections;

public class PracticeManager : MonoBehaviour {
	public static PracticeManager s_instance;

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
	
	}
}
