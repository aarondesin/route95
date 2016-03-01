﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SongPiece {

	public static int DEFAULT_MEASURES = 1;
	
	public string name; 
	public List<List<Riff>> riffs = new List<List<Riff>> ();
	public int measures;

	// Default constructor makes an empty 1-measure SongPiece
	public SongPiece () {
		riffs = new List<List<Riff>> () {
			new List<Riff>()
		};
		measures = DEFAULT_MEASURES;
	}

	/*public SongPiece (string newName) {
		riffs = new List<List<Riff>> () {
			new List<Riff>()
		};
		measures = 1;
		name = newName;
	}*/

	public SongPiece (string loadString) {
		string[] vars = loadString.Split(new char[]{save_load.itemSeparator, save_load.noteSeparator});

		name = vars[0];
		Debug.Log("SongPiece.name = "+name);

		for (int i=1; i<vars.Length; i++) {
			if (vars[i].Length == 0) break;
			Riff riff = MusicManager.instance.RiffByString(vars[i]);
			if (riff == null) {
				Debug.LogError ("SongPiece(): riff \"" + vars[i] +"\" invalid.");
			} else {
				riffs[0].Add(riff);
			}
		}
		measures = DEFAULT_MEASURES;
	}

	//public void Toggle (

	public void PlaySongPiece (int pos){ // plays all the notes at pos
		int measure = pos/4;
		foreach (Riff riff in riffs[measure]) {
			riff.PlayRiff (pos % 4);
		}
	}

	public override string ToString () {
		string result = "";
		result += name + save_load.itemSeparator;
		for (int i = 0; i < riffs.Count; i++) {
			result += i.ToString() + save_load.itemSeparator;
			foreach (Riff riff in riffs[i]) {
				result += riff.name + save_load.noteSeparator;
			}

		}
		// measures
		return result;
	}

}
