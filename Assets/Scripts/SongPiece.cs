using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SongPiece {

	public static int DEFAULT_MEASURES = 1;

	[SerializeField]
	public string name; 

	[SerializeField]
	public List<Measure> measures = new List<Measure> ();
	//public int measures;

	// Default constructor makes an empty 1-measure SongPiece
	public SongPiece () {
		//name = MusicManager.instance.currentProject.songPieces.Count.ToString();
		measures = new List<Measure> () {
			new Measure()
		};
		//measures = DEFAULT_MEASURES;
		//MusicManager.instance.AddSongPiece(this);
		//MusicManager.instance.AddSongPieceToSong(this);
	}

	/*public SongPiece (string newName) {
		riffs = new List<List<Riff>> () {
			new List<Riff>()
		};
		measures = 1;
		name = newName;
	}*/

	public SongPiece (string loadString) {
		//string[] vars = loadString.Split(new char[]{save_load.itemSeparator, save_load.noteSeparator});
		string [] vars = loadString.Split (SaveLoad.noteSeparator);

		name = vars[0];
		Debug.Log("SongPiece.name = "+name);

		measures = new List<Measure> () {
			new Measure ()
		};

		for (int i=1; i<vars.Length; i++) {
			if (vars[i].Length == 0) continue;

			//if (int
			Riff riff = MusicManager.instance.RiffByString(vars[i]);
			if (riff == null) {
				Debug.LogError ("SongPiece(): riff \"" + vars[i] +"\" invalid.");
			} else {
				measures[0].riffs.Add(riff);
			}
		}
		//measures = DEFAULT_MEASURES;
	}

	//public void Toggle (

	public void PlaySongPiece (int pos){ // plays all the notes at pos
		int measureNum = pos/4;
		foreach (Riff riff in measures[measureNum].riffs) {
			riff.PlayRiff (pos % 4);
		}
	}

	public override string ToString () {
		string result = "";
		//result += name + save_load.itemSeparator;
		result += name + SaveLoad.noteSeparator;
		/*for (int i = 0; i < riffs.Count; i++) {
			result += i.ToString() + save_load.itemSeparator;
			foreach (Riff riff in riffs[i]) {
				result += riff.name + save_load.noteSeparator;
			}

		}*/
		foreach (Riff riff in measures[0].riffs) {
			result += riff.name + SaveLoad.noteSeparator;
		}
		// measures
		return result;
	}

}
