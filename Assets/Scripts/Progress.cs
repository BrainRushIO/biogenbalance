using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Progress : MonoBehaviour {

	public Slider familiarizeScrollbar, acquireScrollbar, practiceScrollBar, validateScrollBar;
	public Text familiarizeProgressText, acquireProgressText, practiceProgressText, validateProgressText, overallProgressText;
	public Image overallProgressImage;

	void Start () {
		UIManager.s_instance.ToggleSidePanel( true, false );

		familiarizeScrollbar.value = acquireScrollbar.value = practiceScrollBar.value = validateScrollBar.value = overallProgressImage.fillAmount = 0f;
		overallProgressText.text = "0.0%";

		int f = 0;
		if( ApplicationManager.s_instance.playerData.f_semi )
			f++;
		if( ApplicationManager.s_instance.playerData.f_micro )
			f++;

		int a = 0;
		if( ApplicationManager.s_instance.playerData.a_choose )
			a++;
		if( ApplicationManager.s_instance.playerData.a_prepare )
			a++;
		if( ApplicationManager.s_instance.playerData.a_calibrate )
			a++;
		if( ApplicationManager.s_instance.playerData.a_use )
			a++;

		int p = 0;
		if( ApplicationManager.s_instance.playerData.p_choose )
			p++;
		if( ApplicationManager.s_instance.playerData.p_prepare )
			p++;
		if( ApplicationManager.s_instance.playerData.p_calibrate )
			p++;
		if( ApplicationManager.s_instance.playerData.p_use )
			p++;
		if( ApplicationManager.s_instance.playerData.p_full )
			p++;

		int v = ( ApplicationManager.s_instance.playerData.validate ) ? 1 : 0;

		familiarizeProgressText.text = f + "/2";
		acquireProgressText.text = a + "/4";
		practiceProgressText.text = p + "/5";
		validateProgressText.text = v + "/1";

		StartCoroutine( StartLerps(f, a, p, v) );
	}

	private IEnumerator StartLerps( int f, int a, int p, int v ) {
		yield return LerpScrollBar( familiarizeScrollbar, 0.75f, (float)f/2f );
		yield return LerpScrollBar( acquireScrollbar, 0.75f, (float)a/4f );
		yield return LerpScrollBar( practiceScrollBar, 0.75f, (float)p/5f );
		yield return LerpScrollBar( validateScrollBar, 0.75f,(float)v/1f );
		yield return new WaitForSeconds( 0.5f );
		yield return LerpOverall( 1.5f, ((float)f+(float)a+(float)p+(float)v)/12f );
	}

	private IEnumerator LerpScrollBar( Slider scrollBar, float duration, float lerpToValue ) {
		if( lerpToValue == 0f )
			yield break;

		float startTime = Time.time;

		while( duration > Time.time - startTime ) {
			scrollBar.value = Mathf.SmoothStep( 0f, lerpToValue, (Time.time-startTime)/duration );
			yield return null;
		}

		scrollBar.value = lerpToValue;
	}

	private IEnumerator LerpOverall( float duration, float lerpToValue ) {
		float startTime = Time.time;

		while( duration > Time.time - startTime ) {
			float value = Mathf.SmoothStep( 0f, lerpToValue, (Time.time-startTime)/duration );
			overallProgressImage.fillAmount = value;
			overallProgressText.text = (value*100f).ToString("F1") + "%";
			yield return null;
		}

		overallProgressImage.fillAmount = lerpToValue;
		overallProgressText.text = (lerpToValue*100f).ToString("F1") + "%";
	}

}
