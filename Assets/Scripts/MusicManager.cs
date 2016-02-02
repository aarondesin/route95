using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;// need for using lists
using System.IO; // need for path operations

public enum PercussionInstrument {
	Drums
};

public enum MelodicInstrument {
	ElectricGuitar
};

public enum Instrument {
	Drums,
	ElectricGuitar
};

public enum Key{
	DFlat,
	DMajor,
	EFlat,
	EMajor,
	FMajor

};


public class MusicManager : MonoBehaviour {
	
	public static Key currentKey = Key.EMajor;
	public static Instrument currentInstrument = Instrument.Drums;// value will be passed from key button
	List<Riff> riffs = new List<Riff>();

	public static Dictionary<string, AudioClip> Sounds = new Dictionary<string, AudioClip>();

	List<string> soundsToLoad = new List<string>() {
		"Audio/Instruments/Percussion/Kick",
		"Audio/Instruments/Percussion/Snare",
		"Audio/Instruments/Percussion/Tom",
		"Audio/Instruments/Percussion/Hat",
	};

	public void LoadAllAudioClips (List<string> paths) {
		foreach (string path in paths) {
			LoadAudioClip  (path);
		}
	}

	static void LoadAudioClip (string path) {
		AudioClip sound = (AudioClip) Resources.Load (path);
		if (sound == null) {
			Debug.LogError("Failed to load AudioClip at "+path);
		} else {
			Debug.Log("Loaded AudioClip at "+path);
			Sounds.Add (Path.GetFileNameWithoutExtension (path), sound);
		}
	}

	public void AddRiff () {
		Riff temp = new Riff ();
		InstrumentSetup.currentRiff = temp;
		riffs.Add (temp);
	}

	void Start () {
		LoadAllAudioClips (soundsToLoad);
	}
}
	
	
	/*public void Play (int pos) {
	//foreach (MusicManager.Key hit in riff[pos]) {
	//MusicManager.PlayInstrument (hit);
	//}
}*/
	
	/*public static void PlayInstrument (Riffs hit) {
		switch (hit) {
		case Percussion.Kick:
			kick.Play();
			break;
		case Percussion.Tom:
			tom.Play();
			break;
		case Percussion.Snare:
			snare.Play();
			break;
		case Percussion.Hat:
			hat.Play();
			break;
		}
	}*/
	// No newline at end of file