using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

[System.Serializable]
public class Project {
	const int MAX_SONGS = 16;
	const int MAX_RIFFS = 128;

	[SerializeField]
	public string name;

	[SerializeField]
	public List<string> songPaths;

	[NonSerialized]
	public List<Song> songs;

	[NonSerialized]
	private List<SongPiece> songPieces;

	[NonSerialized]
	private List<Measure> measures;

	[NonSerialized]
	private List<Riff> riffs;

	[NonSerialized]
	private List<Beat> beats;

	public Project () {
		name = "New Project";
		songPaths = new List<string>();
		songs = new List<Song>();
		songPieces = new List<SongPiece>();
		measures = new List<Measure>();
		riffs = new List<Riff>();
		beats = new List<Beat>();
	}

	[OnDeserialized()]
	public void Refresh (StreamingContext context) {
		Debug.Log("Project.Refresh()");
		if (songPaths == null) songPaths = new List<string>();
		if (songs == null) songs = new List<Song>();
		if (songPieces == null) songPieces = new List<SongPiece>();
		if (measures == null) measures = new List<Measure>();
		if (riffs == null) riffs = new List<Riff>();
		if (beats == null) beats = new List<Beat>();
		foreach (string path in songPaths) AddSong(SaveLoad.LoadSong(path));
	}

	[OnSerializing()]
	internal void UpdatePaths (StreamingContext context) {
		Debug.Log("Project.UpdatePaths");
		songPaths.Clear();
		foreach (Song song in songs) {
			string path = Application.persistentDataPath + GameManager.instance.songSaveFolder +
				song.name + SaveLoad.songSaveExtension;
			songPaths.Add (path);
		}
	}

	public void AddSong (Song song) {
		Song foundSong = null;
		foreach (Song s in songs) {
			if (song.Equals (s)) {
				foundSong = s;
				break;
			}
		}
		if (foundSong != null) {
			songs.Add(foundSong);
		} else {
			songs.Add(song);
			songPieces.AddRange (song.songPieces);
			measures.AddRange (song.measures);
			riffs.AddRange (song.riffs);
			beats.AddRange (song.beats);
		}
	}

	public bool Empty {
		get {
			return songPieces.Count != 0 && measures.Count != 0 &&
				riffs.Count != 0 && beats.Count != 0;
		}
	}

}
