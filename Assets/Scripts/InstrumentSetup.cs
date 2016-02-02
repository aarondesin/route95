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

	public delegate void RecordFunc (Note newNote, int pos);

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
			MakePercussionButtons (new GameObject[4], "Kick", 0, new Note (MusicManager.Sounds["Kick"]), kickIcon);
			MakePercussionButtons (new GameObject[4], "Snare", 1, new Note (MusicManager.Sounds["Snare"]), snareIcon);
			MakePercussionButtons (new GameObject[4], "Tom", 2, new Note (MusicManager.Sounds["Tom"]), tomIcon);
			MakePercussionButtons (new GameObject[4], "Hat", 3, new Note (MusicManager.Sounds["Hat"]), hatIcon);
			break;
		}
	}

	// Initializes a melodic setup menu
	void InitializeMelodicSetup (MelodicInstrument meloInst) {
		switch (meloInst) {
			case MelodicInstrument.ElectricGuitar:
			// Make rows of buttons for notes (in a grid)
			//MakeMelodicButtons (new GameObject[4], "E2", 0, new Note () { sound = Sounds.ElectricGuitarE2 });
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
		icon.GetComponent<RectTransform>().sizeDelta = new Vector2 (64f, 64f);
		icon.GetComponent<RectTransform>().anchorMin = new Vector2 (0f, 0.0f);
		icon.GetComponent<RectTransform>().anchorMax = new Vector2 (0f, 0.0f);
		icon.GetComponent<RectTransform>().anchoredPosition = new Vector2 (GetComponent<RectTransform>().sizeDelta.x/(2f*(float)(array.Length+1)),
			(float)(MAX_NUMNOTES-(2*row+1))*(GetComponent<RectTransform>().offsetMax.y-GetComponent<RectTransform>().offsetMin.y)/((float)MAX_NUMNOTES));
		for (int i=0; i<array.Length; i++) {
			GameObject bt = new GameObject();
			bt.name = title+i;
			bt.AddComponent<RectTransform>();
			bt.AddComponent<CanvasRenderer>();
			bt.AddComponent<Image>();
			bt.GetComponent<Image>().sprite = percussionEmpty;
			bt.AddComponent<Button>();
			bt.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());
			bt.GetComponent<RectTransform>().sizeDelta = new Vector2(64f, 64f);
			bt.GetComponent<RectTransform>().anchorMin = new Vector2 (0f, 0.0f);
			bt.GetComponent<RectTransform>().anchorMax = new Vector2 (0f, 0.0f);
			bt.GetComponent<RectTransform>().anchoredPosition = 
				new Vector2 (
					((float)(2*i+3)*GetComponent<RectTransform>().sizeDelta.x/(2f*(float)(array.Length+1))), 
					(float)(MAX_NUMNOTES-(2*row+1))*(GetComponent<RectTransform>().offsetMax.y-GetComponent<RectTransform>().offsetMin.y)/((float)MAX_NUMNOTES)
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
		for (int i=0; i<array.Length; i++) {
			GameObject bt = new GameObject();
			bt.name = title+i;
			bt.AddComponent<RectTransform>();
			bt.AddComponent<CanvasRenderer>();
			bt.AddComponent<Image>();
			bt.GetComponent<Image>().sprite = melodicEmpty;
			bt.AddComponent<Button>();
			//bt.GetComponent<RectTransform>().SetParent(par);
			//bt.GetComponent<RectTransform>().sizeDelta = new Vector2(par.sizeDelta.y/2f, par.sizeDelta.y/2f);
			bt.GetComponent<RectTransform>().anchorMin = new Vector2 (0f, 0.5f);
			bt.GetComponent<RectTransform>().anchorMax = new Vector2 (0f, 0.5f);
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
