using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SongPiece {

	const int DEFAULT_MEASURES = 1;

	[SerializeField]
	public string name; 

	[SerializeField]
	public int index;

	[SerializeField]
	public List<int> measureIndices = new List<int>();

	// Default constructor makes an empty 1-measure SongPiece
	public SongPiece () {
		measureIndices = new List<int> ();
	}

	//public void Toggle (

	public void PlaySongPiece (int pos){ // plays all the notes at pos
		int measureNum = pos/4;
		Song song = MusicManager.instance.currentSong;
		Measure measure = song.measures[measureNum];
		foreach (int r in measure.riffIndices) {
			Riff riff = song.riffs[r];
			riff.PlayRiff (pos % 4);
		}
	}

}
