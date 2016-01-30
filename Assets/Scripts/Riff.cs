using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Riff {

	public enum Instrument{
		Drums,
		Guitar,
		Bass
		
	};

	List<Note> notes = new List<Note>();
	 
	public bool pause = true; // if player is looping the riff or just want silent
	public MusicManager.Key currentKey = MusicManager.Key.CMajor;
				
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



	public void playriff(){
	
	}
	
	public static void checkPlay(bool select){
		if (select) {

		}
	}
}

