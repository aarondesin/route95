using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class to hold all scale info.
/// These must be compiled by KeyManager.
/// </summary>
public class Scale {

	#region Scale Enums

	/// <summary>
	/// Musical note.
	/// </summary>
	public enum Note {
		Root,
		Second,
		Third,
		Fourth,
		Fifth,
		Sixth,
		Seventh
	}

	#endregion
	#region Scale Vars

	List<string> _root;     // All roots/octaves in scale
	List<string> _second;   // All seconds in scale
	List<string> _third;    // All thirds in scale
	List<string> _fourth;   // All fourths in scale
	List<string> _fifth;    // All fifths in scale
	List<string> _sixth;    // All sixths in scale
	List<string> _seventh;  // All sevenths in scale

	List<string> _allNotes; // All notes in scale

	#endregion
	#region Scale Methods

	/// <summary>
	/// Default constructor.
	/// </summary>
	public Scale () {

		// Init lists
		_root = new List<string> ();
		_second = new List<string> ();
		_third = new List<string> ();
		_fourth = new List<string> ();
		_fifth = new List<string> ();
		_sixth = new List<string> ();
		_seventh = new List<string> ();

		_allNotes = new List<string> ();
	}

    public int Count { get { return _allNotes.Count; } }

    public string NoteAt (int index) {
        return _allNotes[index];
    }

    public void AddRoot (string note) {
        _root.Add(note);
        _allNotes.Add(note);
    }

    public void AddSecond (string note) {
        _second.Add(note);
        _allNotes.Add(note);
    }

    public void AddThird (string note) {
        _third.Add(note);
        _allNotes.Add(note);
    }

    public void AddFourth (string note) {
        _fourth.Add(note);
        _allNotes.Add(note);
    }
    public void AddFifth (string note) {
        _fifth.Add(note);
        _allNotes.Add(note);
    }

    public void AddSixth (string note) {
        _sixth.Add(note);
        _allNotes.Add(note);
    }

    public void AddSeventh (string note) {
        _seventh.Add(note);
        _allNotes.Add(note);
    }


	#endregion

}
