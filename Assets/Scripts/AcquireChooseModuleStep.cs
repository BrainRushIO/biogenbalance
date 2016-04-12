using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AcquireChooseModuleStep : BaseAcquireModuleStep {

	protected override void Start () {
		base.Start();
	}

	/// <summary>
	/// Returns the dictionary containing the bools for toggling on and off objects in the scene.
	/// </summary>
	/// <returns>The step init data.</returns>
	public override Dictionary<string, bool> GetStepInitData() {
		return new Dictionary<string, bool>();
	}

	/// <summary>
	/// Executes the step logic. This is called from the Submodule Manager. Any logic that can't be expressed via simple bool toggles goes here. The index is the sibling index of this object.
	/// </summary>
	public override void ExecuteStepLogic() {
		int index = transform.GetSiblingIndex();

		switch( index )
		{
		default:
			Debug.LogError( "Cannot execute step logic for index "+ index +". Index out of range." );
			break;
		}
	}
}
