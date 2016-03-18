using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ApplicationManager : MonoBehaviour {
	public static ApplicationManager s_instance;

	private Dictionary<string, string> scenesDictionary;

	void Awake() {
		if( s_instance == null ) {
			s_instance = this;
			DontDestroyOnLoad( gameObject );
		} else {
			DestroyImmediate( gameObject );
		}
	}

	void Start () {
		InitSceneDictionary();
	}

	/// <summary>
	/// Initialized the scene dictionary that holds all the scene names.
	/// </summary>
	void InitSceneDictionary() {
		scenesDictionary = new Dictionary<string, string>();
		scenesDictionary.Add( "F1", "F1_SemiMicroBalance" );
		scenesDictionary.Add( "F2", "F2_MicroBalance" );
		scenesDictionary.Add( "A1", "A1_ChooseBalance" );
		scenesDictionary.Add( "A2", "A2_BalanceCalibration" );
		scenesDictionary.Add( "A3", "A3_PreparingBalance" );
		scenesDictionary.Add( "A4", "A4_UseBalance" );
		scenesDictionary.Add( "P1", "P1_ChooseBalance" );
		scenesDictionary.Add( "P2", "P2_BalanceCalibration" );
		scenesDictionary.Add( "P3", "P3_PreparingBalance" );
		scenesDictionary.Add( "P4", "P4_UseBalance" );
		scenesDictionary.Add( "P5", "P5_Full" );
		scenesDictionary.Add( "V1", "V1_Validate" );
	}

	void Update () {
	
	}

	/// <summary>
	/// Loads the selected scene when one of the top UI buttons is pressed.
	/// </summary>
	/// <param name="sceneInitials">The initials that correspond to the specific scene.</param>
	public void LoadScene( string sceneInitials ) {
		string newScene = scenesDictionary[sceneInitials];
		SceneManager.LoadScene( newScene );
		Debug.Log( "Loaded new scene: " + newScene );
	}
}
