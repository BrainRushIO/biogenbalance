using UnityEngine;
using System.Collections;

public abstract class BaseAcquireSubmodule : MonoBehaviour {
	public static BaseAcquireSubmodule s_instance;

	public BaseAcquireModuleStep[] moduleSteps;

	protected  bool[] inputs;

	void Awake() {
		if( s_instance == null ) {
			s_instance = this;
			Init();
		} else {
			Debug.LogWarning( "Deleting duplicate BaseAcquireModule named " + gameObject.name );
		}
	}

	protected virtual void Init () {
		moduleSteps = GetComponentsInChildren<BaseAcquireModuleStep>();

		if( moduleSteps == null || moduleSteps.Length == 0) {
			Debug.LogError( "Could not find any children under BaseAcquireModule named: " + gameObject.name );
		}
	}

	public abstract void UpdateSceneContents( int stepIndex );

	/// <summary>
	/// Resets the scene objects to their default position.
	/// </summary>
	public abstract void ResetScene();

	public Transform GetStepCameraTransform( int stepIndex ) {
		if( moduleSteps == null ) {
			Debug.LogError( "No list of module steps exists." );
			return null;
		}
		if( stepIndex >= moduleSteps.Length ) {
			Debug.LogError( "Provided index out of range." );
			return null;
		}

		return moduleSteps[stepIndex].cameraPosition;
	}
}
