﻿using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class ApplicationManager : MonoBehaviour {
	public static ApplicationManager s_instance;

	public enum ApplicationMode { Intro, Familiarize, Acquire, Practice, Validate, Progress, Settings }
	public ApplicationMode currentApplicationMode = ApplicationMode.Familiarize;

	public enum MouseMode { Pointer, Rotate, Pan, Forceps }
	public MouseMode currentMouseMode = MouseMode.Pointer;

	public enum SpecialCursorMode { None, OpenHand, ClosedHand, PointingHand }
	public SpecialCursorMode currentSpecialCursorMode = SpecialCursorMode.None;

	private Dictionary<string, string> scenesDictionary;

	public bool userIsInteractingWithUI = false;
	public bool messageWindowActive = true;
	public PlayerData playerData;
	public Resolution[] resolutions;
	public int currentResolution;

	void Awake() {
		if( s_instance == null ) {
			s_instance = this;
			DontDestroyOnLoad( gameObject );

			resolutions = Screen.resolutions;
			currentResolution = ( resolutions.Length > 1 ) ? resolutions.Length-2 : resolutions.Length-1;
			Screen.SetResolution( resolutions[currentResolution].width, resolutions[currentResolution].height, false );
		} else {
			DestroyImmediate( gameObject );
		}
	}

	void Start () {
		InitSceneDictionary();
		Cursor.lockState = CursorLockMode.None;
		Load();
	}

	/// <summary>
	/// Initialized the scene dictionary that holds all the scene names.
	/// </summary>
	void InitSceneDictionary() {
		scenesDictionary = new Dictionary<string, string>();
		scenesDictionary.Add( "F1", "F1_SemiMicroBalance" );
		scenesDictionary.Add( "F2", "F2_MicroBalance" );
		scenesDictionary.Add( "A1", "A1_ChooseBalance" );
		scenesDictionary.Add( "A2", "A2_PreparingBalance" );
		scenesDictionary.Add( "A3", "A3_BalanceCalibration" );
		scenesDictionary.Add( "A4", "A4_UseBalance" );
		scenesDictionary.Add( "P1", "P1_ChooseBalance" );
		scenesDictionary.Add( "P2", "P2_PreparingBalance" );
		scenesDictionary.Add( "P3", "P3_BalanceCalibration" );
		scenesDictionary.Add( "P4", "P4_UseBalance" );
		scenesDictionary.Add( "P5", "P5_Full" );
		scenesDictionary.Add( "V1", "V1_Validate" );
		scenesDictionary.Add( "R", "Progress" );
		scenesDictionary.Add( "S", "Settings" );
	}

	void Update () {
	
	}

	/// <summary>
	/// Loads the selected scene when one of the top UI buttons is pressed.
	/// </summary>
	/// <param name="sceneInitials">The initials that correspond to the specific scene.</param>
	public void LoadScene( string sceneInitials ) {
		string newScene = scenesDictionary[sceneInitials];

		if (newScene == SceneManager.GetActiveScene().name)
			return;

		switch( currentApplicationMode )
		{
		case ApplicationMode.Intro:
			CloseMessageWindow();
			break;
		}

		// Change state
		if( sceneInitials.Contains("F") ) {
			currentApplicationMode = ApplicationMode.Familiarize;
		} else if ( sceneInitials.Contains("A") ) {
			currentApplicationMode = ApplicationMode.Acquire;
		} else if ( sceneInitials.Contains("P") ) {
			currentApplicationMode = ApplicationMode.Practice;
		} else if ( sceneInitials.Contains("V") ) {
			currentApplicationMode = ApplicationMode.Validate;
		} else if ( sceneInitials.Contains("R") ) {
			currentApplicationMode = ApplicationMode.Progress;
		} else if ( sceneInitials.Contains("S") ) {
			currentApplicationMode = ApplicationMode.Settings;
		}

		SetSpecialMouseMode( (int)SpecialCursorMode.None );
		UIManager.s_instance.ClearListView();
		UIManager.s_instance.UpdateDescriptionViewText( "" );
		SceneManager.LoadScene( newScene );
		UIManager.s_instance.HighlightMenuButton( ((int)currentApplicationMode)-1 );
	}

	public void ForceLoadScene( string sceneInitials ) {
		string newScene = scenesDictionary[sceneInitials];

		switch( currentApplicationMode )
		{
		case ApplicationMode.Intro:
			CloseMessageWindow();
			break;
		}

		// Change state
		if( sceneInitials.Contains("F") ) {
			currentApplicationMode = ApplicationMode.Familiarize;
		} else if ( sceneInitials.Contains("A") ) {
			currentApplicationMode = ApplicationMode.Acquire;
		} else if ( sceneInitials.Contains("P") ) {
			currentApplicationMode = ApplicationMode.Practice;
		} else if ( sceneInitials.Contains("V") ) {
			currentApplicationMode = ApplicationMode.Validate;
		} else if ( sceneInitials.Contains("R") ) {
			currentApplicationMode = ApplicationMode.Progress;
		} else if ( sceneInitials.Contains("S") ) {
			currentApplicationMode = ApplicationMode.Settings;
		}

		SetSpecialMouseMode( (int)SpecialCursorMode.None );
		UIManager.s_instance.ClearListView();
		UIManager.s_instance.UpdateDescriptionViewText( "" );
		SceneManager.LoadScene( newScene );
		UIManager.s_instance.HighlightMenuButton( ((int)currentApplicationMode)-1 );
	}

	public void ChangeMouseMode( int newMouseMode ) {
		currentMouseMode = (MouseMode)newMouseMode;
		UIManager.s_instance.ToggleToolHighlight( newMouseMode );
		UIManager.s_instance.UpdateMouseCursor();
	}

	public void CloseMessageWindow() {
		messageWindowActive = false;
		if( UIManager.s_instance.messageWindow != null )
			UIManager.s_instance.messageWindow.gameObject.SetActive( false );
	}

	public void SetSpecialMouseMode( int selection ) {
		currentSpecialCursorMode = (SpecialCursorMode)selection;
		UIManager.s_instance.UpdateMouseCursor();

	}

	public void Save() {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create( Application.persistentDataPath +"/user_data.stem" );
		bf.Serialize(file, playerData);
		file.Close();
		Debug.Log( "Saved file." );
	}

	private void Load() {
		if( File.Exists( Application.persistentDataPath + "/user_data.stem" ) )
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open( Application.persistentDataPath +"/user_data.stem", FileMode.Open );
			PlayerData data = (PlayerData)bf.Deserialize(file);
			file.Close();
			Debug.Log( "Loaded file." );

			playerData.f_semi = data.f_semi;
			playerData.f_micro = data.f_micro;
			playerData.a_choose = data.a_choose;
			playerData.a_prepare = data.a_prepare;
			playerData.a_calibrate = data.a_calibrate;
			playerData.a_use = data.a_use;
			playerData.p_choose = data.p_choose;
			playerData.p_prepare = data.p_prepare;
			playerData.p_calibrate = data.p_calibrate;
			playerData.p_use = data.p_use;
			playerData.p_full = data.p_full;
			playerData.validate = data.validate;
			playerData.completionTime = data.completionTime;
		} else {
			Debug.Log( "Could not find load file." );
		}
	}

	public void ClearData() {
		if( File.Exists( Application.persistentDataPath + "/user_data.stem" ) ) {
			File.Delete( Application.persistentDataPath + "/user_data.stem" );
		}
			
		playerData.f_semi = false;
		playerData.f_micro = false;
		playerData.a_choose = false;
		playerData.a_prepare = false;
		playerData.a_calibrate = false;
		playerData.a_use = false;
		playerData.p_choose = false;
		playerData.p_prepare = false;
		playerData.p_calibrate = false;
		playerData.p_use = false;
		playerData.p_full = false;
		playerData.validate = false;
		playerData. completionTime = float.NaN;
	}
}

[Serializable]
public class PlayerData
{
	// Familiarize
	public bool f_semi = false;
	public bool f_micro = false;
	// Acquire
	public bool a_choose = false;
	public bool a_prepare = false;
	public bool a_calibrate = false;
	public bool a_use = false;
	// Practice
	public bool p_choose = false;
	public bool p_prepare = false;
	public bool p_calibrate = false;
	public bool p_use = false;
	public bool p_full = false;
	// Validate
	public bool validate = false;
	public float completionTime = float.NaN;

}