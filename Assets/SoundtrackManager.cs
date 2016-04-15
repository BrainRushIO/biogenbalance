using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SoundtrackManager : MonoBehaviour {

	public static SoundtrackManager s_instance;
	float currentVolume =1f, musicVolume=1f;
//	[SerializeField] Slider volumeSlider, musicSlider;
	public AudioSource ambientLab, buttonBeep, placeObject1, placeObject2, rice1, rice2, servo, slidingDoor;

	void Awake () {
		if (s_instance == null) {
			s_instance = this;
		} else {
			Destroy(gameObject);
		}		
	}

//	IEnumerator FadeOutOceanAudioSource() {
//		while (oceanBreeze.volume > 0.0f) {					//where x is sound track file
//			oceanBreeze.volume -= 0.01f;
//			yield return new WaitForSeconds (.002f);
//		}
//		oceanBreeze.Stop ();
//		isFadingOutOceanAudio = false;
//	}
//
//	IEnumerator FadeOutAudioSource(AudioSource x) { //call from elsewhere
//		while (x.volume > 0.0f) {					//where x is sound track file
//			x.volume -= 0.01f;
//			yield return new WaitForSeconds (.002f);
//		}
//		x.Stop ();
//	}

	public void SetVolume() {
//		currentVolume = volumeSlider.value;
	}

	public void SetMusicVolume() {
//		musicVolume = musicSlider.value;
//		oceanBreeze.volume = musicVolume;
//		music.volume = musicVolume;
	}

	public void PlayAudioSource(AudioSource x) { //call from elsewhere
//		if( x.name == "ocean" && isFadingOutOceanAudio ) {
//			StopCoroutine ("FadeOutOceanAudioSource");
//		}

		x.volume = currentVolume;
		x.Play ();
	}
}