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

	public static Riff currentRiff = new Riff (); // current riff being edited

	public static float baseButtonScale = 1f; // base button sizes
	public static int MAX_NUMNOTES = 8; // maximum number of notes (rows)
	public static int numRows;

	// Calls appropriate Setup() function based on current instrument
	public void Initialize () {
		switch (MusicManager.currentInstrument) {
		case Instrument.RockDrums:
			InitializePercussionSetup (PercussionInstrument.RockDrums);
			break;
		case Instrument.ElectricGuitar:
			InitializeMelodicSetup (MelodicInstrument.ElectricGuitar);
			break;
		}

	}

	// Initializes a percussion setup menu
	void InitializePercussionSetup (PercussionInstrument percInst) {
		switch (percInst) {
		case PercussionInstrument.RockDrums:
			// Make rows of buttons for drums
			numRows = 4;
			MakePercussionButtons (new GameObject[4], "Kick", 0, new Note (MusicManager.Sounds["RockDrums_Kick"]), kickIcon);
			MakePercussionButtons (new GameObject[4], "Snare", 1, new Note (MusicManager.Sounds["RockDrums_Snare"]), snareIcon);
			MakePercussionButtons (new GameObject[4], "Tom", 2, new Note (MusicManager.Sounds["RockDrums_Tom"]), tomIcon);
			MakePercussionButtons (new GameObject[4], "Hat", 3, new Note (MusicManager.Sounds["RockDrums_Hat"]), hatIcon);
			break;
		}
	}

	// Initializes a melodic setup menu
	void InitializeMelodicSetup (MelodicInstrument meloInst) {
		switch (meloInst) {
			case MelodicInstrument.ElectricGuitar:
			// Make rows of buttons for notes (in a grid)
			numRows = 7;
			MakeMelodicButtons (new GameObject[4], "E2", 0, new Note (MusicManager.Sounds["ElectricGuitar_E2"]));
			MakeMelodicButtons (new GameObject[4], "F#2", 1, new Note (MusicManager.Sounds["ElectricGuitar_F#2"]));
			MakeMelodicButtons (new GameObject[4], "G#2", 2, new Note (MusicManager.Sounds["ElectricGuitar_G#2"]));
			MakeMelodicButtons (new GameObject[4], "A2", 3, new Note (MusicManager.Sounds["ElectricGuitar_A2"]));
			MakeMelodicButtons (new GameObject[4], "B2", 4, new Note (MusicManager.Sounds["ElectricGuitar_B2"]));
			MakeMelodicButtons (new GameObject[4], "C#3", 5, new Note (MusicManager.Sounds["ElectricGuitar_C#3"]));
			MakeMelodicButtons (new GameObject[4], "D#3", 6, new Note (MusicManager.Sounds["ElectricGuitar_D#3"]));
			break;
		}
	}

	// Creates all buttons for percussion setup
	void MakePercussionButtons (GameObject[] array, string title, int row, Note note, Sprite iconGraphic) {
		// Make icon for note
		GameObject icon = new GameObject();
		icon.name = title+"Icon";
		icon.AddComponent<RectTransform>();
		icon.AddComponent<CanvasRenderer>();
		icon.AddComponent<Image>();
		icon.GetComponent<Image>().sprite = iconGraphic;
		icon.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());
		float buttonWidth = GetComponent<RectTransform>().rect.height/(1.5f*numRows);
		icon.GetComponent<RectTransform>().sizeDelta = new Vector2 (buttonWidth, buttonWidth);
		icon.GetComponent<RectTransform>().localScale = new Vector3 (baseButtonScale, baseButtonScale, baseButtonScale);
		icon.GetComponent<RectTransform>().anchorMin = new Vector2 (0f, 0.0f);
		icon.GetComponent<RectTransform>().anchorMax = new Vector2 (0f, 0.0f);
		icon.GetComponent<RectTransform>().anchoredPosition = new Vector2 (GetComponent<RectTransform>().sizeDelta.x/(2f*(float)(array.Length+1)),
			(float)(MAX_NUMNOTES-(2*row+1))*(GetComponent<RectTransform>().offsetMax.y-GetComponent<RectTransform>().offsetMin.y)/((float)(2*numRows)));
		for (int i=0; i<array.Length; i++) {
			GameObject bt = new GameObject();
			bt.name = title+"_"+i;
			bt.AddComponent<RectTransform>();
			bt.AddComponent<CanvasRenderer>();
			bt.AddComponent<Image>();
			bt.GetComponent<Image>().sprite = percussionEmpty;
			bt.AddComponent<Button>();
			bt.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());
			bt.GetComponent<RectTransform>().sizeDelta = new Vector2(buttonWidth, buttonWidth);
			bt.GetComponent<RectTransform>().anchorMin = new Vector2 (0f, 0.0f);
			bt.GetComponent<RectTransform>().anchorMax = new Vector2 (0f, 0.0f);
			bt.GetComponent<RectTransform>().anchoredPosition = 
				new Vector2 (
					((float)(2*i+3)*GetComponent<RectTransform>().sizeDelta.x/(2f*(float)(array.Length+1))), 
					(float)(MAX_NUMNOTES-(2*row+1))*(GetComponent<RectTransform>().offsetMax.y-GetComponent<RectTransform>().offsetMin.y)/((float)(2*numRows))
				);
			//if (i%(4*MusicRiff.MAX_NUMSUBDIVS)%4 == 0) {
				bt.GetComponent<RectTransform>().localScale = new Vector3 (baseButtonScale, baseButtonScale, baseButtonScale);
			/*} else if (i%(4*MusicRiff.MAX_NUMSUBDIVS)%4-2 == 0) {
				bt.GetComponent<RectTransform>().localScale = new Vector3 (0.75f*baseButtonScale, 0.75f*baseButtonScale, 0.75f*baseButtonScale);
			} else if (i%(4*MusicRiff.MAX_NUMSUBDIVS)%4-1 == 0 || i%(4*MusicRiff.MAX_NUMSUBDIVS)%4-3 == 0) {
				bt.GetComponent<RectTransform>().localScale = new Vector3 (0.5f*baseButtonScale, 0.5f*baseButtonScale, 0.5f*baseButtonScale);
			}*/
			int num = i;
			bt.GetComponent<Button>().onClick.AddListener(()=>{InstrumentSetup.currentRiff.Toggle(note, num);});
			bt.GetComponent<Button>().onClick.AddListener(()=>{Toggle(bt.GetComponent<Button>());});
			array[i] = bt;
		}
	}

	void MakeMelodicButtons (GameObject[] array, string title, int row, Note note) {
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
		text.GetComponent<RectTransform>().anchoredPosition = new Vector2 (GetComponent<RectTransform>().sizeDelta.x/(2f*(float)(array.Length+1)),
			(float)(2*numRows-(2*row+1))*(GetComponent<RectTransform>().offsetMax.y-GetComponent<RectTransform>().offsetMin.y)/((float)(2*numRows)));
		for (int i=0; i<array.Length; i++) {
			GameObject bt = new GameObject();
			bt.name = title+"_"+i;
			bt.AddComponent<RectTransform>();
			bt.AddComponent<CanvasRenderer>();
			bt.AddComponent<Image>();
			bt.GetComponent<Image>().sprite = melodicEmpty;
			bt.AddComponent<Button>();
			bt.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());
			bt.GetComponent<RectTransform>().sizeDelta = new Vector2(164f, 63.5f);
			bt.GetComponent<RectTransform>().anchorMin = new Vector2 (0f, 0.0f);
			bt.GetComponent<RectTransform>().anchorMax = new Vector2 (0f, 0.0f);
			bt.GetComponent<RectTransform>().anchoredPosition = 
				new Vector2 (
					((float)(2*i+3)*GetComponent<RectTransform>().sizeDelta.x/(2f*(float)(array.Length+1))), 
					(float)(2*numRows-(2*row+1))*(GetComponent<RectTransform>().offsetMax.y-GetComponent<RectTransform>().offsetMin.y)/((float)(2*numRows))
				);
			//bt.GetComponent<RectTransform>().anchoredPosition = new Vector2 (((float)i*par.sizeDelta.x/((float)array.Length))+par.sizeDelta.y/2f, 0.0f);
			//if (i%(4*MusicRiff.MAX_NUMSUBDIVS)%4 == 0) {
			bt.GetComponent<RectTransform>().localScale = new Vector3 (baseButtonScale, baseButtonScale, baseButtonScale);
			/*} else if (i%(4*MusicRiff.MAX_NUMSUBDIVS)%4-2 == 0) {
				bt.GetComponent<RectTransform>().localScale = new Vector3 (0.75f*baseButtonScale, 0.75f*baseButtonScale, 0.75f*baseButtonScale);
			} else if (i%(4*MusicRiff.MAX_NUMSUBDIVS)%4-1 == 0 || i%(4*MusicRiff.MAX_NUMSUBDIVS)%4-3 == 0) {
				bt.GetComponent<RectTransform>().localScale = new Vector3 (0.5f*baseButtonScale, 0.5f*baseButtonScale, 0.5f*baseButtonScale);
			}*/
			int num = i;
			bt.GetComponent<Button>().onClick.AddListener(()=>{InstrumentSetup.currentRiff.Toggle(note, num);});
			bt.GetComponent<Button>().onClick.AddListener(()=>{Toggle(bt.GetComponent<Button>());});
			array[i] = bt;
		}
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
		
}
