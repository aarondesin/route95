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
	public boolean play = true; // if note is playing at certain points
	public boolean pause = true; // if player is looping the riff or just want silent
				
	public static void Sounds(Key currentInstrument, Key currentKey){
		switch (currentInstrument) {
		case Drums:

			for (int i = 0; i < numSounds; i++) {
				notes<i> = soundList [0, i];
			}


			break;
		case Guitar:


			break;
		case Bass:
			
			break;
			
		}
	}
	
	public static void checkPlay(boolean select){
		if(select)

		}
}

