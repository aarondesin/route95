using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class to handle initialization of the riff editor.
/// </summary>
public class InstrumentSetup : MonoBehaviour {

	#region InstrumentSetup Vars

	public static InstrumentSetup instance; // Quick reference to this instance
	public RiffAI riffai;                   // RiffAI instance to use (inactive)
	public static Riff currentRiff;         // Current riff being edited
	
	//-----------------------------------------------------------------------------------------------------------------
	[Header("UI Settings")]

	public float baseButtonScale = 1f;      // Multiplier for button scale
	public float buttonWidth = 96f;        // Base width of buttons
	public float buttonSpacing = 4f;        // Spacing between buttons

	int numNotes;                           // Number of rows
	int numButtons;                         // Number of columns
	int maxOctaves;                         // Maximum number of octaves supported by scale
	int octavesShown = 2;                   // Number of octaves currently shown
	
	List<GameObject> buttons;               // List of all buttons and objects
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

	public List<EffectSlider> effectSliders;

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
	public Sprite powerSuggestion;
	public Sprite octaveSuggestion;

	#endregion
	#region Unity Callbacks

	void Awake () {
		instance = this;
		nameInputField.onEndEdit.AddListener(delegate { currentRiff.name = nameInputField.text; });
		buttons = new List<GameObject>();
		buttonGrid = new List<List<GameObject>>();
		suggestions = new List<GameObject>();
		notePanel = gameObject.RectTransform();
	}

	#endregion
	#region InstrumentSetup Methods

	/// <summary>
	/// Sets up riff editor and calls appropriate init function.
	/// </summary>
	public void Initialize () {
		
		// Check if riff is valid
		if (currentRiff == null) {
			Debug.LogError ("InstrumentSetup.Initialize(): no riff selected!");
			return;
		}
			
		// Initialize effect sliders
		foreach (EffectSlider effectSlider in effectSliders) {
			if (effectSlider == null) continue;
			effectSlider.gameObject.SetActive(true);
			effectSlider.Initialize();
		}

		// Initialize effect status sprites
		AudioSource source = MusicManager.instance.instrumentAudioSources[currentRiff.instrument];

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
			
		HideSliders ();

		// Clear all previous buttons
		Cleanup();

		// Set up riff editor properties
		nameInputField.text = currentRiff.name;
		playRiffButton.GetComponent<Image>().sprite = GameManager.instance.playIcon;
		MakeBeatNumbers ();
		UpdateTempoText();

		numButtons = Riff.MAX_BEATS;

		// Refresh button grid
		buttonGrid.Clear ();
		for(int n = 0; n<numButtons; n++) buttonGrid.Add(new List<GameObject>());

		// Create note buttons
		if (currentRiff.instrument.type == Instrument.Type.Percussion)
			InitializePercussionSetup ((PercussionInstrument)currentRiff.instrument);
		else if (currentRiff.instrument.type == Instrument.Type.Melodic)
			InitializeMelodicSetup ((MelodicInstrument)currentRiff.instrument);
		else Debug.LogError(currentRiff.instrument.name + " unable to initialize.");

		// Set initial scrollbar values
		scrollBarH.value = 0.01f;
		scrollBarV.value = 0.99f;
		SyncHorizontalScrollViews(scrollBarH);
		SyncVerticalScrollViews(scrollBarV);

		// Update riff volume slider
		riffVolumeSlider.value = currentRiff.volume;
	}

	/// <summary>
	/// Removes all existing buttons.
	/// </summary>
	public void Cleanup () {
		foreach (GameObject button in buttons) Destroy(button);
		buttons.Clear();
		foreach (List<GameObject> list in buttonGrid) list.Clear();
	}

	/// <summary>
	/// Creates beat numbers.
	/// </summary>
	public void MakeBeatNumbers () {
		for (int i=0; i < Riff.MAX_BEATS/4 ; i++) {
			GameObject beatNumber = UIHelpers.MakeText((i+1).ToString());
			beatNumber.SetParent (beatsBar_tr);
			beatNumber.SetSideWidth (48f);
			beatNumber.AnchorAtPoint (0f, 0.5f);
			beatNumber.SetPosition2D ((0.5f + i * 4) * buttonWidth + buttonSpacing * (i * 4 + 1), 0f);
			beatNumber.SetTextAlignment (TextAnchor.MiddleCenter);
			beatNumber.SetFontSize(30);
			buttons.Add (beatNumber);
		}
	}

	/// <summary>
	/// Initializes percussion riff editor.
	/// </summary>
	/// <param name="percInst">Percussion instrument to use.</param>
	void InitializePercussionSetup (PercussionInstrument percInst) {

		List<string> set = KeyManager.instance.percussionSets[percInst];
		int numDrums = set.Count;

		// Resize note panel
		float x = (numButtons)*buttonWidth + (numButtons+1)*buttonSpacing;
		float y = (numDrums)*buttonWidth + (numDrums+1)*buttonSpacing;
		notePanel.sizeDelta = new Vector2 (x, y);

		// Resize drum icons panel
		iconBar_tr.sizeDelta = new Vector2 (iconBar_tr.sizeDelta.x, notePanel.sizeDelta.y);

		// Resize beat number panel
		beatsBar_tr.sizeDelta = new Vector2 (notePanel.sizeDelta.x, beatsBar_tr.sizeDelta.y);
		beatsBar_tr.localScale = Vector3.one;

		// Make rows
		int i = 0;
		foreach (string note in set) 
			MakePercussionButtons (note, i++, note, percInst.icons[note]);
	}

	/// <summary>
	/// Initializes melodic riff editor.
	/// </summary>
	/// <param name="meloInst">Melodic instrument to use.</param>
	void InitializeMelodicSetup (MelodicInstrument meloInst) {

		// Calculate available octaves
		List<string> sounds = Sounds.soundsToLoad[currentRiff.instrument.codeName];
		maxOctaves = (int)Mathf.CeilToInt( (float)(sounds.Count) / 12f);
		if (maxOctaves < octavesShown) octavesShown = maxOctaves;

		// Gather key/scale info
		Key key = MusicManager.instance.currentSong.key;
		ScaleInfo scale = ScaleInfo.AllScales[MusicManager.instance.currentSong.scale];

		// Find starting note
		int startIndex = meloInst.startingNote[key];

		// Calculate number of notes
		int totalNotes = Sounds.soundsToLoad[currentRiff.instrument.codeName].Count;
		numNotes = 1 + 12 * octavesShown;

		// Resize note button panel
		notePanel.sizeDelta = new Vector2 (
			numButtons * buttonWidth + (numButtons + 1) * buttonSpacing,
			numNotes * buttonWidth + (numNotes + 1) * buttonSpacing
		);

		// Resize note text/icon bar
		iconBar_tr.sizeDelta = new Vector2 (iconBar_tr.sizeDelta.x, notePanel.sizeDelta.y);

		// Resize beats bar
		beatsBar_tr.sizeDelta = new Vector2 (notePanel.sizeDelta.x, beatsBar_tr.sizeDelta.y);

		for (int i = 0; i < numNotes && i < totalNotes; i++) {
			string note = Sounds.soundsToLoad[currentRiff.instrument.codeName][i+startIndex];
			bool inScale = KeyManager.instance.scales[key][scale][meloInst].allNotes.Contains(note);
			MakeMelodicButtons (note.Split('_')[1], i, note, inScale);
		}
			
	}

	// Creates all buttons for percussion setup
	void MakePercussionButtons (string title, int row, string soundName, Sprite iconGraphic) {

		// Make icon for note
		GameObject drumIcon = UIHelpers.MakeImage (title, iconGraphic);
		drumIcon.SetParent (iconBar_tr); 
		drumIcon.SetSideWidth (buttonWidth);
		drumIcon.AnchorAtPoint (0.5f, 1.0f);
		drumIcon.SetPosition2D (0f, -buttonWidth - (buttonWidth+buttonSpacing)*row);

		buttons.Add(drumIcon);
			
		for (int i = 0; i < numButtons; i ++) {
			int num = i;

			// Check if note is already in riff
			bool noteExists = currentRiff.Lookup(soundName, num);

			// Get volume if so
			float vol = noteExists ? currentRiff.VolumeOfNote(soundName, num) : 1f;

			// Create note button
			Sprite graphic = (currentRiff.Lookup(soundName, num) ? percussionFilled : percussionEmpty);
			GameObject button = UIHelpers.MakeButton (title+"_"+ i, graphic);
			button.SetParent(notePanel);
			button.SetSideWidth(buttonWidth);
			button.AnchorAtPoint(0f, 1f);
			
			button.SetPosition2D(((float)num + 0.5f) * buttonWidth + (num+1)*buttonSpacing, 
				-((float)row + 0.5f) * buttonWidth - (row+1)*buttonSpacing);

			// Add StopScrolling tag
			button.tag = "StopScrolling";

			// Change scale based on beat
			RectTransform button_tr = button.RectTransform();
			button_tr.localScale = Vector3.one;

			// Half beat
			if (i % 4 - 2 == 0) button_tr.localScale *= 0.75f;

			// Quarter beat
			else if (i % 4 - 1 == 0 || i %4 - 3 == 0) button_tr.localScale *= 0.5f;

			// Create note
			Note note = new Note (soundName, vol, 1f);

			// Create volume slider
			GameObject volume = UIHelpers.MakeImage(title+"_volume");
			RectTransform volume_tr = volume.RectTransform();
			volume_tr.SetParent(button_tr);
			volume_tr.sizeDelta = button_tr.sizeDelta * 1.3f;
			volume_tr.localScale = button_tr.localScale * 1.3f;
			volume_tr.AnchorAtPoint(0.5f, 0.5f);
			volume_tr.anchoredPosition3D = Vector3.zero;

			Image volume_img = volume.Image();
			volume_img.sprite = GameManager.instance.volumeIcon;
			volume_img.type = Image.Type.Filled;
			volume_img.fillAmount = vol;

			// Setup volume slider
			NoteButton noteButton = button.AddComponent<NoteButton>();
			noteButton.targetNote = note;
			noteButton.volumeImage = volume.GetComponent<Image>();

			// Create show/hide toggle
			ShowHide bt_sh = button.AddComponent<ShowHide>();
			bt_sh.objects = new List<GameObject>() { volume};
			bt_sh.enabled = currentRiff.Lookup(note, num);

			// Initially hide volume slider
			volume.SetActive(false);

			// Add button functionality
			button.Button().onClick.AddListener(()=>{
				if (!InputManager.instance.IsDragging) {
					bool n = currentRiff.Toggle(note, num);
					if (!n) bt_sh.Hide();
					bt_sh.enabled = n;
					button.Image().sprite = (n ? percussionFilled : percussionEmpty);
					if (n) bt_sh.Show();
				}
			});

			// Register button
			buttons.Add(button);
			buttonGrid[i].Add(button);
		}
	}

	void MakeMelodicButtons (string title, int row, string fileName, bool inScale) {
		int numButtons = Riff.MAX_BEATS;
		Color transparentWhite = new Color (1f, 1f, 1f, 0.5f);

		// Create note text
		GameObject noteText = UIHelpers.MakeText (title);
		noteText.SetParent (iconBar_tr);
		noteText.SetSideWidth (buttonWidth);
		noteText.AnchorAtPoint (0.5f, 1f);
		noteText.SetPosition2D (0f, - (0.5f + row) * buttonWidth - buttonSpacing * (row + 1));
		noteText.SetTextAlignment (TextAnchor.MiddleCenter);
		noteText.SetFontSize (30);

		buttons.Add (noteText);

		// Change text color depending on whether or not the note is in the scale
		noteText.Text().color = (inScale ? Color.white : transparentWhite);

		for (int i=0; i<numButtons; i++) {
			int num = i;

			// Check if note is already in riff
			bool noteExists = currentRiff.Lookup(fileName, num);

			// Get volume if so
			float vol = noteExists ? currentRiff.VolumeOfNote(fileName, num) : 1f;

			// Make note button
			Sprite graphic = (currentRiff.Lookup(fileName, num) ? melodicFilled : melodicEmpty);
			GameObject button = UIHelpers.MakeButton(title+"_"+i, graphic);
			button.SetParent (notePanel);
			button.SetSize2D (buttonWidth, buttonWidth/2f);
			button.AnchorAtPoint (0f, 1f);
			button.SetPosition2D(((float)num + 0.5f) * buttonWidth + (num+1)*buttonSpacing, 
				-((float)row + 0.5f) * buttonWidth - (row+1)*buttonSpacing);
			
			// Add StopScrolling tag
			button.tag = "StopScrolling";
					
			// Change scale based on position of note
			RectTransform bt_tr = button.RectTransform();
			bt_tr.localScale = Vector3.one;
			if (i % 4 - 2 == 0) bt_tr.localScale *= 0.75f;
			else if (i % 4 - 1 == 0 || i % 4 - 3 == 0) bt_tr.localScale *= 0.5f;

			// Create note
			Note note = new Note (fileName, vol, 1f);

			// Create volume slider
			GameObject volume = UIHelpers.MakeImage(title+"_volume");
			RectTransform volume_tr = volume.RectTransform();
			volume_tr.SetParent(bt_tr);
			volume_tr.sizeDelta = bt_tr.sizeDelta;
			volume_tr.localScale = new Vector3 (
				bt_tr.localScale.x * 1.3f,
				bt_tr.localScale.y * 2.5f,
				1f
			);

			volume_tr.AnchorAtPoint(0.5f, 0.5f);
			volume_tr.anchoredPosition3D = Vector3.zero;

			Image volume_img = volume.Image();
			volume_img.sprite = GameManager.instance.melodicVolumeIcon;
			volume_img.type = Image.Type.Filled;
			volume_img.fillAmount = vol;

			// Setup volume slider
			NoteButton noteButton = button.AddComponent<NoteButton>();
			noteButton.targetNote = note;
			noteButton.volumeImage = volume.GetComponent<Image>();

			// Create show/hide toggle
			ShowHide bt_sh = button.AddComponent<ShowHide>();
			bt_sh.objects = new List<GameObject>() { volume};
			bt_sh.enabled = currentRiff.Lookup(note, num);

			// Initially hide volume slider
			volume.SetActive(false);

			// Setup button
			button.Button().onClick.AddListener(()=>{
				if (!InputManager.instance.IsDragging) {
					bool n = currentRiff.Toggle(note, num);
					if (n) {
						SuggestChords(num, row);
						bt_sh.Show();
					} else {
						ClearSuggestions();
						bt_sh.Hide();
					}
					bt_sh.enabled = n;
					button.Image().sprite = (n ? melodicFilled : melodicEmpty);
				}
			});

			// Register button
			buttons.Add(button);
			buttonGrid[num].Add(button);
		}
	}

	/// <summary>
	/// Sets the volume of the riff.
	/// Called from the riff volume slider.
	/// </summary>
	/// <param name="slider"></param>
	public void SetRiffVolume (Slider slider) {
		currentRiff.volume = slider.value;
	}

	/// <summary>
	/// Syncs note panel and beat number panel.
	/// </summary>
	/// <param name="slider"></param>
	public void SyncHorizontalScrollViews (Scrollbar slider) {
		scrollBarH.value = slider.value;
		beatsBar.value = slider.value;
	}

	/// <summary>
	/// Syncs note panel and note names/drum icons.
	/// </summary>
	/// <param name="slider"></param>
	public void SyncVerticalScrollViews (Scrollbar slider) {
		scrollBarV.value = slider.value;
		iconBar.value = slider.value;
	}

	/// <summary>
	/// Increments the number of octaves shown.
	/// </summary>
	public void IncreaseOctavesShown () {
		if (octavesShown < maxOctaves) {
			octavesShown++;
			Initialize();
		}
	}

	/// <summary>
	/// Decrements the number of octaves shown.
	/// </summary>
	public void DecreaseOctavesShown () {
		if (octavesShown > 1) {
			octavesShown--;
			Initialize();
		}
	}

	/// <summary>
	/// Updates the play riff button art.
	/// </summary>
	public void UpdatePlayRiffButtonArt () {
		playRiffButton.sprite = MusicManager.instance.playing ?
			GameManager.instance.pauseIcon :
			GameManager.instance.playIcon;
	}

	/// <summary>
	/// Updates the text for the tempo selector.
	/// </summary>
	public void UpdateTempoText () {
		tempoText.text = MusicManager.instance.tempo.ToString();
	}

	/// <summary>
	/// Hides all effect sliders.
	/// </summary>
	public void HideSliders () {

		if (sliders == null) return;

		foreach (GameObject slider in sliders)
			slider.SetActive(false);
	}

	/// <summary>
	/// Hides all sliders and toggles the indicated slider.
	/// </summary>
	/// <param name="obj"></param>
	public void HideSliders (GameObject obj) {
		bool st = obj.activeSelf;
		HideSliders();
		obj.SetActive(!st);
	}

	/*void Suggest () {
		int posX = riffai.FindHintXPosition (currentRiff, subdivsShown);
		Debug.Log ("posX = " + posX);
		//Debug.Log ("curr inst " + currentRiff.instrument);

		Key key = MusicManager.instance.currentSong.key;
		ScaleInfo scale = ScaleInfo.AllScales[MusicManager.instance.currentSong.scale];

		int posY = riffai.FindHintYPosition (currentRiff, KeyManager.instance.scales [key][scale][(MelodicInstrument)currentRiff.instrument], subdivsShown);
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
			//Debug.Log ("curr key " + MusicManager.instance.currentKey);
			//Debug.Log (" curr inst " +InstrumentSetup.currentRiff.instrument);
			//Debug.Log (" guitar " + Instrument.ElectricGuitar);
			Debug.Log("buttongrid x length: " + buttonGrid.Count);
			Debug.Log("buttongrid Y length: " + buttonGrid[posX].Count);
			//Debug.Log ("processedX: " + processedX);
			Suggest (buttonGrid[posX][posY]);

		}
	}*/

	void Suggest (GameObject button) {
		Debug.Log ("in Suggest");
		Sprite img = button.GetComponent<Image>().sprite;
		if (currentRiff.instrument.type == Instrument.Type.Percussion && img != percussionFilled) {
			button.GetComponent<Image>().sprite = percussionSuggested;
		} else if (img != melodicFilled) {
			button.GetComponent<Image>().sprite  = melodicSuggested;
		}
	}

	/// <summary>
	/// Suggests chords based on a clicked note.
	/// </summary>
	/// <param name="column"></param>
	/// <param name="row"></param>
	void SuggestChords (int column, int row) {

		// Clear previous suggestions
		ClearSuggestions();

		Song song = MusicManager.instance.currentSong;

		// Suggest minor/major harmony
		if (song.scale == ScaleInfo.Minor.scaleIndex && 
			row+3 < buttonGrid[column].Count) SuggestMinorChord (buttonGrid[column][row+3]);
		else if (song.scale == ScaleInfo.Major.scaleIndex &&
			row+4 < buttonGrid[column].Count) SuggestMajorChord (buttonGrid[column][row+4]);

		// Suggest power chord
		if (row+7 < buttonGrid[column].Count) 
			SuggestPowerChord(buttonGrid[column][row+7]);

		// Suggest octave
		if (row+12 < buttonGrid[0].Count) 
			SuggestOctave(buttonGrid[column][row+12]);
	}

	/// <summary>
	/// Clears all suggestion icons.
	/// </summary>
	void ClearSuggestions () {
		foreach (GameObject suggestion in suggestions) Destroy(suggestion);
		suggestions.Clear();
		GameManager.instance.HideTooltip();
	}

	/// <summary>
	/// Suggests a minor chord.
	/// </summary>
	/// <param name="button"></param>
	void SuggestMinorChord (GameObject button) {
		RectTransform tr = button.RectTransform();

		GameObject suggestion = UIHelpers.MakeImage (
			"Minor", 
			minorSuggestion, 
			tr,
			new Vector2 (tr.sizeDelta.y, tr.sizeDelta.y),
			new Vector2 (tr.sizeDelta.x*0.5f, -tr.sizeDelta.y*0.5f)
		);
		Tooltip tooltip = suggestion.AddComponent<Tooltip>();
		tooltip.text = "Minor Chord (sad)";
		suggestions.Add(suggestion);
	}

	/// <summary>
	/// Suggests a major chord.
	/// </summary>
	/// <param name="button"></param>
	void SuggestMajorChord (GameObject button) {
		RectTransform tr = button.RectTransform();

		GameObject suggestion = UIHelpers.MakeImage (
			"Major", 
			minorSuggestion, 
			tr,
			new Vector2 (tr.sizeDelta.y, tr.sizeDelta.y),
			new Vector2 (tr.sizeDelta.x*0.5f, -tr.sizeDelta.y*0.5f)
		);
		Tooltip tooltip = suggestion.AddComponent<Tooltip>();
		tooltip.text = "Major Chord (happy)";
		suggestions.Add(suggestion);
	}

	/// <summary>
	/// Suggests a power chord.
	/// </summary>
	/// <param name="button"></param>
	void SuggestPowerChord (GameObject button) {
		RectTransform tr = button.RectTransform();

		GameObject suggestion = UIHelpers.MakeImage (
			"Power", 
			powerSuggestion, 
			tr, 
			new Vector2 (
				tr.sizeDelta.y,
				tr.sizeDelta.y
			),
			new Vector2 (
				tr.sizeDelta.x*0.5f, 
				-tr.sizeDelta.y*0.5f
			)
		);
		suggestion.AddComponent<Tooltip>();
		suggestion.GetComponent<Tooltip>().text = "Power Chord (powerful)";
		suggestions.Add(suggestion);
	}

	/// <summary>
	/// Suggests an octave.
	/// </summary>
	/// <param name="button"></param>
	void SuggestOctave (GameObject button) {
		RectTransform tr = button.RectTransform();

		GameObject suggestion = UIHelpers.MakeImage (
			"Octave", 
			octaveSuggestion, 
			tr, 
			new Vector2 (
				tr.sizeDelta.y,
				tr.sizeDelta.y
			),
			new Vector2 (
				tr.sizeDelta.x*0.5f, 
				-tr.sizeDelta.y*0.5f
			)
		);
		suggestion.AddComponent<Tooltip>();
		suggestion.GetComponent<Tooltip>().text = "Octave (neutral)";
		suggestions.Add(suggestion);
	}

	#endregion
}
