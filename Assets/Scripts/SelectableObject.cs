using UnityEngine;
using System.Collections;

public class SelectableObject : MonoBehaviour {

	public enum SelectableObjectType { None, CalibrationWeight, WeighContainer, WeighPan, RiceContainer, LeftDoor, RightDoor, OnButton, TareButton }
	public SelectableObjectType objectType;

	void Start() {
		if( objectType == SelectableObjectType.None )
			Debug.LogWarning( name + "'s SelectableObjectType is set to None." );
		if( gameObject.tag != "Selectable" )
			Debug.LogWarning( name + " is not tagged as \"Selectable\" but has a Selectable component on it. Setting it to \"Selectable\"" );
		gameObject.tag = "Selectable";
	}
}
