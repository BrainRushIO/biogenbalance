using UnityEngine;
using System.Collections;

public class PracticeFullCourseModuleStep : BasePracticeModuleStep {

	protected override void Start () {
		base.Start();
	}

	/// <summary>
	/// Executes the step logic. This is called from the Submodule Manager. Any logic that can't be expressed via simple bool toggles goes here. The index is the sibling index of this object.
	/// </summary>
	public override void ExecuteStepLogic() {
		int index = transform.GetSiblingIndex();
		switch( index )
		{
		default:
			//			Debug.Log( "No step logic for this index." );
			break;
		}

		//		Debug.LogError( "Cannot execute step logic for index "+ index +". Index out of range." );
	}
}
