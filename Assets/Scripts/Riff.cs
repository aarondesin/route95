using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;// need for using lists

public class Riff {

	public Instrument currentInstrument; // instrument used for this riff
	public List<List<Note>> notes = new List<List<Note>>(); // contains notes
	public int subdivs; // 0 = 4ths, 1 = 8ths, 2 = 16ths
	public string name; // user-defined name of the riff
	public bool cutSelf = true; // if true, sounds will cut themselves off

	public bool pause = true; // if player is looping the riff or just want silent

	// Default constructor makes an empty 4-beat riff
	public Riff () {
		notes.Add (new List<Note> ());
		notes.Add (new List<Note> ());
		notes.Add (new List<Note> ());
		notes.Add (new List<Note> ());
	}

	public void SetInstrument (Instrument inst) {
		currentInstrument = inst;
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