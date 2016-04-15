using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Song {

	[SerializeField]
	public string name = "New Song";

	[SerializeField]
	public Key key;

	[SerializeField]
	public List<SongPiece> songPieces = new List<SongPiece>();

	public int measures;
	public int beats;

	// Default constructor creates a song of 4 1-measure song pieces
	public Song () {
		songPieces = new List<SongPiece>() {
			new SongPiece() { name = "SongPiece1" },
			new SongPiece() { name = "SongPiece2" },
			new SongPiece() { name = "SongPiece3" },
			new SongPiece() { name = "SongPiece4" }
		};
	}

	/*public Song (string loadFile) {
		if (loadFile.Length == 0) {
			//Debug.LogError("Failed to load song.");
			throw new FailedToLoadException("Song() given an empty string.");
		}

		try {
			string[] vars = loadFile.Split(SaveLoad.itemSeparator);
			name = vars[0];
			Debug.Log(vars[1]);
			Key key = (Key)Enum.Parse(typeof (Key), vars[1]);
			MusicManager.instance.SetKey( key );
			MusicManager.instance.tempo = (Tempo)Enum.Parse(typeof (Tempo), vars[2]);
			string[] pieces = vars[3].Split(SaveLoad.noteSeparator);
			foreach (string songPiece in pieces) {
				if (songPiece.Length == 0) continue;
				Debug.Log("Finding songpiece "+songPiece+".");
				songPieces.Add(MusicManager.instance.songPiecesByName[songPiece]);
			}
		} catch (IndexOutOfRangeException) {
			//Debug.LogError("Failed to load song.");
			throw new FailedToLoadException("Song() given an invalid input.");
		} //catch (KeyNotFoundException) {
			//throw new FailedToLoadException("Song.Song(): unable to find songpiece");
		//}
	}*/

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

	// Combine all song pieces into one 2-dimensional list
	//public void Compile () {
	//	foreach (List<Riff> riffs in 

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

	/*public override string ToString () {
		string result = "";
		result += name + SaveLoad.itemSeparator;
		result += Enum.GetName(typeof (Key), MusicManager.instance.currentKey) + save_load.itemSeparator;
		result += Enum.GetName(typeof (Tempo), MusicManager.instance.tempo) + save_load.itemSeparator;
		foreach (SongPiece songPiece in songPieces) {
			result += songPiece.name + save_load.noteSeparator;
		}
		return result;
	}*/
}
