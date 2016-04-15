using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Beat {

	[SerializeField]
	public List<Note> notes;

	// Add a note to the beat
	public void Add (Note note) {
		notes.Add(note);
	}

	// Remove a particular note from the beat
	public void Remove (Note note) {
		notes.Remove(note);
	}

	// Removes all notes in the beat
	public void Clear () {
		notes.Clear();
	}

	// Returns number of notes at beat
	public int NumNotes () {
		return notes.Count;
	}
}
