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
		/*BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Open(Application.persistentDataPath + "/riffInfo.dat", FileMode.Open);

		riffdata data = new riffdata ();
		data.riff = InstrumentSetup.currentRiff;
		bf.Serialize (file, data);
		file.Close ();
		Debug.Log ("we saved hopefully");
*/
	}

	
	public void Loadriff(){

		if(File.Exists(Application.persistentDataPath + "/riffInfo.data")){
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open(Application.persistentDataPath + "/riffInfo.dat", FileMode.Open);
		
			riffdata data = (riffdata)bf.Deserialize (file);
			file.Close ();
			Debug.Log ("we load hopefully");
		}

	}

	public void SaveSong () {
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
		//return output;
	}
	
}

 [Serializable]
class riffdata{
	
	public Riff riff;
	public int beat;


}
