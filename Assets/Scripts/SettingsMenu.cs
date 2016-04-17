using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {

	Dropdown resolutionDropdown;

	void Start () {
		resolutionDropdown.ClearOptions();

		for( int i = 0; i < ApplicationManager.s_instance.resolutions.Length; i++ ) {
			Dropdown.OptionData newOption = new Dropdown.OptionData();
			newOption.text = ApplicationManager.s_instance.resolutions[i].width +"x"+ ApplicationManager.s_instance.resolutions[i].height;
			resolutionDropdown.options.Add( newOption );
		}

		resolutionDropdown.value = ApplicationManager.s_instance.currentResolution;
	}

	public void ClearUserData() {
		ApplicationManager.s_instance.ClearData();
	}

	public void UpdateResolution() {
		ApplicationManager.s_instance.currentResolution = resolutionDropdown.value;
		Screen.SetResolution( ApplicationManager.s_instance.resolutions[ApplicationManager.s_instance.currentResolution].width, ApplicationManager.s_instance.resolutions[ApplicationManager.s_instance.currentResolution].height, false );
	}
}
