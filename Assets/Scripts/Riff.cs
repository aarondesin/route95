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

	//Float values from audio effect knobs

	[SerializeField]
	public float distortionLevel = 0;
	[SerializeField]
	public float echoDecayRatio = 1;
	[SerializeField]
	public float echoDelay = 0;
	[SerializeField]
	public float echoDryMix = 0;
	[SerializeField]
	public float reverbDecayTime = 0;
	[SerializeField]
	public float reverbLevel = 0;
	[SerializeField]
	public float fuzzLevel = 0;
	[SerializeField]
	public float tremoloRate = 0;
	[SerializeField]
	public float chorusDryMix = 0;
	[SerializeField]
	public float chorusRate= 0;
	[SerializeField]
	public float chorusDepth = 0;
	[SerializeField]
	public float flangerDelay = 0;
	[SerializeField]
	public float flangerDryMix = 0;

	//
	// VARIABLES SAVED IN PROJECT
	//

	[SerializeField]
	public string name; // user-defined name of the riff
	[SerializeField]
	public int instrumentIndex; // instrument used for this riff
	[SerializeField]
	public List<Beat> beats = new List<Beat>(); // contains notes
	[SerializeField]
	public bool cutSelf = true; // if true, sounds will cut themselves off
	[SerializeField]
	public int beatsShown = 4;

	//
	// VARIABLES NOT SAVED IN PROJECT
	//
	[NonSerialized]
	public Instrument instrument;
	[NonSerialized]
	public int copy = 0; // not saved

	// Default constructor makes an empty 4-beat riff (and accounts for all subdivs)
	public Riff () {
		for (int i=0; i<(int)Mathf.Pow(2f, (float)MAX_SUBDIVS+2); i++) {
			beats.Add (new Beat ());
		}
		copy = 0;
		instrument = Instrument.AllInstruments[instrumentIndex];
		Debug.Log ("Current instrument is " + instrument);
	}

	// Constructor to make a riff of a certain number of beats
	public Riff (int length) {
		for (int i=0; i<length*(int)Mathf.Pow(2f, (float)MAX_SUBDIVS); i++) {
			beats.Add (new Beat());
		}
		copy = 0;
		instrument = Instrument.AllInstruments[instrumentIndex];
	}

	public int Length () {
		return beats.Count;
	}

	public bool Lookup (string filename, int pos) {
		Note temp = new Note(filename);
		return Lookup (temp, pos);
	}

	// Returns true is a note is found at a position
	public bool Lookup (Note newNote, int pos) {
		try {
			//return notes[pos].Contains(newNote);
			foreach (Note note in beats[pos].notes) {
				if (note.filename == newNote.filename) return true;
			}
			return false;
		} catch (ArgumentOutOfRangeException) {
			Debug.LogError("Tried to access pos "+pos+" in "+Length()+"-long riff!");
			return false;
		}
	}

	public float VolumeOfNote(string soundName, int pos) {
		foreach (Note note in beats[pos].notes) {
			if (note.filename == soundName) {
				Debug.Log (note.volume);
				return note.volume;
			}
		}
		return 1f;
	}
		

	public void RemoveNote (Note newNote, int pos) {
		foreach (Note note in beats[pos].notes) {
			if (note.filename == newNote.filename) {
				beats[pos].notes.Remove(note);
				return;
			}
		}
	}

	// Removes all notes at position
	public void Clear (int pos) {
		beats[pos].Clear();
	}

	// Adds or removes a note at pos
	public void Toggle (Note newNote, int pos) {
		// Lookup
		if (Lookup(newNote, pos)) {
				// Note with same sound is already there
			RemoveNote (newNote, pos);
			return;
		}
		// Note not already there
		beats [pos].Add (newNote);
		newNote.PlayNote(MusicManager.instance.instrumentAudioSources[Instrument.AllInstruments[instrumentIndex]]);
	}
	public void PlayRiffLoop (AudioClip clip) {
		MusicManager.instance.LoopRiff.Stop();
		MusicManager.instance.LoopRiff.clip = clip;
		MusicManager.instance.LoopRiff.loop = true;
	}

	// Plays all the notes at pos
	public void PlayRiff (int pos) { 
		try {
			if (beats[pos].NumNotes() != 0) {
				if (cutSelf) MusicManager.instance.instrumentAudioSources[Instrument.AllInstruments[instrumentIndex]].Stop();
				foreach (Note note in beats[pos].notes) {
					note.PlayNote(MusicManager.instance.instrumentAudioSources[Instrument.AllInstruments[instrumentIndex]]);
				}
			}
		} catch (ArgumentOutOfRangeException) {
			Debug.LogError("Tried to play out of range of song! Pos: "+pos);
		}
	}

	public void ShowMore () {
		if (beatsShown < MAX_BEATS) {
			beatsShown += 4;
			if (beatsShown > beats.Count/(int)Mathf.Pow(2f, MAX_SUBDIVS)) {
				for (int i=0; i<4*(int)Mathf.Pow(2f, MAX_SUBDIVS); i++) beats.Add(new Beat());
			}
		}
	}

	public void ShowLess () {
		if (beatsShown > 4) {
			beatsShown -= 4;
		}
	}

	/*// FORMAT:
	// name@instrument@cutSelf@beatsShown@beat@notesatbeat@otherbeat@notesatotherbeat@othernotesatotherbeat
	public override string ToString () {
		string result = name;
		if (copy != 0) result += " (" + copy.ToString() + ")";
		result += save_load.itemSeparator;
		result += (string)Enum.GetName (typeof(Instrument), (int)instrument) + save_load.itemSeparator;
		result += cutSelf.ToString () + save_load.itemSeparator;
		result += beatsShown.ToString() + save_load.itemSeparator;
		for (int i = 0; i < notes.Count; i++) {
			result += i.ToString() + save_load.itemSeparator;
			foreach (Note note in notes[i]) {
				result += note.ToString () + save_load.noteSeparator;
			}
		}
		Debug.Log("Riff.ToString(): "+result);
		return result;
	}*/
		
}
