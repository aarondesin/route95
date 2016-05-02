using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Audio;

public class EffectButton : MonoBehaviour {

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

	public void updatefuzzLevel(Slider slider){ // getting rid later
		AudioSource source = MusicManager.instance.instrumentAudioSources [InstrumentSetup.currentRiff.instrument];
		InstrumentSetup.currentRiff.distortionLevel = slider.value;
		source.gameObject.GetComponent<AudioDistortionFilter>().distortionLevel = InstrumentSetup.currentRiff.distortionLevel;
	}

	public void updatetremoloRate(Slider slider){

		//InstrumentSetup.currentRiff.tremoloRate = Slider.value;			
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

	public void updateflangerDelay(Slider slider){

		//InstrumentSetup.currentRiff.flangerDelay = Slider.value;			
	}
		

	public void updateflangerDryMix(Slider slider){

		//InstrumentSetup.currentRiff.flangerDryMix = Slider.value;			
	}
}

