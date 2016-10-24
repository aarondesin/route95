using Route95.Core;
using Route95.Music;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Route95.UI {

    /// <summary>
    /// Class to handle initialization of the riff editor.
    /// </summary>
    public class RiffEditor : MenuBase<RiffEditor> {

        #region RiffEditor Vars

        /// <summary>
        /// Sound to use when enabling an effect.
        /// </summary>
        [Tooltip("Sound to use when enabling an effect.")]
        [SerializeField]
        AudioClip _enableEffectSound;

        /// <summary>
        /// Sound to use when disabling an effect.
        /// </summary>
        [Tooltip("Sound to use when disabling an effect.")]
        [SerializeField]
        AudioClip _disableEffectSound;

        public bool initialized = false;
        public static Riff CurrentRiff;         // Current riff being edited

        //-----------------------------------------------------------------------------------------------------------------
        [Header("UI Settings")]

        public float buttonSpacing = 4f;          // Spacing between buttons
        public float halfNoteScale = 0.8f;        // Scaler for half beat notes
        public float quarterNoteScale = 0.6f;     // Scaler for quarter beat notes
        public float volumeScale = 1.3f;          // Scaler for volume slider
        public float evenBackgroundAlpha = 0.05f; // Alpha for background on even notes
        public float oddBackgroundAlpha = 0.03f;  // Alpha for background on odd notes
        public float scrollbarWidth = 20f;        // Width of scrollbars
        public float initialScrollV = 0.99f;      // Initial vertical scroll value

        Vector2 squareSize;                       // Size of an intersection between grids
        Vector2 buttonSize;                       // Size of a button (smaller than square size)

        int numNotes;                           // Number of rows
        int numButtons;                         // Number of columns
        int maxOctaves;                         // Maximum number of octaves supported by scale
        int octavesShown = 2;                   // Number of octaves currently shown

        List<GameObject> objects;               // List of all buttons and objects
        List<GameObject> columnBackgrounds;           // List of all backgrounds
        List<GameObject> rowBackgrounds;
        List<List<GameObject>> buttonGrid;      // 2D grid of buttons (for AI)
        List<GameObject> suggestions;           // List of suggestion objects

        //-----------------------------------------------------------------------------------------------------------------
        [Header("UI References")]

        public InputField nameInputField;
        public Scrollbar scrollBarH;
        public Scrollbar scrollBarV;
        RectTransform notePanel;
        public Image playRiffButton;
        public Text tempoText;
        public Scrollbar iconBar;
        public RectTransform iconBar_tr;
        public List<GameObject> sliders;
        public Scrollbar beatsBar;
        public RectTransform beatsBar_tr;
        public Slider riffVolumeSlider;
        public GameObject octaveParent;

        //public List<EffectSlider> effectSliders;

        public Image distortionButton;
        public Image tremoloButton;
        public Image chorusButton;
        public Image flangerButton;
        public Image echoButton;
        public Image reverbButton;

        //-----------------------------------------------------------------------------------------------------------------
        [Header("UI Art")]

        // Icons for percussion setup buttons
        public Sprite percussionEmpty;
        public Sprite percussionFilled;
        public Sprite percussionSuggested;

        // Icons for melodic setup buttons
        public Sprite melodicEmpty;
        public Sprite melodicFilled;
        public Sprite melodicSuggested;

        public Sprite minorSuggestion;
        public Sprite majorSuggestion;
        public Sprite powerSuggestion;
        public Sprite octaveSuggestion;

        #endregion
        #region Unity Callbacks

        new void Awake() {
            base.Awake();

            // Init vars
            notePanel = GetComponent<RectTransform>();

            // Init lists
            objects = new List<GameObject>();
            columnBackgrounds = new List<GameObject>();
            rowBackgrounds = new List<GameObject>();
            buttonGrid = new List<List<GameObject>>();
            suggestions = new List<GameObject>();

            // Set up riff name input field
            nameInputField.onEndEdit.AddListener(delegate {
                CurrentRiff.Name = nameInputField.text;
            });
        }

        #endregion
        #region RiffEditor Methods

        /// <summary>
        /// Sets up riff editor and calls appropriate init function.
        /// </summary>
        public void Initialize() {
            initialized = false;

            // Check if riff is valid
            if (CurrentRiff == null) {
                Debug.LogError("RiffEditor.Initialize(): no riff selected!");
                return;
            }

            // Initialize effect sliders
            /*foreach (EffectSlider effectSlider in effectSliders) {
                effectSlider.gameObject.SetActive(true);
                effectSlider.Initialize();
            }*/

            // Initialize effect status sprites
            AudioSource source = MusicManager.Instance.GetAudioSource(CurrentRiff.Instrument);

            // Initialize effect toggle sprites
            distortionButton.sprite =
                source.GetComponent<AudioDistortionFilter>().enabled ? percussionFilled : percussionEmpty;
            tremoloButton.sprite =
                source.GetComponent<AudioTremoloFilter>().enabled ? percussionFilled : percussionEmpty;
            chorusButton.sprite =
                source.GetComponent<AudioChorusFilter>().enabled ? percussionFilled : percussionEmpty;
            flangerButton.sprite =
                source.GetComponent<AudioChorusFilter>().enabled ? percussionFilled : percussionEmpty;
            echoButton.sprite =
                source.GetComponent<AudioEchoFilter>().enabled ? percussionFilled : percussionEmpty;
            reverbButton.sprite =
                source.GetComponent<AudioReverbFilter>().enabled ? percussionFilled : percussionEmpty;

            HideSliders();

            // Clear all previous buttons
            Cleanup();

            // Set up riff editor properties
            nameInputField.text = CurrentRiff.Name;
            playRiffButton.sprite = UIManager.Instance.PlayIcon;
            UpdateTempoText();

            numButtons = Riff.MAX_BEATS;

            // Set initial scrollbar values
            SyncScrollbars();

            // Refresh button grid
            buttonGrid.Clear();
            for (int n = 0; n < numButtons; n++) buttonGrid.Add(new List<GameObject>());

            // Create note buttons
            if (CurrentRiff.Instrument.InstrumentType == Instrument.Type.Percussion)
                InitializePercussionSetup((PercussionInstrument)CurrentRiff.Instrument);
            else if (CurrentRiff.Instrument.InstrumentType == Instrument.Type.Melodic)
                InitializeMelodicSetup((MelodicInstrument)CurrentRiff.Instrument);
            else Debug.LogError(CurrentRiff.Instrument.Name + " unable to initialize.");

            // Update riff volume slider
            riffVolumeSlider.value = CurrentRiff.Volume;

            initialized = true;
        }

        /// <summary>
        /// Removes all existing buttons.
        /// </summary>
        public void Cleanup() {
            columnBackgrounds.Clear();
            rowBackgrounds.Clear();
            foreach (List<GameObject> list in buttonGrid) list.Clear();

            foreach (GameObject obj in objects) Destroy(obj);
            objects.Clear();
        }

        /// <summary>
        /// Plays an effects on noise.
        /// </summary>
        public void EffectsOn() {
            MusicManager.PlayMenuSound(_enableEffectSound);
        }

        /// <summary>
        /// Plays an effects off noise.
        /// </summary>
        public void EffectsOff() {
            MusicManager.PlayMenuSound(_disableEffectSound);
        }

        /// <summary>
        /// Creates beat numbers.
        /// </summary>
        public void MakeBeatNumbers(float size) {
            for (int i = 0; i < Riff.MAX_BEATS / 4; i++) {
                GameObject beatNumber = UIHelpers.MakeText((i + 1).ToString());
                beatNumber.SetParent(beatsBar_tr);
                beatNumber.SetSideWidth(size);
                beatNumber.AnchorAtPoint(0f, 0.5f);
                beatNumber.SetPosition2D((0.5f + i * 4) * size + buttonSpacing * (i * 4 + 1), 0f);
                beatNumber.SetTextAlignment(TextAnchor.MiddleCenter);
                beatNumber.SetFontSize(30);
                objects.Add(beatNumber);
            }
        }

        /// <summary>
        /// Initializes percussion riff editor.
        /// </summary>
        /// <param name="percInst">Percussion instrument to use.</param>
        void InitializePercussionSetup(PercussionInstrument percInst) {

            octaveParent.SetActive(false);

            // Get all available drum notes
            List<string> set = KeyManager.Instance.GetNoteSet(percInst);
            int numDrums = set.Count;

            // Calculate square size
            squareSize = new Vector2(
                notePanel.rect.width / (float)Riff.MAX_BEATS,
                notePanel.rect.width / (float)Riff.MAX_BEATS
            );

            // Calculate button size
            buttonSize = new Vector2(
                (squareSize.x - buttonSpacing) / volumeScale,
                (squareSize.y - buttonSpacing) / volumeScale
            );

            // Resize note panel
            float height = Mathf.Max(squareSize.y * numDrums,
                ((RectTransform)notePanel.parent).rect.height);
            notePanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

            // Resize drum icons panel
            iconBar_tr.sizeDelta = new Vector2(
                iconBar_tr.sizeDelta.x, // Keep default width
                notePanel.sizeDelta.y   // Use height of note panel
            );

            // Resize beat number panel
            beatsBar_tr.sizeDelta = new Vector2(
                notePanel.sizeDelta.x,  // Use width of note panel
                beatsBar_tr.sizeDelta.y // Keep default height
            );
            beatsBar_tr.ResetScaleRot();

            MakeBeatNumbers(squareSize.x);

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
            float y = rowBackgrounds[row].RectTransform().anchoredPosition3D.y;

            // Make icon for note
            GameObject drumIcon = UIHelpers.MakeImage(title, iconGraphic);
            drumIcon.SetParent(iconBar_tr);
            drumIcon.SetSideWidth(squareSize.x);
            drumIcon.AnchorAtPoint(0.5f, 1.0f);
            drumIcon.RectTransform().ResetScaleRot();
            drumIcon.SetPosition2D(0f, y);
            drumIcon.AddComponent<Tooltippable>().message = title;
            objects.Add(drumIcon);

            // Make note buttons
            for (int i = 0; i < numButtons; i++) {

                // Make a reference copy
                int num = i;

                // Calculate x position of this button
                float x = columnBackgrounds[num].RectTransform().anchoredPosition3D.x;

                // Calculate scale
                float scale = (
                    i % 4 - 2 == 0 ? halfNoteScale :                      // Half notes
                    (i % 4 - 1 == 0 || i % 4 - 3 == 0 ? quarterNoteScale : // Quarter notes
                    1f)                                                   // Whole notes
                );

                // Check if note is already in riff
                bool noteExists = CurrentRiff.Lookup(fileName, num);

                // Get or create note
                Note note = noteExists ?
                    CurrentRiff.GetNote(fileName, num) :
                    new Note(fileName);

                // Create note button
                Sprite graphic = (noteExists ? percussionFilled : percussionEmpty);
                GameObject button = UIHelpers.MakeButton(title + "_" + i, graphic);
                button.SetParent(notePanel);
                button.SetSize2D(buttonSize);
                button.AnchorAtPoint(0f, 1f);
                button.SetPosition2D(x, y);

                // Add StopScrolling tag
                button.tag = "StopScrolling";

                // Change scale based on beat
                RectTransform button_tr = button.RectTransform();
                button_tr.ResetScaleRot();
                button_tr.localScale = new Vector3(scale, scale, scale);

                // Create volume slider
                GameObject volume = UIHelpers.MakeImage(title + "_volume");
                RectTransform volume_tr = volume.RectTransform();
                volume_tr.SetParent(button_tr);
                volume_tr.sizeDelta = buttonSize;
                volume_tr.localScale = Vector3.one * volumeScale;
                volume_tr.AnchorAtPoint(0.5f, 0.5f);
                volume_tr.anchoredPosition3D = Vector3.zero;

                Image volume_img = volume.Image();
                volume_img.sprite = UIManager.Instance.PercussionVolumeIcon;
                volume_img.type = Image.Type.Filled;
                volume_img.fillAmount = note.Volume;

                // Setup volume slider
                NoteButton noteButton = button.AddComponent<NoteButton>();
                noteButton.targetNote = note;
                noteButton.volumeImage = volume.GetComponent<Image>();
                noteButton.UpdateButtonArt();

                // Create show/hide toggle
                ShowHide bt_sh = button.AddComponent<ShowHide>();
                bt_sh.objects = new List<GameObject>() { volume };
                bt_sh.enabled = CurrentRiff.Lookup(note, num);

                // Initially hide volume slider
                volume.SetActive(false);

                // Add button functionality
                button.Button().onClick.AddListener(() => {
                    if (!InputManager.Instance.IsDragging) {
                        bool n = CurrentRiff.Toggle(note, num);
                        if (!n) bt_sh.Hide();
                        bt_sh.enabled = n;
                        button.Image().sprite = (n ? percussionFilled : percussionEmpty);
                        if (n) bt_sh.Show();
                    }
                });

                // Register button
                objects.Add(button);
                buttonGrid[i].Add(button);
            }
        }

        /// <summary>
        /// Initializes melodic riff editor.
        /// </summary>
        /// <param name="meloInst">Melodic instrument to use.</param>
        void InitializeMelodicSetup(MelodicInstrument meloInst) {

            octaveParent.SetActive(true);

            Song song = MusicManager.Instance.CurrentSong;
            Instrument instrument = CurrentRiff.Instrument;

            // Calculate available octaves
            List<string> sounds = Sounds.SoundsToLoad[instrument.CodeName];
            maxOctaves = Mathf.CeilToInt((float)sounds.Count / 12f);
            if (maxOctaves < octavesShown) octavesShown = maxOctaves;

            // Gather key/scale info
            Key key = song.Key;
            ScaleInfo scale = ScaleInfo.AllScales[song.Scale];

            // Find starting note
            int startIndex = meloInst.StartingNote(key);

            // Calculate number of notes
            int totalNotes = Sounds.SoundsToLoad[instrument.CodeName].Count;
            numNotes = 1 + 12 * octavesShown;

            // Set size of grid intersections
            squareSize = new Vector2(
                notePanel.rect.width / Riff.MAX_BEATS,
                notePanel.rect.width / Riff.MAX_BEATS / 2f
            );

            // Set size of buttons
            buttonSize = new Vector2(
                (squareSize.x - buttonSpacing) / volumeScale,
                (squareSize.y - buttonSpacing) / volumeScale
            );

            // Resize note panel
            float height = Mathf.Max(squareSize.y * numNotes,
                ((RectTransform)notePanel.parent).rect.height);
            notePanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);


            // Resize beats bar
            beatsBar_tr.sizeDelta = new Vector2(
                notePanel.sizeDelta.x,  // Use width of note panel
                beatsBar_tr.sizeDelta.y // Keep default height
            );

            // Resize note text/icon bar
            iconBar_tr.sizeDelta = new Vector2(
                iconBar_tr.sizeDelta.x, // Keep default width
                notePanel.sizeDelta.y   // Use height of note panel
            );

            MakeBeatNumbers(squareSize.x);

            // Make column backgrounds
            MakeGrid(numNotes);

            // Make rows of note buttons
            for (int i = 0; i < numNotes && i < totalNotes; i++) {
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
            GameObject bg = rowBackgrounds[row];
            float y = bg.RectTransform().anchoredPosition3D.y;

            // Create note text
            GameObject noteText = UIHelpers.MakeText(title);
            noteText.SetParent(iconBar_tr);
            noteText.SetSideWidth(squareSize.x);
            noteText.AnchorAtPoint(0.5f, 1f);
            noteText.SetPosition2D(0f, y);
            noteText.SetTextAlignment(TextAnchor.MiddleCenter);
            noteText.SetFontSize(30);
            objects.Add(noteText);

            // Change text color depending on whether or not the note is in the scale
            if (inScale) {
                noteText.Text().color = Color.white;
                bg.Image().SetAlpha(bg.Image().color.a * 3f);
            }
            else
                noteText.Text().color = transparentWhite;

            // Make note buttons
            for (int i = 0; i < numButtons; i++) {

                // Make reference copy
                int num = i;

                // Calculate x position of this button
                float x = columnBackgrounds[num].RectTransform().anchoredPosition3D.x;

                // Calculate scale
                float scale = (
                    i % 4 - 2 == 0 ? halfNoteScale :                      // Half notes
                    (i % 4 - 1 == 0 || i % 4 - 3 == 0 ? quarterNoteScale : // Quarter notes
                    1f)                                                   // Whole notes
                );

                // Check if note is already in riff
                bool noteExists = CurrentRiff.Lookup(fileName, num);

                // Get or create note
                Note note = noteExists ?
                    CurrentRiff.GetNote(fileName, num) :
                    new Note(fileName);

                // Make note button
                Sprite graphic = (CurrentRiff.Lookup(fileName, num) ? melodicFilled : melodicEmpty);
                GameObject button = UIHelpers.MakeButton(title + "_" + i, graphic);
                button.SetParent(notePanel);
                button.SetSize2D(buttonSize.x, buttonSize.y);
                button.AnchorAtPoint(0f, 1f);
                button.SetPosition2D(x, y);

                // Add StopScrolling tag
                button.tag = "StopScrolling";

                // Change scale based on beat
                RectTransform button_tr = button.RectTransform();
                button_tr.ResetScaleRot();
                button_tr.localScale = new Vector3(scale, scale, scale);

                // Create volume slider
                GameObject volume = UIHelpers.MakeImage(title + "_volume");
                RectTransform volume_tr = volume.RectTransform();
                volume_tr.SetParent(button_tr);
                volume_tr.sizeDelta = button_tr.sizeDelta;
                volume_tr.localScale = new Vector3(
                    button_tr.localScale.x * volumeScale,
                    button_tr.localScale.y * 2f * volumeScale,
                    1f
                );

                volume_tr.AnchorAtPoint(0.5f, 0.5f);
                volume_tr.anchoredPosition3D = Vector3.zero;

                Image volume_img = volume.Image();
                volume_img.sprite = UIManager.Instance.MelodicVolumeIcon;
                volume_img.type = Image.Type.Filled;
                volume_img.fillAmount = note.Volume;

                // Setup volume slider
                NoteButton noteButton = button.AddComponent<NoteButton>();
                noteButton.targetNote = note;
                noteButton.volumeImage = volume.Image();
                noteButton.UpdateButtonArt();

                // Create show/hide toggle
                ShowHide bt_sh = button.AddComponent<ShowHide>();
                bt_sh.objects = new List<GameObject>() { volume };
                bt_sh.enabled = CurrentRiff.Lookup(note, num);

                // Initially hide volume slider
                volume.SetActive(false);

                // Setup button
                button.Button().onClick.AddListener(() => {
                    if (!InputManager.Instance.IsDragging) {
                        bool n = CurrentRiff.Toggle(note, num);
                        if (n) {
                            SuggestChords(num, row);
                            bt_sh.Show();
                        }
                        else {
                            ClearSuggestions();
                            bt_sh.Hide();
                        }
                        bt_sh.enabled = n;
                        button.Image().sprite = (n ? melodicFilled : melodicEmpty);
                    }
                });

                // Register button
                objects.Add(button);
                buttonGrid[num].Add(button);
            }
        }

        void MakeGrid(int numNotes) {
            // Make column backgrounds
            for (int column = 0; column < Riff.MAX_BEATS; column++) {
                GameObject columnBackground =
                    UIHelpers.MakeImage("ColumnBackground_" + column, UIManager.Instance.FillSprite);
                columnBackground.Image().SetAlpha(column % 2 == 0 ? evenBackgroundAlpha : oddBackgroundAlpha);
                columnBackground.SetParent(notePanel);
                columnBackground.SetSize2D(squareSize.x, notePanel.rect.height);
                columnBackground.RectTransform().ResetScaleRot();
                columnBackground.AnchorAtPoint(0f, 0.5f);
                columnBackground.SetPosition2D(squareSize.x * (column + 0.5f), 0f);
                columnBackgrounds.Add(columnBackground);
                objects.Add(columnBackground);
            }

            // Make row backgrounds
            for (int row = 0; row < numNotes; row++) {
                GameObject rowBackground =
                    UIHelpers.MakeImage("RowBackground_" + row, UIManager.Instance.FillSprite);
                rowBackground.Image().SetAlpha(row % 2 == 0 ? evenBackgroundAlpha : oddBackgroundAlpha);
                rowBackground.SetParent(notePanel);
                rowBackground.SetSize2D(notePanel.rect.width, squareSize.y);
                rowBackground.RectTransform().ResetScaleRot();
                rowBackground.AnchorAtPoint(0.5f, 1f);
                rowBackground.SetPosition2D(0f, squareSize.y * -(row + 0.5f));
                rowBackgrounds.Add(rowBackground);
                objects.Add(rowBackground);
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
            scrollBarV.value = initialScrollV;
            SyncVerticalScrollViews(scrollBarV);
        }

        /// <summary>
        /// Syncs note panel and note names/drum icons.
        /// </summary>
        /// <param name="slider"></param>
        public void SyncVerticalScrollViews(Scrollbar slider) {
            scrollBarV.value = slider.value;
            iconBar.value = slider.value;
        }

        /// <summary>
        /// Increments the number of octaves shown.
        /// </summary>
        public void IncreaseOctavesShown() {
            if (octavesShown < maxOctaves) {
                octavesShown++;
                Initialize();
            }
        }

        /// <summary>
        /// Decrements the number of octaves shown.
        /// </summary>
        public void DecreaseOctavesShown() {
            if (octavesShown > 1) {
                octavesShown--;
                Initialize();
            }
        }

        /// <summary>
        /// Updates the play riff button art.
        /// </summary>
        public void UpdatePlayRiffButtonArt() {
            playRiffButton.sprite = MusicManager.Instance.IsPlaying ?
                UIManager.Instance.PauseIcon :
                UIManager.Instance.PlayIcon;
        }

        /// <summary>
        /// Updates the text for the tempo selector.
        /// </summary>
        public void UpdateTempoText() {
            tempoText.text = MusicManager.Instance.Tempo.ToString();
        }

        /// <summary>
        /// Hides all effect sliders.
        /// </summary>
        public void HideSliders() {

            if (sliders == null) return;

            foreach (GameObject slider in sliders)
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

        /*void Suggest () {
            int posX = riffai.FindHintXPosition (CurrentRiff, subdivsShown);
            Debug.Log ("posX = " + posX);
            //Debug.Log ("curr inst " + CurrentRiff.instrument);

            Key key = MusicManager.Instance.currentSong.key;
            ScaleInfo scale = ScaleInfo.AllScales[MusicManager.Instance.currentSong.scale];

            int posY = riffai.FindHintYPosition (CurrentRiff, KeyManager.Instance.scales [key][scale][(MelodicInstrument)CurrentRiff.instrument], subdivsShown);
            Debug.Log ("posY = " + posY);
            if (posX >= buttonGrid.Count || posX < 0) {
                Debug.Log ("Suggestion X out of bounds!");
                return;
            }
            if (posY >= buttonGrid[0].Count || posY < 0) {
                Debug.Log ("Suggestion Y out of bounds!");
                return;
            } 
            else {
                //int subPower = Convert.ToInt32(Math.Pow(2, subdivsShown));
                //Debug.Log("posx "+posX+" subdivs pow" + subPower);
                //int processedX = (posX * (subPower)) -1;
                //int processedX = (posX * (2)) -1;
                Debug.Log("Suggesting "+posX+" " + posY);
                //Debug.Log ("curr key " + MusicManager.Instance.currentKey);
                //Debug.Log (" curr inst " +RiffEditor.CurrentRiff.instrument);
                //Debug.Log (" guitar " + Instrument.ElectricGuitar);
                Debug.Log("buttongrid x length: " + buttonGrid.Count);
                Debug.Log("buttongrid Y length: " + buttonGrid[posX].Count);
                //Debug.Log ("processedX: " + processedX);
                Suggest (buttonGrid[posX][posY]);

            }
        }*/

        void Suggest(GameObject button) {
            Debug.Log("in Suggest");
            Sprite img = button.GetComponent<Image>().sprite;
            if (CurrentRiff.Instrument.InstrumentType == Instrument.Type.Percussion && img != percussionFilled) {
                button.GetComponent<Image>().sprite = percussionSuggested;
            }
            else if (img != melodicFilled) {
                button.GetComponent<Image>().sprite = melodicSuggested;
            }
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
                row + 3 < buttonGrid[column].Count) SuggestMinorChord(buttonGrid[column][row + 3]);
            else if (song.Scale == ScaleInfo.Major.ScaleIndex &&
                row + 4 < buttonGrid[column].Count) SuggestMajorChord(buttonGrid[column][row + 4]);

            // Suggest power chord
            if (row + 7 < buttonGrid[column].Count)
                SuggestPowerChord(buttonGrid[column][row + 7]);

            // Suggest octave
            if (row + 12 < buttonGrid[0].Count)
                SuggestOctave(buttonGrid[column][row + 12]);
        }

        /// <summary>
        /// Clears all suggestion icons.
        /// </summary>
        void ClearSuggestions() {
            foreach (GameObject suggestion in suggestions) Destroy(suggestion);
            suggestions.Clear();
            UIManager.Instance.HideMenu(Tooltip.Instance);
        }

        void CreateSuggestion(GameObject button, string title, Sprite graphic, string tooltip) {
            RectTransform tr = button.RectTransform();

            GameObject suggestion = UIHelpers.MakeButton(
                title,
                graphic,
                tr,
                new Vector2(tr.sizeDelta.y, tr.sizeDelta.y),
                Vector2.zero
            );
            suggestion.Button().onClick.AddListener(delegate { button.Button().onClick.Invoke(); });
            suggestion.RectTransform().localScale = tr.localScale;
            suggestion.AddComponent<Tooltippable>().message = tooltip;
            suggestions.Add(suggestion);
        }

        /// <summary>
        /// Suggests a minor chord.
        /// </summary>
        /// <param name="button"></param>
        void SuggestMinorChord(GameObject button) {
            CreateSuggestion(button, "Minor", minorSuggestion, "Minor chord (sad)");
        }

        /// <summary>
        /// Suggests a major chord.
        /// </summary>
        /// <param name="button"></param>
        void SuggestMajorChord(GameObject button) {
            CreateSuggestion(button, "Major", majorSuggestion, "Major chord (happy)");
        }

        /// <summary>
        /// Suggests a power chord.
        /// </summary>
        /// <param name="button"></param>
        void SuggestPowerChord(GameObject button) {
            CreateSuggestion(button, "Power", powerSuggestion, "Power chord (powerful)");
        }

        /// <summary>
        /// Suggests an octave.
        /// </summary>
        /// <param name="button"></param>
        void SuggestOctave(GameObject button) {
            CreateSuggestion(button, "Octave", octaveSuggestion, "Octave");
        }

        #endregion
    }
}
