using UnityEditor;
using UnityEngine;
using System; // for enum stuff
using System.Collections;
using System.Collections.Generic;// need for using lists

public class Riff {
	public static int MAX_SUBDIVS = 2;

	public string name; // user-defined name of the riff
	public Instrument currentInstrument; // instrument used for this riff
	public List<List<Note>> notes = new List<List<Note>>(); // contains notes
	public bool cutSelf = true; // if true, sounds will cut themselves off

	// Default constructor makes an empty 4-beat riff (and accounts for all subdivs)
	public Riff () {
		for (int i=0; i<(int)Mathf.Pow(2f, (float)MAX_SUBDIVS+2); i++) {
			notes.Add (new List<Note> ());
		}
	}

	// Constructor to make a riff of a certain number of beats
	public Riff (int length) {
		for (int i=0; i<length*(int)Mathf.Pow(2f, (float)MAX_SUBDIVS); i++) {
			notes.Add (new List<Note>());
		}
	}

	// Loading from a string
	public Riff (string instrumentString) {
		currentInstrument = (Instrument)Enum.Parse (typeof(Instrument), instrumentString);
	}

	public void SetInstrument (Instrument inst) {
		currentInstrument = inst;
	}

	public int GetLength () {
		return notes.Count;
	}

	// Returns true is a note is found at a position
	public bool Lookup (Note newNote, int pos) {
		//return notes[pos].Contains(newNote);
		foreach (Note note in notes[pos]) {
			if (note.sound == newNote.sound) return true;
		}
		return false;
	}

	public void RemoveNote (Note newNote, int pos) {
		foreach (Note note in notes[pos]) {
			if (note.sound == newNote.sound) {
				notes[pos].Remove(note);
				return;
			}
		}
	}

	// Removes all notes at position
	public void Clear (int pos) {
		notes[pos].Clear();
	}

	// Adds or removes a note at pos
	public void Toggle (Note newNote, int pos) {
		// Lookup
		//foreach (Note note in notes[pos]) {
			//if (newNote.sound == note.sound) {
		if (Lookup(newNote, pos)) {
				// Note with same sound is already there
				//notes [pos].Remove (note);
			//notes[pos].Remove(newNote);
			RemoveNote (newNote, pos);
			//Debug.Log("removed note");
				return;
			}
		//}
		// Note not already there
		notes [pos].Add (newNote);
		//Debug.Log (newNote.ToString());
		//Debug.Log(this.ToString());
		MusicManager.instance.PlayOneShot(newNote.sound);
		//Debug.Log ("added note");
	}
	public void PlayRiffLoop (AudioClip clip) {
		MusicManager.instance.LoopRiff.Stop();
		MusicManager.instance.LoopRiff.clip = clip;
		MusicManager.instance.LoopRiff.loop = true;
	}

	// Plays all the notes at pos
	public void PlayRiff (int pos) { 
		//Debug.Log ("before for loop");


		foreach (Note note in notes[pos]) {
			//Debug.Log("inside for loop " + pos);
			if (cutSelf) 
				//MusicManager.instance.OneShot.Stop();
				MusicManager.instance.instrumentAudioSources[currentInstrument].Stop();
			//if (MusicManager.instance.instrumentAudioSources[currentInstrument] == null) Debug.Log("shit");
			note.PlayNote(MusicManager.instance.instrumentAudioSources[currentInstrument]);

		}

		
	}

	public string ToString () {
		string result = "";
		result += name + "@";
		result += Enum.GetName (typeof(Instrument), (int)currentInstrument) + '@';
		for (int i = 0; i < notes.Count; i++) {
			result += i+"@";
			foreach (Note note in notes[i]) {
				result += note.ToString () + ",";
			}
		}
		result += cutSelf.ToString ();
		return result;
	}

	/*
	// Returns true if two riffs are the same (used for lookup)
	public bool Compare (Riff other) {
		bool result = true;
		foreach (List<Note> beat in notes) {
			foreach (
	*/
}

/* To filter all notes into keys
void Sounds(Instrument currentInstrument, MusicManager.Key currentKey){
	switch (currentInstrument) {
	case Instrument.Drums:

		break;

	}
}
*/