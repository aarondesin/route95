using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Project {
	public const int MAX_SONGS = 16;
	public const int MAX_RIFFS = 128;

	[SerializeField]
	public string name;

	[SerializeField]
	public List<Song> songs;

	[SerializeField]
	public List<Riff> riffs;

	public Project () {
		name = "New Project";
		songs = new List<Song>();
		riffs = new List<Riff>();
	}

	public void AddSong (Song song) {
		
	}

	/*public void RemoveSongAt (int i) {
		if (i > 0 && i < MAX_SONGS) {
			songs[i] = null;
			for (int j=i; j<MAX_SONGS-1; j++) {
				songs[j] = songs[j+1];
			}
			songs[MAX_SONGS-1] = null;
		}
	}*/

	/*public bool Full () {
		return songs[songs.Length-1] != null;
	}*/

	public bool Empty () {
		return songs.Count == 0;
	}

}
