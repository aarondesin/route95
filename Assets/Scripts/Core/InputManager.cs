// InputManager.cs
// ©2016 Aaron Desin

using Route95.Music;
using Route95.UI;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Route95.Core {

    /// <summary>
    /// Instanced class to handle all keyboard and mouse input.
    /// </summary>
    public class InputManager : SingletonMonoBehaviour<InputManager> {

        #region InputManager Vars

        [Header("InputManager Values")]

        /// <summary>
        /// Amount by which volume changes when pressed.
        /// </summary>
        [SerializeField]
        float _volumeChangeAmount;

        /// <summary>
        /// Current selected object.
        /// </summary>
        [Tooltip("Current selected object.")]
        [SerializeField]
        GameObject _selected;

        /// <summary>
        /// List of scroll views to disable while dragging.
        /// </summary>
        [Tooltip("List of scroll views to disable while dragging.")]
        [SerializeField]
        List<ScrollRect> _scrollviews;

        /// <summary>
        /// Mouse position during last frame.
        /// </summary>
        Vector3 _prevMouse = Vector3.zero;

        /// <summary>
        /// Change in mouse position between last frame and this frame.
        /// </summary>
        Vector3 _mouseDelta;

        /// <summary>
        /// Position clicked.
        /// </summary>
        Vector3 _clickPosition;

        /// <summary>
        /// All note audiosources.
        /// </summary>
        List<AudioSource> _audioSources;

        /// <summary>
        /// Number of frames dragged.
        /// </summary>
        int _framesDragged = 0;

        /// <summary>
        /// Number of frames a user must hold down to drag.
        /// </summary>
        [Tooltip("Number of frames user must hold click to be considered a drag.")]
        [SerializeField]
        int _dragThreshold = 30;

        #endregion
        #region Key Mappings

        /// <summary>
        /// Mappings of number keys to instruments.
        /// </summary>
        public static Dictionary<KeyCode, Instrument> keyToInstrument = 
            new Dictionary<KeyCode, Instrument>() {
            { KeyCode.Alpha1, PercussionInstrument.RockDrums },
            { KeyCode.Alpha2, PercussionInstrument.ExoticPercussion },
            { KeyCode.Alpha3, MelodicInstrument.ElectricGuitar },
            { KeyCode.Alpha4, MelodicInstrument.ElectricBass },
            { KeyCode.Alpha5, MelodicInstrument.AcousticGuitar },
            { KeyCode.Alpha6, MelodicInstrument.ClassicalGuitar },
            { KeyCode.Alpha7, MelodicInstrument.PipeOrgan },
            { KeyCode.Alpha8, MelodicInstrument.Keyboard },
            { KeyCode.Alpha9, MelodicInstrument.Trumpet }
        };

        /// <summary>
        /// Mappings of letter keys to note indices.
        /// </summary>
        public static Dictionary<KeyCode, int> keyToNote = 
            new Dictionary<KeyCode, int>() {
            { KeyCode.P, 0 },
            { KeyCode.O, 1 },
            { KeyCode.I, 2 },
            { KeyCode.U, 3 },
            { KeyCode.Y, 4 },
            { KeyCode.T, 5 },
            { KeyCode.R, 6 },
            { KeyCode.E, 7 },
            { KeyCode.W, 8 },
            { KeyCode.Q, 9 },
            { KeyCode.L, 10 },
            { KeyCode.K, 11 },
            { KeyCode.J, 12 },
            { KeyCode.H, 13 },
            { KeyCode.G, 14 },
            { KeyCode.F, 15 },
            { KeyCode.D, 16 },
            { KeyCode.S, 17 },
            { KeyCode.A, 18 },
            { KeyCode.M, 19 },
            { KeyCode.N, 20 },
            { KeyCode.B, 21 },
            { KeyCode.V, 22 },
            { KeyCode.C, 23 },
            { KeyCode.X, 24 },
            { KeyCode.Z, 25 }
        };

        #endregion
        #region Unity Callbacks

        new void Awake() {
            base.Awake();

            // Initialize audio sources for all notes
            _audioSources = new List<AudioSource>();
            MakeNoteAudioSources();
        }

        void Update() {

            // Update mouse delta
            _mouseDelta = Input.mousePosition - _prevMouse;

            switch (GameManager.Instance.CurrentState) {

                // Live mode
                case GameManager.State.Live:

                    CheckForPause();

                    MusicManager music = MusicManager.Instance;
                    Instrument inst = music.CurrentInstrument;

                    // If available, get current playing song
                    Song song = null;
                    Project project = music.CurrentProject;
                    int songIndex = music.CurrentPlayingSong;
                    if (!project.Empty)
                        song = project.Songs[songIndex];

                    // Check for tempo up/down
                    else if (Input.GetKeyDown(KeyCode.UpArrow))
                        music.IncreaseTempo();
                    else if (Input.GetKeyDown(KeyCode.DownArrow))
                        music.DecreaseTempo();

                    // Check for instrument volume up/down
                    else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                        AudioSource source = music.GetAudioSource(inst);
                        if (source.volume >= _volumeChangeAmount)
                            source.volume -= _volumeChangeAmount;
                    }
                    else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                        AudioSource source = music.GetAudioSource(inst);
                        if (source.volume <= _volumeChangeAmount)
                            source.volume += _volumeChangeAmount;
                    }

                    else {

                        // Check for instruments switch
                        foreach (var key in keyToInstrument) {
                            if (Input.GetKeyDown(key.Key)) {
                                SwitchInstrument(keyToInstrument[key.Key]);
                                LiveInstrumentIcons.Instance.WakeLiveUI();
                            }
                        }

                        // Check for note presses
                        foreach (var keyPress in keyToNote.Keys.ToList()) {
                            if (Input.GetKeyDown(keyPress)) {

                                int noteIndex;
                                AudioSource source = 
                                    _audioSources[keyToNote[keyPress]];
                                KeyManager keyManager = KeyManager.Instance;

                                // If percussion is selected
                                if (inst.type == Instrument.Type.Percussion) {
                                    var percNotes = keyManager.GetNoteSet((PercussionInstrument)inst);
                                    noteIndex = percNotes.Count - 1 - keyToNote[keyPress];
                                    if (noteIndex >= 0) {
                                        Note note = new Note(percNotes[noteIndex]);
                                        note.PlayNote(source, 1f, false);

                                        WorldManager.Instance.ShowWeatherEffect (note);
                                    }

                                    // If melodic is selected (must be a valid song)
                                }
                                else if (song != null && song.Scale != -1 && song.Key != Key.None) {

                                    Key key = MusicManager.Instance.CurrentSong.Key;
                                    ScaleInfo scaleType = ScaleInfo.AllScales[MusicManager.Instance.CurrentSong.Scale];
                                    MelodicInstrument meloInst = inst as MelodicInstrument;
                                    Scale scale = KeyManager.Instance.GetScale(key, scaleType, meloInst);


                                    noteIndex = scale.Count - 1 - keyToNote[keyPress];
                                    if (noteIndex >= 0) {
                                        Note note = new Note(scale.NoteAt(noteIndex));
                                        if (note != null) note.PlayNote(source, 1f, true);
                                    }

                                    // If electric bass
                                    if (inst.codeName == "ElectricBass") {

                                        // Cause terrain deformation
                                        WorldManager.Instance.DeformRandom();

                                        // All other melodic instruments
                                    }
                                    else {
                                        switch (inst.family) {
                                            case Instrument.Family.Guitar:
                                                MusicManager.Instance.guitarNotes++;
                                                break;
                                            case Instrument.Family.Keyboard:
                                                MusicManager.Instance.keyboardNotes++;
                                                break;
                                            case Instrument.Family.Brass:
                                                MusicManager.Instance.brassNotes++;
                                                break;
                                        }
                                    }
                                }

                                // If key released, stop note
                            }
                            else if (Input.GetKeyUp(keyPress) && inst.type != Instrument.Type.Percussion)
                                _audioSources[keyToNote[keyPress]].Stop();
                        }
                    }
                    break;

                // Setup mode
                case GameManager.State.Setup:

                    // If left mouse button released
                    if (Input.GetMouseButtonUp(0)) {
                        if (_selected != null) {

                            // Call OnMouseUp() if possible
                            if (_selected.GetComponent<DraggableButton>() != null)
                                _selected.GetComponent<DraggableButton>().OnMouseUp();
                        }

                        // Clear selected
                        _selected = null;

                        // Unfreeze scroll views
                        UnfreezeAllScrollviews();

                        // Reset click position
                        _clickPosition = Vector3.zero;

                        // If left mouse button clicked
                    }
                    else if (Input.GetMouseButtonDown(0)) {

                        // Get click position
                        _clickPosition = Input.mousePosition;

                        // Get selected object from EventSystem
                        _selected = EventSystem.current.currentSelectedGameObject;

                        // If something was clicked
                        if (_selected != null) {

                            // Freeze scroll views if necessary
                            if (_selected.tag == "StopScrolling") FreezeAllScrollviews();

                            // Call OnMouseDown() if possible
                            if (_selected.GetComponent<DraggableButton>() != null)
                                _selected.GetComponent<DraggableButton>().OnMouseDown();
                        }

                        // If mouse button held down or not down
                    }
                    else {

                        // If held down on an object
                        if (_selected) {

                            // Increment frames dragged
                            _framesDragged++;

                            // Call Drag() if possible
                            if (_selected.GetComponent<DraggableButton>() != null)
                                _selected.GetComponent<DraggableButton>().Drag(Input.mousePosition - _clickPosition);

                            // If no object, reset frames dragged
                        }
                        else _framesDragged = 0;
                    }

                    break;
            }

            // If in free camera mode
            if (CameraControl.Instance.state == CameraControl.State.Free) {

                // Space -> start car movement
                if (Input.GetKeyDown(KeyCode.Space)) UIManager.Instance.SwitchToLive();

                // Left/Right -> adjust time of day
                else if (Input.GetKeyDown(KeyCode.LeftArrow)) WorldManager.Instance.timeOfDay -= Mathf.PI / 16f;
                else if (Input.GetKeyDown(KeyCode.RightArrow)) WorldManager.Instance.timeOfDay += Mathf.PI / 16f;
            }

            // Update prevMouse
            _prevMouse = Input.mousePosition;
        }

        #endregion
        #region InputManager Methods

        /// <summary>
        /// Returns whether or not the user is dragging (read-only).
        /// </summary>
        public bool IsDragging { get {
            return _framesDragged >= _dragThreshold;
        } }

        void MakeNoteAudioSources () {
            for (int i = 0; i < 26; i++) {
                // Create GameObject
                GameObject obj = new GameObject("Note" + i + "Source");
                obj.transform.parent = transform;

                // Set default volume
                AudioSource temp = obj.AddComponent<AudioSource>();
                temp.volume = 1.0f;
                _audioSources.Add(temp);
            }
        }

        /// <summary>
        /// Pauses if the key is pressed.
        /// </summary>
        void CheckForPause () {
            if (Input.GetKeyDown(KeyCode.Escape))
                GameManager.Instance.TogglePause();
        }

        /// <summary>
        /// Freezes all selected scrollviews.
        /// </summary>
        void FreezeAllScrollviews() {
            foreach (ScrollRect scrollview in _scrollviews)
                scrollview.enabled = false;
        }

        /// <summary>
        /// Unfreezes all selected scrollviews.
        /// </summary>
        void UnfreezeAllScrollviews() {
            foreach (ScrollRect scrollview in _scrollviews)
                scrollview.enabled = true;
        }

        /// <summary>
        /// Switches the instrument.
        /// </summary>
        /// <param name="instrument">Instrument.</param>
        void SwitchInstrument(Instrument instrument) {
            Instrument inst = MusicManager.Instance.CurrentInstrument;
            if (instrument != inst) {
                InstrumentDisplay.Instance.FadeGlow(inst.index);
                MusicManager.Instance.CurrentInstrument = instrument;
                InstrumentDisplay.Instance.WakeGlow(instrument.index);
                MusicManager.PlayMenuSound(instrument.switchSound);
                InstrumentDisplay.Instance.Refresh();
            }
        }

        #endregion
    }
}
