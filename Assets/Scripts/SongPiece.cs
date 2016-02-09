using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SongPiece {
	
	public string name;
	public List<List<Riff>> riffs = new List<List<Riff>> ();
	public int measures;

	// Default constructor makes an empty 1-measure SongPiece
	public SongPiece () {
		riffs = new List<List<Riff>> () {
			new List<Riff>()
		};
		measures = 1;
	}

	//public void Toggle (

	public void PlaySongPiece (int pos){ // plays all the notes at pos
		int measure = pos/4;
		foreach (Riff riff in riffs[measure]) {
			riff.PlayRiff (pos % 4);
		}
	}
		

}
