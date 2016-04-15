using UnityEngine;
using System.Collections;

public class RotateObject : MonoBehaviour {

	private enum RotationAxis { X, Y, Z }
	[SerializeField]
	private RotationAxis rotAxis = RotationAxis.Y;

	public float rotSpeed = -20f;

	private Transform thisTransform;

	void Start () {
		thisTransform = transform;
	}

	void Update () {
		// Defaults to RotationAxis.Y
		Vector3 axis = Vector3.up;
		switch( rotAxis ) 
		{
		case RotationAxis.X:
			axis = Vector3.right;
			break;
		case RotationAxis.Z:
			axis = Vector3.forward;
			break;
		}

		thisTransform.Rotate( axis, rotSpeed*Time.deltaTime );
	}
}
