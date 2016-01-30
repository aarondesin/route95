using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Riff {

	public enum Instrument{
		Drums,
		Guitar,
		Bass
		
	};

	List<Note> notes = new List<Note>(); // contains notes
	 
	public bool pause = true; // if player is looping the riff or just want silent
	public MusicManager.Key currentKey = MusicManager.Key.EMajor;
	public static int drumRiffIndex = 0;

	void Sounds(Instrument currentInstrument, MusicManager.Key currentKey){
		switch (currentInstrument) {
		case Instrument.Drums:

			for (int i = 0; i < Note.numSounds; i++) {
				//notes.Add(Note.soundList [0, i]);

			}


			break;
		case Instrument.Guitar:


			break;
		case Instrument.Bass:
			
			break;
			
		}
	}



	public void playriff(int pos){ // plays all the notes within the sequencer aka the riff 
		notes[drumRiffIndex].Play(pos);
	
	}

	
	/*public void Play (int pos) {
		foreach (Instrument hit in riff[pos]) {
			MusicManager.PlayPercussion (hit);
		}
	}*/
	
	public static void checkPlay(bool select){
		if (select) {

		}
	}
}

