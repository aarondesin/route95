using Route95.Core;

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Route95.Music {

    /// <summary>
    /// Class to store all project data.
    /// </summary>
    [System.Serializable]
    public class Project {

        #region Vars

        [Tooltip("Name of the project.")]
        [SerializeField]
        public string name;

        [Tooltip("File paths of all songs in the project.")]
        [SerializeField]
        public List<string> songPaths;

        [SerializeField]
        List<Song> _songs;            // All songs used in the project

        [NonSerialized]
        private List<SongPiece> songPieces; // All songpieces used in the project

        [NonSerialized]
        private List<Measure> measures;     // All measures used in the project

        [NonSerialized]
        private List<Riff> riffs;           // All riffs used in the project

        [NonSerialized]
        private List<Beat> beats;           // All beats used in the project

        #endregion
        #region Properties

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
            name = "New Project";

            // Init lists
            songPaths = new List<string>();
            _songs = new List<Song>();
            songPieces = new List<SongPiece>();
            measures = new List<Measure>();
            riffs = new List<Riff>();
            beats = new List<Beat>();
        }

        /// <summary>
        /// Initializes any data not loaded.
        /// Called after deserialization.
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized()]
        public void Refresh(StreamingContext context) {

            // Init any null lists
            if (songPaths == null) songPaths = new List<string>();
            if (_songs == null) _songs = new List<Song>();
            if (songPieces == null) songPieces = new List<SongPiece>();
            if (measures == null) measures = new List<Measure>();
            if (riffs == null) riffs = new List<Riff>();
            if (beats == null) beats = new List<Beat>();

            // Load all listed songs
            foreach (string path in songPaths) AddSong(SaveLoad.LoadSong(path));
        }

        /// <summary>
        /// Generates paths for all songs used in the project.
        /// Called on serialization.
        /// </summary>
        /// <param name="context"></param>
        [OnSerializing()]
        internal void UpdatePaths(StreamingContext context) {

            // Refresh song paths
            songPaths.Clear();

            // For each song in the project
            foreach (Song song in _songs) {

                // Save song
                SaveLoad.SaveSong(song);

                // Generate and add path
                string path = Application.dataPath + GameManager.Instance.SongSaveFolder +
                    song.Name + SaveLoad.songSaveExtension;
                songPaths.Add(path);
            }
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

                if (songPieces == null) songPieces = new List<SongPiece>();
                if (song.SongPieces != null) songPieces.AddRange(song.SongPieces);

                if (measures == null) measures = new List<Measure>();
                if (song.Measures != null) measures.AddRange(song.Measures);

                if (riffs == null) riffs = new List<Riff>();
                if (song.Riffs != null) riffs.AddRange(song.Riffs);

                if (beats == null) beats = new List<Beat>();
                if (song.Beats != null) beats.AddRange(song.Beats);
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
