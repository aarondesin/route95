/*using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DrumRiffSetup : MonoBehaviour {
	
	bool initialized;
	
	public Sprite empty;
	public Sprite filled;
	
	public GameObject kickSetup;
	public GameObject snareSetup;
	public GameObject tomSetup;
	public GameObject hatSetup;
	
	public GameObject[] kickButtons;
	public GameObject[] snareButtons;
	public GameObject[] tomButtons;
	public GameObject[] hatButtons;
	
	public Text subdivText;
	
	public static float baseButtonScale = 1.5f;
	
	public delegate void RecordFunc (int pos);
	
	void Start () {
		initialized = false;
	}
	
	// fills an array with title# buttons that call rec onClick();
	void MakeButtons (GameObject[] array, string title, RectTransform par, RecordFunc rec) {
		for (int i=0; i<array.Length; i++) {
			GameObject bt = new GameObject();
			bt.name = title+i;
			bt.AddComponent<RectTransform>();
			bt.AddComponent<CanvasRenderer>();
			bt.AddComponent<Image>();
			bt.GetComponent<Image>().sprite = empty;
			bt.AddComponent<Button>();
			bt.GetComponent<RectTransform>().SetParent(par);
			bt.GetComponent<RectTransform>().sizeDelta = new Vector2(par.sizeDelta.y/2f, par.sizeDelta.y/2f);
			bt.GetComponent<RectTransform>().anchorMin = new Vector2 (0f, 0.5f);
			bt.GetComponent<RectTransform>().anchorMax = new Vector2 (0f, 0.5f);
			bt.GetComponent<RectTransform>().anchoredPosition = new Vector2 (((float)i*par.sizeDelta.x/((float)array.Length))+par.sizeDelta.y/2f, 0.0f);
			if (i%(4*MusicManager.MusicRiff.MAX_NUMSUBDIVS)%4 == 0) {
				bt.GetComponent<RectTransform>().localScale = new Vector3 (baseButtonScale, baseButtonScale, baseButtonScale);
			} else if (i%(4*MusicManager.MusicRiff.MAX_NUMSUBDIVS)%4-2 == 0) {
				bt.GetComponent<RectTransform>().localScale = new Vector3 (0.75f*baseButtonScale, 0.75f*baseButtonScale, 0.75f*baseButtonScale);
			} else if (i%(4*MusicManager.MusicRiff.MAX_NUMSUBDIVS)%4-1 == 0 || i%(4*MusicManager.MusicRiff.MAX_NUMSUBDIVS)%4-3 == 0) {
				bt.GetComponent<RectTransform>().localScale = new Vector3 (0.5f*baseButtonScale, 0.5f*baseButtonScale, 0.5f*baseButtonScale);
			}
			int num = i;
			bt.GetComponent<Button>().onClick.AddListener(()=>{rec(num);});
			array[i] = bt;
		}
	}
	
	void Update () {
		
		// initialize menus after all Start() calls
		if (!initialized) {
			Initialize();
			Setup();
		}
	}
	
	void Setup () {
		
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
		
	}
	
	void Initialize () {
		initialized = true;
		MusicManager.OnChangeDrumRiff += Renew;
		MusicManager.OnChangeDrumRiffProperties += Setup;
		
	}
	
	void Renew () {
		for (int i=0; i<kickButtons.Length; i++) {
			if (MusicManager.drumRiffs[MusicManager.drumRiffIndex].LookUp(i, MusicManager.Percussion.Kick)) kickButtons[i].GetComponent<Button>().image.sprite = filled;
			else kickButtons[i].GetComponent<Button>().image.sprite = empty;
		}
		for (int i=0; i<snareButtons.Length; i++) {
			if (MusicManager.drumRiffs[MusicManager.drumRiffIndex].LookUp(i, MusicManager.Percussion.Snare)) snareButtons[i].GetComponent<Button>().image.sprite = filled;
			else snareButtons[i].GetComponent<Button>().image.sprite = empty;
		}
		for (int i=0; i<tomButtons.Length; i++) {
			if (MusicManager.drumRiffs[MusicManager.drumRiffIndex].LookUp(i, MusicManager.Percussion.Tom)) tomButtons[i].GetComponent<Button>().image.sprite = filled;
			else tomButtons[i].GetComponent<Button>().image.sprite = empty;
		}
		for (int i=0; i<hatButtons.Length; i++) {
			if (MusicManager.drumRiffs[MusicManager.drumRiffIndex].LookUp(i, MusicManager.Percussion.Hat)) hatButtons[i].GetComponent<Button>().image.sprite = filled;
			else hatButtons[i].GetComponent<Button>().image.sprite = empty;
		}
	}
	
}*/
