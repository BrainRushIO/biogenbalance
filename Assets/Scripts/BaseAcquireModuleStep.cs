using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The class that include the input and setup data for a single step in the Acquire module.
/// </summary>
public abstract class BaseAcquireModuleStep : MonoBehaviour {

	public Transform cameraPosition, cameraPivot;

	protected Dictionary<string, bool> objectToggles;

	protected virtual void Start() {
		objectToggles = new Dictionary<string, bool>();
	}

	public abstract Dictionary<string, bool> GetStepInitData();

	public abstract void ExecuteStepLogic();
}
