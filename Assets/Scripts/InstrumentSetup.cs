using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class InstrumentSetup : MonoBehaviour {

	public static InstrumentSetup instance;

	#region InstrumentSetup Vars

	public const int MAX_NUMNOTES = 8;

	public Riff currentRiff; // current riff being edited

	public RiffAI riffai;

	// Icons for percussion setup buttons
	public Sprite percussionEmpty;
	public Sprite percussionFilled;
	public Sprite percussionSuggested;

	// Icons for melodic setup buttons
	public Sprite melodicEmpty;
	public Sprite melodicFilled;
	public Sprite melodicSuggested;

	public static float baseButtonScale = 1f; // base button sizes

	public static int numRows;
	public static int subdivsShown = 2; // number of subdivisions to show
	public int octavesShown = 2;
	int maxOctaves;

	List<GameObject> buttons = new List<GameObject>();
	List<List<GameObject>> buttonGrid = new List<List<GameObject>>();

	public Scrollbar scrollBarH;
	public Scrollbar scrollBarV;

	public InputField nameInputField;

	public GameObject playRiffButton;

	public GameObject beatsText;
	public Text tempoText;

	float buttonWidth = 128f;
	float buttonSpacing = 8f;

	List<GameObject> suggestions = new List<GameObject>();
	public Sprite minorSuggestion;
	public Sprite powerSuggestion;
	public Sprite octaveSuggestion;

	public Scrollbar iconBar;
	public RectTransform iconBar_tr;

	public Scrollbar beatsBar;
	public RectTransform beatsBar_tr;

	int numNotes;
	int numButtons;

	public List<GameObject> sliders;

	#endregion
	#region Unity Callbacks

	void Start () {
		instance = this;
		nameInputField.onEndEdit.AddListener(delegate { currentRiff.name = nameInputField.text; });
		//riffai = new RiffAI ();

	}

	#endregion
	#region InstrumentSetup Methods

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

	// Calls appropriate Setup() function based on current instrument
	public void Initialize () {
		
		if (currentRiff == null) {
			Debug.LogError ("InstrumentSetup.Initialize(): no riff selected!");
			return;
		}

		HideSliders ();

		// Clear all previous buttons
		Cleanup();

		// Set up riff editor properties
		nameInputField.text = currentRiff.name;
		playRiffButton.GetComponent<Image>().sprite = GameManager.instance.playIcon;
		MakeBeatNumbers ();
		UpdateBeatsText();
		UpdateTempoText();
		scrollBarH.value = 0.01f;
		scrollBarV.value = 0.99f;

		numButtons = (int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS+2)*currentRiff.beatsShown/4;

		// Create note buttons
		if (currentRiff.instrument.type == Instrument.Type.Percussion)
			InitializePercussionSetup ((PercussionInstrument)currentRiff.instrument);
		else if (currentRiff.instrument.type == Instrument.Type.Melodic)
			InitializeMelodicSetup ((MelodicInstrument)currentRiff.instrument);
		else Debug.LogError(currentRiff.instrument.name + " unable to initialize.");

		//AudioSource source = MusicManager.instance.instrumentAudioSources [currentRiff.instrument];
		//source.gameObject.GetComponent<AudioDistortionFilter>().distortionLevel = currentRiff.distortionLevel;

	}

	// Removes all existing buttons
	public void Cleanup () {
		foreach (GameObject button in buttons) Destroy(button);
		buttons.Clear();
		foreach (List<GameObject> list in buttonGrid) list.Clear();
	}

	public void MakeBeatNumbers () {
		for (int i=0; i<currentRiff.beatsShown; i++) {
			buttons.Add(MakeText((i+1).ToString(), beatsBar_tr,
				new Vector2 (48f, 48f),
				new Vector2 (
					buttonWidth*2f + (buttonWidth+buttonSpacing)*(i*(int)Mathf.Pow(2f,Riff.MAX_SUBDIVS)),
					-beatsBar_tr.rect.height/2f
				)
			));
		}
	}

	// Initializes a percussion setup menu
	void InitializePercussionSetup (PercussionInstrument percInst) {
		int i=0;
		foreach (string note in KeyManager.instance.percussionSets[percInst]) 
			MakePercussionButtons (note, i++, note, percInst.icons[note]);
	}

	// Initializes a melodic setup menu
	void InitializeMelodicSetup (MelodicInstrument meloInst) {
		buttonGrid.Clear();
		for(int n = 0; n<(int)currentRiff.beatsShown*(int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS); n++) {
			buttonGrid.Add(new List<GameObject>());
		}
		//int i = 0;

		maxOctaves = (int)Mathf.CeilToInt( (float)(Sounds.soundsToLoad[currentRiff.instrument.codeName].Count) / 12f);
		if (maxOctaves < octavesShown) octavesShown = maxOctaves;

		Key key = MusicManager.instance.currentSong.key;
		ScaleInfo scale = ScaleInfo.AllScales[MusicManager.instance.currentSong.scale];
		int startIndex = meloInst.startingNote[key];
		int totalNotes = Sounds.soundsToLoad[currentRiff.instrument.codeName].Count;
		numNotes = startIndex + 12 * octavesShown + 1;

		GetComponent<RectTransform>().sizeDelta = new Vector2 (
			(numButtons+1)*buttonWidth + numButtons*buttonSpacing,
			(numNotes+1)*buttonWidth + numNotes*buttonSpacing
		);

		for (int i = startIndex; i < numNotes && i < totalNotes; i++) {
			string note = Sounds.soundsToLoad[currentRiff.instrument.codeName][i];
			bool inScale = KeyManager.instance.scales[key][scale][meloInst].allNotes.Contains(note);
			MakeMelodicButtons (note.Split('_')[1], i, note, inScale);
			//i++;
		}
			
	}

	// Creates all buttons for percussion setup
	void MakePercussionButtons (string title, int row, string soundName, Sprite iconGraphic) {
		
		buttonGrid.Clear ();
		for(int n = 0; n<numButtons; n++) {
			buttonGrid.Add(new List<GameObject>());
		}
		GetComponent<RectTransform>().sizeDelta = new Vector2 (
			(numButtons+1)*buttonWidth + numButtons*buttonSpacing,
			(row+2)*buttonWidth + row*buttonSpacing
		);
		iconBar_tr.sizeDelta = new Vector2 (iconBar_tr.sizeDelta.x, GetComponent<RectTransform>().sizeDelta.y);
		beatsBar_tr.sizeDelta = new Vector2 (GetComponent<RectTransform>().sizeDelta.x, beatsBar_tr.sizeDelta.y);
		beatsBar_tr.localScale = new Vector3 (1f, 1f, 1f);

		// Make icon for note
		buttons.Add(MakeIcon (title, iconGraphic, 
			iconBar_tr, new Vector2 (0.75f*buttonWidth, 0.75f*buttonWidth),
			new Vector2 (
				iconBar_tr.rect.width/2f,
				-buttonWidth - (buttonWidth+buttonSpacing)*row
			))
		);

		for (int i=0; i<numButtons; i+=(int)Mathf.Pow(2f, (float)(Riff.MAX_SUBDIVS-subdivsShown))) { // 0=4 1=2 2=1
			int num = i;
			bool noteExists = currentRiff.Lookup(soundName, num);
			float vol = 1f;
			if (noteExists) vol = currentRiff.VolumeOfNote(soundName, num);
			GameObject bt = MakeButton(title+"_"+i, 
				(currentRiff.Lookup(soundName, num) ? percussionFilled : percussionEmpty),
				GetComponent<RectTransform>(),
				new Vector2 (buttonWidth*0.75f, buttonWidth*0.75f),
				new Vector2 (
					buttonWidth + (buttonWidth+buttonSpacing)*num,
					-buttonWidth - (buttonWidth+buttonSpacing)*row
				)
			);
			bt.tag = "StopScrolling";
			bt.AddComponent<NoteButton>();
			if (i%(4*Riff.MAX_SUBDIVS)%4 == 0) {
				bt.GetComponent<RectTransform>().localScale = new Vector3 (baseButtonScale, baseButtonScale, baseButtonScale);
			} else if (i%(4*Riff.MAX_SUBDIVS)%4-2 == 0) {
				bt.GetComponent<RectTransform>().localScale = new Vector3 (0.75f*baseButtonScale, 0.75f*baseButtonScale, 0.75f*baseButtonScale);
			} else if (i%(4*Riff.MAX_SUBDIVS)%4-1 == 0 || i%(4*Riff.MAX_SUBDIVS)%4-3 == 0) {
				bt.GetComponent<RectTransform>().localScale = new Vector3 (0.5f*baseButtonScale, 0.5f*baseButtonScale, 0.5f*baseButtonScale);
			}
				
			Note note = new Note (soundName, vol, 1f);
			GameObject volume = UI.MakeImage(title+"_volume");
			volume.GetComponent<RectTransform>().SetParent(bt.GetComponent<RectTransform>());
			volume.GetComponent<RectTransform>().sizeDelta = bt.GetComponent<RectTransform>().sizeDelta * 1.3f;
			volume.GetComponent<RectTransform>().localScale = bt.GetComponent<RectTransform>().localScale * 1.3f;
			volume.GetComponent<RectTransform>().anchorMin = new Vector2 (0.5f, 0.5f);
			volume.GetComponent<RectTransform>().anchorMax = new Vector2 (0.5f, 0.5f);
			volume.GetComponent<RectTransform>().anchoredPosition3D = new Vector3 (0f, 0f, 0f);
			volume.GetComponent<Image>().sprite = GameManager.instance.volumeIcon;
			volume.GetComponent<Image>().type = Image.Type.Filled;
			if (vol != 1f) Debug.Log(vol);
			volume.GetComponent<Image>().fillAmount = vol;


			bt.GetComponent<NoteButton>().targetNote = note;
			bt.GetComponent<NoteButton>().volumeImage = volume.GetComponent<Image>();

			bt.AddComponent<ShowHide>();
			bt.GetComponent<ShowHide>().objects = new List<GameObject>() { volume};
			bt.GetComponent<ShowHide>().transitionType = TransitionType.Instant;
			bt.GetComponent<ShowHide>().enabled = currentRiff.Lookup(note, num);

			volume.SetActive(false);


			bt.GetComponent<Button>().onClick.AddListener(()=>{
				currentRiff.Toggle(note, num);
				bt.GetComponent<ShowHide>().enabled = currentRiff.Lookup(note, num);
				// (riffai.FindHintXPosition(riffai.FindSimilarCase (currentRiff), currentRiff));
				Toggle(bt.GetComponent<Button>());
			});

			buttons.Add(bt);
			buttonGrid[i].Add(bt);
			buttons.Add(volume);
		}
	}

	void MakeMelodicButtons (string title, int row, string fileName, bool inScale) {
		int numButtons = (int)currentRiff.beatsShown*(int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS);
		//Debug.Log ("numbuttons (x length) " + numButtons);
		//buttonGrid.Clear ();

		iconBar_tr.sizeDelta = new Vector2 (iconBar_tr.sizeDelta.x, GetComponent<RectTransform>().sizeDelta.y);
		beatsBar_tr.sizeDelta = new Vector2 (GetComponent<RectTransform>().sizeDelta.x, beatsBar_tr.sizeDelta.y);

		GameObject txt = MakeText (title,
			iconBar_tr,
			new Vector2 (buttonWidth, buttonWidth),
			new Vector2 (
				iconBar_tr.rect.width/2f,
				-buttonWidth - (buttonWidth+buttonSpacing)*row
			)
		);
		Color color = new Color (1f, 1f, 1f, 0.5f);
		txt.GetComponent<Text>().color = (inScale ? Color.white : color);
		buttons.Add(txt);

		for (int i=0; i<numButtons; i+=(int)Mathf.Pow(2f, (float)(Riff.MAX_SUBDIVS-subdivsShown))) {
			int num = i;
			GameObject bt = MakeButton(title+"_"+i,
				(currentRiff.Lookup(fileName, num) ? melodicFilled : melodicEmpty),
				GetComponent<RectTransform>(),
				new Vector2(buttonWidth, buttonWidth/2f),
				new Vector2 (
					buttonWidth + (buttonWidth+buttonSpacing)*num,
					-buttonWidth - (buttonWidth+buttonSpacing)*row
				));
			float vol = 1f;
			if (i%(4*Riff.MAX_SUBDIVS)%4 == 0) {
				bt.GetComponent<RectTransform>().localScale = new Vector3 (baseButtonScale, baseButtonScale, baseButtonScale);
			} else if (i%(4*Riff.MAX_SUBDIVS)%4-2 == 0) {
				bt.GetComponent<RectTransform>().localScale = new Vector3 (0.75f*baseButtonScale, 0.75f*baseButtonScale, 0.75f*baseButtonScale);
				vol = 0.9f;
			} else if (i%(4*Riff.MAX_SUBDIVS)%4-1 == 0 || i%(4*Riff.MAX_SUBDIVS)%4-3 == 0) {
				bt.GetComponent<RectTransform>().localScale = new Vector3 (0.5f*baseButtonScale, 0.5f*baseButtonScale, 0.5f*baseButtonScale);
				vol = 0.8f;
			}
			bt.GetComponent<Button>().onClick.AddListener(()=>{
				currentRiff.Toggle(new Note(fileName, vol, 1f), num);
				if (currentRiff.Lookup(new Note(fileName, vol, 1f),num)) SuggestChords(num, row);
				else ClearSuggestions();
				//Suggest ();
				//SuggestChords(bt);
				
				Toggle(bt.GetComponent<Button>());
			});
			buttons.Add(bt);
			buttonGrid[num].Add(bt);

		}
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

	public void SyncScrollViews () {
		iconBar.value = scrollBarV.value;
		beatsBar.value = scrollBarH.value;
	}

	public void IncreaseSubdivisions () {
		if (subdivsShown < Riff.MAX_SUBDIVS) {
			subdivsShown++;
			Initialize();
		}
	}

	public void DecreaseSubdivisions () {
		if (subdivsShown > 0) {
			subdivsShown--;
			Initialize();
		}
	}

	public void IncreaseBeatsShown () {
		currentRiff.ShowMore();
		Initialize();

	}

	public void DecreaseBeatsShown () {
		currentRiff.ShowLess();
		Initialize();
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
		beatsText.GetComponent<Text>().text = "Beats: "+ currentRiff.beatsShown.ToString();
	}

	public void UpdateTempoText () {
		tempoText.text = MusicManager.instance.tempo.ToString();
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
		if (row+2 < buttonGrid[0].Count)
			SuggestMinorChord(buttonGrid[column][row+2]);
		if (row+4 < buttonGrid[0].Count) 
			SuggestPowerChord(buttonGrid[column][row+4]);
		if (row+7 < buttonGrid[0].Count) 
			SuggestOctave(buttonGrid[column][row+7]);
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
