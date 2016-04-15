using UnityEngine;
using System.Collections;

public class PracticePrepareModuleStep : BasePracticeModuleStep {

	[Header("Object Toggles")]
	public bool toggleClickedNewPositionButton = false;
	public bool toggleBalanceIsLeveled = false;

	[Header("Inputs")]
	public bool inputClickedNewPositionButton = false;
	public bool inputBalanceIsLeveled = false;

	protected void Awake() {
		objectToggles = new bool[2];
		objectToggles[0] = toggleClickedNewPositionButton;
		objectToggles[1] = toggleBalanceIsLeveled;

		inputs = new bool[2];
		inputs[0] = inputClickedNewPositionButton;
		inputs[1] = inputBalanceIsLeveled;
	}

	/// <summary>
	/// Executes the step logic. This is called from the Submodule Manager. Any logic that can't be expressed via simple bool toggles goes here. The index is the sibling index of this object.
	/// </summary>
	public override void ExecuteStepLogic() {
		int index = transform.GetSiblingIndex();
		switch( index )
		{
		case 0:
			break;
		case 1:
			ApplicationManager.s_instance.ChangeMouseMode( (int)ApplicationManager.MouseMode.Pointer );
			UIManager.s_instance.ToggleToolsActive( true, false, false, false );
			PracticeManager.s_instance.orbitCam.canZoom = true;
			break;
		default:
			Debug.LogWarning( "No step logic for this index." );
			break;
		}
	}
}
