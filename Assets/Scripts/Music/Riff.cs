using UnityEngine;
using System; // for enum stuff
using System.Collections;
using System.Collections.Generic;// need for using lists
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class Riff {

	[NonSerialized]
	public const int MAX_SUBDIVS = 2;
	[NonSerialized]
	public const int MAX_BEATS = 32;

	[SerializeField]
	public int index;

	//Float values from audio effect knobs
	// Distortion
	[SerializeField]
	public bool distortionEnabled = false;
	[SerializeField]
	public float distortionLevel = 0f;

	// Tremolo
	[SerializeField]
	public bool tremoloEnabled = false;
	[SerializeField]
	public float tremoloRate = 0f;
	[SerializeField]
	public float tremoloDepth = 0f;

	// Chorus
	[SerializeField]
	public bool chorusEnabled = false;
	[SerializeField]
	public float chorusDryMix = 0f;
	[SerializeField]
	public float chorusRate= 0f;
	[SerializeField]
	public float chorusDepth = 0f;

	// Echo
	[SerializeField]
	public bool echoEnabled = false;
	[SerializeField]
	public float echoDecayRatio = 1f;
	[SerializeField]
	public float echoDelay = 0f;
	[SerializeField]
	public float echoDryMix = 0f;

	// Reverb
	[SerializeField]
	public bool reverbEnabled = false;
	[SerializeField]
	public float reverbDecayTime = 0f;
	[SerializeField]
	public float reverbLevel = 0f;

	// Flanger
	[SerializeField]
	public bool flangerEnabled = false;
	[SerializeField]
	public float flangerRate = Mathf.PI/32f;
	[SerializeField]
	public float flangerDryMix = 0f;

	//
	// VARIABLES SAVED IN PROJECT
	//

	[SerializeField]
	public string name; // user-defined name of the riff
	[SerializeField]
	public int instrumentIndex = 0; // instrument used for this riff
	[SerializeField]
	public List<int> beatIndices = new List<int>(); // contains notes
	[SerializeField]
	public bool cutSelf = true; // if true, sounds will cut themselves off
	[SerializeField]
	public int beatsShown = 4;
	[SerializeField]
	public float volume = 1f;

	//
	// VARIABLES NOT SAVED IN PROJECT
	//
	[NonSerialized]
	public Instrument instrument;

	public int Length () {
		return beatIndices.Count;
	}

	public bool Lookup (string filename, int pos) {
		Note temp = new Note(filename);
		return Lookup (temp, pos);
	}

	// Returns true is a note is found at a position
	public bool Lookup (Note newNote, int pos) {
		Song song = MusicManager.instance.currentSong;
		Beat beat = song.beats[beatIndices[pos]];
		try {
			//return notes[pos].Contains(newNote);
			foreach (Note note in beat.notes) {
				if (note.filename == newNote.filename) return true;
			}
			return false;
		} catch (ArgumentOutOfRangeException) {
			Debug.LogError("Tried to access pos "+pos+" in "+Length()+"-long riff!");
			return false;
		}
	}

	public float VolumeOfNote(string soundName, int pos) {
		Song song = MusicManager.instance.currentSong;
		Beat beat = song.beats[beatIndices[pos]];
		foreach (Note note in beat.notes) {
			if (note.filename == soundName) {
				Debug.Log (note.volume);
				return note.volume;
			}
		}
		return 1f;
	}
		

	public void RemoveNote (Note newNote, int pos) {
		Song song = MusicManager.instance.currentSong;
		Beat beat = song.beats[beatIndices[pos]];
		foreach (Note note in beat.notes) {
			if (note.filename == newNote.filename) {
				beat.notes.Remove(note);
				return;
			}
		}
	}

	// Removes all notes at position
	public void Clear (int pos) {
		Song song = MusicManager.instance.currentSong;
		Beat beat = song.beats[beatIndices[pos]];
		beat.Clear();
	}

	// Adds or removes a note at pos
	public bool Toggle (Note newNote, int pos) {
		Song song = MusicManager.instance.currentSong;
		Beat beat = song.beats[beatIndices[pos]];
		// Lookup
		if (Lookup(newNote, pos)) {
				// Note with same sound is already there
			RemoveNote (newNote, pos);
			return false;
		}
		// Note not already there
		beat.Add (newNote);
		newNote.PlayNote(MusicManager.instance.instrumentAudioSources[Instrument.AllInstruments[instrumentIndex]]);
		return true;
	}
	public void PlayRiffLoop (AudioClip clip) {
		MusicManager.instance.LoopRiff.Stop();
		MusicManager.instance.LoopRiff.clip = clip;
		MusicManager.instance.LoopRiff.loop = true;
	}

	// Plays all the notes at pos
	public void PlayRiff (int pos) { 
		try {
			Song song = MusicManager.instance.currentSong;
			Beat beat = song.beats[beatIndices[pos]];
			if (beat.NumNotes() != 0) {
				AudioSource source = MusicManager.instance.instrumentAudioSources[Instrument.AllInstruments[instrumentIndex]];
				source.GetComponent<AudioDistortionFilter>().distortionLevel = distortionLevel;
				source.GetComponent<AudioTremoloFilter>().depth = tremoloDepth;
				source.GetComponent<AudioTremoloFilter>().rate = tremoloRate;
				source.GetComponent<AudioEchoFilter>().decayRatio = echoDecayRatio;
				source.GetComponent<AudioEchoFilter>().delay = echoDelay;
				source.GetComponent<AudioEchoFilter>().dryMix = echoDryMix;
				source.GetComponent<AudioReverbFilter>().decayTime = reverbDecayTime;
				source.GetComponent<AudioReverbFilter>().reverbLevel = reverbLevel;
				source.GetComponent<AudioChorusFilter>().dryMix = chorusDryMix;
				source.GetComponent<AudioChorusFilter>().rate = chorusRate;
				source.GetComponent<AudioChorusFilter>().depth = chorusDepth;
				//source.GetComponent<AudioFlangerFilter>().delay = flangerDelay;
				source.GetComponent<AudioFlangerFilter>().dryMix = flangerDryMix;

				if (cutSelf) MusicManager.instance.instrumentAudioSources[Instrument.AllInstruments[instrumentIndex]].Stop();
				foreach (Note note in beat.notes) {
					
					note.PlayNote(source, volume);
				}
			}
		} catch (ArgumentOutOfRangeException) {
			Debug.LogError("Tried to play out of range of song! Pos: "+pos);
		}
	}
		
}
