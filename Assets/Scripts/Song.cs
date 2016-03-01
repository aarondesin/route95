using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Song {

	public string name = "New Song";
	public List<SongPiece> songPieces = new List<SongPiece>();
	//List<List<Riff>> compiledSong = new List<List<Riff>> ();
	public int measures;
	public int beats;

	// Default constructor creates a song of 4 1-measure song pieces
	public Song () {
		songPieces = new List<SongPiece>() {
			new SongPiece("SongPieceOne"), new SongPiece("SongPieceTwo"), new SongPiece("SongPieceThree"), new SongPiece("SongPieceFour")
		};
	}

	public void PlaySong (int pos) {
		/*int totalMeasures = NumMeasures ();
		foreach (SongPiece songPiece in songPieces) {
			int measure = pos / 4;
		}*/
		// For now, all song pieces are assumed to be one measure long
		foreach (Riff riff in songPieces[pos/(int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS+2)].riffs[0]) {
			riff.PlayRiff(pos%(int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS+2));
		}
	}

	public void ToggleRiff (Riff newRiff, int pos) {
		foreach (Riff riff in songPieces[pos].riffs[0]) {
			if (newRiff == riff) {
				// Riff is already there
				songPieces[pos].riffs[0].Remove (newRiff);
				return;
			}
		}
		// Riff not already there
		songPieces[pos].riffs[0].Add (newRiff);
	}

	// Combine all song pieces into one 2-dimensional list
	//public void Compile () {
	//	foreach (List<Riff> riffs in 

	// Iterates through whole song and and adds number of measures in song pieces
	public int NumMeasures () {
		int temp = 0;
		foreach (SongPiece songPiece in songPieces) {
			temp += songPiece.measures;
		}
		return temp;
	}

	public void CompileSong() {
		measures = NumMeasures();
		beats = measures*(int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS+2);
	}

	public override string ToString () {
		string result = "";
		result += name + "#";
		foreach (SongPiece songPiece in songPieces) {
			result += songPiece.name + ",";
		}
		return result;
	}
}
