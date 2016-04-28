using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;// need for using lists

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class Note {

	#region Note Vars

	const float DEFAULT_VOLUME = 1f;
	const float DEFAULT_DURATION = 1f;
	
	//public AudioClip sound;
	[SerializeField]
	public string filename; // name of audio clip
	[SerializeField]
	public float volume = DEFAULT_VOLUME;
	[SerializeField]
	public float duration = DEFAULT_DURATION;

	#endregion
	#region Note Methods

	public Note () {
		filename = null;
	}

	public Note (string fileName) {
		filename = fileName;
	}

	public Note (string fileName, float vol, float dur) {
		if (!MusicManager.SoundClips.ContainsKey(fileName)) {
			Debug.LogError ("Note.Note(): filename \"" + fileName + "\" invalid!");
			filename = null;
		} else {
			filename = fileName;
		}
		volume = vol;
		duration = dur;
	}

	// Play note on a specific AudioSource
	public void PlayNote (AudioSource source) {
		if (!source.enabled) source.enabled = true;
		source.PlayOneShot(MusicManager.SoundClips[filename], volume*source.volume);
	}

	public void PlayNote (AudioSource source, float newVolume) {
		if (!source.enabled) source.enabled = true;
		source.PlayOneShot(MusicManager.SoundClips[filename], newVolume*volume*source.volume);
	}

	// Play note on a specific AudioSource, cutting off?
	public void PlayNote (AudioSource source, bool cutoff) {
		if (!source.enabled) source.enabled = true;
		if (cutoff) source.Stop();
		source.PlayOneShot(MusicManager.SoundClips[filename], volume*source.volume);
	}

	public bool Equals (Note other) {
		return filename == other.filename || (this == null && other == null);
	}

	#endregion
}
