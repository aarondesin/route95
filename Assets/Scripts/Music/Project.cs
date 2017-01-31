// Project.cs
// ©2016 Team 95

using Route95.Core;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using UnityEngine;

namespace Route95.Music {

    /// <summary>
    /// Class to store all project data.
    /// </summary>
    [Serializable]
    public class Project {

        #region Vars

        /// <summary>
        /// Name of the project.
        /// </summary>
        [Tooltip("Name of the project.")]
        [SerializeField]
        string _name;

        /// <summary>
        /// File paths of all songs in the project.
        /// </summary>
        [Tooltip("File paths of all songs in the project.")]
        [SerializeField]
        List<string> _songPaths;

        /// <summary>
        /// All songs used in the project.
        /// </summary>
        [SerializeField]
        List<Song> _songs;

        /// <summary>
        /// All songpieces used in the project.
        /// </summary>
        [NonSerialized]
        List<SongPiece> _songPieces;

        /// <summary>
        /// All measures used in the project.
        /// </summary>
        [NonSerialized]
        List<Measure> _measures;

        /// <summary>
        /// All riffs used in the project.
        /// </summary>
        [NonSerialized]
        List<Riff> _riffs;

        /// <summary>
        /// All beats used in the project.
        /// </summary>
        [NonSerialized]
        List<Beat> _beats;

        #endregion
        #region Properties

        /// <summary>
        /// Returns the name of this project (read-only).
        /// </summary>
        public string Name { get { return _name; } }

        /// <summary>
        /// Returns a list of all songs in this project (read-only).
        /// </summary>
        public List<Song> Songs { get { return _songs; } }

        /// <summary>
        /// Returns whether or not the project has no songs (read-only).
        /// </summary>
        public bool Empty { get { return _songs.Count == 0; } }

        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Project() {

            // Default name
            _name = "New Project";

            // Init lists
            _songPaths = new List<string>();
            _songs = new List<Song>();
            _songPieces = new List<SongPiece>();
            _measures = new List<Measure>();
            _riffs = new List<Riff>();
            _beats = new List<Beat>();
        }

        /// <summary>
        /// Initializes any data not loaded.
        /// Called after deserialization.
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized()]
        public void Refresh(StreamingContext context) {

            // Init any null lists
            if (_songPaths == null) _songPaths = new List<string>();
            if (_songs == null) _songs = new List<Song>();
            if (_songPieces == null) _songPieces = new List<SongPiece>();
            if (_measures == null) _measures = new List<Measure>();
            if (_riffs == null) _riffs = new List<Riff>();
            if (_beats == null) _beats = new List<Beat>();

            // Load all listed songs
            foreach (string path in _songPaths) AddSong(SaveLoad.LoadSong(path));
        }

        /// <summary>
        /// Generates paths for all songs used in the project.
        /// Called on serialization.
        /// </summary>
        /// <param name="context"></param>
        [OnSerializing()]
        internal void UpdatePaths(StreamingContext context) {

            // Refresh song paths
            _songPaths.Clear();

            // For each song in the project
            foreach (Song song in _songs) {

                // Save song
                SaveLoad.SaveSong(song);

                // Generate and add path
                string path = Application.dataPath + GameManager.Instance.SongSaveFolder +
                    song.Name + SaveLoad.SONG_SAVE_EXT;
                _songPaths.Add(path);
            }
        }

        /// <summary>
        /// Sets the name of this project.
        /// </summary>
        /// <param name="name">New project name.</param>
        public void SetName (string name) {
            _name = name;
        }

        /// <summary>
        /// Adds a song to the project.
        /// </summary>
        /// <param name="song">Song to add.</param>
        public void AddSong(Song song) {

            // Check if song is valid
            if (song == null) {
                Debug.LogError("Project.AddSong(): tried to add null song!");
                return;
            }

            // Search for duplicates
            Song foundSong = null;
            foreach (Song s in _songs) {
                if (song.Equals(s)) {
                    foundSong = s;
                    break;
                }
            }

            // If match found, add a copy of the song
            if (foundSong != null) _songs.Add(foundSong);

            // Otherwise, add song and all data
            else {
                if (_songs == null) _songs = new List<Song>();
                _songs.Add(song);

                if (_songPieces == null) _songPieces = new List<SongPiece>();
                if (song.SongPieces != null) _songPieces.AddRange(song.SongPieces);

                if (_measures == null) _measures = new List<Measure>();
                if (song.Measures != null) _measures.AddRange(song.Measures);

                if (_riffs == null) _riffs = new List<Riff>();
                if (song.Riffs != null) _riffs.AddRange(song.Riffs);

                if (_beats == null) _beats = new List<Beat>();
                if (song.Beats != null) _beats.AddRange(song.Beats);
            }
        }

        /// <summary>
        /// Removes a song at the specified index.
        /// </summary>
        /// <param name="index">Index of song to remove.</param>
        public void RemoveSong(int index) {

            // Remove song
            _songs.RemoveAt(index);
        }
    }
}
