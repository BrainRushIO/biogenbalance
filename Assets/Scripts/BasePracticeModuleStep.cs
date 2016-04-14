using UnityEngine;
using System.Collections;

public abstract class BasePracticeModuleStep : MonoBehaviour {
	protected bool[] objectToggles;

	protected virtual void Start() {
	}

	/// <summary>
	/// Returns the dictionary containing the bools for toggling on and off objects in the scene.
	/// </summary>
	/// <returns>The step init data.</returns>
	public bool[] GetStepInitData() {
		return objectToggles;
	}

	public abstract void ExecuteStepLogic();
}
