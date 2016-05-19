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
	public float buttonWidth = 128f;        // Base width of buttons
	public float buttonSpacing = 8f;        // Spacing between buttons

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
	public GameObject playRiffButton;
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
	}

	#endregion
	#region InstrumentSetup Methods

	// Calls appropriate Setup() function based on current instrument
	public void Initialize () {
		
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
		UpdateBeatsText();
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

		scrollBarH.value = 0.01f;
		scrollBarV.value = 0.99f;

		riffVolumeSlider.value = currentRiff.volume;
	}

	// Removes all existing buttons
	public void Cleanup () {
		foreach (GameObject button in buttons) Destroy(button);
		buttons.Clear();
		foreach (List<GameObject> list in buttonGrid) list.Clear();
	}

	public void MakeBeatNumbers () {
		for (int i=0; i<Riff.MAX_BEATS/4; i++) {
			buttons.Add(MakeText((i+1).ToString(), beatsBar_tr,
				new Vector2 (48f, 48f),
				new Vector2 (
					buttonWidth + (buttonWidth+buttonSpacing)*(i*Riff.MAX_BEATS),
					//buttonWidth + (buttonWidth+buttonSpacing)*num,
					-beatsBar_tr.rect.height/2f
				)
			));
		}
	}

	// Initializes a percussion setup menu
	void InitializePercussionSetup (PercussionInstrument percInst) {

		List<string> set = KeyManager.instance.percussionSets[percInst];
		int numDrums = set.Count;

		// Resize note panel
		RectTransform tr = gameObject.RectTransform();
		tr.sizeDelta = new Vector2 (
			(numButtons+1)*buttonWidth + numButtons*buttonSpacing,
			(numDrums+1)*buttonWidth + numDrums*buttonSpacing
		);

		// Resize drum icons panel
		iconBar_tr.sizeDelta = new Vector2 (iconBar_tr.sizeDelta.x, tr.sizeDelta.y);

		// Resize beat number panel
		beatsBar_tr.sizeDelta = new Vector2 (tr.sizeDelta.x, beatsBar_tr.sizeDelta.y);
		beatsBar_tr.localScale = new Vector3 (1f, 1f, 1f);

		int i=0;
		foreach (string note in set) 
			MakePercussionButtons (note, i++, note, percInst.icons[note]);
	}

	// Initializes a melodic setup menu
	void InitializeMelodicSetup (MelodicInstrument meloInst) {

		// Calculate available octaves
		maxOctaves = (int)Mathf.CeilToInt( (float)(Sounds.soundsToLoad[currentRiff.instrument.codeName].Count) / 12f);
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
		RectTransform tr = gameObject.RectTransform();
		tr.sizeDelta = new Vector2 (
			(numButtons+1)*buttonWidth + numButtons*buttonSpacing,
			(numNotes+1)*buttonWidth + numNotes*buttonSpacing
		);

		// Resize note text bar
		iconBar_tr.sizeDelta = new Vector2 (iconBar_tr.sizeDelta.x, tr.sizeDelta.y);

		// Resize beats bar
		beatsBar_tr.sizeDelta = new Vector2 (tr.sizeDelta.x, beatsBar_tr.sizeDelta.y);

		for (int i = 0; i < numNotes && i < totalNotes; i++) {
			string note = Sounds.soundsToLoad[currentRiff.instrument.codeName][i+startIndex];
			bool inScale = KeyManager.instance.scales[key][scale][meloInst].allNotes.Contains(note);
			MakeMelodicButtons (note.Split('_')[1], i, note, inScale);
		}
			
	}

	// Creates all buttons for percussion setup
	void MakePercussionButtons (string title, int row, string soundName, Sprite iconGraphic) {

		// Make icon for note
		buttons.Add(MakeIcon (title, iconGraphic, 
			iconBar_tr, new Vector2 (0.75f*buttonWidth, 0.75f*buttonWidth),
			new Vector2 (
				iconBar_tr.rect.width/2f,
				-buttonWidth - (buttonWidth+buttonSpacing)*row
			))
		);
			
		for (int i = 0; i < numButtons; i ++) {
			int num = i;

			// Check if note is already in riff
			bool noteExists = currentRiff.Lookup(soundName, num);

			// Get volume if so
			float vol = noteExists ? currentRiff.VolumeOfNote(soundName, num) : 1f;

			// Create note button
			GameObject bt = MakeButton(title+"_"+i, 
				(currentRiff.Lookup(soundName, num) ? percussionFilled : percussionEmpty),
				GetComponent<RectTransform>(),
				new Vector2 (buttonWidth*0.75f, buttonWidth*0.75f),
				new Vector2 (
					buttonWidth + (buttonWidth+buttonSpacing)*num,
					-buttonWidth - (buttonWidth+buttonSpacing)*row
				)
			);

			// Change scale based on beat
			RectTransform bt_tr = bt.RectTransform();
			if (i % 4 == 0) // Down beat
				bt_tr.localScale = new Vector3 (baseButtonScale, baseButtonScale, baseButtonScale);
			else if (i % 4 - 2 == 0) // Half beat
				bt_tr.localScale = new Vector3 (0.75f*baseButtonScale, 0.75f*baseButtonScale, 0.75f*baseButtonScale);
			else if (i % 4 - 1 == 0 || i %4 - 3 == 0) // Quarter beat
				bt_tr.localScale = new Vector3 (0.5f*baseButtonScale, 0.5f*baseButtonScale, 0.5f*baseButtonScale);

			// Add StopScrolling tag
			bt.tag = "StopScrolling";

			// Create note
			Note note = new Note (soundName, vol, 1f);

			// Create volume slider
			GameObject volume = UIHelpers.MakeImage(title+"_volume");
			RectTransform volume_tr = volume.RectTransform();
			volume_tr.SetParent(bt_tr);
			volume_tr.sizeDelta = bt_tr.sizeDelta * 1.3f;
			volume_tr.localScale = bt_tr.localScale * 1.3f;
			volume_tr.AnchorAtPoint(0.5f, 0.5f);
			volume_tr.anchoredPosition3D = Vector3.zero;

			Image volume_img = volume.Image();
			volume_img.sprite = GameManager.instance.volumeIcon;
			volume_img.type = Image.Type.Filled;
			volume_img.fillAmount = vol;

			// Setup volume slider
			NoteButton noteButton = bt.AddComponent<NoteButton>();
			noteButton.targetNote = note;
			noteButton.volumeImage = volume.GetComponent<Image>();

			// Create show/hide toggle
			ShowHide bt_sh = bt.AddComponent<ShowHide>();
			bt_sh.objects = new List<GameObject>() { volume};
			bt_sh.transitionType = ShowHide.TransitionType.Instant;
			bt_sh.enabled = currentRiff.Lookup(note, num);

			// Initially hide volume slider
			volume.SetActive(false);

			// Add button functionality
			bt.Button().onClick.AddListener(()=>{
				if (!InputManager.instance.IsDragging) {
					bool n = currentRiff.Toggle(note, num);
					if (!n) bt_sh.Hide();
					bt_sh.enabled = n;
					bt.Image().sprite = (n ? percussionFilled : percussionEmpty);
					if (n) bt_sh.Show();
				}
			});

			// Register button
			buttons.Add(bt);
			buttonGrid[i].Add(bt);
			buttons.Add(volume);
		}
	}

	void MakeMelodicButtons (string title, int row, string fileName, bool inScale) {
		int numButtons = Riff.MAX_BEATS;
		Color transparentWhite = new Color (1f, 1f, 1f, 0.5f);

		// Create note text
		GameObject txt = MakeText (title,
			iconBar_tr,
			new Vector2 (buttonWidth, buttonWidth),
			new Vector2 (
				iconBar_tr.rect.width/2f,
				-buttonWidth - (buttonWidth+buttonSpacing)*row
			)
		);

		// Change text color depending on whether or not the note is in the scale
		txt.Text().color = (inScale ? Color.white : transparentWhite);

		for (int i=0; i<numButtons; i++) {
			int num = i;

			// Check if note is already in riff
			bool noteExists = currentRiff.Lookup(fileName, num);

			// Get volume if so
			float vol = noteExists ? currentRiff.VolumeOfNote(fileName, num) : 1f;

			// Make note button
			GameObject bt = MakeButton(title+"_"+i,
				(currentRiff.Lookup(fileName, num) ? melodicFilled : melodicEmpty),
				GetComponent<RectTransform>(),
				new Vector2(buttonWidth, buttonWidth/2f),
				new Vector2 (
					buttonWidth + (buttonWidth+buttonSpacing)*num,
					-buttonWidth - (buttonWidth+buttonSpacing)*row
				)
			);
					
			// Change scale based on position of note
			RectTransform bt_tr = bt.RectTransform();
			if (i % 4 == 0)
				bt_tr.localScale = new Vector3 (baseButtonScale, baseButtonScale, baseButtonScale);
			else if (i % 4 - 2 == 0)
				bt_tr.localScale = new Vector3 (0.75f*baseButtonScale, 0.75f*baseButtonScale, 0.75f*baseButtonScale);
			else if (i % 4 - 1 == 0 || i % 4 - 3 == 0)
				bt_tr.localScale = new Vector3 (0.5f*baseButtonScale, 0.5f*baseButtonScale, 0.5f*baseButtonScale);

			// Add StopScrolling tag
			bt.tag = "StopScrolling";

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
			NoteButton noteButton = bt.AddComponent<NoteButton>();
			noteButton.targetNote = note;
			noteButton.volumeImage = volume.GetComponent<Image>();

			// Create show/hide toggle
			ShowHide bt_sh = bt.AddComponent<ShowHide>();
			bt_sh.objects = new List<GameObject>() { volume};
			bt_sh.enabled = currentRiff.Lookup(note, num);

			// Initially hide volume slider
			volume.SetActive(false);

			// Setup button
			bt.Button().onClick.AddListener(()=>{
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
					bt.Image().sprite = (n ? melodicFilled : melodicEmpty);
				}
			});

			// Register button
			buttons.Add(txt);
			buttons.Add(bt);
			buttonGrid[num].Add(bt);
		}
	}

	public void SetRiffVolume (Slider slider) {
		currentRiff.volume = slider.value;
	}

	GameObject MakeButton (string title, Sprite image, RectTransform parent, Vector2 sizeD, Vector2 pos) {
		GameObject button = new GameObject();
		button.name = title+"Text";
		button.AddComponent<RectTransform>();
		button.AddComponent<CanvasRenderer>();
		button.AddComponent<Image>();
		button.GetComponent<Image>().sprite = image;
		button.AddComponent<Button>();
		button.GetComponent<RectTransform>().SetParent(parent);
		button.GetComponent<RectTransform>().anchorMin = new Vector2 (0f, 1f);
		button.GetComponent<RectTransform>().anchorMax = new Vector2 (0f, 1f);
		button.GetComponent<RectTransform>().sizeDelta = sizeD;
		button.GetComponent<RectTransform>().anchoredPosition = pos;
		return button;
	}

	GameObject MakeText (string title, RectTransform parent, Vector2 sizeD, Vector2 pos) {
		GameObject text = new GameObject();
		text.AddComponent<CanvasRenderer>();
		text.AddComponent<RectTransform>();
		text.GetComponent<RectTransform>().SetParent(parent);
		text.GetComponent<RectTransform>().sizeDelta = sizeD;
		text.GetComponent<RectTransform>().localScale = new Vector3 (1f, 1f, 1f);
		text.AddComponent<Text>();
		text.GetComponent<Text>().text = title;
		text.GetComponent<Text>().fontSize = 30;
		text.GetComponent<Text>().font = GameManager.instance.font;
		text.GetComponent<Text>().fontStyle = FontStyle.Normal;
		text.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
		text.GetComponent<RectTransform>().anchorMin = new Vector2 (0f, 1f);
		text.GetComponent<RectTransform>().anchorMax = new Vector2 (0f, 1f);
		text.GetComponent<RectTransform>().anchoredPosition = pos;
		return text;
	}

	GameObject MakeIcon (string title, Sprite graphic, RectTransform parent, Vector2 sizeD, Vector2 pos) {
		GameObject icon = new GameObject();
		icon.name = title+"Icon";
		icon.AddComponent<RectTransform>();
		icon.AddComponent<CanvasRenderer>();
		icon.AddComponent<Image>();
		icon.GetComponent<Image>().sprite = graphic;
		icon.GetComponent<RectTransform>().SetParent(parent);
		icon.GetComponent<RectTransform>().sizeDelta = sizeD;
		icon.GetComponent<RectTransform>().localScale = new Vector3 (1f, 1f, 1f);
		icon.GetComponent<RectTransform>().anchorMin = new Vector2 (0f, 1f);
		icon.GetComponent<RectTransform>().anchorMax = new Vector2 (0f, 1f);
		icon.GetComponent<RectTransform>().anchoredPosition = pos;
		return icon;
	}

	// Flips button art
	void Toggle (Button button) {
		Sprite img = button.GetComponent<Image>().sprite;
		if (img == melodicEmpty || img == melodicSuggested) {
			button.GetComponent<Image>().sprite = melodicFilled;
		} else if (img == melodicFilled) {
			button.GetComponent<Image>().sprite = melodicEmpty;
		} else if (img == percussionEmpty || img == percussionSuggested) {
			button.GetComponent<Image>().sprite = percussionFilled;
		} else {
			button.GetComponent<Image>().sprite = percussionEmpty;
		}
	}

	public void SyncHorizontalScrollViews (Scrollbar slider) {
		scrollBarH.value = slider.value;
		beatsBar.value = slider.value;
	}

	public void SyncVerticalScrollViews (Scrollbar slider) {
		scrollBarV.value = slider.value;
		iconBar.value = slider.value;
	}

	public void IncreaseOctavesShown () {
		if (octavesShown < maxOctaves) octavesShown++;
		Initialize();
	}

	public void DecreaseOctavesShown () {
		if (octavesShown > 1) octavesShown--;
		Initialize();
	}

	public void TogglePlayRiffButton () {
		if (playRiffButton.GetComponent<Image>().sprite == GameManager.instance.playIcon) 
			playRiffButton.GetComponent<Image>().sprite = GameManager.instance.pauseIcon;
		else playRiffButton.GetComponent<Image>().sprite = GameManager.instance.playIcon;
	}

	void UpdateBeatsText() {
		//beatsText.GetComponent<Text>().text = "Beats: "+ currentRiff.beatsShown.ToString();
	}

	public void UpdateTempoText () {
		tempoText.text = MusicManager.instance.tempo.ToString();
	}

	public void HideSliders () {
		if (sliders != null) {
			foreach (GameObject slider in sliders) {
				slider.SetActive(false);
			}
		}
	}

	public void HideSliders (GameObject obj) {
		bool st = obj.activeSelf;
		HideSliders();
		obj.SetActive(!st);
	}

	void Suggest () {
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
	}

	void Suggest (GameObject button) {
		Debug.Log ("in Suggest");
		Sprite img = button.GetComponent<Image>().sprite;
		if (currentRiff.instrument.type == Instrument.Type.Percussion && img != percussionFilled) {
			button.GetComponent<Image>().sprite = percussionSuggested;
		} else if (img != melodicFilled) {
			button.GetComponent<Image>().sprite  = melodicSuggested;
		}
	}

	void SuggestChords (int column, int row) {
		ClearSuggestions();
		if (row+3 < buttonGrid[0].Count)
			SuggestMinorChord(buttonGrid[column][row+3]);
		if (row+7 < buttonGrid[0].Count) 
			SuggestPowerChord(buttonGrid[column][row+7]);
		if (row+12 < buttonGrid[0].Count) 
			SuggestOctave(buttonGrid[column][row+12]);
	}

	void ClearSuggestions () {
		foreach (GameObject suggestion in suggestions) Destroy(suggestion);
		suggestions.Clear();
		GameManager.instance.HideTooltip();
	}

	void SuggestMinorChord (GameObject button) {
		GameObject suggestion = MakeIcon (
			"Minor", 
			minorSuggestion, 
			button.GetComponent<RectTransform>(), 
			new Vector2 (
				button.GetComponent<RectTransform>().sizeDelta.y,
				button.GetComponent<RectTransform>().sizeDelta.y
			),
			new Vector2 (
				button.GetComponent<RectTransform>().sizeDelta.x*0.5f, 
				-button.GetComponent<RectTransform>().sizeDelta.y*0.5f
			)
		);
		suggestion.AddComponent<Tooltip>();
		suggestion.GetComponent<Tooltip>().text = "Minor Chord (sad)";
		suggestions.Add(suggestion);
	}

	void SuggestPowerChord (GameObject button) {
		GameObject suggestion = MakeIcon (
			"Power", 
			powerSuggestion, 
			button.GetComponent<RectTransform>(), 
			new Vector2 (
				button.GetComponent<RectTransform>().sizeDelta.y,
				button.GetComponent<RectTransform>().sizeDelta.y
			),
			new Vector2 (
				button.GetComponent<RectTransform>().sizeDelta.x*0.5f, 
				-button.GetComponent<RectTransform>().sizeDelta.y*0.5f
			)
		);
		suggestion.AddComponent<Tooltip>();
		suggestion.GetComponent<Tooltip>().text = "Power Chord (powerful)";
		suggestions.Add(suggestion);
	}

	void SuggestOctave (GameObject button) {
		GameObject suggestion = MakeIcon (
			"Octave", 
			octaveSuggestion, 
			button.GetComponent<RectTransform>(), 
			new Vector2 (
				button.GetComponent<RectTransform>().sizeDelta.y,
				button.GetComponent<RectTransform>().sizeDelta.y
			),
			new Vector2 (
				button.GetComponent<RectTransform>().sizeDelta.x*0.5f, 
				-button.GetComponent<RectTransform>().sizeDelta.y*0.5f
			)
		);
		suggestion.AddComponent<Tooltip>();
		suggestion.GetComponent<Tooltip>().text = "Octave (neutral)";
		suggestions.Add(suggestion);
	}

	#endregion
}
