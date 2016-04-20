using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Song {

	[SerializeField]
	public string name;

	[SerializeField]
	public Key key = Key.None;

	[SerializeField]
	public int scale = -1; // scale index

	[SerializeField]
	public List<SongPiece> songPieces = new List<SongPiece>();

	public int measures;
	public int beats;

	// Default constructor creates a song of 4 1-measure song pieces
	public Song () {
		name = "New Song";
		songPieces = new List<SongPiece>() {
			new SongPiece() { name = "SongPiece1" },
			new SongPiece() { name = "SongPiece2" },
			new SongPiece() { name = "SongPiece3" },
			new SongPiece() { name = "SongPiece4" }
		};
	}
		
	public void PlaySong (int pos) {
		try {
			// For now, all song pieces are assumed to be one measure long
			foreach (Riff riff in songPieces[pos/(int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS+2)].measures[0].riffs) {
				riff.PlayRiff(pos%(int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS+2));
			}
		} catch (ArgumentOutOfRangeException) {
			Debug.LogError ("Song.PlaySong(): index out of range! "+pos);
		}
	}

	public void PlaySongExceptFor (int pos, Instrument instrument) {
		try {
			// For now, all song pieces are assumed to be one measure long
			foreach (Riff riff in songPieces[pos/(int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS+2)].measures[0].riffs) {
				if (riff.instrument != instrument) riff.PlayRiff(pos%(int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS+2));
			}
		} catch (ArgumentOutOfRangeException) {
			Debug.LogError ("Song.PlaySong(): index out of range! "+pos);
		}
	}

	public void RemoveAt (int pos, Instrument inst) {
		foreach (Riff riff in songPieces[pos/(int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS+2)].measures[0].riffs) {
			if (riff.instrument == inst) {
				riff.beats[pos%(int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS+2)].Clear();
			}
		}
	}

	public void ToggleRiff (Riff newRiff, int pos) {
		foreach (Riff riff in songPieces[pos].measures[0].riffs) {
			if (riff.instrument == newRiff.instrument) {
				if (newRiff == riff) {
					// Riff is already there
					songPieces[pos].measures[0].riffs.Remove (newRiff);
				} else {
					songPieces[pos].measures[0].riffs.Remove (riff);
					songPieces[pos].measures[0].riffs.Add (newRiff);
				}
				return;
			}
		}
		// Riff not already there
		songPieces[pos].measures[0].riffs.Add (newRiff);
	}
		
	// Iterates through whole song and and adds number of measures in song pieces
	public int NumMeasures () {
		int temp = 0;
		foreach (SongPiece songPiece in songPieces) {
			temp += songPiece.measures.Count;
		}
		return temp;
	}

	public void CompileSong() {
		measures = NumMeasures();
		beats = measures*(int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS+2);
		Debug.Log("Song.CompileSong(): beats: "+beats+ " measures: "+measures);
	}

	public void AddNewSongPiece() {
		songPieces.Add(new SongPiece());
	}
		
}
