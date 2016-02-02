using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;// need for using lists

public class Riff {

	Instrument currentInstrument; // instrument used for this riff
	List<List<Note>> notes = new List<List<Note>>(); // contains notes
	public int subdivs; // 0 = 4ths, 1 = 8ths, 2 = 16ths

	public bool pause = true; // if player is looping the riff or just want silent

	public Riff () {
		notes.Add (new List<Note> ());
		notes.Add (new List<Note> ());
		notes.Add (new List<Note> ());
		notes.Add (new List<Note> ());
	}

	public void SetInstrument (Instrument inst) {
		currentInstrument = inst;
	}

	public void Toggle (Note newNote, int pos) {
		// Lookup
		foreach (Note note in notes[pos]) {
			if (newNote.sound == note.sound) {
				// Note with same sound is already there
				notes [pos].Remove (note);
				Debug.Log ("removed note");
				return;
			}
		}
		// Note not already there
		notes [pos].Add (newNote);
		newNote.PlayNote();
		Debug.Log ("added note");
	}

	public void PlayRiff (int pos){ // plays all the notes at pos
		foreach (Note note in notes[pos]) {
			note.PlayNote();
		}
	}
	
}

/* To filter all notes into keys
void Sounds(Instrument currentInstrument, MusicManager.Key currentKey){
	switch (currentInstrument) {
	case Instrument.Drums:

		break;

	}
}
*/