using UnityEngine;
using System.Collections;

public class PracticeChooseModuleStep : BasePracticeModuleStep {

	/// <summary>
	/// Executes the step logic. This is called from the Submodule Manager. Any logic that can't be expressed via simple bool toggles goes here. The index is the sibling index of this object.
	/// </summary>
	public override void ExecuteStepLogic() {
		// Fun fact. All of the logic is just gonna be in the parent manager.

//		int index = transform.GetSiblingIndex();
//		switch( index )
//		{
//		case 0:
//			break;
//		case 1:
//			break;
//		default:
//			break;
//		}
	}
}
