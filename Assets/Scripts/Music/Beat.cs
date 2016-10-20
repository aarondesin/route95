using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Route95.Music {

    /// <summary>
    /// Class to store all notes at a certain beat.
    /// </summary>
    [Serializable]
    public class Beat {

        #region Serialized Vars

        /// <summary>
        /// Project-specific index.
        /// </summary>
        [SerializeField]
        int _index;

        /// <summary>
        /// List of notes.
        /// </summary>
        [SerializeField]
        List<Note> _notes;

        #endregion
        #region Properties

        /// <summary>
        /// Returns the list of notes in this beat (read-only).
        /// </summary>
        public List<Note> Notes { get { return _notes; } }

        /// <summary>
        /// Get: Returns the index of this beat.
        /// Set: Sets the index of this beat.
        /// </summary>
        public int Index {
            get { return _index; }
            set { _index = value; }
        }

        #endregion
        #region Beat Methods

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Beat() {
            _notes = new List<Note>();
        }

        /// <summary>
        /// Adds a note to the beat.
        /// </summary>
        /// <param name="note">Note to add.</param>
        public void Add(Note note) {
            _notes.Add(note);
        }

        /// <summary>
        /// Removes a note from the beat.
        /// </summary>
        /// <param name="note">Note to remove.</param>
        public void Remove(Note note) {
            _notes.Remove(note);
        }

        /// <summary>
        /// Clears all notes in the beat.
        /// </summary>
        public void Clear() {
            _notes.Clear();
        }

        /// <summary>
        /// Returns the number of notes in the beat (read-only).
        /// </summary>
        public int NoteCount { get { return _notes.Count; }  }

        #endregion
    }
}
