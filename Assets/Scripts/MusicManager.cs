
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;// need for using lists

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

	public void AddRiff () {
		Riff temp = new Riff ();
		InstrumentSetup.currentRiff = temp;
		riffs.Add (temp);
	}

	void Start () {
		Sounds.Load ();
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