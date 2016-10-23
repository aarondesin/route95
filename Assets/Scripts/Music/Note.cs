// Note.cs
// ©2016 Team 95

using UnityEngine;

namespace Route95.Music {

    /// <summary>
    /// Class to store note data.
    /// </summary>
    [System.Serializable]
    public class Note {

        /// <summary>
        /// Enum for the type of percussion.
        /// </summary>
        public enum PercType {
            None,
            Kick,
            Snare,
            Tom,
            Shaker,
            Cymbal,
            Hat,
            Wood
        }

        #region Note Vars

        /// <summary>
        /// Default note volume.
        /// </summary>
        const float DEFAULT_VOLUME = 0.75f;

        /// <summary>
        /// Default note duration.
        /// </summary>
        const float DEFAULT_DURATION = 1f;

        /// <summary>
        /// Filename of audio clip.
        /// </summary>
        [SerializeField]
        string _filename;

        /// <summary>
        /// Note volume.
        /// </summary>
        [SerializeField]
        float _volume = DEFAULT_VOLUME;

        /// <summary>
        /// Note duration.
        /// </summary>
        [SerializeField]
        float _duration = DEFAULT_DURATION;

        /// <summary>
        /// Type of percussion (if applicable).
        /// </summary>
        [SerializeField]
        PercType _percType = PercType.None;

        #endregion
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Note() {
            _filename = null;
        }

        /// <summary>
        /// Filename/volume/duration constructor.
        /// </summary>
        /// <param name="fileName">Filename to use.</param>
        /// <param name="vol">Initial volume.</param>
        /// <param name="dur">Initial duration.</param>
        public Note(string fileName, float vol = DEFAULT_VOLUME, float dur = DEFAULT_DURATION) {

            // Check if MM has sound
            if (!MusicManager.SoundClips.ContainsKey(fileName)) {
                Debug.LogError("Note.Note(): filename \"" + fileName + "\" invalid!");
                _filename = null;
            }
            else _filename = fileName;

            // Assign percussion type, if applicable
            if (IsKick())        _percType = PercType.Kick;
            else if (IsCymbal()) _percType = PercType.Cymbal;
            else if (IsHat())    _percType = PercType.Hat;
            else if (IsShaker()) _percType = PercType.Shaker;
            else if (IsSnare())  _percType = PercType.Snare;
            else if (IsTom())    _percType = PercType.Tom;
            else if (IsWood())   _percType = PercType.Wood;

            // Set vars
            _volume = vol;
            _duration = dur;
        }

        #endregion
        #region Properties

        /// <summary>
        /// Returns this note's filename (read-only).
        /// </summary>
        public string Filename { get { return _filename; } }

        /// <summary>
        /// Returns the volume of this note (read-only).
        /// </summary>
        public float Volume { get { return _volume; } }

        #endregion
        #region Methods

        /// <summary>
        /// Plays a note.
        /// </summary>
        /// <param name="source">AudioSource on which to play the note.</param>
        /// <param name="newVolume">Volume scaler.</param>
        /// <param name="cutoff">Cut the AudioSource before playing?</param>
        public void PlayNote(AudioSource source, float newVolume = 1f, bool cutoff = false) {
            if (!source.enabled) source.enabled = true;
            else if (cutoff) source.Stop();
            source.PlayOneShot(MusicManager.SoundClips[_filename], newVolume * _volume * source.volume);
        }

        /// <summary>
        /// Returns whether or not the other note is the same.
        /// </summary>
        /// <param name="other">Note to compare.</param>
        public bool Equals(Note other) {
            return _filename == other._filename || (this == null && other == null);
        }

        /// <summary>
        /// Checks if a note is a kick.
        /// </summary>
        bool IsKick() {
            return _filename == "Audio/Instruments/Percussion/RockDrums/RockDrums_Kick";
        }

        /// <summary>
        /// Checks if a note is a snare.
        /// </summary>
        bool IsSnare() {
            return _filename == "Audio/Instruments/Percussion/RockDrums/RockDrums_Snare";
        }

        /// <summary>
        /// Checks if a note is a tom.
        /// </summary>
        bool IsTom() {
            return _filename == "Audio/Instruments/Percussion/RockDrums/RockDrums_LowTom" ||
                _filename == "Audio/Instruments/Percussion/RockDrums/RockDrums_MidTom" ||
                _filename == "Audio/Instruments/Percussion/RockDrums/RockDrums_HiTom";
        }

        /// <summary>
        /// Checks if a note is a shaker.
        /// </summary>
        bool IsShaker() {
            return
                _filename == "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Maracas1" ||
                _filename == "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Maracas2";
        }

        /// <summary>
        /// Checks if a note is a cymbal.
        /// </summary>
        public bool IsCymbal() {
            return _filename == "Audio/Instruments/Percussion/RockDrums/RockDrums_Crash";
        }

        /// <summary>
        /// Checks if a note is a hat.
        /// </summary>
        public bool IsHat() {
            return
                _filename == "Audio/Instruments/Percussion/RockDrums/RockDrums_Hat" ||
                _filename == "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Tambourine";
        }

        /// <summary>
        /// Checks if a note is wood (claves, castinets, cowbell, jam block).
        /// </summary>
        public bool IsWood() {
            return
                _filename == "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Castinets" ||
                _filename == "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Claves" ||
                _filename == "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Cowbell" ||
                _filename == "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Cowbell2" ||
                _filename == "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_JamBlock";
        }

        #endregion
    }
}