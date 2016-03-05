using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

[Serializable]
public class FailedToLoadException: ApplicationException {
	public FailedToLoadException (string info, Exception standard): base(info, standard) {}
	public FailedToLoadException (string info): base(info) {}
	public FailedToLoadException() {}
}

public class save_load : MonoBehaviour {
	public static char saveSeparator = '$'; // separates riffs, songpieces, song
	public static char itemSeparator = '@';
	public static char riffSeparator = '%';
	public static char noteSeparator = ',';
	//char[] separators = { '$', ',', '#', '%' };

	public static string saveExtension = ".r95";


	public void SaveCurrentProject () {
		BinaryFormatter bf = new BinaryFormatter ();

		//string path = Application.persistentDataPath + "/" + InstrumentSetup.currentRiff.name;
		string directoryPath = Application.persistentDataPath + "/";
		string filePath = directoryPath + MusicManager.instance.currentSong.name + saveExtension;
		//Debug.Log (directoryPath);
		//Debug.Log (filePath);
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
		Prompt.instance.PromptMessage("Save Project", "Successfully saved project!", "Okay");
	}
		

	public static void LoadFile (string path) {
		if (File.Exists (path)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (path, FileMode.Open);

			try {
				riffdata data = (riffdata)bf.Deserialize (file);
				Debug.Log (data.songData);


				// Split file into input for all three load processes
				string[] words = data.songData.Split(saveSeparator);
				List<Riff> tempRiffs = LoadRiffs (words [0]);
				List<SongPiece> tempSongPieces = LoadSongPieces (words [1]);
				Song tempSong = LoadSong (words [2]);

				//MusicManager.instance.riffs.AddRange(tempRiffs);
				foreach (Riff riff in tempRiffs) {
					MusicManager.instance.AddRiff(riff);
				}
				MusicManager.instance.songPieces.AddRange(tempSongPieces);
				MusicManager.instance.currentSong = tempSong;

				SongArrangeSetup.instance.Refresh();

				Prompt.instance.PromptMessage("Load Project", "Successfully loaded project!", "Okay");
			} catch (SerializationException) {
				Debug.LogError ("save_load.LoadFile(): Failed to deserialize file, probably empty.");
				Prompt.instance.PromptMessage("Failed to load project", "File is empty.", "Okay");
			} catch (EndOfStreamException) {
				Debug.LogError ("save_load.LoadFile(): Attempted to read past end of stream, file is wrong format?");
				Prompt.instance.PromptMessage("Failed to load project", "File is corrupted.", "Okay");
			} catch (FailedToLoadException f) {
				Debug.LogError("FailedToLoadException: "+f);
				Prompt.instance.PromptMessage("Failed to load project", "File is corrupted.", "Okay");
			} finally {
				file.Close ();
			}
		} else {
			Debug.LogError ("save_load.LoadFile(): File \'"+path+"\' doesn't exist.");
			Prompt.instance.PromptMessage("Failed to load project", "Could not find file.", "Okay");
		}
		
	}

	public static List<Riff> LoadRiffs (string riffString) {
		string[] riffs = riffString.Split (riffSeparator);

		if (riffs.Length == 0) {
			//Debug.LogError("save_load.LoadRiffs(): No riffs found");
			throw new FailedToLoadException("No riffs found.");
		}

		List<Riff> temp = new List<Riff>();

		int loadedRiffsCounter = 0;
		foreach (string riff in riffs) {
			if (riff.Length == 0)
				break;
			temp.Add (new Riff(riff));
			loadedRiffsCounter++;
		}
		Debug.Log("Loaded "+loadedRiffsCounter+" riffs.");
		return temp;
	}

	public static List<SongPiece> LoadSongPieces (string songPieceString) {
		if (songPieceString.Length == 0) {
			throw new FailedToLoadException("No song pieces found.");
		}
		string[] songPieces = songPieceString.Split(itemSeparator);

		List<SongPiece> temp = new List<SongPiece>();
		int loadedSongPiecesCounter = 0;
		foreach (string songPiece in songPieces) {
			if (songPiece.Length == 0) break;
			loadedSongPiecesCounter++;
			temp.Add(new SongPiece(songPiece));
			//MusicManager.instance.AddSongPiece (new SongPiece(songPiece));
		}
		Debug.Log("Loaded "+loadedSongPiecesCounter+" song pieces.");
		return temp;
	}

	public static Song LoadSong (string songString) {
		//MusicManager.instance.currentSong = new Song(songString);
		return new Song(songString);
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

		Debug.Log("SaveSong():" +output);
		return output;
	}


	public void LoadSong(){

	}

	static bool IsCopy (string str) {
		string[] split = str.Split(new char[]{'(', ')'});
		foreach (string s in split)
		Debug.Log(s);
		int temp;
		return int.TryParse(split[split.Length-1], out temp);
	}

	static string IncrementCopy (string str) {
		string[] split = str.Split(new char[]{'(', ')'});
		int copy;
		int.TryParse(split[split.Length-1], out copy);
		string result = "";
		for (int i=0; i<split.Length-1; i++) {
			result += split[i];
		}
		copy++;
		result += "("+copy.ToString()+")";
		return result;
	}

}

 [Serializable]
class riffdata{
	
	//public Riff riff;
	//public int beat;
	public string songData;

}
