using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class FailedToLoadException: ApplicationException {
	public FailedToLoadException (string info, Exception standard): base(info, standard) {}
	public FailedToLoadException (string info): base(info) {}
	public FailedToLoadException() {}
}

public class SaveLoad {
	public static char saveSeparator = '$'; // separates riffs, songpieces, song
	public static char itemSeparator = '@';
	public static char riffSeparator = '%';
	public static char noteSeparator = ',';
	//char[] separators = { '$', ',', '#', '%' };

	public static string projectSaveExtension = ".r95p";
	public static string songSaveExtension = ".r95s";

	public static void SaveCurrentProject () {
		BinaryFormatter bf = new BinaryFormatter ();

		string directoryPath = Application.persistentDataPath + "/Projects/";
		string filePath = directoryPath + MusicManager.instance.currentProject.name + projectSaveExtension;

		Directory.CreateDirectory (directoryPath);
		FileStream file = File.Open(filePath, FileMode.Create);
	
		bf.Serialize (file, MusicManager.instance.currentProject);

		file.Close ();
		Prompt.instance.PromptMessage("Save Project", "Successfully saved project!", "Okay");
	}

	public static void LoadProject (string path) {
		if (File.Exists(path)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open (path, FileMode.Open);

			Project backupProject = MusicManager.instance.currentProject;
			Song backupSong = MusicManager.instance.currentSong;

			try {
				Project project = (Project)bf.Deserialize(file);
				MusicManager.instance.currentProject = project;
				if (!project.Empty())
					MusicManager.instance.currentSong = project.songs[0];
				else MusicManager.instance.NewSong();

				project.Refresh();
				PlaylistBrowser.instance.RefreshName();
				SongArrangeSetup.instance.Refresh();
				SongTimeline.instance.RefreshTimeline();

				GameManager.instance.GoToPlaylistMenu();

				Prompt.instance.PromptMessage("Load Project", "Successfully loaded project!", "Okay");
			} catch (SerializationException) {
				Debug.LogError ("SaveLoad.LoadFile(): Failed to deserialize file, probably empty.");
				Prompt.instance.PromptMessage("Failed to load project", "File is empty.", "Okay");
			} catch (EndOfStreamException) {
				Debug.LogError ("SaveLoad.LoadFile(): Attempted to read past end of stream, file is wrong format?");
				Prompt.instance.PromptMessage("Failed to load project", "File is corrupted.", "Okay");
			} catch (ArgumentException) {
				Debug.LogError ("SaveLoad.LoadFile(): Failed to load a riff or song piece. Already exists?");
				Prompt.instance.PromptMessage("Failed to load project", "File is corrupted.", "Okay");
			} catch (FailedToLoadException f) {
				Debug.LogError("FailedToLoadException: "+f);
				MusicManager.instance.currentProject = backupProject;
				MusicManager.instance.currentSong = backupSong;
				Prompt.instance.PromptMessage("Failed to load project", "File is corrupted.", "Okay");
			} finally {
				file.Close ();
			}
		} else {
			Debug.LogError ("SaveLoad.LoadProject(): Project \'"+path+"\' doesn't exist.");
			Prompt.instance.PromptMessage("Failed to load project", "Could not find file.", "Okay");
		}
	}

	public static void SaveCurrentSong () {
		BinaryFormatter bf = new BinaryFormatter ();

		string directoryPath = Application.persistentDataPath + "/Songs/";
		string filePath = directoryPath + MusicManager.instance.currentSong.name + songSaveExtension;

		Directory.CreateDirectory (directoryPath);
		FileStream file = File.Open(filePath, FileMode.Create);

		bf.Serialize (file, MusicManager.instance.currentSong);

		file.Close ();
		Prompt.instance.PromptMessage("Save Project", "Successfully saved Song!", "Okay");
	}

	public static void LoadSong (string path) {
		if (File.Exists(path)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open (path, FileMode.Open);

			Song backupSong = MusicManager.instance.currentSong;

			try {
				Song song = (Song)bf.Deserialize(file);
				MusicManager.instance.currentProject.AddSong(song);
				MusicManager.instance.currentSong = song;

				SongArrangeSetup.instance.Refresh();
				SongTimeline.instance.RefreshTimeline();

				Prompt.instance.PromptMessage("Load Song", "Successfully loaded Song!", "Okay");
			} catch (SerializationException) {
				Debug.LogError ("SaveLoad.LoadSong(): Failed to deserialize file, probably empty.");
				Prompt.instance.PromptMessage("Failed to load song", "File is empty.", "Okay");
			} catch (EndOfStreamException) {
				Debug.LogError ("SaveLoad.LoadSong(): Attempted to read past end of stream, file is wrong format?");
				Prompt.instance.PromptMessage("Failed to load song", "File is corrupted.", "Okay");
			} catch (ArgumentException) {
				Debug.LogError ("SaveLoad.LoadSong(): Failed to load a riff or song piece. Already exists?");
				Prompt.instance.PromptMessage("Failed to load song", "File is corrupted.", "Okay");
			} catch (FailedToLoadException f) {
				Debug.LogError("FailedToLoadException: "+f);
				MusicManager.instance.currentSong = backupSong;
				Prompt.instance.PromptMessage("Failed to load song", "File is corrupted.", "Okay");
			} finally {
				file.Close ();
			}
		} else {
			Debug.LogError ("SaveLoad.LoadSong(): Song \'"+path+"\' doesn't exist.");
			Prompt.instance.PromptMessage("Failed to load Song", "Could not find file.", "Okay");
		}
	}
		

	/*public static void LoadFile (string path) {
		if (File.Exists (path)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (path, FileMode.Open);

			Song backupSong = MusicManager.instance.currentSong;
			List<Riff> backupRiffs = MusicManager.instance.riffs;
			List<SongPiece> backupSongPieces = MusicManager.instance.songPieces;
			Dictionary<string, SongPiece> backupSongPiecesByName = MusicManager.instance.songPiecesByName;

			try {
				

				riffdata data = (riffdata)bf.Deserialize (file);
				Debug.Log (data.songData);


				// Split file into input for all three load processes
				string[] words = data.songData.Split(saveSeparator);

				List<Riff> tempRiffs = LoadRiffs (words [0]);
				MusicManager.instance.riffs = new List<Riff>();
				foreach (Riff riff in tempRiffs) MusicManager.instance.AddRiff(riff);

				List<SongPiece> tempSongPieces = LoadSongPieces (words [1]);
				MusicManager.instance.songPieces = new List<SongPiece>();
				MusicManager.instance.songPiecesByName = new Dictionary<string, SongPiece>();
				foreach (SongPiece songPiece in tempSongPieces) MusicManager.instance.AddSongPiece(songPiece);
					
				MusicManager.instance.currentSong = LoadSong (words [2]);

				SongArrangeSetup.instance.Refresh();
				SongTimeline.instance.RefreshTimeline();

				Prompt.instance.PromptMessage("Load Project", "Successfully loaded project!", "Okay");

				//CameraControl.instance.MoveToPosition(CameraControl.instance.ViewRadio);
				GameManager.instance.SwitchToSetup();
			} catch (SerializationException) {
				Debug.LogError ("save_load.LoadFile(): Failed to deserialize file, probably empty.");
				Prompt.instance.PromptMessage("Failed to load project", "File is empty.", "Okay");
			} catch (EndOfStreamException) {
				Debug.LogError ("save_load.LoadFile(): Attempted to read past end of stream, file is wrong format?");
				Prompt.instance.PromptMessage("Failed to load project", "File is corrupted.", "Okay");
			} catch (ArgumentException) {
				Debug.LogError ("save_load.LoadFile(): Failed to load a riff or song piece. Already exists?");
				Prompt.instance.PromptMessage("Failed to load project", "File is corrupted.", "Okay");
			} catch (FailedToLoadException f) {
				Debug.LogError("FailedToLoadException: "+f);
				MusicManager.instance.currentSong = backupSong;
				MusicManager.instance.riffs = backupRiffs;
				MusicManager.instance.songPieces = backupSongPieces;
				MusicManager.instance.songPiecesByName = backupSongPiecesByName;
				Prompt.instance.PromptMessage("Failed to load project", "File is corrupted.", "Okay");
			} finally {
				file.Close ();
			}
		} else {
			Debug.LogError ("save_load.LoadFile(): File \'"+path+"\' doesn't exist.");
			Prompt.instance.PromptMessage("Failed to load project", "Could not find file.", "Okay");
		}
		
	}*/

	/*public static List<Riff> LoadRiffs (string riffString) {
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
			if (songPiece.Length == 0 || songPiece == "") break;
			loadedSongPiecesCounter++;
			SongPiece sp = new SongPiece(songPiece);
			temp.Add(sp);
		}
		Debug.Log("Loaded "+loadedSongPiecesCounter+" song pieces.");
		return temp;
	}*/

	/*public static Song LoadSong (string songString) {
		//MusicManager.instance.currentSong = new Song(songString);
		return new Song(songString);
	}*/
		

	/*public static string SaveSong () {
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
		// (SongPiece songPiece in MusicManager.instance.currentSong.songPieces) {
		//	output += songPiece.name + "#";
		//}
		output += MusicManager.instance.currentSong.ToString ();

		Debug.Log("SaveSong():" +output);
		return output;
	}*/

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
