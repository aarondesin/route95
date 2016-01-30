
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;// need for using lists

public class MusicManager : MonoBehaviour {
	
	public enum Key{
		DFlat,
		DMajor,
		EFlat,
		EMajor,
		FMajor
		
	};
	
	
	
	public Key currentKey = Key.EMajor;
	public Riff.Instrument currentInstrument = Riff.Instrument.Drums;// value will be passed from key button
	
	List<List<Note>> notes = new List<List<Note>>();
	
	
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
	\ No newline at end of file