// Riff.cs
// ©2016 Team 95

using Route95.World;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Route95.Music {

    /// <summary>
    /// Class to store sequences of beats, as well as effect data.
    /// </summary>
    [System.Serializable]
    public class Riff {

        #region NonSerialized Riff Vars

        /// <summary>
        /// Maximum number of 16th notes in a riff.
        /// </summary>
        [NonSerialized]
        public const int MAX_BEATS = 16;

        /// <summary>
        /// Instrument used in riff.
        /// </summary>
        [NonSerialized]
        Instrument _instrument;

        /// <summary>
        /// Source to use to play notes.
        /// </summary>
        [NonSerialized]
        AudioSource _source;

        /// <summary>
        /// Distortion filter.
        /// </summary>
        [NonSerialized]
        AudioDistortionFilter _distortion;

        /// <summary>
        /// Tremolo filter.
        /// </summary>
        [NonSerialized]
        AudioTremoloFilter _tremolo;

        /// <summary>
        /// Chorus filter.
        /// </summary>
        [NonSerialized]
        AudioChorusFilter _chorus;

        /// <summary>
        /// Flanger filter.
        /// </summary>
        [NonSerialized]
        AudioFlangerFilter _flanger;

        /// <summary>
        /// Echo filter.
        /// </summary>
        [NonSerialized]
        AudioEchoFilter _echo;

        /// <summary>
        /// Reverb filter.
        /// </summary>
        [NonSerialized]
        AudioReverbFilter _reverb;

        #endregion
        #region Serialized Riff Vars

        /// <summary>
        /// User-defined name of the riff.
        /// </summary>
        [SerializeField]
        string _name;

        /// <summary>
        /// Index of instrument used for this riff.
        /// </summary>
        [SerializeField]
        int _instrumentIndex = 0;

        /// <summary>
        /// List of indices of all beats used in this riff.
        /// </summary>
        [SerializeField]
        List<int> _beatIndices;

        /// <summary>
        /// If true, sounds will cut themselves off.
        /// </summary>
        [SerializeField]
        bool _cutSelf = true;

        /// <summary>
        /// Volume scaler for all riff notes.
        /// </summary>
        [SerializeField]
        float _volume = 0.8f;

        /// <summary>
        /// Stereo panning value.
        /// </summary>
        [SerializeField]
        float _panning = 0f;

        /// <summary>
        /// Project-assigned riff index/
        /// </summary>
        [SerializeField]
        int _index;

        /// <summary>
        /// Is distortion enabled on this riff?
        /// </summary>
        [SerializeField]
        bool _distortionEnabled = false;

        /// <summary>
        /// Level of distortion.
        /// </summary>
        [SerializeField]
        float _distortionLevel = 0f;

        /// <summary>
        /// Is tremolo enabled on this riff?
        /// </summary>
        [SerializeField]
        bool _tremoloEnabled = false;

        /// <summary>
        /// Rate of tremolo oscillation.
        /// </summary>
        [SerializeField]
        float _tremoloRate = 0f;

        /// <summary>
        /// Amplitude of tremolo oscillation
        /// </summary>
        [SerializeField]
        float _tremoloDepth = 0f;

        /// <summary>
        /// Is chorus enabled on this riff?
        /// </summary>
        [SerializeField]
        bool _chorusEnabled = false;

        /// <summary>
        /// Ratio of dry/wet output.
        /// </summary>
        [SerializeField]
        float _chorusDryMix = 0f;

        /// <summary>
        /// Rate of chorus oscillation.
        /// </summary>
        [SerializeField]
        float _chorusRate = 0f;

        /// <summary>
        /// Depth of chorus oscillation.
        /// </summary>
        [SerializeField]
        float _chorusDepth = 0f;

        /// <summary>
        /// Is echo enabled on this riff?
        /// </summary>
        [SerializeField]
        bool _echoEnabled = false;

        /// <summary>
        /// Ratio of echo output to previous output.
        /// </summary>
        [SerializeField]
        float _echoDecayRatio = 1f;

        /// <summary>
        /// Echo delay.
        /// </summary>
        [SerializeField]
        float _echoDelay = 0f;

        /// <summary>
        /// Echo dry/wet ratio.
        /// </summary>
        [SerializeField]
        float _echoDryMix = 0f;

        /// <summary>
        /// Is reverb enabled on this riff?
        /// </summary>
        [SerializeField]
        bool _reverbEnabled = false;

        /// <summary>
        /// Reverb decay time.
        /// </summary>
        [SerializeField]
        float _reverbDecayTime = 0f;

        /// <summary>
        /// Level of reverb.
        /// </summary>
        [SerializeField]
        float _reverbLevel = 0f;

        /// <summary>
        /// Is flanger enabled on this riff?
        /// </summary>
        [SerializeField]
        bool _flangerEnabled = false;

        /// <summary>
        /// Rate of flanger oscillation.
        /// </summary>
        [SerializeField]
        float _flangerRate = Mathf.PI / 32f;

        /// <summary>
        /// Flanger dry/wet mix.
        /// </summary>
        [SerializeField]
        float _flangerDryMix = 0f;

        #endregion
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Riff(int instIndex = 0) {
            _beatIndices = new List<int>();
            _instrumentIndex = instIndex;
            Refresh();
        }

        #endregion
        #region Properties

        /// <summary>
        /// Returns the index of this riff.
        /// </summary>
        public int Index { get { return _index; } }

        /// <summary>
        /// Returns the index of the instrument used in this riff (read-only).
        /// </summary>
        public int InstrumentIndex { get { return _instrumentIndex; } }

        /// <summary>
        /// Returns the length of the riff, in number of beats (read-only).
        /// </summary>
        public int Length { get { return _beatIndices.Count; } }

        #endregion
        #region Methods

        public void Refresh() {

            // Init references
            if (_instrument == null) _instrument = Instrument.AllInstruments[_instrumentIndex];
            _source = MusicManager.Instance.GetAudioSource(_instrument);
            _distortion = _source.GetComponent<AudioDistortionFilter>();
            _tremolo = _source.GetComponent<AudioTremoloFilter>();
            _chorus = _source.GetComponent<AudioChorusFilter>();
            _flanger = _source.GetComponent<AudioFlangerFilter>();
            _echo = _source.GetComponent<AudioEchoFilter>();
            _reverb = _source.GetComponent<AudioReverbFilter>();
        }

        /// <summary>
        /// Checks if a note exists in the riff.
        /// </summary>
        /// <param name="filename">Note filename.</param>
        /// <param name="pos">Beat position.</param>
        public bool Lookup(string filename, int pos) {
            Note temp = new Note(filename);
            return Lookup(temp, pos);
        }

        /// <summary>
        /// Checks if a note exists in the riff.
        /// </summary>
        /// <param name="newNote">Note.</param>
        /// <param name="pos">Beat position.</param>
        public bool Lookup(Note newNote, int pos) {
            Song song = MusicManager.Instance.CurrentSong;
            Beat beat = song.Beats[_beatIndices[pos]];

            try {

                // Check each note in beat
                foreach (Note note in beat.Notes)
                    if (note.Filename == newNote.Filename) return true;

                return false;

                // Catch invalid beat checks
            }
            catch (ArgumentOutOfRangeException) {
                Debug.LogError("Tried to access pos " + pos + " in " + Length + "-long riff!");
                return false;
            }
        }

        /// <summary>
        /// Gets the note with the given filename and position.
        /// </summary>
        public Note GetNote(string fileName, int pos) {
            Song song = MusicManager.Instance.CurrentSong;
            Beat beat = song.Beats[_beatIndices[pos]];

            foreach (Note note in beat.Notes)
                if (note.Filename == fileName) return note;

            return null;
        }

        /// <summary>
        /// Toggles a note at the given position.
        /// </summary>
        /// <param name="newNote">Note to toggle.</param>
        /// <param name="pos">Position at which to add note.</param>
        public bool Toggle(Note newNote, int pos) {
            Song song = MusicManager.Instance.CurrentSong;
            Beat beat = song.Beats[_beatIndices[pos]];
            Instrument instrument = Instrument.AllInstruments[_instrumentIndex];

            // Check if note exists
            if (Lookup(newNote, pos)) {
                RemoveNote(newNote, pos);
                return false;
            }

            // If doesn't exist, add note
            beat.Add(newNote);

            // Play note
            _source.panStereo = _panning;
            newNote.PlayNote(_source, _volume, true);

            // Do environmental effects
            if (instrument.InstrumentType == Instrument.Type.Percussion) {
                WorldManager.Instance.ShowWeatherEffect (newNote);
            }
            else {
                if (instrument == MelodicInstrument.ElectricBass) WorldManager.Instance.DeformRandom();
                else {
                    switch (instrument.InstrumentFamily) {
                        case Instrument.Family.Guitar:
                            MusicManager.Instance.RegisterGuitarNote();
                            break;
                        case Instrument.Family.Keyboard:
                            MusicManager.Instance.RegisterKeyboardNote();
                            break;
                        case Instrument.Family.Brass:
                            MusicManager.Instance.RegisterBrassNote();
                            break;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Plays all notes at the given beat.
        /// </summary>
        /// <param name="pos">Beat to play notes from.</param>
        public void PlayRiff(int pos) {
            try {
                Song song = MusicManager.Instance.CurrentSong;
                Beat beat = song.Beats[_beatIndices[pos]];

                // Skip if empty
                if (beat.NoteCount == 0) return;

                _source.panStereo = _panning;

                // Update effect levels
                if (_distortionEnabled) {
                    _distortion.enabled = true;
                    _distortion.distortionLevel = _distortionLevel;
                }
                else _distortion.enabled = false;

                if (_tremoloEnabled) {
                    _tremolo.enabled = true;
                    _tremolo.depth = _tremoloDepth;
                    _tremolo.rate = _tremoloRate;
                }
                else _tremolo.enabled = false;

                if (_echoEnabled) {
                    _echo.enabled = true;
                    _echo.decayRatio = _echoDecayRatio;
                    _echo.delay = _echoDelay;
                    _echo.dryMix = _echoDryMix;
                }
                else _echo.enabled = false;

                if (_reverbEnabled) {
                    _reverb.enabled = true;
                    _reverb.decayTime = _reverbDecayTime;
                    _reverb.reverbLevel = _reverbLevel;
                }
                else _reverb.enabled = false;

                if (_chorusEnabled) {
                    _chorus.enabled = true;
                    _chorus.dryMix = _chorusDryMix;
                    _chorus.rate = _chorusRate;
                    _chorus.depth = _chorusDepth;
                }
                else _chorus.enabled = false;

                if (_flangerEnabled) {
                    _flanger.enabled = true;
                    _flanger.rate = _flangerRate;
                    _flanger.dryMix = _flangerDryMix;
                }
                else _flanger.enabled = false;

                // Cutoff
                if (_cutSelf) _source.Stop();

                // For each note
                foreach (Note note in beat.Notes) {

                    // Play note
                    note.PlayNote(_source, _volume);

                    // Do environmental effects
                    if (_instrument.InstrumentType == Instrument.Type.Percussion) {
                        WorldManager.Instance.ShowWeatherEffect (note);
                    }
                    else {
                        if (_instrument == MelodicInstrument.ElectricBass)
                            WorldManager.Instance.DeformRandom();

                        else {
                            switch (_instrument.InstrumentFamily) {
                                case Instrument.Family.Guitar:
                                    MusicManager.Instance.RegisterGuitarNote();
                                    break;
                                case Instrument.Family.Keyboard:
                                    MusicManager.Instance.RegisterKeyboardNote();
                                    break;
                                case Instrument.Family.Brass:
                                    MusicManager.Instance.RegisterBrassNote();
                                    break;
                            }

                        }
                    }
                }
            }
            catch (ArgumentOutOfRangeException) {
                Debug.LogError("Tried to play out of range of song! Pos: " + pos);
            }
        }

        /// <summary>
        /// Removes the given note from a beat.
        /// </summary>
        /// <param name="newNote">Note to remove.</param>
        /// <param name="pos">Beat to remove note from.</param>
        public void RemoveNote(Note newNote, int pos) {
            Song song = MusicManager.Instance.CurrentSong;
            Beat beat = song.Beats[_beatIndices[pos]];


            // Look for note in beat
            foreach (Note note in beat.Notes)
                if (note.Filename == newNote.Filename) {
                    beat.Notes.Remove(note);
                    return;
                }
        }

        /// <summary>
        /// Removes all notes at the given beat.
        /// </summary>
        /// <param name="pos">Beat to remove notes at.</param>
        public void Clear(int pos) {
            Song song = MusicManager.Instance.CurrentSong;
            Beat beat = song.Beats[_beatIndices[pos]];
            beat.Clear();
        }

        /// <summary>
        /// Returns the volume of a note with the given filename.
        /// </summary>
        /// <param name="fileName">Filename of note to find.</param>
        /// <param name="pos">Beat to look in.</param>
        public float VolumeOfNote(string fileName, int pos) {
            Song song = MusicManager.Instance.CurrentSong;
            Beat beat = song.Beats[_beatIndices[pos]];

            // Check each note in beat
            foreach (Note note in beat.Notes)
                if (note.Filename == fileName) return note.Volume;

            // Return 1f if not found
            return 1f;
        }

        #endregion
    }
}
