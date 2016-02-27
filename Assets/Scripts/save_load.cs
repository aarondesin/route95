﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

public class save_load : MonoBehaviour {
	public static char saveSeparator = '$';
	public static char itemSeparator = '@';
	public static char riffSeparator = '%';
	public static char noteSeparator = ',';
	//char[] separators = { '$', ',', '#', '%' };

	public static string saveExtension = ".r95";

	public void Saveriff(){
		Debug.Log (SaveSong ());
		BinaryFormatter bf = new BinaryFormatter ();

		//string path = Application.persistentDataPath + "/" + InstrumentSetup.currentRiff.name;
		string directoryPath = Application.persistentDataPath + "/";
		string filePath = directoryPath + MusicManager.instance.currentSong.name + saveExtension;
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

		file.Close ();
		Debug.Log ("we saved hopefully");
		Prompt.instance.PromptMessage("Save Project", "Successfully saved project!", "Okay");
	}

	
	public void Loadriff(){
		string directoryPath = Application.persistentDataPath + "/";
		//string filePath = directoryPath + MusicManager.instance.currentSong.name + ".dat";
		string filePath = directoryPath + "New Song.dat";
		Debug.Log (directoryPath);
		if (File.Exists (filePath)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (filePath, FileMode.Open);

			riffdata data = (riffdata)bf.Deserialize (file);
			Debug.Log (data.songData);
			file.Close ();
			//Debug.Log ("we load hopefully");
			string[] words = data.songData.Split('$');
			LoadRiffs (words [0]);
			LoadSongPieces (words [1]);
			LoadSong (words [2]);
			/*foreach (string s in words) {
				string[] elements = s.Split ('@');
				foreach (string word in elements) {
					Debug.Log(word);
				}
			}*/
		} else
			Debug.Log ("file doesn't exist");

	}

	public static void SaveFile (string path) {
		BinaryFormatter bf = new BinaryFormatter ();

		//string path = Application.persistentDataPath + "/" + InstrumentSetup.currentRiff.name;
		string directoryPath = Application.persistentDataPath + "/";
		string filePath = directoryPath + MusicManager.instance.currentSong.name + saveExtension;
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

		file.Close ();

	}

	public static void LoadFile (string path) {
		if (File.Exists (path)) {
			Debug.Log("wat");
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (path, FileMode.Open);

			try {
				riffdata data = (riffdata)bf.Deserialize (file);
				Debug.Log (data.songData);
				file.Close ();

				// Split file into input for all three load processes
				string[] words = data.songData.Split(saveSeparator);
				LoadRiffs (words [0]);
				LoadSongPieces (words [1]);
				LoadSong (words [2]);

				Prompt.instance.PromptMessage("Load Project", "Successfully loaded project!", "Okay");
			} catch (SerializationException) {
				Debug.LogError ("save_load.LoadFile(): Failed to deserialize file, probably empty.");
				Prompt.instance.PromptMessage("Failed to load project", "File is empty.", "Okay");
			} catch (EndOfStreamException) {
				Debug.LogError ("save_load.LoadFile(): Attempted to read past end of stream, file is wrong format?");
				Prompt.instance.PromptMessage("Failed to load project", "File is corrupted.", "Okay");
			}
			/*foreach (string s in words) {
				string[] elements = s.Split ('@');
				foreach (string word in elements) {
					Debug.Log(word);
				}
			}*/
		} else {
			Debug.LogError ("save_load.LoadFile(): File \'"+path+"\' doesn't exist.");
			Prompt.instance.PromptMessage("Failed to load project", "Could not find file.", "Okay");
		}
		
	}

	public static void LoadRiffs (string riffString) {
		string[] riffs = riffString.Split (riffSeparator);

		if (riffs.Length == 0) {
			Debug.LogError("save_load.LoadRiffs(): No riffs found");
			return;
		}
		foreach (string riff in riffs) {
			if (riff.Length == 0)
				break;
			string[] strings = riff.Split (new char[]{itemSeparator,noteSeparator});
			Riff newRiff = new Riff ();
			newRiff.name = strings [0];
			Debug.Log ("Riff name: " +newRiff.name);
			newRiff.currentInstrument = (Instrument)Enum.Parse (typeof(Instrument), strings [1]);
			Debug.Log ("Instrument: " + (Instrument)Enum.Parse (typeof(Instrument), strings [1]));
			newRiff.cutSelf = (strings [2] == "True" ? true : false);
			Debug.Log ("cutSelf: " + newRiff.cutSelf.ToString ());
			int currentBeat = 0; // current beat being loaded in
			for (int i=3; i<strings.Length-1; i++) {
				string item = strings [i];
				Debug.Log ("Parsing " + item);
				int itemval;
				//Debug.Log (item);
				//if (Int32.Parse(item) == null) { // riff
				if (!int.TryParse(item, out itemval)) {
					Debug.Log("Note "+item+" at "+currentBeat);
					newRiff.notes [currentBeat].Add (new Note (item));
				} else {
					//int itemval;
					bool beat = int.TryParse (item,out itemval);
					if (beat == false) {
						Debug.Log (item + "not a number");
					} else {
						currentBeat = itemval;
						Debug.Log ("Now on beat " + itemval);
					}
				}
			}
			MusicManager.instance.AddRiff (newRiff);
		}
	}

	public static void LoadSongPieces (string songPieceString) {
		
	}

	public static void LoadSong (string songString) {
	}
		

	public static string SaveSong () {
		string output = "";


		//output += InstrumentSetup.currentRiff.ToString() + riffSeparator;
		//Debug.Log(output);
		// save all riffs
		foreach (Riff riff in MusicManager.instance.riffs) {
			output += riff.ToString () + riffSeparator;
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

	public void LoadSong(){

	}
	
}

 [Serializable]
class riffdata{
	
	//public Riff riff;
	//public int beat;
	public string songData;

}
