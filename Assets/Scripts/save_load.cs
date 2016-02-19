using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class save_load : MonoBehaviour {
	char saveSeparator = '$';
	char itemSeparator = '#';

	public void Saveriff(){
		Debug.Log (SaveSong ());
		BinaryFormatter bf = new BinaryFormatter ();

		//string path = Application.persistentDataPath + "/" + InstrumentSetup.currentRiff.name;
		string directoryPath = Application.persistentDataPath + "/";
		string filePath = directoryPath + MusicManager.instance.currentSong.name + ".dat";
		Debug.Log (directoryPath);
		Debug.Log (filePath);
		Directory.CreateDirectory (directoryPath);
		FileStream file = File.Open(filePath, FileMode.Create);
		//string test = SaveToString ();
		//Debug.Log (test);

		riffdata data = new riffdata ();
		//data.riff = InstrumentSetup.currentRiff;
		//data.riffName = InstrumentSetup.currentRiff.name;
		//data.pos = InstrumentSetup.currentRiff.intPos;
		data.songData = SaveSong();
		bf.Serialize (file, data);
		//Debug.Log ("Note Name: " + data.noteName);
		//Debug.Log ("Riff Name: " + data.riffName);
		//Debug.Log ("List of Positions: " + data.pos);
		//Debug.Log ("Beat: " + data.beat);
		file.Close ();
		Debug.Log ("we saved hopefully");

	}

	
	/*public void Loadriff(){
		string path = Application.persistentDataPath + "/" + InstrumentSetup.currentRiff.name;
		Debug.Log ("load is being called");
		Debug.Log (path);
		if (File.Exists (path + "/riffInfo.data")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (path + "/riffInfo.dat", FileMode.Open);

			riffdata data = (riffdata)bf.Deserialize (file);
			Debug.Log ("Note Name: " + data.noteName);
			Debug.Log ("Riff Name: " + data.riffName);
			Debug.Log ("List of Positions: " + data.pos);
			Debug.Log ("Beat: " + data.beat);
			file.Close ();
			Debug.Log ("we load hopefully");
		} else
			Debug.Log ("file doesn't exist");

	}*/

	public string SaveSong () {
		string output = "";

		// save all riffs
		foreach (Riff riff in MusicManager.instance.riffs) {
			output += riff.ToString () + itemSeparator;
		}

		// separate riff/songpiece saving
		output += saveSeparator;

		// save all song pieces
		foreach (SongPiece songPiece in MusicManager.instance.currentSong.songPieces) {
			output += songPiece.ToString () + itemSeparator;
		}

		// separate songpiece/song saving
		output += saveSeparator;

		// save song
		/*foreach (SongPiece songPiece in MusicManager.instance.currentSong.songPieces) {
			output += songPiece.name + "#";
		}*/
		output += MusicManager.instance.currentSong.ToString ();

		Debug.Log("SaveString():" +output);
		return output;
	}
	
}

 [Serializable]
class riffdata{
	
	//public Riff riff;
	//public int beat;
	public string songData;

}
