using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ReadoutDisplay : MonoBehaviour {
	public static ReadoutDisplay s_instance;

	public Text readoutPlusText, readoutNumberText, readoutUnitsText;

	public bool calibrationCoroutineStarted = false;
	public bool turningOnCoroutineStarted = false;
	public bool balanceOn = false;
	public bool balanceCalibrated = false;
	public bool hasStableReading = false;
	public bool doorsAreOpen = false;

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

	public void WeighObject( float weight ) {
		StartCoroutine( DisplayGibberishToNumber( weight ) );
	}

	private IEnumerator DisplayGibberishToNumber( float weight ) {
		while( doorsAreOpen ) {
			readoutNumberText.text = Random.Range( 0f, weight+30f ).ToString("F4");
			yield return new WaitForSeconds( 0.25f );
		}

		float startTime = Time.time;
		float lerpTime1 = 3f;
		float lerpTime2 = 2f;
		float currNumber = weight;

		while( lerpTime1 >= Time.time-startTime ) {
			currNumber = Mathf.Lerp( 0f, weight+Random.Range( 1f, 3f), (Time.time-startTime)/lerpTime1 );
			readoutNumberText.text = currNumber.ToString("F4");
			yield return new WaitForSeconds( 0.25f );
		}
		startTime = Time.time;
		while( lerpTime2 >= Time.time-startTime ) {
			;
			readoutNumberText.text = Mathf.Lerp( currNumber, weight, (Time.time-startTime)/lerpTime1 ).ToString("F4");
			yield return new WaitForSeconds( 0.3f );
		}
		readoutNumberText.text = weight.ToString("F4");
		hasStableReading = true;
	}
}
