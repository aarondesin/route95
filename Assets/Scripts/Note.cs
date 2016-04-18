using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;// need for using lists

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class Note {
	static float DEFAULT_VOLUME = 1f;
	static float DEFAULT_DURATION = 1f;
	//static char varSeparator = '|';
	
	//public AudioClip sound;
	[SerializeField]
	public string filename; // name of audio clip
	[SerializeField]
	public float volume;
	[SerializeField]
	public float duration;

	public Note () {
		filename = null;
		volume = DEFAULT_VOLUME;
		duration = DEFAULT_DURATION;
	}

	public Note (string fileName) {
		filename = fileName;
		volume = DEFAULT_VOLUME;
		duration = DEFAULT_DURATION;
	}

	/*public Note (string loadString) {
		
		string[] vars = loadString.Split(varSeparator);

		//Note result = new Note();

		// Set filename
		//result.filename = vars[0];
		filename = vars[0];
		if (!MusicManager.SoundClips.ContainsKey(filename)) {
			Debug.LogError ("Note.Note(): filename \"" + filename + "\" invalid!");
			//result.sound = null;
			sound = null;
		} else {
			//result.sound = MusicManager.Sounds [filename];
			sound = MusicManager.SoundClips [filename];
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
	}*/

	public Note (string fileName, float vol, float dur) {
		if (!MusicManager.SoundClips.ContainsKey(fileName)) {
			Debug.LogError ("Note.Note(): filename \"" + fileName + "\" invalid!");
			//result.sound = null;
			filename = null;
		} else {
			filename = fileName;
		}
		volume = vol;
		duration = dur;
	}

	// Play note on a specific AudioSource
	public void PlayNote (AudioSource source) {
		source.PlayOneShot(MusicManager.SoundClips[filename], volume*source.volume);
	}

	// Play note on a specific AudioSource, cutting off?
	public void PlayNote (AudioSource source, bool cutoff) {
		if (cutoff) source.Stop();
		source.PlayOneShot(MusicManager.SoundClips[filename], volume*source.volume);
	}

	public bool Equals (Note other) {
		return filename == other.filename || (this == null && other == null);
	}

	/*// Returns note in filename|duration|volume format
	public override string ToString () {
		string result = "";
		result += filename + varSeparator;
		result += duration.ToString() + varSeparator;
		result += volume.ToString();
		Debug.Log("Note.ToString(): "+result);
		return result;
	}*/
}
