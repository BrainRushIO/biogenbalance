using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class ConnectLineRenderer : MonoBehaviour {

	public Transform endPoint;

	private LineRenderer myLineRenderer;

	void Start () {
		myLineRenderer = GetComponent<LineRenderer>();
		Vector3[] rendererPositions = new Vector3[2] {transform.position, endPoint.position};
		myLineRenderer.SetPositions( rendererPositions );
	}
}
