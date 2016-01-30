using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;// need for using lists

public class MusicManager : MonoBehaviour {
	public enum Key{
		DFlat,
		DMajor,
		EMajor,
		EFlat,
		FMajor

	};



	public Key currentKey = Key.EMajor;
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


