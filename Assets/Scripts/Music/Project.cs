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
			SaveLoad.SaveSong(song);
			string path = Application.persistentDataPath + GameManager.instance.songSaveFolder +
				song.name + SaveLoad.songSaveExtension;
			songPaths.Add (path);
		}
	}

	public void AddSong (Song song) {
		if (song == null) {
			Debug.LogError ("Project.AddSong(): tried to add null song!");
			return;
		}
		
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
			if (songs == null) songs = new List<Song>();
			songs.Add(song);

			if (songPieces == null) songPieces = new List<SongPiece>();
			if (song.songPieces != null) songPieces.AddRange (song.songPieces);

			if (measures == null) measures = new List<Measure>();
			if (song.measures != null) measures.AddRange (song.measures);

			if (riffs == null) riffs = new List<Riff>();
			if (song.riffs != null) riffs.AddRange (song.riffs);

			if (beats == null) beats = new List<Beat>();
			if (song.beats != null) beats.AddRange (song.beats);
		}
	}

	public void RemoveSong (int index) {
		//Song song = songs[index];
		songs.RemoveAt(index);
		if (MusicManager.instance.currentPlayingSong == index) {
			MusicManager.instance.currentPlayingSong = 0;
			if (songs.Count > 0) {
				MusicManager.instance.currentSong = songs[0];
			} else {
				MusicManager.instance.currentSong = null;
			}
		}
	}
		
	public bool Empty {
		get {
			return songs.Count == 0;
		}
	}

}
