using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class InstrumentSetup : MonoBehaviour {

	public static InstrumentSetup instance;

	public RiffAI riffai;
	public static bool DICKSAUCE = true;

	// Icons for percussion setup buttons
	public Sprite percussionEmpty;
	public Sprite percussionFilled;
	public Sprite percussionSuggested;

	// Icons for melodic setup buttons
	public Sprite melodicEmpty;
	public Sprite melodicFilled;
	public Sprite melodicSuggested;

	// Icons for instruments
	public Sprite kickIcon;
	public Sprite snareIcon;
	public Sprite tomIcon;
	public Sprite hatIcon;

	public static Riff currentRiff; // current riff being edited

	public static float baseButtonScale = 1f; // base button sizes
	public static int MAX_NUMNOTES = 8; // maximum number of notes (rows)
	public static int numRows;
	public static int subdivsShown = 1; // number of subdivisions to show

	List<GameObject> buttons = new List<GameObject>();
	List<List<GameObject>> buttonGrid = new List<List<GameObject>>();

	public Scrollbar scrollBarH;
	public Scrollbar scrollBarV;

	public InputField nameInputField;

	public GameObject playRiffButton;
	public Sprite play;
	public Sprite pause;

	public GameObject beatsText;
	public Text tempoText;

	float buttonWidth = 128f;
	float buttonSpacing = 8f;

	List<GameObject> suggestions = new List<GameObject>();
	public Sprite minorSuggestion;
	public Sprite powerSuggestion;
	public Sprite octaveSuggestion;

	void Start () {
		instance = this;
		nameInputField.onEndEdit.AddListener(delegate { currentRiff.name = nameInputField.text; });
		riffai = new RiffAI ();

	}

	// Calls appropriate Setup() function based on current instrument
	public void Initialize () {
		Cleanup();
		if (currentRiff == null) currentRiff = MusicManager.instance.riffs[0];
		nameInputField.text = currentRiff.name;
		MakeBeatNumbers ();
		//switch (MusicManager.currentInstrument) {
		switch (currentRiff.instrument) {
		case Instrument.RockDrums:
			InitializePercussionSetup (PercussionInstrument.RockDrums);
			break;
		case Instrument.ElectricGuitar:
			InitializeMelodicSetup (MelodicInstrument.ElectricGuitar);
			break;
		case Instrument.ElectricBass:
			InitializeMelodicSetup (MelodicInstrument.ElectricBass);
			break;
		}
		scrollBarH.value = 0.01f;
		scrollBarV.value = 0.99f;
		playRiffButton.GetComponent<Image>().sprite = play;
		UpdateBeatsText();
		UpdateTempoText();
	}

	// Removes all existing buttons
	public void Cleanup () {
		foreach (GameObject button in buttons) {
			Destroy(button);
		}
		buttons.Clear();
		foreach (List<GameObject> list in buttonGrid) {
			list.Clear();
		}
	}

	public void MakeBeatNumbers () {
		for (int i=0; i<currentRiff.beatsShown; i++) {
			buttons.Add(MakeText((i+1).ToString(), GetComponent<RectTransform>(),
				new Vector2 (48f, 48f),
				new Vector2 (
					buttonWidth*2f + (buttonWidth+buttonSpacing)*(i*(int)Mathf.Pow(2f,Riff.MAX_SUBDIVS)),
					-0.5f*buttonWidth
				)
			));
		}
	}

	// Initializes a percussion setup menu
	void InitializePercussionSetup (PercussionInstrument percInst) {
		switch (percInst) {
		case PercussionInstrument.RockDrums:
			// Make rows of buttons for drums
			numRows = 4;
			MakePercussionButtons ("Kick", 0, "Audio/Instruments/Percussion/RockDrums_Kick", kickIcon);
			MakePercussionButtons ("Snare", 1, "Audio/Instruments/Percussion/RockDrums_Snare", snareIcon);
			MakePercussionButtons ("Tom", 2, "Audio/Instruments/Percussion/RockDrums_Tom", tomIcon);
			MakePercussionButtons ("Hat", 3, "Audio/Instruments/Percussion/RockDrums_Hat", hatIcon);
			MakePercussionButtons ("Crash", 4, "Audio/Instruments/Percussion/RockDrums_Crash", hatIcon);
			break;
		}
	}

	// Initializes a melodic setup menu
	void InitializeMelodicSetup (MelodicInstrument meloInst) {
		buttonGrid.Clear();
		for(int n = 0; n<(int)currentRiff.beatsShown*(int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS); n++) {
			buttonGrid.Add(new List<GameObject>());
		}
		int i = 0;
		switch (meloInst) {
		case MelodicInstrument.ElectricGuitar:

			// Make rows of buttons for notes (in a grid)
			foreach (string note in KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].allNotes) {
				MakeMelodicButtons (note.Split ('_') [1], i, note);
				i++;
			}
			break;

		case MelodicInstrument.ElectricBass: 

			// Make rows of buttons for notes (in a grid)
			foreach (string note in KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].allNotes) {
				//if (note == null)
				//Debug.Log ("dick");
				MakeMelodicButtons (note.Split ('_') [1], i, note);
				i++;
			}
			break;
		}
	}

	// Creates all buttons for percussion setup
	void MakePercussionButtons (string title, int row, string soundName, Sprite iconGraphic) {
		int numButtons = (int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS+2)*currentRiff.beatsShown/4;
		buttonGrid.Clear ();
		for(int n = 0; n<numButtons; n++) {
			buttonGrid.Add(new List<GameObject>());
		}
		GetComponent<RectTransform>().sizeDelta = new Vector2 (
			(numButtons+2)*buttonWidth + numButtons*buttonSpacing,
			(row+2)*buttonWidth + row*buttonSpacing
		);

		// Make icon for note
		buttons.Add(MakeIcon (title, iconGraphic, 
			GetComponent<RectTransform>(), new Vector2 (0.75f*buttonWidth, 0.75f*buttonWidth),
			new Vector2 (
				buttonWidth,
				-buttonWidth - (buttonWidth+buttonSpacing)*row
			))
		);

		for (int i=0; i<numButtons; i+=(int)Mathf.Pow(2f, (float)(Riff.MAX_SUBDIVS-subdivsShown))) { // 0=4 1=2 2=1
			int num = i;
			GameObject bt = MakeButton(title+"_"+i, 
				(currentRiff.Lookup(soundName, num) ? percussionFilled : percussionEmpty),
				GetComponent<RectTransform>(),
				new Vector2 (buttonWidth*0.75f, buttonWidth*0.75f),
				new Vector2 (
					2f*buttonWidth + (buttonWidth+buttonSpacing)*num,
					-buttonWidth - (buttonWidth+buttonSpacing)*row
				)
			);
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
				InstrumentSetup.currentRiff.Toggle(new Note (soundName, vol, 1f), num);
				// (riffai.FindHintXPosition(riffai.FindSimilarCase (currentRiff), currentRiff));
				Toggle(bt.GetComponent<Button>());
			});
			buttons.Add(bt);
			buttonGrid[i].Add(bt);
		}
	}

	void MakeMelodicButtons (string title, int row, string fileName) {
		int numButtons = (int)currentRiff.beatsShown*(int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS);
		Debug.Log ("numbuttons (x length) " + numButtons);
		//buttonGrid.Clear ();
		GetComponent<RectTransform>().sizeDelta = new Vector2 (
			(numButtons+2)*buttonWidth + numButtons*buttonSpacing,
			(row+2)*buttonWidth + row*buttonSpacing
		);

		buttons.Add(MakeText (title,
			GetComponent<RectTransform>(),
			new Vector2 (buttonWidth, buttonWidth),
			new Vector2 (
				buttonWidth,
				-buttonWidth - (buttonWidth+buttonSpacing)*row
			)
		));

		for (int i=0; i<numButtons; i+=(int)Mathf.Pow(2f, (float)(Riff.MAX_SUBDIVS-subdivsShown))) {
			int num = i;
			GameObject bt = MakeButton(title+"_"+i,
				(currentRiff.Lookup(fileName, num) ? melodicFilled : melodicEmpty),
				GetComponent<RectTransform>(),
				new Vector2(buttonWidth, buttonWidth/2f),
				new Vector2 (
					2f*buttonWidth + (buttonWidth+buttonSpacing)*num,
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
				InstrumentSetup.currentRiff.Toggle(new Note(fileName, vol, 1f), num);
				if (InstrumentSetup.currentRiff.Lookup(new Note(fileName, vol, 1f),num)) SuggestChords(num, row);
				else ClearSuggestions();
				Suggest ();
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
		text.GetComponent<Text>().fontSize = 36;
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

	public void TogglePlayRiffButton () {
		if (playRiffButton.GetComponent<Image>().sprite == play) playRiffButton.GetComponent<Image>().sprite = pause;
		else playRiffButton.GetComponent<Image>().sprite = play;
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

		int posY = riffai.FindHintYPosition (currentRiff, KeyManager.instance.scales [MusicManager.instance.currentKey] [ currentRiff.instrument], subdivsShown);
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
			int subPower = Convert.ToInt32(Math.Pow(2, subdivsShown));
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
		if (currentRiff.instrument == Instrument.RockDrums && img != percussionFilled) {
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
}
