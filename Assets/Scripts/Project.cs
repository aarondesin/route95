using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Project {
	public string name;
	public List<Song> songs;
	public List<SongPiece> songPieces;
	public List<Riff> riffs;

	public void AddNewSongPiece() {
		songPieces.Add(new SongPiece());
	}
}
