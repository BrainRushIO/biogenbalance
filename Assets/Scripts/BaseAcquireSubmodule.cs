using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseAcquireSubmodule : MonoBehaviour {
	public static BaseAcquireSubmodule s_instance;

	public BaseAcquireModuleStep[] moduleSteps;

	protected  Dictionary<string, bool> inputs;

	void Awake() {
		if( s_instance == null ) {
			s_instance = this;
		} else {
			Debug.LogWarning( "Deleting duplicate BaseAcquireModule named " + gameObject.name );
		}
	}

	protected virtual void Start () {
		moduleSteps = GetComponentsInChildren<BaseAcquireModuleStep>();

		if( moduleSteps == null || moduleSteps.Length == 0) {
			Debug.LogError( "Could not find any children under BaseAcquireModule named: " + gameObject.name );
		}
	}

	public virtual void UpdateSceneContents( int stepIndex ) {
	}

	/// <summary>
	/// Resets the scene objects to their default position.
	/// </summary>
	public virtual void ResetScene() {
		
	}
}
