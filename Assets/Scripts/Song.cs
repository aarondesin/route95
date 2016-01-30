using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Song {

	List<SongPiece> songPieces = new List<SongPiece>();
	//List<List<Riff>> compiledSong = new List<List<Riff>> ();

	public void PlaySong (int pos) {
		int totalMeasures = NumMeasures ();
		foreach (SongPiece songPiece in songPieces) {
			int measure = pos / 4;
		}
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

}
