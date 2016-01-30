using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SongPiece {
	
	string name;
	List<List<Riff>> riffs = new List<List<Riff>> ();
	public int measures;

	public void PlaySongPiece (int pos){ // plays all the notes at pos
		int measure = pos/4;
		foreach (Riff riff in riffs[measure]) {
			riff.PlayRiff (pos % 4);
		}
	}
		

}
