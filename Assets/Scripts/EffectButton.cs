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

	/*public void updateechoDecayRatio(Slider slider){

		InstrumentSetup.currentRiff.echoDecayRatio = Slider.value;			
	}


	public void echoDelay(Slider slider){

		InstrumentSetup.currentRiff.echoDelay = Slider.value;			
	}


	public void updateechoDryMix(Slider slider){

		InstrumentSetup.currentRiff.echoDryMix = Slider.value;			
	}

	public void updatereverbDecayTime(Slider slider){

		InstrumentSetup.currentRiff.reverbDecayTime = Slider.value;			
	}


	public void updatereverbLevel(Slider slider){

		InstrumentSetup.currentRiff.reverbLevel = Slider.value;			
	}

	public void updatefuzzLevel(Slider slider){

		InstrumentSetup.currentRiff.fuzzLevel = Slider.value;			
	}

	public void updatetremoloRate(Slider slider){

		InstrumentSetup.currentRiff.tremoloRate = Slider.value;			
	}

	public void updatechorusDryMix(Slider slider){

		InstrumentSetup.currentRiff.chorusDryMix = Slider.value;			
	}

	public void updatechorusRate(Slider slider){

		InstrumentSetup.currentRiff.chorusRate = Slider.value;			
	}

	public void updatechorusDepth(Slider slider){

		InstrumentSetup.currentRiff.chorusDepth = Slider.value;			
	}

	public void updateflangerDelay(Slider slider){

		InstrumentSetup.currentRiff.flangerDelay = Slider.value;			
	}
		

	public void updateflangerDryMix(Slider slider){

		InstrumentSetup.currentRiff.flangerDryMix = Slider.value;			
	}*/
}

