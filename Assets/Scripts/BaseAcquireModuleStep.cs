using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The class that include the input and setup data for a single step in the Acquire module.
/// </summary>
public abstract class BaseAcquireModuleStep : MonoBehaviour {

	public Transform cameraPosition;

	protected bool[] objectToggles;

	protected virtual void Start() {
		if( cameraPosition == null )
			Debug.LogWarning( "The AcquireModuleStep on "+ gameObject.name + " is missing a cameraPosition." );
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
