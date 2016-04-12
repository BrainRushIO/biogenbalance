using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct AcquireModuleStepInitializationData {
	public Dictionary<string, bool> objectToggles;
	public Dictionary<string, string> messagesToParent;
}

/// <summary>
/// The class that include the input and setup data for a single step in the Acquire module.
/// </summary>
public abstract class BaseAcquireModuleStep : MonoBehaviour {

	public Transform cameraPosition, cameraPivot;
	public AcquireModuleStepInitializationData initData;

	protected virtual void Start() {
		initData = new AcquireModuleStepInitializationData();
	}

	public abstract AcquireModuleStepInitializationData GetStepInitData();

	public abstract void ExecuteStepLogic( int index );
}
