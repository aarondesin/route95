using UnityEditor;
using System.Collections;
using System.Collections.Generic;// need for using lists

public class MusicManager{
	public enum Key{
		CMajor,
		CSharp,
		DMajor,
		DSharp,
		EMajor

	};

	public class MusicRiff {
		
		public static int MIN_NUMSUBDIVS = 1;
		public static int DEFAULT_NUMSUBDIVS = 1;
		public static int MAX_NUMSUBDIVS = 3;
		
		public static int MIN_NUMBARS = 1;
		public static int DEFAULT_NUMBARS = 1;
		public static int MAX_NUMBARS = 1;
		
		
		public int subdivs; // 1: 4/4, 2: 8/8, 3: 16/16
		public int bars; // number of bars
		
	}

	public Key currentKey = Key.CMajor;
	public Riff.Instrument currentInstrument = Riff.Instrument.Drums;// value will be passed from key button

	/*public static void Searchkey(Key currentkey){
		for(int i = 0; i <= 4; i++){
			if(currentkey == Key.i) // doesn't work but trying to go through index of Key
		}*/
	
	List<Riff>Riffs = new List<Riff>();// stores individal riffs 
	List<List<Riff>>songpiece = new List<List<Riff>>();// stores the song pieces\

	public static float tempo = 120f;
	private float beattimer;


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