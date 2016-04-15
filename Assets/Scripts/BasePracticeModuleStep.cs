using UnityEngine;
using System.Collections;

public abstract class BasePracticeModuleStep : MonoBehaviour {

	public Transform cameraPosition;
	public Transform pivotPosition;

	protected bool[] objectToggles;
	protected bool[] inputs;

	protected virtual void Start() {
	}

	/// <summary>
	/// Returns the dictionary containing the bools for toggling on and off objects in the scene.
	/// </summary>
	/// <returns>The step init data.</returns>
	public bool[] GetToggles() {
		return objectToggles;
	}

	public bool[] GetInputs() {
		return inputs;
	}

	public Transform GetCameraPosition() {
		return cameraPosition;
	}

	public Transform GetPivotPosition() {
		return pivotPosition;
	}

	public abstract void ExecuteStepLogic();
}
