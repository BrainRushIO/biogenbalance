using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class ConnectLineRenderer : MonoBehaviour {

	public Transform endPoint;
	public bool update = false;

	private LineRenderer myLineRenderer;

	void Start () {
		myLineRenderer = GetComponent<LineRenderer>();
		Vector3[] rendererPositions = new Vector3[2] {transform.position, endPoint.position};
		myLineRenderer.SetPositions( rendererPositions );
	}

	void LateUpdate() {
		if( !update )
			return;

		myLineRenderer.SetPosition( 0, transform.position );
		myLineRenderer.SetPosition( 1, endPoint.position );
	}
}
