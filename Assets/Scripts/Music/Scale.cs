// Scale.cs
// ©2016 Team 95

using System.Collections.Generic;

namespace Route95.Music {

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

        /// <summary>
        /// All roots/octaves in scale.
        /// </summary>
        List<string> _root;

        /// <summary>
        /// All seconds in scale.
        /// </summary>
        List<string> _second;

        /// <summary>
        /// All thirds in scale.
        /// </summary>
        List<string> _third;

        /// <summary>
        /// All fourths in scale
        /// </summary>
        List<string> _fourth;

        /// <summary>
        /// All fifths in scale.
        /// </summary>
        List<string> _fifth;

        /// <summary>
        /// All sixths in scale.
        /// </summary>
        List<string> _sixth;

        /// <summary>
        /// All sevenths in scale.
        /// </summary>
        List<string> _seventh;

        /// <summary>
        /// All notes in scale.
        /// </summary>
        List<string> _allNotes;

        #endregion
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Scale() {

            // Init lists
            _root = new List<string>();
            _second = new List<string>();
            _third = new List<string>();
            _fourth = new List<string>();
            _fifth = new List<string>();
            _sixth = new List<string>();
            _seventh = new List<string>();

            _allNotes = new List<string>();
        }

        #endregion
        #region Properties

        /// <summary>
        /// Returns the number of notes in this scale (read-only).
        /// </summary>
        public int NoteCount { get { return _allNotes.Count; } }

        #endregion
        #region Methods

        /// <summary>
        /// Returns the note at the given index.
        /// </summary>
        public string NoteAt(int index) {
            return _allNotes[index];
        }

        /// <summary>
        /// Returns true if the given note is in this scale.
        /// </summary>
        public bool HasNote(string note) {
            return _allNotes.Contains(note);
        }

        /// <summary>
        /// Adds a root note.
        /// </summary>
        public void AddRoot(string note) {
            _root.Add(note);
            _allNotes.Add(note);
        }

        /// <summary>
        /// Adds a second note.
        /// </summary>
        public void AddSecond(string note) {
            _second.Add(note);
            _allNotes.Add(note);
        }

        /// <summary>
        /// Adds a third note.
        /// </summary>
        public void AddThird(string note) {
            _third.Add(note);
            _allNotes.Add(note);
        }

        /// <summary>
        /// Adds a fourth note.
        /// </summary>
        public void AddFourth(string note) {
            _fourth.Add(note);
            _allNotes.Add(note);
        }

        /// <summary>
        /// Adds a fifth note.
        /// </summary>
        public void AddFifth(string note) {
            _fifth.Add(note);
            _allNotes.Add(note);
        }

        /// <summary>
        /// Adds a sixth note.
        /// </summary>
        public void AddSixth(string note) {
            _sixth.Add(note);
            _allNotes.Add(note);
        }

        /// <summary>
        /// Adds a seventh note.
        /// </summary>
        public void AddSeventh(string note) {
            _seventh.Add(note);
            _allNotes.Add(note);
        }

        #endregion
    }
}
