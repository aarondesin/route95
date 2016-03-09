using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum Menu {
	None,
	Main,
	KeySelect,
	SongArrange,
	RiffEdit,
	PostPlay
};

public enum Mode {
	Setup,
	Live,
	Postplay
};

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	public Font font;

	// Parent objects for all menu UI objects
	public GameObject mainMenu;
	public GameObject keySelectMenu;
	public GameObject songArrangeMenu;
	public GameObject riffEditMenu;
	public GameObject postPlayMenu;
	public GameObject pauseMenu;

	// Pop-up menu prompts
	public GameObject addRiffPrompt; // "Add Riff"
	public GameObject loadProjectPrompt; // "Load Project"
	public GameObject prompt; // Generic pop-up prompt
	public GameObject confirmExitPrompt; // "Would you like to exit..."

	// Menu-specific buttons
	public GameObject keySelectConfirmButton;

	// Parent objects for universal system buttons
	public GameObject systemButtons; // "Settings" and "Exit"
	public Image livePlayQuitPrompt; // "Exit"
	public GameObject liveIcons;
	public GameObject songProgressBar;
	public GameObject loopIcon;

	public int loadingSpeed;
	public GameObject loadingScreen;
	public GameObject loadingBar;
	public GameObject loadingMessage;

	public GameObject tooltip;
	public float tooltipDistance;

	public Dictionary<Menu, GameObject> menus; 
		
	public Menu currentMenu = Menu.None;
	public Mode currentMode = Mode.Setup;

	// Global save path
	public string savePath; 

	// Icon fading vars
	public float fadeWaitTime;
	public float fadeSpeed;
	[SerializeField]
	float fadeTimer;
	Vector3 prevMouse = new Vector3 (0f,0f,0f);

	public bool paused = false;
	bool initialized = false;
	int loadPhase = -1;
	float startLoadTime;
	public int loadProgress = 0;
	int loadValue = 0;

	bool hasShownLiveHelp = false;

	void Start () {
		if (instance) Debug.LogError ("GameManager: multiple instances! There should only be one.", gameObject);
		else instance = this;
		savePath = Application.persistentDataPath;
		//Sounds.Load();

		// Initialize set of all menus
		menus = new Dictionary<Menu, GameObject>() {
			{ Menu.Main, mainMenu },
			{ Menu.KeySelect, keySelectMenu },
			{ Menu.SongArrange, songArrangeMenu },
			{ Menu.RiffEdit, riffEditMenu },
			{ Menu.PostPlay, postPlayMenu }
		};

		loadProjectPrompt.SetActive(true);
		loadingScreen.SetActive(true);
		prompt.SetActive(true);
	}

	void Initialize () {
		SongTimeline.instance.MakeColumns();
		SongTimeline.instance.scrollbar.GetComponent<Scrollbar>().value = 0f;
		
		// Hide all menus and display default menu (main)
		DisableMenu(menus[Menu.KeySelect]);
		DisableMenu(menus[Menu.SongArrange]);
		DisableMenu(menus[Menu.RiffEdit]);
		DisableMenu (menus [Menu.PostPlay]);
		DisableMenu(addRiffPrompt);
		DisableMenu(loadProjectPrompt);
		DisableMenu(prompt);
		DisableMenu(pauseMenu);
		DisableMenu(liveIcons);
		DisableMenu(tooltip);
		DisableMenu(songProgressBar);
		SwitchToMenu(Menu.Main);
		Load();
		//StartCoroutine("Load");
		initialized = true;
	}

	void Load () {
	//IEnumerator Load () {
		startLoadTime = Time.realtimeSinceStartup;
		loadValue = (int)Instrument.NUM_INSTRUMENTS+
			Sounds.soundsToLoad.Count + WorldManager.instance.decorationPaths.Count +
			WorldManager.instance.MAX_DECORATIONS;


		loadingScreen.SetActive(true);
		LoadNext();
		//startLoadTime = Time.realtimeSinceStartup;

		//loadingScreen.SetActive(false);

	}

	public void LoadNext() {
		loadPhase++;
		switch (loadPhase) {
		case 0:
			MusicManager.instance.Load();
			break;
		case 1:
			WorldManager.instance.Load();
			break;
		case 2:
			//Debug.Log("dick");
			FinishLoading();
			break;
		}
	}
		
	public void IncrementLoadProgress() {
		loadProgress++;
		loadingBar.GetComponent<Slider>().value = (float)loadProgress/(float)loadValue;
	}

	void FinishLoading() {
		Debug.Log("Completed initial load in "+(Time.realtimeSinceStartup-startLoadTime).ToString("0.0000")+" seconds.");
		loadingScreen.SetActive(false);
	}

	public void ChangeLoadingMessage (string message) {
		loadingMessage.GetComponent<Text>().text = message;
	}

	void Update () {
		if (!initialized) Initialize();

		// Fade exit icon
		if (currentMode == Mode.Live) {
			if (!paused) {
				Color temp = livePlayQuitPrompt.color;
				if (prevMouse != Input.mousePosition) {
					//temp.a = 1f;
					//fadeTimer = fadeWaitTime;
					WakeLiveUI();
					prevMouse = Input.mousePosition;
				} else {
						if (fadeTimer <= 0f) {
							temp.a -= fadeSpeed;
					} else {
							fadeTimer--;
					}
					livePlayQuitPrompt.color = temp;
					foreach (Image image in liveIcons.GetComponentsInChildren<Image>()) {
						image.color = temp;
						if(image.GetComponentInChildren<Text>())
							image.GetComponentInChildren<Text>().color = temp;
					}
				}
			}
		} else {
			if (tooltip.activeSelf) {
				RectTransform tr = tooltip.GetComponent<RectTransform>();
				Vector2 realPosition = new Vector2 (
					Input.mousePosition.x / Screen.width * ((RectTransform)tr.parent).rect.width, 
					Input.mousePosition.y / Screen.height * ((RectTransform)tr.parent).rect.height
				);
				tr.anchoredPosition3D = new Vector3 (
					(realPosition.x > 0.5f*Screen.width ?
						realPosition.x-tr.rect.width/2f-tooltipDistance : realPosition.x+tr.rect.width/2f+tooltipDistance),
					(realPosition.y > 0.5f*Screen.height ? 
						realPosition.y-tr.rect.height/2f-tooltipDistance : realPosition.x+tr.rect.height/2f+tooltipDistance),
					0f
				);
			}
			livePlayQuitPrompt.color = Color.white;
			//livePlayQuitPrompt.color = Color.white;
		}
	}
		
	// From a button, call GameManager.instance.SwitchToMenu (Menu.x)
	public void SwitchToMenu (Menu menu) {
		if (currentMenu != Menu.None) DisableMenu (menus[currentMenu]);
		EnableMenu (menus[menu]);
		currentMenu = menu;
	}

	// Button inspector-friendly verison
	public void SwitchToMenu (int menu) {
		if (currentMenu != Menu.None) DisableMenu (menus[currentMenu]);
		EnableMenu (menus[(Menu)menu]);
		currentMenu = (Menu)menu;
	}

	// Swtich from setup to live mode
	public void SwitchToLive () {
		paused = false;
		currentMode = Mode.Live;
		InputManager.instance.gameObject.SetActive(true);
		DisableMenu(menus[Menu.SongArrange]);
		InstrumentDisplay.instance.Refresh();
		MusicManager.instance.currentSong.CompileSong();
		Debug.Log (MusicManager.instance.currentSong.ToString ());
		EnableMenu(liveIcons);
		EnableMenu(songProgressBar);
		if (MusicManager.instance.loopSong) EnableMenu(loopIcon);
		else DisableMenu(loopIcon);

		if (!hasShownLiveHelp) {
			ShowLiveHelp();
			hasShownLiveHelp = true;
		}

		//sets player to moving
		TESTPlayerMovement.moving = true;
	}

	// Switch from live mode to postplay
	public void SwitchToPostplay () {
		paused = false;
		currentMode = Mode.Postplay;
		MusicManager.instance.StopPlaying();
		SwitchToMenu (Menu.PostPlay);
		//CameraControl.instance.MoveToPosition(CameraControl.instance.ViewRadio);
		//SwitchToMenu(Menu.SongArrange);
		TESTPlayerMovement.moving = false;
		livePlayQuitPrompt.GetComponent<Image>().color = Color.white;
	}

	// Returns to song arrangement
	public void SwitchToSetup () {
		paused = false;
		MusicManager.instance.StopPlaying();
		currentMode = Mode.Setup;
		SwitchToMenu (Menu.SongArrange);
		CameraControl.instance.MoveToPosition (CameraControl.instance.ViewRadio);
		livePlayQuitPrompt.GetComponent<Image>().color = Color.white;
		TESTPlayerMovement.moving = false;
		DisableMenu(loopIcon);
	}

	// Returns to key selection
	public void NewSong () {
		paused = false;
		MusicManager.instance.StopPlaying();
		currentMode = Mode.Setup;
		SwitchToMenu (Menu.KeySelect);
		CameraControl.instance.MoveToPosition (CameraControl.instance.ViewOutsideCar);
		livePlayQuitPrompt.GetComponent<Image>().color = Color.white;
		TESTPlayerMovement.moving = false;
		DisableMenu(loopIcon);
	}
		
	// Toggle visibility of system buttons
	public void ToggleSystemButtons () {
		systemButtons.SetActive(!systemButtons.activeSelf);
	}

	// Enable visibility of key selection confirm button (after user has clicked a key)
	public void EnableKeySelectConfirmButton () {
		keySelectConfirmButton.SetActive(true);
	}

	// Disable visibility of key selection confirm button (going to key selection menu)
	public void DisableKeySelectConfirmButton () {
		keySelectConfirmButton.SetActive(false);
	} 

	// Hides "Add Riff" prompt
	public void DisableAddRiffPrompt() {
		addRiffPrompt.SetActive(false);
	}

	public void WakeLiveUI () {
		fadeTimer = fadeWaitTime;
		Color color = Color.white;
		color.a = 1f;
		livePlayQuitPrompt.color = color;
		foreach (Image image in liveIcons.GetComponentsInChildren<Image>()) {
			image.color = color;
			if (image.GetComponentInChildren<Text>())
				image.GetComponentInChildren<Text>().color = color;
		}
	}

	// Shows a menu
	void EnableMenu (GameObject menuObject) {
		menuObject.SetActive(true);
	}

	// Hides a menu
	void DisableMenu (GameObject menuObject) {
		menuObject.SetActive(false);
	}

	public void AttemptExit () {
		switch (currentMode) {
		case Mode.Setup: case Mode.Postplay:
			confirmExitPrompt.SetActive(true);
			break;
		case Mode.Live:
			Pause();
			break;
		}
	}

	public void ShowTooltip (string message) {
		tooltip.SetActive(true);
		tooltip.GetComponent<Text>().text = message;
	}

	public void HideTooltip () {
		tooltip.SetActive(false);
	}

	public void EnableLoadProjectPrompt () {
		LoadProjectPrompt.instance.gameObject.SetActive(true);
		LoadProjectPrompt.instance.PopulateList();
	}

	public void DisableLoadProjectPrompt () {
		LoadProjectPrompt.instance.gameObject.SetActive(false);
	}

	public void TogglePause () {
		if (paused) Unpause();
		else Pause();
	}

	public void Pause () {
		paused = true;
		pauseMenu.SetActive(true);
	}

	public void Unpause () {
		paused = false;
		pauseMenu.SetActive(false);
	}

	public void Exit () {
		Application.Quit();
	}

	public void ShowLiveHelp() {
		Prompt.instance.PromptMessage("Live Mode", "Use number keys 1-3 to switch instruments, and letter keys Q to play with those instruments (more in the final version!)", "Okay");
		Pause();
	}
		
}
