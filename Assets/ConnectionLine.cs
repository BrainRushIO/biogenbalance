using UnityEngine;
using System.Collections;

public class ConnectionLine : MonoBehaviour {

	public Transform start;
	public Transform end;



	// Use this for initialization
	void Start () {
		//Vector3 midpoint = new Vector3 ((1 + start.position.x + end.position.x)/2, (1 + start.position.y + end.position.y)/2, (start.position.z + end.position.z)/2);

		GetComponent<LineRenderer> ().SetPosition (0, start.position);
		//GetComponent<LineRenderer> ().SetPosition (1, midpoint);
		GetComponent<LineRenderer>().SetPosition(1, end.position);
	}
	
	// Update is called once per frame
	void Update () {
		//Vector3 midpoint = new Vector3 ((1 + start.position.x + end.position.x)/2, (1 + start.position.y + end.position.y)/2, (start.position.z + end.position.z)/2);

		GetComponent<LineRenderer> ().SetPosition (0, start.position);
		//GetComponent<LineRenderer> ().SetPosition (1, midpoint);
		GetComponent<LineRenderer>().SetPosition(1, end.position);
	}
}
