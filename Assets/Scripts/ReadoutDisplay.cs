using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ReadoutDisplay : MonoBehaviour {
	public static ReadoutDisplay s_instance;

	public Text readoutPlusText, readoutNumberText, readoutUnitsText;

	private bool calibrationCoroutineStarted = false;
	private bool turningOnCoroutineStarted = false;
	private bool balanceOn = false;
	private bool balanceCalibrated = false;

	void Awake() {
		if( s_instance == null ) {
			s_instance = this;
		} else {
			Debug.LogWarning( "Deleting duplicate ReadoutDisplay named: " + name );
			DestroyImmediate( gameObject );
		}
	}

	public void ToggleDisplay( bool toggleNumbers, bool toggleUnits, bool togglePlus ) {
		readoutNumberText.enabled = toggleNumbers;
		readoutUnitsText.enabled = toggleUnits;
		readoutPlusText.enabled = togglePlus;
	}

	public void TurnBalanceOn() {
		if( balanceOn )
			return;

		if( !turningOnCoroutineStarted && !calibrationCoroutineStarted )
		StartCoroutine( TurnBalanceOnCoroutine() );
	}

	private IEnumerator TurnBalanceOnCoroutine() {
		turningOnCoroutineStarted = true;
		SoundtrackManager.s_instance.PlayAudioSource( SoundtrackManager.s_instance.buttonBeep );

		readoutNumberText.enabled = true;
		readoutUnitsText.enabled = true;
		readoutPlusText.enabled = true;
		readoutNumberText.text = "888.8888";
		yield return new WaitForSeconds( 2f );
		readoutUnitsText.enabled = false;
		readoutNumberText.enabled = false;
		yield return new WaitForSeconds( 3f );
		readoutUnitsText.enabled = true;
		readoutNumberText.enabled = true;
		readoutNumberText.text = "0.0000";
		readoutPlusText.enabled = false;

		SoundtrackManager.s_instance.PlayAudioSource( SoundtrackManager.s_instance.buttonBeep );
		turningOnCoroutineStarted = false;
		balanceOn = true;
	}

	public void PlayCalibrationModeAnimation() {
		if( balanceCalibrated || !balanceOn )
			return;

		if( !calibrationCoroutineStarted && !turningOnCoroutineStarted )
			StartCoroutine( CalibrationAnimation() );
	}

	private IEnumerator CalibrationAnimation() {
		calibrationCoroutineStarted = true;

		readoutNumberText.enabled = true;
		readoutNumberText.enabled = false;
		yield return new WaitForSeconds( 0.1f );
		readoutNumberText.enabled = true;
		readoutNumberText.text = ( "200.0000" );
		readoutPlusText.enabled = true;
		
		if( ApplicationManager.s_instance.currentApplicationMode == ApplicationManager.ApplicationMode.Practice ) 
		{
			if( PracticeManager.s_instance.moduleType == PracticeManager.PracticeModule.Calibrate ) 
			{
				PracticeManager.s_instance.submoduleManager.GetComponent<PracticeCalibrateBalanceManager>().ToggleBalanceCalibrationMode( true );
			} 
			else if( PracticeManager.s_instance.moduleType == PracticeManager.PracticeModule.Use ) 
			{
				PracticeManager.s_instance.submoduleManager.GetComponent<PracticeUseBalanceManager>().ToggleBalanceCalibrationMode( true );
			} 
			else if( PracticeManager.s_instance.moduleType == PracticeManager.PracticeModule.FullCourse ) 
			{
				PracticeManager.s_instance.submoduleManager.GetComponent<PracticeFullCourseManager>().ToggleBalanceCalibrationMode( true );
			}
		}

		SoundtrackManager.s_instance.PlayAudioSource( SoundtrackManager.s_instance.buttonBeep );
		calibrationCoroutineStarted = false;
		balanceCalibrated = true;
	}

	public void ZeroOut() {
		readoutNumberText.text = "0.0000";
	}
}
