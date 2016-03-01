using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;// need for using lists


public class Note {
	static float DEFAULT_VOLUME = 1f;
	static float DEFAULT_DURATION = 1f;
	static char varSeparator = '|';
	
	public AudioClip sound;
	public string filename; // name of audio clip
	float volume;
	float duration;

	public Note () {
		sound = null;
		filename = null;
		volume = DEFAULT_VOLUME;
		duration = DEFAULT_DURATION;
	}

	/*public Note (string fileName) {
		if (!MusicManager.Sounds.ContainsKey(fileName)) {
			Debug.LogError ("Note.Note(): filename \"" + fileName + "\" invalid!");
			sound = null;
		} else {
			filename = fileName;
			sound = MusicManager.Sounds [fileName];
		}
		volume = DEFAULT_VOLUME;
		duration = DEFAULT_DURATION;
	}

	public Note (string fileName, float vol) {
		if (!MusicManager.Sounds.ContainsKey(fileName)) {
			Debug.LogError ("Note.Note(): filename \"" + fileName + "\" invalid!");
			sound = null;
		} else {
			filename = fileName;
			sound = MusicManager.Sounds [fileName];
		}
		volume = vol;
		duration = DEFAULT_DURATION;
	}*/

	public Note (string loadString) {
		
		string[] vars = loadString.Split(varSeparator);

		//Note result = new Note();

		// Set filename
		//result.filename = vars[0];
		filename = vars[0];
		if (!MusicManager.Sounds.ContainsKey(filename)) {
			Debug.LogError ("Note.Note(): filename \"" + filename + "\" invalid!");
			//result.sound = null;
			sound = null;
		} else {
			//result.sound = MusicManager.Sounds [filename];
			sound = MusicManager.Sounds [filename];
		}

		if (vars.Length == 1) {
			volume = DEFAULT_VOLUME;
			duration = DEFAULT_DURATION;
		} else {

			// Set duration
			try {
				//result.duration = float.Parse(vars[1]);
				duration = float.Parse(vars[1]);
			} catch (FormatException) {
				Debug.LogError("Note.Note(): invalid duration");
				//result.duration = DEFAULT_DURATION;
				duration = DEFAULT_DURATION;
			}

			// Set volume
			try {
				//result.volume = float.Parse(vars[2]);
				volume = float.Parse(vars[2]);
			} catch (FormatException) {
				Debug.LogError ("Note.Note(): invalid volume");
				//result.volume = DEFAULT_VOLUME;
				volume = DEFAULT_VOLUME;
			}
		}

		//return result;
	}
		
	public Note (AudioClip newSound) {

		if (newSound == null) Debug.LogError ("Note(): was passed null sound!");
		sound = newSound;
		//Debug.Log("note exist?" + newSound);
		duration = 1f;
		volume = 1f;

	}

	// FIX ME!!
	public void PlayNote () {
		//Debug.Log ("do nothing");
		MusicManager.instance.OneShot.PlayOneShot(sound,volume);
		//AudioSource.clip = Note.sound;
		// AudioSource.Play();
		// sound.Play();
	}

	// Play note on a specific AudioSource
	public void PlayNote (AudioSource source) {
		source.PlayOneShot(sound, volume);
	}

	// Play note on a specific AudioSource, cutting off?
	public void PlayNote (AudioSource source, bool cutoff) {
		if (cutoff) source.Stop();
		source.PlayOneShot(sound, volume);
	}

	// Returns note in filename|duration|volume format
	public override string ToString () {
		string result = "";
		result += filename + varSeparator;
		result += duration.ToString() + varSeparator;
		result += volume.ToString();
		Debug.Log("Note.ToString(): "+result);
		return result;
	}
}
