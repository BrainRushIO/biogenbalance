using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public GameObject flareGroup;
	public Toggle abramsToggle;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (abramsToggle.isOn) {
			flareGroup.SetActive(true);
		}
		else {
			flareGroup.SetActive (false);
		}

	}
}
