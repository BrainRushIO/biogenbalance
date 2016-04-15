using UnityEngine;
using System.Collections;

public class CamRectAdjust : MonoBehaviour {

	public bool updateRect = true;

	private Camera myCam;

	void Start() {
		myCam = GetComponent<Camera>();
	}

	void Update () {
		if( !updateRect ) 
			return;

		Rect newRect = myCam.rect;
		newRect.position = new Vector2( 1f - newRect.width, 0f );
		myCam.rect = newRect;
	}
}
