﻿// Song.cs
// ©2016 Team 95

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using UnityEngine;

namespace Route95.Music {

    /// <summary>
    /// Class to store all song data.
    /// </summary>
    [System.Serializable]
    public class Song {

        #region Serialized Song Vars

        /// <summary>
        /// User-defined name of song.
        /// </summary>
        [SerializeField]
        string _name;

        /// <summary>
        /// Musical key of song.
        /// </summary>
        [SerializeField]
        Key _key = Key.None;

        /// <summary>
        /// Index of scale used in song.
        /// </summary>
        [SerializeField]
        int _scale = -1;

        /// <summary>
        /// All song pieces used in song.
        /// </summary>
        [SerializeField]
        List<SongPiece> _songPieces;

        /// <summary>
        /// All measures used in song.
        /// </summary>
        [SerializeField]
        List<Measure> _measures;

        /// <summary>
        /// All riffs used in song.
        /// </summary>
        [SerializeField]
        List<Riff> _riffs;

        /// <summary>
        /// All beats used in song.
        /// </summary>
        [SerializeField]
        List<Beat> _beats;

        /// <summary>
        /// Indices of song pieces used.
        /// </summary>
        [SerializeField]
        List<int> _songPieceIndices;

        /// <summary>
        /// Get: Returns the key of this song.
        /// Set: Sets the key of this song.
        /// </summary>
        public Key Key {
            get { return _key; }
            set { _key = value; }
        }

        /// <summary>
        /// Get: Returns the scale of this song.
        /// Set: Sets the scale of this song.
        /// </summary>
        public int Scale {
            get { return _scale; }
            set { _scale = value; }
        }

        /// <summary>
        /// Returns the list of song piece indices in this song (read-only).
        /// </summary>
        public List<int> SongPieceIndices {
            get { return _songPieceIndices; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Song() {
            // Default name
            _name = "New Song";

            // Init lists
            _songPieceIndices = new List<int>();
            _songPieces = new List<SongPiece>();
            _measures = new List<Measure>();
            _riffs = new List<Riff>();
            _beats = new List<Beat>();
        }

        /// <summary>
        /// Refreshes all data that was not loaded.
        /// Called after deserialization.
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized()]
        internal void Refresh(StreamingContext context) {

            // Init any uninitialized lists
            if (_songPieceIndices == null) _songPieceIndices = new List<int>();
            if (_songPieces == null) _songPieces = new List<SongPiece>();
            if (_measures == null) _measures = new List<Measure>();
            if (_riffs == null) _riffs = new List<Riff>();
            if (_beats == null) _beats = new List<Beat>();

            // Refresh riffs
            foreach (Riff riff in _riffs) riff.Refresh();
        }

        /// <summary>
        /// Get: Returns the name of the song.
        /// Set: Sets the name of the song.
        /// </summary>
        public string Name {
            get { return _name; }
            set { _name = value; }
        }

        public List<Beat> Beats { get { return _beats; } }

        public List<Riff> Riffs { get { return _riffs; } }

        public List<Measure> Measures { get { return _measures; } }

        /// <summary>
        /// Returns the number of beats in the song.
        /// </summary>
        public int BeatsCount {
            get {
                return _songPieceIndices.Count * 16;
            }
        }

        /// <summary>
        /// Returns the number of measures in the song.
        /// </summary>
        public int MeasuresCount {
            get {
                return _songPieceIndices.Count;
            }
        }

        public List<SongPiece> SongPieces { get { return _songPieces; } }

        /// <summary>
        /// Checks if a song has the same name as another.
        /// </summary>
        /// <param name="other">Song to compare to.</param>
        /// <returns>True if the other song has the same name.</returns>
        public bool Equals(Song other) {
            return _name == other.Name;
        }

        /// <summary>
        /// Creates a new song piece and registers it.
        /// </summary>
        /// <returns></returns>
        public SongPiece NewSongPiece() {

            // Create new song piece
            SongPiece songPiece = new SongPiece();

            // Init measures
            Measure measure = NewMeasure();
            songPiece.measureIndices.Add(measure.index);

            // Register song piece
            RegisterSongPiece(songPiece);

            return songPiece;
        }

        /// <summary>
        /// Adds a song piece to the song and assigns it an index.
        /// </summary>
        /// <param name="songPiece">Song piece to register.</param>
        public void RegisterSongPiece(SongPiece songPiece) {
            songPiece.index = _songPieces.Count;
            _songPieces.Add(songPiece);
            _songPieceIndices.Add(songPiece.index);
        }

        /// <summary>
        /// Adds a riff to the song and assigns it an index,
        /// and does the same with all of its beats.
        /// </summary>
        /// <param name="riff">Riff to register.</param>
        public void RegisterRiff(Riff riff) {
            riff.index = _riffs.Count;
            _riffs.Add(riff);
            for (int i = 0; i < 16; i++) {
                Beat beat = new Beat();
                beat.Index = _beats.Count;
                riff.beatIndices.Add(beat.Index);
                _beats.Add(beat);
            }
        }

        /// <summary>
        /// Adds a new measure and registers it.
        /// </summary>
        /// <returns></returns>
        public Measure NewMeasure() {
            Measure measure = new Measure();
            measure.index = _measures.Count;
            _measures.Add(measure);
            return measure;
        }

        /// <summary>
        /// Plays all notes at the given beat.
        /// </summary>
        /// <param name="pos">Beat at which to play notes.</param>
        public void PlaySong(int pos) {
            try {
                SongPiece songPiece = _songPieces[_songPieceIndices[pos / Riff.MAX_BEATS]];
                Measure measure = _measures[songPiece.measureIndices[0]];

                // Play all notes
                foreach (int i in measure.riffIndices) _riffs[i].PlayRiff(pos % Riff.MAX_BEATS);

            }
            catch (ArgumentOutOfRangeException) {
                Debug.LogError("Song.PlaySong(): index out of range! " + pos);
            }
        }

        /// <summary>
        /// Toggles a riff at the given position.
        /// </summary>
        /// <param name="newRiff">Riff to toggle.</param>
        /// <param name="pos">Measure to toggle.</param>
        public void ToggleRiff(Riff newRiff, int pos) {
            SongPiece songPiece = _songPieces[_songPieceIndices[pos]];
            Measure measure = _measures[songPiece.measureIndices[0]];

            // For each riff in measure
            foreach (int r in measure.riffIndices) {
                Riff riff = _riffs[r];

                // Skip if riff not the same instrument
                if (riff.instrument != newRiff.instrument) continue;

                // Remove riff if it exists
                if (newRiff == riff) measure.riffIndices.Remove(newRiff.index);

                // Replace riff with new one
                else {
                    measure.riffIndices.Remove(riff.index);
                    measure.riffIndices.InsertSorted<int>(newRiff.index, false);
                }

                return;
            }

            // Riff not already there
            measure.riffIndices.InsertSorted<int>(newRiff.index, false);
        }

        #endregion
    }
}
