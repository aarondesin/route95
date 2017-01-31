// RiffEditor.cs
// ©2016 Team 95

using Route95.Core;
using Route95.Music;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Route95.UI {

    /// <summary>
    /// Class to handle initialization of the riff editor.
    /// </summary>
    public class RiffEditor : MenuBase<RiffEditor> {

        #region RiffEditor Vars

        bool _initialized = false;

		/// <summary>
		/// Current riff being edited.
		/// </summary>
        public static Riff CurrentRiff;

        //-----------------------------------------------------------------------------------------------------------------
        [Header("UI Settings")]

		/// <summary>
		/// Spacing between buttons.
		/// </summary>
		[SerializeField]
        float _buttonSpacing = 4f;

		/// <summary>
		/// Scaler for half beat notes.
		/// </summary>
		[SerializeField]
        float _halfNoteScale = 0.8f;

		/// <summary>
		/// Scaler for quarter beat notes.
		/// </summary>
		[SerializeField]
        float _quarterNoteScale = 0.6f;

		/// <summary>
		/// Scaler for volume slider.
		/// </summary>
		[SerializeField]
        float _volumeScale = 1.3f;

		/// <summary>
		/// Alpha for background on even notes.
		/// </summary>
		[SerializeField]
        float _evenBackgroundAlpha = 0.05f;

		/// <summary>
		/// Alpha for background on odd notes.
		/// </summary>
		[SerializeField]
        float _oddBackgroundAlpha = 0.03f;

		/// <summary>
		/// Width of scrollbars.
		/// </summary>
		[SerializeField]
        float _scrollbarWidth = 20f;

		/// <summary>
		/// Initial vertical scroll value.
		/// </summary>
        float _initialScrollV = 0.99f;

		/// <summary>
		/// Size of an intersection between grids.
		/// </summary>
        Vector2 _squareSize;

		/// <summary>
		/// Size of a button (smaller than square size).
		/// </summary>
        Vector2 _buttonSize;

		/// <summary>
		/// Number of rows.
		/// </summary>
        int _numNotes;

		/// <summary>
		/// Number of columns.
		/// </summary>
        int _numButtons;

		/// <summary>
		/// Maximum number of octaves supported by scale.
		/// </summary>
        int _maxOctaves;

		/// <summary>
		/// Number of octaves currently shown.
		/// </summary>
        int _octavesShown = 2;

		/// <summary>
		/// List of all buttons and objects.
		/// </summary>
        List<GameObject> _objects;

		/// <summary>
		/// List of all backgrounds.
		/// </summary>
        List<GameObject> _columnBackgrounds;

		/// <summary>
		/// 
		/// </summary>
        List<GameObject> _rowBackgrounds;

		/// <summary>
		/// 2D grid of buttons (for AI).
		/// </summary>
        List<List<GameObject>> _buttonGrid;

		/// <summary>
		/// List of suggestion objects.
		/// </summary>
        List<GameObject> _suggestions;

        //-----------------------------------------------------------------------------------------------------------------
        [Header("UI References")]

        InputField _nameInputField;

		/// <summary>
		/// 
		/// </summary>
        Scrollbar _scrollBarH;

		/// <summary>
		/// 
		/// </summary>
        Scrollbar _scrollBarV;

		/// <summary>
		/// 
		/// </summary>
        RectTransform _notePanel;

		/// <summary>
		/// 
		/// </summary>
        Image _playRiffButton;

		/// <summary>
		/// 
		/// </summary>
        Text _tempoText;

		/// <summary>
		/// 
		/// </summary>
        Scrollbar _iconBar;

		/// <summary>
		/// 
		/// </summary>
        RectTransform _iconBar_tr;

		/// <summary>
		/// 
		/// </summary>
        List<GameObject> _sliders;

		/// <summary>
		/// 
		/// </summary>
        Scrollbar _beatsBar;

		/// <summary>
		/// 
		/// </summary>
        RectTransform _beatsBar_tr;

		/// <summary>
		/// 
		/// </summary>
        Slider _riffVolumeSlider;

		Slider _riffPanningSlider;

		/// <summary>
		/// 
		/// </summary>
        GameObject _octaveParent;

		/// <summary>
		/// 
		/// </summary>
        Image _distortionButton;

		/// <summary>
		/// 
		/// </summary>
        Image _tremoloButton;

		/// <summary>
		/// 
		/// </summary>
        Image _chorusButton;

		/// <summary>
		/// 
		/// </summary>
        Image _flangerButton;

		/// <summary>
		/// 
		/// </summary>
        Image _echoButton;

		/// <summary>
		/// 
		/// </summary>
        Image _reverbButton;

        #endregion
        #region Unity Callbacks

        new void Awake() {
            base.Awake();

            // Init vars
            _notePanel = GameObject.FindGameObjectWithTag("RiffEditorNotePanel").GetComponent<RectTransform>();
			_nameInputField = GetComponentInChildren<InputField>();
			_playRiffButton = GameObject.FindGameObjectWithTag("RiffEditorPlayRiffButton").GetComponent<Image>();
			_tempoText = GameObject.FindGameObjectWithTag("RiffEditorTempoText").GetComponent<Text>();
			_scrollBarV = GameObject.FindGameObjectWithTag("RiffEditorVerticalScrollbar").GetComponent<Scrollbar>();
			_iconBar = GameObject.FindGameObjectWithTag("RiffEditorNoteIconBar").GetComponent<Scrollbar>();
			_octaveParent = GameObject.FindGameObjectWithTag("RiffEditorOctaveParent");
			_iconBar_tr = GameObject.FindGameObjectWithTag("RiffEditorNoteIconBarContent").GetComponent<RectTransform>();
			_beatsBar_tr = GameObject.FindGameObjectWithTag("RiffEditorBeatsBarContent").GetComponent<RectTransform>();
			_riffVolumeSlider = GameObject.FindGameObjectWithTag("RiffEditorRiffVolumeSlider").GetComponent<Slider>();
			_riffPanningSlider = GameObject.FindGameObjectWithTag("RiffEditorRiffPanningSlider").GetComponent<Slider>();

			_distortionButton = GameObject.FindGameObjectWithTag("RiffEditorDistortionButton").GetComponent<Image>();
			_tremoloButton = GameObject.FindGameObjectWithTag("RiffEditorTremoloButton").GetComponent<Image>();
			_chorusButton = GameObject.FindGameObjectWithTag("RiffEditorChorusButton").GetComponent<Image>();
			_flangerButton = GameObject.FindGameObjectWithTag("RiffEditorFlangerButton").GetComponent<Image>();
			_echoButton = GameObject.FindGameObjectWithTag("RiffEditorEchoButton").GetComponent<Image>();
			_reverbButton = GameObject.FindGameObjectWithTag("RiffEditorReverbButton").GetComponent<Image>();

            // Init lists
            _objects = new List<GameObject>();
            _columnBackgrounds = new List<GameObject>();
            _rowBackgrounds = new List<GameObject>();
            _buttonGrid = new List<List<GameObject>>();
            _suggestions = new List<GameObject>();

            // Set up riff name input field
            _nameInputField.onEndEdit.AddListener(delegate {
                CurrentRiff.Name = _nameInputField.text;
            });
        }

		void Start () {
			UIManager.Instance.onSwitchToRiffEditor.AddListener(Initialize);
			_riffPanningSlider.onValueChanged.AddListener(UpdateRiffPanning);
			_riffVolumeSlider.onValueChanged.AddListener(UpdateRiffVolume);
		}

        #endregion
        #region RiffEditor Methods

        /// <summary>
        /// Sets up riff editor and calls appropriate init function.
        /// </summary>
        public void Initialize() {
            _initialized = false;

            // Check if riff is valid
            if (CurrentRiff == null) {
                Debug.LogError("RiffEditor.Initialize(): no riff selected!");
                return;
            }

            // Initialize effect status sprites
            AudioSource source = MusicManager.Instance.GetAudioSource(CurrentRiff.Instrument);

			Sprite percussionFilled = UIManager.Instance.FilledPercussionNoteIcon;
			Sprite percussionEmpty = UIManager.Instance.EmptyPercussionNoteIcon;

            // Initialize effect toggle sprites
            _distortionButton.sprite =
                source.GetComponent<AudioDistortionFilter>().enabled ? percussionFilled : percussionEmpty;
            _tremoloButton.sprite =
                source.GetComponent<AudioTremoloFilter>().enabled ? percussionFilled : percussionEmpty;
            _chorusButton.sprite =
                source.GetComponent<AudioChorusFilter>().enabled ? percussionFilled : percussionEmpty;
            _flangerButton.sprite =
                source.GetComponent<AudioChorusFilter>().enabled ? percussionFilled : percussionEmpty;
            _echoButton.sprite =
                source.GetComponent<AudioEchoFilter>().enabled ? percussionFilled : percussionEmpty;
            _reverbButton.sprite =
                source.GetComponent<AudioReverbFilter>().enabled ? percussionFilled : percussionEmpty;

            HideSliders();

            // Clear all previous buttons
            Cleanup();

            // Set up riff editor properties
            _nameInputField.text = CurrentRiff.Name;
            _playRiffButton.sprite = UIManager.Instance.PlayIcon;
            UpdateTempoText();

            _numButtons = Riff.MAX_BEATS;

            // Set initial scrollbar values
            SyncScrollbars();

            // Refresh button grid
            _buttonGrid.Clear();
            for (int n = 0; n < _numButtons; n++) _buttonGrid.Add(new List<GameObject>());

            // Create note buttons
            if (CurrentRiff.Instrument.InstrumentType == Instrument.Type.Percussion)
                InitializePercussionSetup((PercussionInstrument)CurrentRiff.Instrument);
            else if (CurrentRiff.Instrument.InstrumentType == Instrument.Type.Melodic)
                InitializeMelodicSetup((MelodicInstrument)CurrentRiff.Instrument);
            else Debug.LogError(CurrentRiff.Instrument.Name + " unable to initialize.");

            // Update riff volume slider
            _riffVolumeSlider.value = CurrentRiff.Volume;

            _initialized = true;
        }

        /// <summary>
        /// Removes all existing buttons.
        /// </summary>
        public void Cleanup() {
            _columnBackgrounds.Clear();
            _rowBackgrounds.Clear();
            foreach (List<GameObject> list in _buttonGrid) list.Clear();

            foreach (GameObject obj in _objects) Destroy(obj);
            _objects.Clear();
        }

        /// <summary>
        /// Creates beat numbers.
        /// </summary>
        public void MakeBeatNumbers(float size) {
            for (int i = 0; i < Riff.MAX_BEATS / 4; i++) {
                GameObject beatNumber = UIHelpers.MakeText((i + 1).ToString());
                beatNumber.SetParent(_beatsBar_tr);
                beatNumber.SetSideWidth(size);
                beatNumber.AnchorAtPoint(0f, 0.5f);
                beatNumber.SetPosition2D((0.5f + i * 4) * size + _buttonSpacing * (i * 4 + 1), 0f);
                beatNumber.SetTextAlignment(TextAnchor.MiddleCenter);
                beatNumber.SetFontSize(30);
                _objects.Add(beatNumber);
            }
        }

        /// <summary>
        /// Initializes percussion riff editor.
        /// </summary>
        /// <param name="percInst">Percussion instrument to use.</param>
        void InitializePercussionSetup(PercussionInstrument percInst) {

            _octaveParent.SetActive(false);

            // Get all available drum notes
            List<string> set = KeyManager.Instance.GetNoteSet(percInst);
            int numDrums = set.Count;

            // Calculate square size
            _squareSize = new Vector2(
                _notePanel.rect.width / (float)Riff.MAX_BEATS,
                _notePanel.rect.width / (float)Riff.MAX_BEATS
            );

            // Calculate button size
            _buttonSize = new Vector2(
                (_squareSize.x - _buttonSpacing) / _volumeScale,
                (_squareSize.y - _buttonSpacing) / _volumeScale
            );

            // Resize note panel
            float height = Mathf.Max(_squareSize.y * numDrums,
                ((RectTransform)_notePanel.parent).rect.height);
            _notePanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

            // Resize drum icons panel
            _iconBar_tr.sizeDelta = new Vector2(
                _iconBar_tr.sizeDelta.x, // Keep default width
                _notePanel.sizeDelta.y   // Use height of note panel
            );

            // Resize beat number panel
            _beatsBar_tr.sizeDelta = new Vector2(
                _notePanel.sizeDelta.x,  // Use width of note panel
                _beatsBar_tr.sizeDelta.y // Keep default height
            );
            _beatsBar_tr.ResetScaleRot();

            MakeBeatNumbers(_squareSize.x);

            // Make column backgrounds
            MakeGrid(numDrums);

            // Make rows of buttons
            int i = 0;
            foreach (string note in set)
                MakePercussionButtons(note.Split('_')[1], i++, note, percInst.GetNoteIcon(note));

        }

        /// <summary>
        /// Creates all percussio note buttons for a drum note.
        /// </summary>
        /// <param name="title">Base name of each button.</param>
        /// <param name="row">Row index.</param>
        /// <param name="soundName">Sound file to use.</param>
        /// <param name="iconGraphic">Icon to use.</param>
        void MakePercussionButtons(string title, int row, string fileName, Sprite iconGraphic) {

            // Calculate y position of buttons in this row
            float y = _rowBackgrounds[row].GetComponent<RectTransform>().anchoredPosition3D.y;

            // Make icon for note
            GameObject drumIcon = UIHelpers.MakeImage(title, iconGraphic);
            drumIcon.SetParent(_iconBar_tr);
            drumIcon.SetSideWidth(_squareSize.x);
            drumIcon.AnchorAtPoint(0.5f, 1.0f);
            drumIcon.GetComponent<RectTransform>().ResetScaleRot();
            drumIcon.SetPosition2D(0f, y);
            drumIcon.AddComponent<Tooltippable>().Message = title;
            _objects.Add(drumIcon);

            // Make note buttons
            for (int i = 0; i < _numButtons; i++) {

                // Make a reference copy
                int num = i;

                // Calculate x position of this button
                float x = _columnBackgrounds[num].GetComponent<RectTransform>().anchoredPosition3D.x;

                // Calculate scale
                float scale = (
                    i % 4 - 2 == 0 ? _halfNoteScale :                      // Half notes
                    (i % 4 - 1 == 0 || i % 4 - 3 == 0 ? _quarterNoteScale : // Quarter notes
                    1f)                                                   // Whole notes
                );

                // Check if note is already in riff
                bool noteExists = CurrentRiff.Lookup(fileName, num);

                // Get or create note
                Note note = noteExists ?
                    CurrentRiff.GetNote(fileName, num) :
                    new Note(fileName);

				Sprite percussionEmpty = UIManager.Instance.EmptyPercussionNoteIcon;
				Sprite percussionFilled = UIManager.Instance.FilledPercussionNoteIcon;

                // Create note button
                Sprite graphic = (noteExists ? percussionFilled : percussionEmpty);
                GameObject button = UIHelpers.MakeButton(title + "_" + i, graphic);
                button.SetParent(_notePanel);
                button.SetSize2D(_buttonSize);
                button.AnchorAtPoint(0f, 1f);
                button.SetPosition2D(x, y);

                // Add StopScrolling tag
                button.tag = "StopScrolling";

                // Change scale based on beat
                RectTransform button_tr = button.GetComponent<RectTransform>();
                button_tr.ResetScaleRot();
                button_tr.localScale = new Vector3(scale, scale, scale);

                // Create volume slider
                GameObject volume = UIHelpers.MakeImage(title + "_volume");
                RectTransform volume_tr = volume.GetComponent<RectTransform>();
                volume_tr.SetParent(button_tr);
                volume_tr.sizeDelta = _buttonSize;
                volume_tr.localScale = Vector3.one * _volumeScale;
                volume_tr.AnchorAtPoint(0.5f, 0.5f);
                volume_tr.anchoredPosition3D = Vector3.zero;

                Image volume_img = volume.GetComponent<Image>();
                volume_img.sprite = UIManager.Instance.PercussionVolumeIcon;
                volume_img.type = Image.Type.Filled;
                volume_img.fillAmount = note.Volume;

                // Setup volume slider
                NoteButton noteButton = button.AddComponent<NoteButton>();
                noteButton.targetNote = note;
                noteButton.volumeImage = volume.GetComponent<Image>();
                noteButton.UpdateButtonArt();

                // Initially hide volume slider
                volume.SetActive(false);

                // Add button functionality
                button.GetComponent<Button>().onClick.AddListener(() => {
                    if (!InputManager.Instance.IsDragging) {
                        bool n = CurrentRiff.Toggle(note, num);
                        button.GetComponent<Image>().sprite = (n ? percussionFilled : percussionEmpty);
                    }
                });

                // Register button
                _objects.Add(button);
                _buttonGrid[i].Add(button);
            }
        }

        /// <summary>
        /// Initializes melodic riff editor.
        /// </summary>
        /// <param name="meloInst">Melodic instrument to use.</param>
        void InitializeMelodicSetup(MelodicInstrument meloInst) {

            _octaveParent.SetActive(true);

            Song song = MusicManager.Instance.CurrentSong;
            Instrument instrument = CurrentRiff.Instrument;

            // Calculate available octaves
            List<string> sounds = Sounds.SoundsToLoad[instrument.CodeName];
            _maxOctaves = Mathf.CeilToInt((float)sounds.Count / 12f);
            if (_maxOctaves < _octavesShown) _octavesShown = _maxOctaves;

            // Gather key/scale info
            Key key = song.Key;
            ScaleInfo scale = ScaleInfo.AllScales[song.Scale];

            // Find starting note
            int startIndex = meloInst.StartingNote(key);

            // Calculate number of notes
            int totalNotes = Sounds.SoundsToLoad[instrument.CodeName].Count;
            _numNotes = 1 + 12 * _octavesShown;

            // Set size of grid intersections
            _squareSize = new Vector2(
                _notePanel.rect.width / Riff.MAX_BEATS,
                _notePanel.rect.width / Riff.MAX_BEATS / 2f
            );

            // Set size of buttons
            _buttonSize = new Vector2(
                (_squareSize.x - _buttonSpacing) / _volumeScale,
                (_squareSize.y - _buttonSpacing) / _volumeScale
            );

            // Resize note panel
            float height = Mathf.Max(_squareSize.y * _numNotes,
                ((RectTransform)_notePanel.parent).rect.height);
            _notePanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);


            // Resize beats bar
            _beatsBar_tr.sizeDelta = new Vector2(
                _notePanel.sizeDelta.x,  // Use width of note panel
                _beatsBar_tr.sizeDelta.y // Keep default height
            );

            // Resize note text/icon bar
            _iconBar_tr.sizeDelta = new Vector2(
                _iconBar_tr.sizeDelta.x, // Keep default width
                _notePanel.sizeDelta.y   // Use height of note panel
            );

            MakeBeatNumbers(_squareSize.x);

            // Make column backgrounds
            MakeGrid(_numNotes);

            // Make rows of note buttons
            for (int i = 0; i < _numNotes && i < totalNotes; i++) {
                string note = Sounds.SoundsToLoad[CurrentRiff.Instrument.CodeName][i + startIndex];
                bool inScale = KeyManager.Instance.GetScale(key, scale, meloInst).HasNote(note);
                MakeMelodicButtons(note.Split('_')[1], i, note, inScale);
            }
        }

        /// <summary>
        /// Creates melodic all note buttons for a note in a row.
        /// </summary>
        /// <param name="title">Base note button title.</param>
        /// <param name="row">Row index.</param>
        /// <param name="fileName">Note filename.</param>
        /// <param name="inScale">Is this note in the selected scale?</param>
        void MakeMelodicButtons(string title, int row, string fileName, bool inScale) {
            Color transparentWhite = new Color(1f, 1f, 1f, 0.5f);

            // Calculate y position of buttons in this row
            GameObject bg = _rowBackgrounds[row];
            float y = bg.GetComponent<RectTransform>().anchoredPosition3D.y;

            // Create note text
            GameObject noteText = UIHelpers.MakeText(title);
            noteText.SetParent(_iconBar_tr);
            noteText.SetSideWidth(_squareSize.x);
            noteText.AnchorAtPoint(0.5f, 1f);
            noteText.SetPosition2D(0f, y);
            noteText.SetTextAlignment(TextAnchor.MiddleCenter);
            noteText.SetFontSize(30);
            _objects.Add(noteText);

            // Change text color depending on whether or not the note is in the scale
            if (inScale) {
                noteText.GetComponent<Text>().color = Color.white;
                bg.GetComponent<Image>().SetAlpha(bg.GetComponent<Image>().color.a * 3f);
            }
            else
                noteText.GetComponent<Text>().color = transparentWhite;

            // Make note buttons
            for (int i = 0; i < _numButtons; i++) {

                // Make reference copy
                int num = i;

                // Calculate x position of this button
                float x = _columnBackgrounds[num].GetComponent<RectTransform>().anchoredPosition3D.x;

                // Calculate scale
                float scale = (
                    i % 4 - 2 == 0 ? _halfNoteScale :                      // Half notes
                    (i % 4 - 1 == 0 || i % 4 - 3 == 0 ? _quarterNoteScale : // Quarter notes
                    1f)                                                   // Whole notes
                );

                // Check if note is already in riff
                bool noteExists = CurrentRiff.Lookup(fileName, num);

                // Get or create note
                Note note = noteExists ?
                    CurrentRiff.GetNote(fileName, num) :
                    new Note(fileName);

				Sprite melodicFilled = UIManager.Instance.FilledMelodicNoteIcon;
				Sprite melodicEmpty = UIManager.Instance.EmptyMelodicNoteIcon;

                // Make note button
                Sprite graphic = (CurrentRiff.Lookup(fileName, num) ? melodicFilled : melodicEmpty);
                GameObject button = UIHelpers.MakeButton(title + "_" + i, graphic);
                button.SetParent(_notePanel);
                button.SetSize2D(_buttonSize.x, _buttonSize.y);
                button.AnchorAtPoint(0f, 1f);
                button.SetPosition2D(x, y);

                // Add StopScrolling tag
                button.tag = "StopScrolling";

                // Change scale based on beat
                RectTransform button_tr = button.GetComponent<RectTransform>();
                button_tr.ResetScaleRot();
                button_tr.localScale = new Vector3(scale, scale, scale);

                // Create volume slider
                GameObject volume = UIHelpers.MakeImage(title + "_volume");
                RectTransform volume_tr = volume.GetComponent<RectTransform>();
                volume_tr.SetParent(button_tr);
                volume_tr.sizeDelta = button_tr.sizeDelta;
                volume_tr.localScale = new Vector3(
                    button_tr.localScale.x * _volumeScale,
                    button_tr.localScale.y * 2f * _volumeScale,
                    1f
                );

                volume_tr.AnchorAtPoint(0.5f, 0.5f);
                volume_tr.anchoredPosition3D = Vector3.zero;

                Image volume_img = volume.GetComponent<Image>();
                volume_img.sprite = UIManager.Instance.MelodicVolumeIcon;
                volume_img.type = Image.Type.Filled;
                volume_img.fillAmount = note.Volume;

                // Setup volume slider
                NoteButton noteButton = button.AddComponent<NoteButton>();
                noteButton.targetNote = note;
                noteButton.volumeImage = volume.GetComponent<Image>();
                noteButton.UpdateButtonArt();

                // Initially hide volume slider
                volume.SetActive(false);

                // Setup button
                button.GetComponent<Button>().onClick.AddListener(() => {
                    if (!InputManager.Instance.IsDragging) {
                        bool n = CurrentRiff.Toggle(note, num);
                        if (n) {
                            SuggestChords(num, row);
                        }
                        else {
                            ClearSuggestions();
                        }
                        //bt_sh.enabled = n;
                        button.GetComponent<Image>().sprite = (n ? melodicFilled : melodicEmpty);
                    }
                });

                // Register button
                _objects.Add(button);
                _buttonGrid[num].Add(button);
            }
        }

        void MakeGrid(int numNotes) {
            // Make column backgrounds
            for (int column = 0; column < Riff.MAX_BEATS; column++) {
                GameObject columnBackground =
                    UIHelpers.MakeImage("ColumnBackground_" + column, UIManager.Instance.FillSprite);
                columnBackground.GetComponent<Image>().SetAlpha(column % 2 == 0 ? _evenBackgroundAlpha : _oddBackgroundAlpha);
                columnBackground.SetParent(_notePanel);
                columnBackground.SetSize2D(_squareSize.x, _notePanel.rect.height);
                columnBackground.GetComponent<RectTransform>().ResetScaleRot();
                columnBackground.AnchorAtPoint(0f, 0.5f);
                columnBackground.SetPosition2D(_squareSize.x * (column + 0.5f), 0f);
                _columnBackgrounds.Add(columnBackground);
                _objects.Add(columnBackground);
            }

            // Make row backgrounds
            for (int row = 0; row < numNotes; row++) {
                GameObject rowBackground =
                    UIHelpers.MakeImage("RowBackground_" + row, UIManager.Instance.FillSprite);
                rowBackground.GetComponent<Image>().SetAlpha(row % 2 == 0 ? _evenBackgroundAlpha : _oddBackgroundAlpha);
                rowBackground.SetParent(_notePanel);
                rowBackground.SetSize2D(_notePanel.rect.width, _squareSize.y);
                rowBackground.GetComponent<RectTransform>().ResetScaleRot();
                rowBackground.AnchorAtPoint(0.5f, 1f);
                rowBackground.SetPosition2D(0f, _squareSize.y * -(row + 0.5f));
                _rowBackgrounds.Add(rowBackground);
                _objects.Add(rowBackground);
            }
        }

        /// <summary>
        /// Sets the volume of the riff.
        /// Called from the riff volume slider.
        /// </summary>
        /// <param name="slider">Slider to use.</param>
        public void SetRiffVolume(Slider slider) {
            CurrentRiff.Volume = slider.value;
        }

        /// <summary>
        /// Sets the panning of the riff.
        /// Called from the riff panning slider.
        /// </summary>
        /// <param name="slider">Slider to use.</param>
        public void SetRiffPanning(Slider slider) {
            CurrentRiff.Panning = slider.value;
        }

        public void SyncScrollbars() {
            _scrollBarV.value = _initialScrollV;
            SyncVerticalScrollViews(_scrollBarV);
        }

        /// <summary>
        /// Syncs note panel and note names/drum icons.
        /// </summary>
        /// <param name="slider"></param>
        public void SyncVerticalScrollViews(Scrollbar slider) {
            _scrollBarV.value = slider.value;
            _iconBar.value = slider.value;
        }

        /// <summary>
        /// Increments the number of octaves shown.
        /// </summary>
        public void IncreaseOctavesShown() {
            if (_octavesShown < _maxOctaves) {
                _octavesShown++;
                Initialize();
            }
        }

        /// <summary>
        /// Decrements the number of octaves shown.
        /// </summary>
        public void DecreaseOctavesShown() {
            if (_octavesShown > 1) {
                _octavesShown--;
                Initialize();
            }
        }

        /// <summary>
        /// Updates the play riff button art.
        /// </summary>
        public void UpdatePlayRiffButtonArt() {
            _playRiffButton.sprite = MusicManager.Instance.IsPlaying ?
                UIManager.Instance.PauseIcon :
                UIManager.Instance.PlayIcon;
        }

        /// <summary>
        /// Updates the text for the tempo selector.
        /// </summary>
        public void UpdateTempoText () {
            _tempoText.text = MusicManager.Instance.Tempo.ToString();
        }

        /// <summary>
        /// Hides all effect sliders.
        /// </summary>
        public void HideSliders() {

            if (_sliders == null) return;

            foreach (GameObject slider in _sliders)
                slider.SetActive(false);
        }

        /// <summary>
        /// Hides all sliders and toggles the indicated slider.
        /// </summary>
        /// <param name="obj"></param>
        public void HideSliders(GameObject obj) {
            bool st = obj.activeSelf;
            HideSliders();
            obj.SetActive(!st);
        }

        /// <summary>
        /// Suggests chords based on a clicked note.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        void SuggestChords(int column, int row) {

            // Clear previous suggestions
            ClearSuggestions();

            Song song = MusicManager.Instance.CurrentSong;

            // Suggest minor/major harmony
            if (song.Scale == ScaleInfo.Minor.ScaleIndex &&
                row + 3 < _buttonGrid[column].Count) SuggestMinorChord(_buttonGrid[column][row + 3]);
            else if (song.Scale == ScaleInfo.Major.ScaleIndex &&
                row + 4 < _buttonGrid[column].Count) SuggestMajorChord(_buttonGrid[column][row + 4]);

            // Suggest power chord
            if (row + 7 < _buttonGrid[column].Count)
                SuggestPowerChord(_buttonGrid[column][row + 7]);

            // Suggest octave
            if (row + 12 < _buttonGrid[0].Count)
                SuggestOctave(_buttonGrid[column][row + 12]);
        }

        /// <summary>
        /// Clears all suggestion icons.
        /// </summary>
        void ClearSuggestions() {
            foreach (GameObject suggestion in _suggestions) Destroy(suggestion);
            _suggestions.Clear();
            UIManager.Instance.HideMenu(Tooltip.Instance);
        }

        void CreateSuggestion(GameObject button, string title, Sprite graphic, string tooltip) {
            RectTransform tr = button.GetComponent<RectTransform>();

            GameObject suggestion = UIHelpers.MakeButton(
                title,
                graphic,
                tr,
                new Vector2(tr.sizeDelta.y, tr.sizeDelta.y),
                Vector2.zero
            );
            suggestion.GetComponent<Button>().onClick.AddListener(delegate { button.GetComponent<Button>().onClick.Invoke(); });
            suggestion.GetComponent<RectTransform>().localScale = tr.localScale;
            suggestion.AddComponent<Tooltippable>().Message = tooltip;
            _suggestions.Add(suggestion);
        }

        /// <summary>
        /// Suggests a minor chord.
        /// </summary>
        /// <param name="button"></param>
        void SuggestMinorChord(GameObject button) {
            CreateSuggestion(button, "Minor", UIManager.Instance.MinorSuggestionIcon, "Minor chord (sad)");
        }

        /// <summary>
        /// Suggests a major chord.
        /// </summary>
        /// <param name="button"></param>
        void SuggestMajorChord(GameObject button) {
            CreateSuggestion(button, "Major", UIManager.Instance.MajorSuggestionIcon, "Major chord (happy)");
        }

        /// <summary>
        /// Suggests a power chord.
        /// </summary>
        /// <param name="button"></param>
        void SuggestPowerChord(GameObject button) {
            CreateSuggestion(button, "Power", UIManager.Instance.PowerSuggestionIcon, "Power chord (powerful)");
        }

        /// <summary>
        /// Suggests an octave.
        /// </summary>
        /// <param name="button"></param>
        void SuggestOctave(GameObject button) {
            CreateSuggestion(button, "Octave", UIManager.Instance.OctaveSuggestionIcon, "Octave");
        }

		void UpdateRiffVolume (float value) {
			CurrentRiff.Volume = value;
		}

		void UpdateRiffPanning (float value) {
			CurrentRiff.Panning = value;
		}

        #endregion
    }
}
