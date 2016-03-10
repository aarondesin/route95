﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InstrumentSetup : MonoBehaviour {

	public static InstrumentSetup instance;

	public RiffAI riffai;
	public bool DICKSAUCE;

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

	void Start () {
		instance = this;
		nameInputField.onEndEdit.AddListener(delegate { currentRiff.name = nameInputField.text; });
		riffai = new RiffAI ();
		DICKSAUCE = true;
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
		scrollBarH.value = 0f;
		scrollBarV.value = 1f;
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
			break;
		}
	}

	// Initializes a melodic setup menu
	void InitializeMelodicSetup (MelodicInstrument meloInst) {
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
				//Suggest (riffai.FindHintXPosition(riffai.FindSimilarCase (currentRiff), currentRiff));
				Toggle(bt.GetComponent<Button>());
			});
			buttons.Add(bt);
			buttonGrid[i].Add(bt);

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

	void Suggest (int pos) {
		if (pos >= buttons.Count) {
			Debug.LogError ("Suggestion out of bounds!");
			return;
		} else {
			Debug.Log("Suggesting "+pos);
			Debug.Log ("curr key " + MusicManager.instance.currentKey);
			Debug.Log (" curr inst " +InstrumentSetup.currentRiff.instrument);
			Debug.Log (" guitar " + Instrument.ElectricGuitar);
			//FOR TESTING PURPOSES

			//if (DICKSAUCE) {
			//CaseLibrary.initializecases ();
			//DICKSAUCE = false;
			//}
			Suggest (buttons[riffai.FindHintYPosition(currentRiff, KeyManager.instance.scales [MusicManager.instance.currentKey] [InstrumentSetup.currentRiff.instrument])*subdivsShown + pos+1]);
		}
	}

	void Suggest (GameObject button) {
		//if (button == null) Debug.Log("fuck", button);
		//if (button.GetComponent<Image>() == null) Debug.Log("fuck", button);
		Sprite img = button.GetComponent<Image>().sprite;
		if (currentRiff.instrument == Instrument.RockDrums && img != percussionFilled) {
			button.GetComponent<Image>().sprite = percussionSuggested;
		} else if (img != melodicFilled) {
			button.GetComponent<Image>().sprite  = melodicSuggested;
		}
	}
}
