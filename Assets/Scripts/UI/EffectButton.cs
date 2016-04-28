using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Audio;

public class EffectButton : MonoBehaviour {

	public void updatedistortionLevel(Slider slider){
		AudioSource source = MusicManager.instance.instrumentAudioSources [InstrumentSetup.instance.currentRiff.instrument];
		InstrumentSetup.instance.currentRiff.distortionLevel = slider.value;
		source.gameObject.GetComponent<AudioDistortionFilter>().distortionLevel = InstrumentSetup.instance.currentRiff.distortionLevel;
		//instrumentAudioSources[MelodicInstrument.ElectricGuitar].gameObject.GetComponent<AudioDistortionFilter>().distortionLevel = 0.9f;
	}

	public void updateechoDecayRatio(Slider slider){
		AudioSource source = MusicManager.instance.instrumentAudioSources [InstrumentSetup.instance.currentRiff.instrument];
		InstrumentSetup.instance.currentRiff.echoDecayRatio = 1-slider.value;
		source.gameObject.GetComponent<AudioEchoFilter>().decayRatio = InstrumentSetup.instance.currentRiff.echoDecayRatio;
	}


	public void updateechoDelay(Slider slider){
		AudioSource source = MusicManager.instance.instrumentAudioSources [InstrumentSetup.instance.currentRiff.instrument];
		InstrumentSetup.instance.currentRiff.echoDelay = slider.value;
		source.gameObject.GetComponent<AudioEchoFilter>().delay = InstrumentSetup.instance.currentRiff.echoDelay;			
	}


	public void updateechoDryMix(Slider slider){
		AudioSource source = MusicManager.instance.instrumentAudioSources [InstrumentSetup.instance.currentRiff.instrument];
		InstrumentSetup.instance.currentRiff.echoDryMix = slider.value;
		source.gameObject.GetComponent<AudioEchoFilter>().dryMix = InstrumentSetup.instance.currentRiff.echoDryMix;		
	}

	public void updatereverbDecayTime(Slider slider){
		AudioSource source = MusicManager.instance.instrumentAudioSources [InstrumentSetup.instance.currentRiff.instrument];
		InstrumentSetup.instance.currentRiff.reverbDecayTime = slider.value;
		source.gameObject.GetComponent<AudioReverbFilter>().decayTime = InstrumentSetup.instance.currentRiff.reverbDecayTime;		
	}


	public void updatereverbLevel(Slider slider){
		AudioSource source = MusicManager.instance.instrumentAudioSources [InstrumentSetup.instance.currentRiff.instrument];
		InstrumentSetup.instance.currentRiff.reverbLevel = slider.value;
		source.gameObject.GetComponent<AudioReverbFilter>().reverbLevel = InstrumentSetup.instance.currentRiff.reverbLevel;	
	}

	public void updatefuzzLevel(Slider slider){ // getting rid later
		AudioSource source = MusicManager.instance.instrumentAudioSources [InstrumentSetup.instance.currentRiff.instrument];
		InstrumentSetup.instance.currentRiff.distortionLevel = slider.value;
		source.gameObject.GetComponent<AudioDistortionFilter>().distortionLevel = InstrumentSetup.instance.currentRiff.distortionLevel;
	}

	public void updatetremoloRate(Slider slider){

		//InstrumentSetup.instance.currentRiff.tremoloRate = Slider.value;			
	}

	public void updatechorusDryMix(Slider slider){
		AudioSource source = MusicManager.instance.instrumentAudioSources [InstrumentSetup.instance.currentRiff.instrument];
		InstrumentSetup.instance.currentRiff.chorusDryMix = slider.value;
		source.gameObject.GetComponent<AudioChorusFilter>().dryMix = InstrumentSetup.instance.currentRiff.chorusDryMix;		
	}

	public void updatechorusRate(Slider slider){
		AudioSource source = MusicManager.instance.instrumentAudioSources [InstrumentSetup.instance.currentRiff.instrument];
		InstrumentSetup.instance.currentRiff.chorusRate = slider.value;
		source.gameObject.GetComponent<AudioChorusFilter>().rate = InstrumentSetup.instance.currentRiff.chorusRate;
	}

	public void updatechorusDepth(Slider slider){
		AudioSource source = MusicManager.instance.instrumentAudioSources [InstrumentSetup.instance.currentRiff.instrument];
		InstrumentSetup.instance.currentRiff.chorusDepth = slider.value;
		source.gameObject.GetComponent<AudioChorusFilter>().depth = InstrumentSetup.instance.currentRiff.chorusDepth;
	}

	public void updateflangerDelay(Slider slider){

		//InstrumentSetup.instance.currentRiff.flangerDelay = Slider.value;			
	}
		

	public void updateflangerDryMix(Slider slider){

		//InstrumentSetup.instance.currentRiff.flangerDryMix = Slider.value;			
	}
}

