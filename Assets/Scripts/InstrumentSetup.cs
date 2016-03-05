using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InstrumentSetup : MonoBehaviour {

	// Icons for percussion setup buttons
	public Sprite percussionEmpty;
	public Sprite percussionFilled;

	// Icons for melodic setup buttons
	public Sprite melodicEmpty;
	public Sprite melodicFilled;

	// Icons for instruments
	public Sprite kickIcon;
	public Sprite snareIcon;
	public Sprite tomIcon;
	public Sprite hatIcon;

	public static Riff currentRiff; // current riff being edited

	public static float baseButtonScale = 1f; // base button sizes
	public static int MAX_NUMNOTES = 8; // maximum number of notes (rows)
	public static int numRows;
	public static int subdivsShown = 2; // number of subdivisions to show

	List<GameObject> buttons = new List<GameObject>();

	public InputField nameInputField;

	void Start () {
		nameInputField.onEndEdit.AddListener(delegate { currentRiff.name = nameInputField.text; });
	}

	// Calls appropriate Setup() function based on current instrument
	public void Initialize () {
		Cleanup();
		if (currentRiff == null) currentRiff = MusicManager.instance.riffs[0];
		nameInputField.text = currentRiff.name;
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

	}

	// Removes all existing buttons
	public void Cleanup () {
		foreach (GameObject button in buttons) {
			Destroy(button);
		}
	}

	// Initializes a percussion setup menu
	void InitializePercussionSetup (PercussionInstrument percInst) {
		switch (percInst) {
		case PercussionInstrument.RockDrums:
			// Make rows of buttons for drums
			numRows = 4;
			MakePercussionButtons ("Kick", 0, new Note ("RockDrums_Kick"), kickIcon);
			MakePercussionButtons ("Snare", 1, new Note ("RockDrums_Snare"), snareIcon);
			MakePercussionButtons ("Tom", 2, new Note ("RockDrums_Tom"), tomIcon);
			MakePercussionButtons ("Hat", 3, new Note ("RockDrums_Hat"), hatIcon);
			break;
		}
	}

	// Initializes a melodic setup menu
	void InitializeMelodicSetup (MelodicInstrument meloInst) {
		switch (meloInst) {
			case MelodicInstrument.ElectricGuitar:
			// Make rows of buttons for notes (in a grid)
			numRows = 7;
			MakeMelodicButtons ("E2", 0, new Note ("ElectricGuitar_E2")); // MakeMelodicButtons(Scale.root.name, 0, Scale.root.note)
			MakeMelodicButtons ("F#2", 1, new Note ("ElectricGuitar_F#2"));
			MakeMelodicButtons ("G#2", 2, new Note ("ElectricGuitar_G#2"));
			MakeMelodicButtons ("A2", 3, new Note ("ElectricGuitar_A2"));
			MakeMelodicButtons ("B2", 4, new Note ("ElectricGuitar_B2"));
			MakeMelodicButtons ("C#3", 5, new Note ("ElectricGuitar_C#3"));
			MakeMelodicButtons ("D#3", 6, new Note ("ElectricGuitar_D#3"));
																		// MakeMelodicButtons(Key.octave, #, Key.octave.note

			/* public class Scale {
			 * public Note root;
			 * public Note second;
			 * ...
			 * ...
			}*/

			/*
			public static Scale EMinor = new Scale () {
				root = MusicManager.Sound"...."
					second1
					...
					octave2 = MuiscManager.Sound
					second2
			}
				switch (MusicManager.currentKey) {
					case Key.EMinor:
				switch (riff.currentInstrument) {
					case Electrci
					..
						..
				public static Scale EMinor_ElectricGuitar = new Scale();
				

				scales[MusicManager.currentKey][currentInstrument];
			*/
			break;

			case MelodicInstrument.ElectricBass: 
			// Make rows of buttons for notes (in a grid)
			numRows = 7;
			MakeMelodicButtons ("E3", 0, new Note (MusicManager.SoundClips ["ElectricBass_E1"]));
					MakeMelodicButtons ("F#3", 1, new Note (MusicManager.SoundClips ["ElectricBass_F#1"]));
					MakeMelodicButtons ("G#3", 2, new Note (MusicManager.SoundClips ["ElectricBass_G#1"]));
					MakeMelodicButtons ("A3", 3, new Note (MusicManager.SoundClips ["ElectricBass_A1"]));
					MakeMelodicButtons ("B3", 4, new Note (MusicManager.SoundClips ["ElectricBass_B1"]));
					MakeMelodicButtons ("C#4", 5, new Note (MusicManager.SoundClips ["ElectricBass_C#2"]));
					MakeMelodicButtons ("D#4", 6, new Note (MusicManager.SoundClips ["ElectricBass_D#2"]));
			break;
		}
	}

	// Creates all buttons for percussion setup
	void MakePercussionButtons (string title, int row, Note note, Sprite iconGraphic) {
		int numButtons = (int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS+2);
		// Make icon for note
		GameObject icon = new GameObject();
		icon.name = title+"Icon";
		icon.AddComponent<RectTransform>();
		icon.AddComponent<CanvasRenderer>();
		icon.AddComponent<Image>();
		icon.GetComponent<Image>().sprite = iconGraphic;
		icon.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());
		float buttonWidth = GetComponent<RectTransform>().rect.height/(2f*numRows);
		icon.GetComponent<RectTransform>().sizeDelta = new Vector2 (buttonWidth, buttonWidth);
		icon.GetComponent<RectTransform>().localScale = new Vector3 (baseButtonScale, baseButtonScale, baseButtonScale);
		icon.GetComponent<RectTransform>().anchorMin = new Vector2 (0f, 0.0f);
		icon.GetComponent<RectTransform>().anchorMax = new Vector2 (0f, 0.0f);
		icon.GetComponent<RectTransform>().anchoredPosition = new Vector2 (
			GetComponent<RectTransform>().sizeDelta.x/numButtons,
			(float)(MAX_NUMNOTES-(2*row+1))*(GetComponent<RectTransform>().offsetMax.y-GetComponent<RectTransform>().offsetMin.y)/((float)(2*numRows)));
		for (int i=0; i<numButtons; i+=(int)Mathf.Pow(2f, (float)(Riff.MAX_SUBDIVS-subdivsShown))) { // 0=4 1=2 2=1
			
			int num = i;
			GameObject bt = new GameObject();
			bt.name = title+"_"+i;
			bt.AddComponent<RectTransform>();
			bt.AddComponent<CanvasRenderer>();
			bt.AddComponent<Image>();
			if (currentRiff.Lookup(note, num)) {
				bt.GetComponent<Image>().sprite = percussionFilled;
				//Debug.Log("kobe");
			} else {
				bt.GetComponent<Image>().sprite = percussionEmpty;
			}
			bt.AddComponent<Button>();
			bt.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());
			bt.GetComponent<RectTransform>().sizeDelta = new Vector2(buttonWidth, buttonWidth);
			bt.GetComponent<RectTransform>().anchorMin = new Vector2 (0f, 0.0f);
			bt.GetComponent<RectTransform>().anchorMax = new Vector2 (0f, 0.0f);
			bt.GetComponent<RectTransform>().anchoredPosition = 
				new Vector2 (
					((float)(2*i+5)*GetComponent<RectTransform>().sizeDelta.x/((float)(2*(numButtons+3)))), 
					(float)(MAX_NUMNOTES-(2*row+1))*(GetComponent<RectTransform>().offsetMax.y-GetComponent<RectTransform>().offsetMin.y)/((float)(2*numRows))
				);
			if (i%(4*Riff.MAX_SUBDIVS)%4 == 0) {
				bt.GetComponent<RectTransform>().localScale = new Vector3 (baseButtonScale, baseButtonScale, baseButtonScale);
			} else if (i%(4*Riff.MAX_SUBDIVS)%4-2 == 0) {
				bt.GetComponent<RectTransform>().localScale = new Vector3 (0.75f*baseButtonScale, 0.75f*baseButtonScale, 0.75f*baseButtonScale);
			} else if (i%(4*Riff.MAX_SUBDIVS)%4-1 == 0 || i%(4*Riff.MAX_SUBDIVS)%4-3 == 0) {
				bt.GetComponent<RectTransform>().localScale = new Vector3 (0.5f*baseButtonScale, 0.5f*baseButtonScale, 0.5f*baseButtonScale);
			}
			bt.GetComponent<Button>().onClick.AddListener(()=>{InstrumentSetup.currentRiff.Toggle(note, num);});
			bt.GetComponent<Button>().onClick.AddListener(()=>{Toggle(bt.GetComponent<Button>());});
			buttons.Add(bt);
		}
		buttons.Add(icon);
	}

	void MakeMelodicButtons (string title, int row, Note note) {
		int numButtons = (int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS+2);
		GameObject text = new GameObject();
		text.name = title+"Text";
		text.AddComponent<RectTransform>();
		text.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());
		float buttonWidth = GetComponent<RectTransform>().rect.height/(1.5f*numRows);
		text.GetComponent<RectTransform>().sizeDelta = new Vector2 (buttonWidth, buttonWidth);
		text.GetComponent<RectTransform>().localScale = new Vector3 (baseButtonScale, baseButtonScale, baseButtonScale);
		text.AddComponent<CanvasRenderer>();
		text.AddComponent<Text>();
		text.GetComponent<Text>().text = title;
		text.GetComponent<Text>().fontSize = 36;
		text.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		text.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
		text.GetComponent<RectTransform>().anchorMin = new Vector2 (0f, 0.0f);
		text.GetComponent<RectTransform>().anchorMax = new Vector2 (0f, 0.0f);
		text.GetComponent<RectTransform>().anchoredPosition = new Vector2 (
			GetComponent<RectTransform>().sizeDelta.x/((float)(2*(numButtons+1))),
			(float)(2*numRows-(2*row+1))*(GetComponent<RectTransform>().offsetMax.y-GetComponent<RectTransform>().offsetMin.y)/((float)(2*numRows)));
		
		for (int i=0; i<numButtons; i+=(int)Mathf.Pow(2f, (float)(Riff.MAX_SUBDIVS-subdivsShown))) {
			int num = i;
			GameObject bt = new GameObject();
			bt.name = title+"_"+i;
			bt.AddComponent<RectTransform>();
			bt.AddComponent<CanvasRenderer>();
			bt.AddComponent<Image>();
			if (currentRiff.Lookup(note, num)) {
				bt.GetComponent<Image>().sprite = melodicFilled;
				//Debug.Log("kobe");
			} else {
				bt.GetComponent<Image>().sprite = melodicEmpty;
			}
			bt.AddComponent<Button>();
			bt.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());
			bt.GetComponent<RectTransform>().sizeDelta = new Vector2(96f, 64f);
			bt.GetComponent<RectTransform>().anchorMin = new Vector2 (0f, 0.0f);
			bt.GetComponent<RectTransform>().anchorMax = new Vector2 (0f, 0.0f);
			bt.GetComponent<RectTransform>().anchoredPosition = 
				new Vector2 (
					((float)(2*i+3)*GetComponent<RectTransform>().sizeDelta.x/((float)(2*(numButtons+1)))), 
					(float)(2*numRows-(2*row+1))*(GetComponent<RectTransform>().offsetMax.y-GetComponent<RectTransform>().offsetMin.y)/((float)(2*numRows))
				);
			if (i%(4*Riff.MAX_SUBDIVS)%4 == 0) {
				bt.GetComponent<RectTransform>().localScale = new Vector3 (baseButtonScale, baseButtonScale, baseButtonScale);
			} else if (i%(4*Riff.MAX_SUBDIVS)%4-2 == 0) {
				bt.GetComponent<RectTransform>().localScale = new Vector3 (0.75f*baseButtonScale, 0.75f*baseButtonScale, 0.75f*baseButtonScale);
			} else if (i%(4*Riff.MAX_SUBDIVS)%4-1 == 0 || i%(4*Riff.MAX_SUBDIVS)%4-3 == 0) {
				bt.GetComponent<RectTransform>().localScale = new Vector3 (0.5f*baseButtonScale, 0.5f*baseButtonScale, 0.5f*baseButtonScale);
			}
			bt.GetComponent<Button>().onClick.AddListener(()=>{InstrumentSetup.currentRiff.Toggle(note, num);});
			bt.GetComponent<Button>().onClick.AddListener(()=>{Toggle(bt.GetComponent<Button>());});
			buttons.Add(bt);
		}
		buttons.Add(text);
	}

	// Flips button art
	void Toggle (Button button) {
		if (button.GetComponent<Image>().sprite == melodicEmpty) {
			button.GetComponent<Image>().sprite = melodicFilled;
		} else if (button.GetComponent<Image>().sprite == melodicFilled) {
			button.GetComponent<Image>().sprite = melodicEmpty;
		} else if (button.GetComponent<Image>().sprite == percussionEmpty) {
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

		
}
