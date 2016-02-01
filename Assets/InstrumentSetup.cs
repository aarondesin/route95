using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InstrumentSetup : MonoBehaviour {

	public Sprite percussionEmpty;
	public Sprite percussionFilled;

	public Sprite melodicEmpty;
	public Sprite melodicFilled;

	public static Riff currentRiff = new Riff (); // current riff being edited

	/*public GameObject[] kickButtons;
	public GameObject[] snareButtons;
	public GameObject[] tomButtons;
	public GameObject[] hatButtons;*/

	public static float baseButtonScale = 2f;

	public delegate void RecordFunc (Note newNote, int pos);

	List<GameObject[]> buttonLists = new List<GameObject[]>();

	public void Initialize () {
		switch (MusicManager.currentInstrument) {
		case Instrument.Drums:
			InitializePercussionSetup (PercussionInstrument.Drums);
			break;
		case Instrument.ElectricGuitar:
			InitializeMelodicSetup (MelodicInstrument.ElectricGuitar);
			break;
		}

	}

	/*void Initialize (PercussionInstrument percInst) {
		InitializePercussionSetup (percInst);
	}

	void Initialize (MelodicInstrument meloInst) {
		InitializeMelodicSetup (meloInst);
	}*/

	void InitializePercussionSetup (PercussionInstrument percInst) {
		switch (percInst) {
		case PercussionInstrument.Drums:
			// Make rows of buttons for drums
			MakePercussionButtons (new GameObject[4], "Kick", 0, new Note (Sounds.Kick ));
			MakePercussionButtons (new GameObject[4], "Snare", 1, new Note (Sounds.Snare ));
			MakePercussionButtons (new GameObject[4], "Tom", 2, new Note (Sounds.Tom ));
			MakePercussionButtons (new GameObject[4], "Hat", 3, new Note (Sounds.Hat ));
			break;
		}
	}

	void InitializeMelodicSetup (MelodicInstrument meloInst) {
		switch (meloInst) {
			case MelodicInstrument.ElectricGuitar:
			// Make rows of buttons for notes (in a grid)
			//MakeMelodicButtons (new GameObject[4], "E2", 0, new Note () { sound = Sounds.ElectricGuitarE2 });
			break;
		}
		
	}

	void MakePercussionButtons (GameObject[] array, string title, int row, Note note) {
		
		for (int i=0; i<array.Length; i++) {
			GameObject bt = new GameObject();
			bt.name = title+i;
			bt.AddComponent<RectTransform>();
			bt.AddComponent<CanvasRenderer>();
			bt.AddComponent<Image>();
			bt.GetComponent<Image>().sprite = percussionEmpty;
			bt.AddComponent<Button>();
			bt.GetComponent<RectTransform>().SetParent(GameObject.Find("Setup").GetComponent<Transform>());
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
			array[i] = bt;
		}
	}

	/*void Setup () {

		//int numButtons = (int)Mathf.Pow(2f,(MusicRiff.MAX_NUMSUBDIVS+1))*MusicRiff.MAX_NUMBARS;
		int numButtons = (int)Mathf.Pow(2f,(MusicManager.drumRiffs[MusicManager.drumRiffIndex].subdivs+1))*MusicManager.drumRiffs[MusicManager.drumRiffIndex].bars;
		int subs = MusicManager.drumRiffs[MusicManager.drumRiffIndex].subdivs;
		subdivText.text =subs.ToString("#");

		if (kickButtons != null) for (int i=0; i<kickButtons.Length; i++) if (kickButtons[i]) Destroy(kickButtons[i]);
		if (snareButtons != null) for (int i=0; i<snareButtons.Length; i++) if (snareButtons[i]) Destroy(snareButtons[i]);
		if (tomButtons != null) for (int i=0; i<tomButtons.Length; i++) if (tomButtons[i]) Destroy(tomButtons[i]);
		if (hatButtons != null) for (int i=0; i<hatButtons.Length; i++) if (hatButtons[i]) Destroy(hatButtons[i]);

		// make kick buttons
		kickButtons = new GameObject[numButtons];
		var k_tr = kickSetup.GetComponent<RectTransform>();
		MakeButtons (kickButtons, "Kick", k_tr, MusicManager.RecordKick);

		// make snare buttons
		snareButtons = new GameObject[numButtons];
		var s_tr = snareSetup.GetComponent<RectTransform>();
		MakeButtons (snareButtons, "Snare", s_tr, MusicManager.RecordSnare);

		// make tom buttons
		tomButtons = new GameObject[numButtons];
		var t_tr = tomSetup.GetComponent<RectTransform>();
		MakeButtons (tomButtons, "Tom", t_tr, MusicManager.RecordTom);

		// make hat buttons
		hatButtons = new GameObject[numButtons];
		var h_tr = hatSetup.GetComponent<RectTransform>();
		MakeButtons (hatButtons, "Hat", h_tr, MusicManager.RecordHat);

		Renew();

	}*/
}
