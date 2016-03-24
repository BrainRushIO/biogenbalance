using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ListViewButton : MonoBehaviour {

	public string key;
	public Text childText;

	void Start() {
		if( FamiliarizeManager.s_instance.familiarizeDictionary.ContainsKey( key ) ) {
			FamiliarizeDictionaryEntry appendedEntry =  FamiliarizeManager.s_instance.familiarizeDictionary[key];
			appendedEntry.button = this;
			FamiliarizeManager.s_instance.familiarizeDictionary[key] = appendedEntry;
		} else {
			Debug.LogWarning( gameObject.name +"'s ListViewButton couldn't find its key in the Dictionary. Key:" +key );
		}
	}

	public void ClickedButton() {
		FamiliarizeManager.s_instance.SelectObjectOfKey( key );
	}
}
