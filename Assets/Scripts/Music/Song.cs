using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

[System.Serializable]
public class Song {

	// 
	// SERIALIZED VARIABLES
	// These will be saved in the project/song save file
	//

	[SerializeField]
	public string name;

	[SerializeField]
	public Key key = Key.None;

	[SerializeField]
	public int scale = -1; // scale index

	[SerializeField]
	public List<SongPiece> songPieces;

	[SerializeField]
	public List<Measure> measures;

	[SerializeField]
	public List<Riff> riffs;

	[SerializeField]
	public List<Beat> beats;

	[SerializeField]
	public List<int> songPieceIndices;

	//
	// NONSERIALIZED VARIABLES
	// These will not be saved
	//

	// Default constructor creates a song of 4 1-measure song pieces
	public Song () {
		name = "New Song";
		songPieceIndices = new List<int>();
		songPieces = new List<SongPiece>();
		measures = new List<Measure>();
		riffs = new List<Riff>();
		beats = new List<Beat>();
	}

	[OnDeserialized()]
	internal void Refresh (StreamingContext context) {
		if (songPieceIndices == null) songPieceIndices = new List<int>();
		if (songPieces == null) songPieces = new List<SongPiece>();
		if (measures == null) measures = new List<Measure>();
		if (riffs == null) riffs = new List<Riff>();
		if (beats == null) beats = new List<Beat>();
		foreach (Riff riff in riffs) {
			riff.instrument = Instrument.AllInstruments[riff.instrumentIndex];
		}
	}

	public int Beats {
		get {
			return songPieceIndices.Count * 16;
		}
	}

	public int Measures {
		get {
			return songPieceIndices.Count;
		}
	}

	public bool Equals (Song other) {
		return name == other.name;
	}

	public void RegisterSongPiece (SongPiece songPiece) {
		songPiece.index = songPieces.Count;
		songPieces.Add (songPiece);
	}

	public void RegisterRiff (Riff riff) {
		riff.index = riffs.Count;
		riffs.Add (riff);
		for (int i=0; i<16; i++) {
			Beat beat = new Beat ();
			beat.index = beats.Count;
			riff.beatIndices.Add (beat.index);
			beats.Add (beat);
		}
	}

	public SongPiece NewSongPiece () {
		SongPiece songPiece = new SongPiece();
		songPiece.index = songPieces.Count;
		Measure measure = NewMeasure();
		songPiece.measureIndices.Add (measure.index);
		songPieces.Add(songPiece);
		songPieceIndices.Add (songPiece.index);
		return songPiece;
	}

	public Measure NewMeasure () {
		Measure measure = new Measure();
		measure.index = measures.Count;
		measures.Add(measure);
		return measure;
	}
		
	public void PlaySong (int pos) {
		try {
			SongPiece songPiece = songPieces[songPieceIndices[pos/(int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS+2)]];
			Measure measure = measures[songPiece.measureIndices[0]];
			// For now, all song pieces are assumed to be one measure long
			foreach (int i in measure.riffIndices) {
				riffs[i].PlayRiff(pos%(int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS+2));
			}
		} catch (ArgumentOutOfRangeException) {
			Debug.LogError ("Song.PlaySong(): index out of range! "+pos);
		}
	}

	public void ToggleRiff (Riff newRiff, int pos) {
		SongPiece songPiece = songPieces[songPieceIndices[pos]];
		Measure measure = measures[songPiece.measureIndices[0]];
		foreach (int r in measure.riffIndices) {
			Riff riff = riffs[r];
			if (riff.instrument == newRiff.instrument) {
				if (newRiff == riff) {
					// Riff is already there
					measure.riffIndices.Remove (newRiff.index);
				} else {
					measure.riffIndices.Remove (riff.index);
					measure.riffIndices.Add (newRiff.index);
				}
				return;
			}
		}
		// Riff not already there
		measure.riffIndices.Add (newRiff.index);
	}

	public void CompileSong() {
		Debug.Log("Song.CompileSong(): beats: "+Beats+ " measures: "+Measures);
	}

}
