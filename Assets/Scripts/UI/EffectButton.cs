using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Audio;

/// <summary>
/// Class to handle effect sliders and buttons.
/// </summary>
public class EffectButton : MonoBehaviour {

	public Image image;

	public void ToggleDistortion () {
		GameObject source =
			MusicManager.instance.instrumentAudioSources [InstrumentSetup.currentRiff.instrument].gameObject;
		source.GetComponent<AudioDistortionFilter>().enabled = !source.GetComponent<AudioDistortionFilter>().enabled;
		image.sprite = (source.GetComponent<AudioDistortionFilter>().enabled ? InstrumentSetup.instance.percussionFilled : InstrumentSetup.instance.percussionEmpty);
	}

	public void ToggleEcho () {
		GameObject source =
			MusicManager.instance.instrumentAudioSources [InstrumentSetup.currentRiff.instrument].gameObject;
		source.GetComponent<AudioEchoFilter>().enabled = !source.GetComponent<AudioEchoFilter>().enabled;
		image.sprite = (source.GetComponent<AudioEchoFilter>().enabled ? InstrumentSetup.instance.percussionFilled : InstrumentSetup.instance.percussionEmpty);
	}

	public void ToggleReverb () {
		GameObject source =
			MusicManager.instance.instrumentAudioSources [InstrumentSetup.currentRiff.instrument].gameObject;
		source.GetComponent<AudioReverbFilter>().enabled = !source.GetComponent<AudioReverbFilter>().enabled;
		image.sprite = (source.GetComponent<AudioReverbFilter>().enabled ? InstrumentSetup.instance.percussionFilled : InstrumentSetup.instance.percussionEmpty);
	}

	public void ToggleTremolo () {
		GameObject source =
			MusicManager.instance.instrumentAudioSources [InstrumentSetup.currentRiff.instrument].gameObject;
		source.GetComponent<AudioTremoloFilter>().enabled = !source.GetComponent<AudioTremoloFilter>().enabled;
		image.sprite = (source.GetComponent<AudioTremoloFilter>().enabled ? InstrumentSetup.instance.percussionFilled : InstrumentSetup.instance.percussionEmpty);
	}

	public void ToggleChorus () {
		GameObject source =
			MusicManager.instance.instrumentAudioSources [InstrumentSetup.currentRiff.instrument].gameObject;
		source.GetComponent<AudioChorusFilter>().enabled = !source.GetComponent<AudioChorusFilter>().enabled;
		image.sprite = (source.GetComponent<AudioChorusFilter>().enabled ? InstrumentSetup.instance.percussionFilled : InstrumentSetup.instance.percussionEmpty);
	}

	public void ToggleFlanger () {
		GameObject source =
			MusicManager.instance.instrumentAudioSources [InstrumentSetup.currentRiff.instrument].gameObject;
		source.GetComponent<AudioFlangerFilter>().enabled = !source.GetComponent<AudioFlangerFilter>().enabled;
		image.sprite = (source.GetComponent<AudioFlangerFilter>().enabled ? InstrumentSetup.instance.percussionFilled : InstrumentSetup.instance.percussionEmpty);
	}

	public void updatedistortionLevel(Slider slider){
		AudioSource source = MusicManager.instance.instrumentAudioSources [InstrumentSetup.currentRiff.instrument];
		InstrumentSetup.currentRiff.distortionLevel = slider.value;
		source.gameObject.GetComponent<AudioDistortionFilter>().distortionLevel = InstrumentSetup.currentRiff.distortionLevel;
		//instrumentAudioSources[MelodicInstrument.ElectricGuitar].gameObject.GetComponent<AudioDistortionFilter>().distortionLevel = 0.9f;
	}

	public void updateechoDecayRatio(Slider slider){
		AudioSource source = MusicManager.instance.instrumentAudioSources [InstrumentSetup.currentRiff.instrument];
		InstrumentSetup.currentRiff.echoDecayRatio = slider.value;
		source.gameObject.GetComponent<AudioEchoFilter>().decayRatio = InstrumentSetup.currentRiff.echoDecayRatio;

	}


	public void updateechoDelay(Slider slider){
		AudioSource source = MusicManager.instance.instrumentAudioSources [InstrumentSetup.currentRiff.instrument];
		InstrumentSetup.currentRiff.echoDelay = slider.value;
		source.gameObject.GetComponent<AudioEchoFilter>().delay = InstrumentSetup.currentRiff.echoDelay;			
	}


	public void updateechoDryMix(Slider slider){
		AudioSource source = MusicManager.instance.instrumentAudioSources [InstrumentSetup.currentRiff.instrument];
		InstrumentSetup.currentRiff.echoDryMix = slider.value;
		source.gameObject.GetComponent<AudioEchoFilter>().dryMix = InstrumentSetup.currentRiff.echoDryMix;		
	}

	public void updatereverbDecayTime(Slider slider){
		AudioSource source = MusicManager.instance.instrumentAudioSources [InstrumentSetup.currentRiff.instrument];
		InstrumentSetup.currentRiff.reverbDecayTime = slider.value;
		source.gameObject.GetComponent<AudioReverbFilter>().decayTime = InstrumentSetup.currentRiff.reverbDecayTime;		
	}


	public void updatereverbLevel(Slider slider){
		AudioSource source = MusicManager.instance.instrumentAudioSources [InstrumentSetup.currentRiff.instrument];
		InstrumentSetup.currentRiff.reverbLevel = slider.value;
		source.gameObject.GetComponent<AudioReverbFilter>().reverbLevel = InstrumentSetup.currentRiff.reverbLevel;	
	}

	public void updatetremoloRate(Slider slider){
		AudioSource source = MusicManager.instance.instrumentAudioSources [InstrumentSetup.currentRiff.instrument];
		InstrumentSetup.currentRiff.tremoloRate = slider.value;		
		source.gameObject.GetComponent<AudioTremoloFilter>().rate = InstrumentSetup.currentRiff.tremoloRate;
	}

	public void updatetremoloDepth(Slider slider){
		AudioSource source = MusicManager.instance.instrumentAudioSources [InstrumentSetup.currentRiff.instrument];
		InstrumentSetup.currentRiff.tremoloDepth = slider.value;		
		source.gameObject.GetComponent<AudioTremoloFilter>().depth = InstrumentSetup.currentRiff.tremoloDepth;
	}

	public void updatechorusDryMix(Slider slider){
		AudioSource source = MusicManager.instance.instrumentAudioSources [InstrumentSetup.currentRiff.instrument];
		InstrumentSetup.currentRiff.chorusDryMix = slider.value;
		source.gameObject.GetComponent<AudioChorusFilter>().dryMix = InstrumentSetup.currentRiff.chorusDryMix;		
	}

	public void updatechorusRate(Slider slider){
		AudioSource source = MusicManager.instance.instrumentAudioSources [InstrumentSetup.currentRiff.instrument];
		InstrumentSetup.currentRiff.chorusRate = slider.value;
		source.gameObject.GetComponent<AudioChorusFilter>().rate = InstrumentSetup.currentRiff.chorusRate;
	}

	public void updatechorusDepth(Slider slider){
		AudioSource source = MusicManager.instance.instrumentAudioSources [InstrumentSetup.currentRiff.instrument];
		InstrumentSetup.currentRiff.chorusDepth = slider.value;
		source.gameObject.GetComponent<AudioChorusFilter>().depth = InstrumentSetup.currentRiff.chorusDepth;
	}

	public void updateflangerRate(Slider slider){
		AudioSource source = MusicManager.instance.instrumentAudioSources [InstrumentSetup.currentRiff.instrument];
		InstrumentSetup.currentRiff.flangerRate = Mathf.PI/32f + Mathf.PI/32f * slider.value;
		source.gameObject.GetComponent<AudioFlangerFilter>().rate = InstrumentSetup.currentRiff.flangerRate;
	}
		

	public void updateflangerDryMix(Slider slider){
		AudioSource source = MusicManager.instance.instrumentAudioSources [InstrumentSetup.currentRiff.instrument];
		InstrumentSetup.currentRiff.flangerDryMix = slider.value;
		source.gameObject.GetComponent<AudioFlangerFilter>().dryMix = InstrumentSetup.currentRiff.flangerDryMix;
	}
}

