using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class MusicManager{
	public enum Key{
		CMajor,
		CSharp,
		DMajor,
		DSharp,
		EMajor

	};

	public Key currentkey;

	/*public static void Searchkey(Key currentkey){
		for(int i = 0; i <= 4; i++){
			if(currentkey == Key.i) // doesn't work but trying to go through index of Key
		}*/
	
	List<Riff>Riffs = new List<Riff>();
	List<List<Riff>>song = new List<List<Riff>>();


	

	public static float tempo = 120f;
	private float beattimer;

}