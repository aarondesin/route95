using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Class for a list of notes, between Riffs and Notes
[System.Serializable]
public class Beat {

	#region Serialized Vars

	[SerializeField]
	public int index;

	[SerializeField]
	public List<Note> notes;

	#endregion
	#region Beat Methods

	// Default constructor
	public Beat () {
		notes = new List<Note> ();
	}

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

	#endregion
}
