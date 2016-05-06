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
				//if (!project.Empty)
				//MusicManager.instance.currentSong = project.songs[0];
				//else MusicManager.instance.NewSong();

				//foreach (Riff riff in project.riffs) Debug.Log(riff.name);
				PlaylistBrowser.instance.RefreshName();
				//SongArrangeSetup.instance.Refresh();
				//SongTimeline.instance.RefreshTimeline();

				GameManager.instance.GoToPlaylistMenu();

				Prompt.instance.PromptMessage("Load Project", "Successfully loaded project!", "Okay");
			} catch (SerializationException) {
				Debug.LogError ("SaveLoad.LoadFile(): Failed to deserialize file, probably empty.");
				Prompt.instance.PromptMessage("Failed to load project", "File is empty.", "Okay");
			} catch (EndOfStreamException) {
				Debug.LogError ("SaveLoad.LoadFile(): Attempted to read past end of stream, file is wrong format?");
				Prompt.instance.PromptMessage("Failed to load project", "File is corrupted.", "Okay");
			}/* catch (ArgumentException) {
				Debug.LogError ("SaveLoad.LoadFile(): Failed to load a riff or song piece. Already exists?");
				Prompt.instance.PromptMessage("Failed to load project", "File is corrupted.", "Okay");
			}*/ catch (FailedToLoadException f) {
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

	public static void LoadSongToProject (string path) {
		MusicManager.instance.currentProject.AddSong(LoadSong(path));
	}

	public static Song LoadSong (string path) {
		if (File.Exists(path)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open (path, FileMode.Open);

			Song backupSong = MusicManager.instance.currentSong;

			try {
				Song song = (Song)bf.Deserialize(file);
	
				Prompt.instance.PromptMessage("Load Song", "Successfully loaded Song!", "Okay");
				return song;

			} catch (SerializationException) {
				Debug.LogError ("SaveLoad.LoadSong(): Failed to deserialize file, probably empty.");
				Prompt.instance.PromptMessage("Failed to load song", "File is empty.", "Okay");
				return null;
			} catch (EndOfStreamException) {
				Debug.LogError ("SaveLoad.LoadSong(): Attempted to read past end of stream, file is wrong format?");
				Prompt.instance.PromptMessage("Failed to load song", "File is corrupted.", "Okay");
				return null;
			//} catch (ArgumentException) {
				//Debug.LogError ("SaveLoad.LoadSong(): Failed to load a riff or song piece. Already exists?");
				//Prompt.instance.PromptMessage("Failed to load song", "File is corrupted.", "Okay");
			} catch (FailedToLoadException f) {
				Debug.LogError("FailedToLoadException: "+f);
				MusicManager.instance.currentSong = backupSong;
				Prompt.instance.PromptMessage("Failed to load song", "File is corrupted.", "Okay");
				return null;
			} finally {
				file.Close ();
			}
		} else {
			Debug.LogError ("SaveLoad.LoadSong(): Song \'"+path+"\' doesn't exist.");
			Prompt.instance.PromptMessage("Failed to load Song", "Could not find file.", "Okay");
		}
		return null;
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
