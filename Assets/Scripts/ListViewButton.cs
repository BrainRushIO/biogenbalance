using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ListViewButton : MonoBehaviour {

	public string key;
	public int listIndex;
//	public int stepIndex;
	public Text childText;
	public Toggle checkBox;

	void Start() {
		if( ApplicationManager.s_instance.currentApplicationMode == ApplicationManager.ApplicationMode.Familiarize ){ 
			if( FamiliarizeManager.s_instance.familiarizeDictionary.ContainsKey( key ) ) {
				FamiliarizeDictionaryEntry appendedEntry = FamiliarizeManager.s_instance.familiarizeDictionary[key];
				appendedEntry.button = this;
				FamiliarizeManager.s_instance.familiarizeDictionary[key] = appendedEntry;
			} else {
				Debug.LogWarning( gameObject.name +"'s ListViewButton couldn't find its key in the Dictionary. Key:" +key );
			}
		}
	}

	public void ClickedButton() {
		if( ApplicationManager.s_instance.currentApplicationMode == ApplicationManager.ApplicationMode.Familiarize )
			FamiliarizeManager.s_instance.SelectObjectOfKey( key );
		if( ApplicationManager.s_instance.currentApplicationMode == ApplicationManager.ApplicationMode.Acquire )
			AcquireManager.s_instance.GoToStep( listIndex );
	}
}
