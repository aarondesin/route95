using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;// need for using lists

public class Riff {

	public Instrument currentInstrument; // instrument used for this riff
	List<List<Note>> notes = new List<List<Note>>(); // contains notes
	public int subdivs; // 0 = 4ths, 1 = 8ths, 2 = 16ths
	public string name; // user-defined name of the riff

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
				//Debug.Log ("removed note");
				return;
			}
		}
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

	public void PlayRiff (int pos){ // plays all the notes at pos
		Debug.Log ("before for loop");


		foreach (Note note in notes[pos]) {
			Debug.Log("inside for loop " + pos);
			note.PlayNote();

			//MusicManager.instance.PlayOneShot(note.sound);
			//MusicManager.instance.PlayRiffLoop(note.sound);
			//yield return new WaitForSeconds(1);


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