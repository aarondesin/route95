using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;// need for using lists

public class Riff {
	
	
	
	List<Note> notes = new List<Note>(); // contains notes
	
	
	public bool pause = true; // if player is looping the riff or just want silent
	public MusicManager.Key currentKey = MusicManager.Key.EMajor;
	public static int drumRiffIndex = 0;
	
	public MusicManager.Key currentKey = MusicManager.Key.CMajor;
	
	void Sounds(Instrument currentInstrument, MusicManager.Key currentKey){
		switch (currentInstrument) {
		case Instrument.Drums:
			
			break;
			
		}
	}
	
	
	
	
	public void playriff(int pos){ // plays all the notes within the sequencer aka the riff 
		notes [drumRiffIndex].Play (pos);
		
	}
	
};