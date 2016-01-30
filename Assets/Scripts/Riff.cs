@@ -1,21 +1,20 @@
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;// need for using lists

public class MusicManager : MonoBehaviour {
public class MusicManager{
	public enum Key{
		DFlat,
		DMajor,
		EFlat,
		Emajor,
		FMajor

	};



	public Key currentKey = Key.EMajor;
	public Riff.Instrument currentInstrument = Riff.Instrument.Drums;// value will be passed from key button
	
	List<List<Note>> notes = new List<List<Note>>();

	
	/*public static void Searchkey(Key currentkey){
@@ -33,3 +32,25 @@ public class MusicManager : MonoBehaviour {
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
\ No newline at end of file