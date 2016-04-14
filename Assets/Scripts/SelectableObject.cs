using UnityEngine;
using System.Collections;

public class SelectableObject : MonoBehaviour {

	public enum SelectableObjectType { None, CalibrationWeight, WeighContainer, WeighPan, RiceContainer, LeftDoor, RightDoor, OnButton, TareButton, LevelingScrews }
	public SelectableObjectType objectType;
}
