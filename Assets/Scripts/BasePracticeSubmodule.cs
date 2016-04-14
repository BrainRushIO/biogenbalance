using UnityEngine;
using System.Collections;

public abstract class BasePracticeSubmodule : MonoBehaviour {
	public static BasePracticeSubmodule s_instance;

	public BasePracticeModuleStep[] moduleSteps;

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
		moduleSteps = GetComponentsInChildren<BasePracticeModuleStep>();

		if( moduleSteps == null || moduleSteps.Length == 0) {
			Debug.LogError( "Could not find any children under BaseAcquireModule named: " + gameObject.name );
		}
	}

	public abstract void UpdateSceneContents( int stepIndex );

	/// <summary>
	/// Resets the scene objects to their default position.
	/// </summary>
	public abstract void ResetScene();
}
