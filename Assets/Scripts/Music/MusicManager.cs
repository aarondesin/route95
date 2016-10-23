// MusicManager.cs
// ©2016 Team 95

using Route95.Core;
using Route95.UI;
using Route95.World;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Audio;

namespace Route95.Music {

    /// <summary>
    /// All musical KeyManager.Instance.
    /// </summary>
    public enum Key {
        None,
        C,
        CSharp,
        D,
        DSharp,
        E,
        F,
        FSharp,
        G,
        GSharp,
        A,
        ASharp,
        B
    };

    /// <summary>
    /// All tempos.
    /// </summary>
    public enum Tempo {
        Slowest,
        Slower,
        Slow,
        Medium,
        Fast,
        Faster,
        Fastest,
        NUM_TEMPOS
    };

    /// <summary>
    /// Instanced MonoBehaviour class to manage all music-related operations.
    /// </summary>
    public class MusicManager : SingletonMonoBehaviour<MusicManager> {

        #region Nested Struct

        /// <summary>
        /// Struct to hold all relevant sound and playback data.
        /// </summary>
        public struct Sound {

            /// <summary>
            /// AudioClip to play.
            /// </summary>
            public AudioClip clip;

            /// <summary>
            /// AudioSource to use for playback.
            /// </summary>
            public AudioSource source;

            /// <summary>
            /// Note volume.
            /// </summary>
            public float volume;
        }

        #endregion
        #region MusicManager Vars

        //-----------------------------------------------------------------------------------------------------------------
        [Header("MusicManager Status")]

        /// <summary>
        /// Global MusicManager audio source.
        /// </summary>
        AudioSource _source;

        /// <summary>
        /// Time at which loading started.
        /// </summary>
        float _loadStartTime;

        /// <summary>
        /// Number of load tasks completed.
        /// </summary>
        int _loadOpsCompleted;

        /// <summary>
        /// Number of tasks to load.
        /// </summary>
        int _loadOpsToDo;

        /// <summary>
        /// Playing right now?
        /// </summary>
        [Tooltip("Is this playing right now?")]
        bool _isPlaying = false;

        /// <summary>
        /// Loop riffs?
        /// </summary>
        [Tooltip("Loop riffs?")]
        bool _loopRiffs = false;

        /// <summary>
        /// Loop playlists?
        /// </summary>
        [Tooltip("Loop playlists?")]
        bool _loopPlaylist = false;

        /// <summary>
        /// Current instrument in live mode.
        /// </summary>
        Instrument _currentInstrument =
            MelodicInstrument.ElectricGuitar;

        /// <summary>
        /// Current tempo.
        /// </summary>
        [Tooltip("Current tempo.")]
        Tempo _tempo = Tempo.Medium;

        /// <summary>
        /// Mappings of tempos to values.
        /// </summary>
        public static Dictionary<Tempo, float> TempoToFloat =
            new Dictionary<Tempo, float>() {
        { Tempo.Slowest, 50f },
        { Tempo.Slower, 70f },
        { Tempo.Slow, 90f },
        { Tempo.Medium, 110f },
        { Tempo.Fast, 130f },
        { Tempo.Faster, 150f },
        { Tempo.Fastest, 170f }
        };

        /// <summary>
        /// Index of currently playing song.
        /// </summary>
        int _currentPlayingSong;

        /// <summary>
        /// Current beat number.
        /// </summary>
        [Tooltip("Current beat.")]
        int _beat;

        /// <summary>
        /// Countdown to next beat (steps).
        /// </summary>
        float _beatTimer;

        /// <summary>
        /// Number of beats elapsed in current song.
        /// </summary>
        [Tooltip("Number of beats elapsed in current song.")]
        int _beatsElapsedInCurrentSong = 0;

        /// <summary>
        /// Number of beats elapsed in playlist.
        /// </summary>
        [Tooltip("Number of beats elapsed in current playlist.")]
        int _beatsElapsedInPlaylist = 0;

        /// <summary>
        /// Number of elapsed guitar notes in current song.
        /// </summary>
        [Tooltip("Number of elapsed guitar notes in current song.")]
        int _guitarNotes = 0;

        /// <summary>
        /// Current density of guitar notes.
        /// </summary>
        [Tooltip("Current density of guitar notes.")]
        [SerializeField]
        float _guitarDensity = 0f;

        /// <summary>
        /// Number of elapsed keyboard notes in current song.
        /// </summary>
        [Tooltip("Number of elapsed keyboard notes in current song.")]
        int _keyboardNotes = 0;

        /// <summary>
        /// Current density of keyboard notes.
        /// </summary>
        [Tooltip("Current density of keyboard notes.")]
        float _keyboardDensity = 0f;

        /// <summary>
        /// Number of elapsed brass notes in current song.
        /// </summary>
        [Tooltip("Number of elapsed brass notes in current song.")]
        public int _brassNotes = 0;

        /// <summary>
        /// Current density of brass notes.
        /// </summary>
        [Tooltip("Current density of brass notes.")]
        float _brassDensity = 0f;

        //-----------------------------------------------------------------------------------------------------------------
        [Header("Object References")]

        /// <summary>
        /// Mixer to use.
        /// </summary>
        [Tooltip("Mixer to use for MusicManager.Instance.")]
        [SerializeField]
        AudioMixer _mixer;

        /// <summary>
        /// AudioSource to use for UI sounds.
        /// </summary>
        [Tooltip("AudioSource to use for UI sounds.")]
        [SerializeField]
        AudioSource _oneShotSource;

        /// <summary>
        /// AudioSource to use for UI riff playback.
        /// </summary>
        [Tooltip("AudioSource to use for UI riff playback.")]
        [SerializeField]
        AudioSource _loopRiffSource;

        //-----------------------------------------------------------------------------------------------------------------
        [Header("Project References")]

        /// <summary>
        /// Current open project.
        /// </summary>
        Project _currentProject;

        /// <summary>
        /// Current song being played/edited.
        /// </summary>
        Song _currentSong;

        //-----------------------------------------------------------------------------------------------------------------
        [Header("Sounds")]

        /// <summary>
        /// Holds all loaded sounds.
        /// </summary>
        public static Dictionary<string, AudioClip> SoundClips =
            new Dictionary<string, AudioClip>();

        /// <summary>
        /// Mapping of instruments to AudioSources.
        /// </summary>
        Dictionary<Instrument, AudioSource> _instrumentAudioSources;

        /// <summary>
        /// If true, plays back riffs and not songs.
        /// </summary>
        bool _riffMode = true;

        #endregion
        #region Unity Callbacks

        new void Awake() {
            base.Awake();

            // Init vars
            _source = GetComponent<AudioSource>();

            // Load instrument lists
            Instrument.LoadInstruments();

            // Calculate number of objects to load
            _loadOpsToDo = Sounds.SoundsToLoad.Count + Instrument.AllInstruments.Count +
                Instrument.AllInstruments.Count * (Enum.GetValues(typeof(Key)).Length - 1) * ScaleInfo.AllScales.Count;
        }

        void FixedUpdate() {

            // Return if not playing or game is paused
            if (!_isPlaying) return;
            if (GameManager.Instance.Paused) return;

            // If new beat
            if (_beatTimer <= 0f) {
                switch (GameManager.Instance.CurrentState) {

                    // Setup mode (riff editor)
                    case GameManager.State.Setup:

                        if (_riffMode) {
                            
                            // Play riff note
                            RiffEditor.CurrentRiff.PlayRiff(_beat++);

                            // Wrap payback
                            if (_beat >= Riff.MAX_BEATS && _loopRiffs) _beat = 0;

                            // Decrement shaker density
                            WorldManager.Instance.shakers -= 2;

                        }
                        else {

                            if (_currentSong.BeatsCount == 0) return;

                            // If song is finished
                            if (_beat >= _currentSong.BeatsCount && _loopRiffs)
                                _beat = 0;

                            // Play notes
                            _currentSong.PlaySong(_beat++);


                        }
                        break;

                    // Live mode
                    case GameManager.State.Live:

                        if (!_currentProject.Empty) {

                            // If song is finished
                            if (_beat >= _currentSong.BeatsCount || _currentSong.BeatsCount == 0) {
                                _beat = 0;

                                // Reset vars
                                _beatsElapsedInCurrentSong = 0;
                                _guitarNotes = 0;
                                _keyboardNotes = 0;
                                _brassNotes = 0;

                                // Reset shaker density
                                WorldManager.Instance.shakers = 0;

                                // If another song available, switch
                                if (_currentPlayingSong < _currentProject.Songs.Count - 1) {
                                    DisableAllAudioSources();
                                    _currentPlayingSong++;
                                    _currentSong = _currentProject.Songs[_currentPlayingSong];

                                    // If no more songs to play
                                }
                                else {

                                    // Loop playlist if possible
                                    if (_loopPlaylist) {
                                        _currentPlayingSong = 0;
                                        _beatsElapsedInPlaylist = 0;

                                        // Otherwise go to postplay menu
                                    }
                                    else UIManager.Instance.SwitchToPostplay();
                                }
                            }

                            if (_currentSong.BeatsCount == 0) return;

                            // Play notes
                            _currentSong.PlaySong(_beat);

                            // Calculate song progress
                            float songTotalTime = _currentSong.BeatsCount * 7200f / TempoToFloat[_tempo] / 4f;
                            float songCurrentTime = (_beat * 7200f / TempoToFloat[_tempo] / 4f) + (7200f / TempoToFloat[_tempo] / 4f) - _beatTimer;
                            SongProgressBar.Instance.SetValue(songCurrentTime / songTotalTime);

                            // Increment vars
                            _beat++;
                            _beatsElapsedInCurrentSong++;
                            _beatsElapsedInPlaylist++;

                            // Update instrument densities
                            _guitarDensity = (float)_guitarNotes / (float)_beatsElapsedInCurrentSong;
                            _keyboardDensity = (float)_keyboardNotes / (float)_beatsElapsedInCurrentSong;
                            _brassDensity = (float)_brassNotes / (float)_beatsElapsedInCurrentSong;
                            if (WorldManager.Instance.shakers > 2) WorldManager.Instance.shakers -= 2;
                            WorldManager.Instance.roadVariance = Mathf.Clamp(_guitarDensity * 0.6f, 0.2f, 0.6f);
                            WorldManager.Instance.roadMaxSlope = Mathf.Clamp(_keyboardDensity * 0.002f, 0.002f, 0.001f);
                            WorldManager.Instance.decorationDensity = Mathf.Clamp(_brassDensity * 2f, 1f, 2f);
                        }
                        break;
                }

                // Reset beat timer
                _beatTimer = 7200f / TempoToFloat[_tempo] / 4f; // 3600f = 60 fps * 60 seconds

                // Decrement beat timer
            }
            else _beatTimer -= 1.667f;
        }

        #endregion
        #region Properties

        /// <summary>
        /// Returns the number of load operations to perform (read-only).
        /// </summary>
        public int LoadOps { get { return _loadOpsToDo; } }

        /// <summary>
        /// Current selected instrument.
        /// </summary>
        public Instrument CurrentInstrument {
            get { return _currentInstrument; }
            set { _currentInstrument = value; }
        }

        /// <summary>
        /// Currently opened project.
        /// </summary>
        public Project CurrentProject {
            get { return _currentProject; }
            set { _currentProject = value; }
        }

        /// <summary>
        /// Returns the index of the current playing sound (read-only).
        /// </summary>
        public int CurrentPlayingSong {
            get { return _currentPlayingSong; }
            set { _currentPlayingSong = value; }
        }

        /// <summary>
        /// Returns the current song being played/edited (read-only).
        /// </summary>
        public Song CurrentSong {
            get { return _currentSong; }
            set { _currentSong = value; }
        }

        #endregion
        #region Load Methods

        /// <summary>
        /// Begins loading MusicManager.
        /// </summary>
        public void Load() {

            // Save loading start time
            _loadStartTime = Time.realtimeSinceStartup;

            // Begin by loading sounds
            StartCoroutine("LoadSounds");
        }

        /// <summary>
        /// Coroutine to load all instrument sounds.
        /// </summary>
        IEnumerator LoadSounds() {

            // Update loading message
            LoadingScreen.Instance.SetLoadingMessage("Tuning instruments...");

            // Mark start time
            float startTime = Time.realtimeSinceStartup;
            int numLoaded = 0;

            // For each sound path
            foreach (KeyValuePair<string, List<string>> list in Sounds.SoundsToLoad) {
                foreach (string path in list.Value) {

                    // Load sound
                    LoadAudioClip(path);
                    numLoaded++;

                    // If over time
                    if (Time.realtimeSinceStartup - startTime > GameManager.TargetDeltaTime) {
                        yield return null;
                        startTime = Time.realtimeSinceStartup;
                        GameManager.Instance.ReportLoaded(numLoaded);
                        numLoaded = 0;
                    }
                }
            }

            // When done, start loading instruments
            yield return StartCoroutine("LoadInstruments");
        }

        /// <summary>
        /// Loads a sound.
        /// </summary>
        /// <param name="path">Sound path.</param>
        void LoadAudioClip(string path) {
            AudioClip sound = (AudioClip)Resources.Load(path);

            if (sound == null) Debug.LogError("Failed to load AudioClip at " + path);
            else SoundClips.Add(path, sound);
        }

        /// <summary>
        /// Coroutine to load instruments.
        /// </summary>
        IEnumerator LoadInstruments() {

            List<string> loadMessages = new List<string>() {
            "Renting instruments...",
            "Grabbing instruments...",
            "Unpacking instruments..."
        };

            // Update loading message
            LoadingScreen.Instance.SetLoadingMessage(loadMessages.Random());

            // Mark start time
            float startTime = Time.realtimeSinceStartup;
            int numLoaded = 0;

            // Init audio source dict
            _instrumentAudioSources = new Dictionary<Instrument, AudioSource>();

            // Foreach instrument
            for (int i = 0; i < Instrument.AllInstruments.Count; i++) {

                // Load instrument data
                Instrument.AllInstruments[i].Load();

                // Create instrument AudioSource GameObject
                GameObject obj = new GameObject(Instrument.AllInstruments[i].Name);
                AudioSource source = obj.AddComponent<AudioSource>();

                // Group instrument under MusicManager
                obj.transform.parent = transform.parent;

                // Connect AudioSource to mixer
                source.outputAudioMixerGroup = _mixer.FindMatchingGroups(obj.name)[0];

                // Connect instrument to AudioSource
                _instrumentAudioSources.Add(Instrument.AllInstruments[i], source);

                // Add distortion filter
                AudioDistortionFilter distortion = obj.AddComponent<AudioDistortionFilter>();
                distortion.enabled = false;

                // Add tremolo filter
                AudioTremoloFilter tremolo = obj.AddComponent<AudioTremoloFilter>();
                tremolo.enabled = false;

                // Add chorus filter
                AudioChorusFilter chorus = obj.AddComponent<AudioChorusFilter>();
                chorus.enabled = false;

                // Add flanger filter
                AudioFlangerFilter flanger = obj.AddComponent<AudioFlangerFilter>();
                flanger.enabled = false;

                // Add echo filter
                AudioEchoFilter echo = obj.AddComponent<AudioEchoFilter>();
                echo.enabled = false;

                // Add reverb filter based on MusicManager's reverb filter
                AudioReverbFilter reverb = obj.AddComponent<AudioReverbFilter>();
                AudioReverbFilter masterReverb = GetComponent<AudioReverbFilter>();
                reverb.dryLevel = masterReverb.dryLevel;
                reverb.room = masterReverb.room;
                reverb.roomHF = masterReverb.roomHF;
                reverb.roomLF = masterReverb.roomLF;
                reverb.decayTime = masterReverb.decayTime;
                reverb.decayHFRatio = masterReverb.decayHFRatio;
                reverb.reflectionsLevel = masterReverb.reflectionsLevel;
                reverb.reflectionsDelay = masterReverb.reflectionsDelay;
                reverb.reverbLevel = masterReverb.reverbLevel;
                reverb.hfReference = masterReverb.hfReference;
                reverb.lfReference = masterReverb.lfReference;
                reverb.diffusion = masterReverb.diffusion;
                reverb.density = masterReverb.density;
                reverb.enabled = false;

                numLoaded++;

                // If over time
                if (Time.realtimeSinceStartup - startTime > GameManager.TargetDeltaTime) {
                    yield return null;
                    startTime = Time.realtimeSinceStartup;
                    GameManager.Instance.ReportLoaded(numLoaded);
                    numLoaded = 0;
                }
            }

            // When done, start building scales
            if (_instrumentAudioSources.Count == Instrument.AllInstruments.Count)
                KeyManager.Instance.DoBuildScales();
            yield return null;
        }

        /// <summary>
        /// Finishes loading MusicManager.
        /// </summary>
        public void FinishLoading() {

            // Report loading time
            Debug.Log("MusicManager.Load(): finished in " + (Time.realtimeSinceStartup - _loadStartTime).ToString("0.0000") + " seconds.");

            // Start loading WorldManager
            WorldManager.Instance.Load();
        }

        #endregion
        #region MusicManager Methods

        /// <summary>
        /// Plays a one-shot AudioClip.
        /// </summary>
        /// <param name="sound">Sound to play.</param>
        /// <param name="volume">Volume scaler.</param>
        public static void PlayMenuSound(AudioClip sound, float volume = 1f) {
            Instance._source.PlayOneShot(sound, volume);
        }

        /// <summary>
        /// Creates a new, blank project.
        /// </summary>
        public void NewProject() {
            _currentProject = new Project();
        }

        /// <summary>
        /// Saves the current project.
        /// </summary>
        public void SaveCurrentProject() {
            SaveLoad.SaveCurrentProject();
        }

        /// <summary>
        /// Creates a new blank song and adds
        /// it to the current project.
        /// </summary>
        public void NewSong() {
            _currentSong = new Song();
            _currentProject.Songs.Add(_currentSong);
        }

        /// <summary>
        /// Saves the current song.
        /// </summary>
        public void SaveCurrentSong() {
            SaveLoad.SaveCurrentSong();
        }

        /// <summary>
        /// Sets the key of the current song.
        /// </summary>
        /// <param name="key">New key (int).</param>
        public void SetKey(int key) {
            _currentSong.Key = (Key)key;
        }

        /// <summary>
        /// Sets the key of the current song.
        /// </summary>
        /// <param name="key">New key.</param>
        public void SetKey(Key key) {
            SetKey((int)key);
        }

        /// <summary>
        /// Toggles whether to loop playlist.
        /// </summary>
        public void ToggleLoopPlaylist() {
            _loopPlaylist = !_loopPlaylist;
        }

        /// <summary>
        /// Toggles looping the current riff.
        /// </summary>
        public void PlayRiffLoop() {
            SongArrangeMenu.Instance.UpdateValue();
            _riffMode = true;
            Loop();
        }

        public void PlaySongLoop() {
            _riffMode = false;
            Loop();
        }

        /// <summary>
        /// Returns the AudioSource for the specified instrument.
        /// </summary>
        /// <param name="inst">Instrument.</param>
        public AudioSource GetAudioSource(Instrument inst) {
            return _instrumentAudioSources[inst];
        }

        public AudioSource[] GetAllAudioSources () {
            return _instrumentAudioSources.Values.ToArray<AudioSource>();
        }

        void Loop() {
            // If looping
            if (_loopRiffs) {

                // Stop doing so
                StopLooping();

                // Stop AudioSource
                Instrument instrument = Instrument.AllInstruments[RiffEditor.CurrentRiff.InstrumentIndex];
                _instrumentAudioSources[instrument].Stop();

                // If not looping, then start
            }
            else {
                _isPlaying = true;
                _loopRiffs = true;
            }
        }

        /// <summary>
        /// Stops looping the current riff.
        /// </summary>
        public void StopLooping() {
            _isPlaying = false;
            _loopRiffs = false;
            _beat = 0;
            Instrument instrument = Instrument.AllInstruments[RiffEditor.CurrentRiff.InstrumentIndex];
            _instrumentAudioSources[instrument].Stop();
        }

        /// <summary>
        /// Disables all instrument audio sources.
        /// </summary>
        void DisableAllAudioSources() {
            foreach (Instrument inst in Instrument.AllInstruments) _instrumentAudioSources[inst].enabled = false;
        }

        /// <summary>
        /// Increases the tempo.
        /// </summary>
        public void IncreaseTempo() {
            if ((int)_tempo < (int)Tempo.NUM_TEMPOS - 1) {
                _tempo = (Tempo)((int)_tempo + 1);
                if (RiffEditor.Instance != null)
                    RiffEditor.Instance.UpdateTempoText();
            }
        }

        /// <summary>
        /// Decreases the tempo.
        /// </summary>
        public void DecreaseTempo() {
            if ((int)_tempo > 0) {
                _tempo = (Tempo)((int)_tempo - 1);
                if (RiffEditor.Instance != null)
                    RiffEditor.Instance.UpdateTempoText();
            }
        }

        /// <summary>
        /// Registers that a guitar note was played.
        /// </summary>
        public void RegisterGuitarNote() {
            _guitarNotes++;
        }

        /// <summary>
        /// Registers that a keyboard note was played.
        /// </summary>
        public void RegisterKeyboardNote() {
            _keyboardNotes++;
        }

        /// <summary>
        /// Registers that a brass note was played.
        /// </summary>
        public void RegisterBrassNote() {
            _brassNotes++;
        }

        /// <summary>
        /// Adds a riff to the current song.
        /// </summary>
        public Riff AddRiff() {

            // Create a new riff
            Riff temp = new Riff();

            // Register the riff with the current song
            _currentSong.RegisterRiff(temp);

            // Update riff editor
            RiffEditor.CurrentRiff = temp;

            // Update song arrange
            SongArrangeMenu.Instance.selectedRiffIndex = temp.Index;
            SongArrangeMenu.Instance.Refresh();

            return temp;
        }

        /// <summary>
        /// Starts playing a song.
        /// </summary>
        public void StartSong() {
            _isPlaying = true;
        }

        /// <summary>
        /// Starts playing a playlist.
        /// </summary>
        public void StartPlaylist() {
            _beatsElapsedInPlaylist = 0;
        }

        /// <summary>
        /// Stops playing a song.
        /// </summary>
        public void StopPlaying() {
            _isPlaying = false;
            _beat = 0;
        }

        #endregion
    }
}