using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour {

	public enum Menu {
		None,
		Main,
		KeySelect,
		SongArrange,
		RiffEdit,
		PostPlay
	};

	public enum Mode {
		Loading,
		Setup,
		Live,
		Postplay
	};

	public static GameManager instance;

	#region GameManager Vars

	[Header("Status")]
	public bool paused = false;
	public Menu currentMenu = Menu.None;
	public Mode currentMode = Mode.Loading;


	bool initialized = false;
	int loadPhase = -1;
	float startLoadTime;
	int loadProgress = 0;
	int loadValue = 0;

	public Dictionary<Menu, GameObject> menus; 

	[Header("UI Settings")]

	[Tooltip("(Live Mode) How long to wait before fading the instrument icons.")]
	public float fadeWaitTime;

	[Tooltip("(Live Mode) How quickly to fade the instrument icons.")]
	public float fadeSpeed;

	float fadeTimer;
	Vector3 prevMouse = new Vector3 (0f,0f,0f);

	[Header("IO Settings")]
	public string projectSaveFolder = "Projects/";
	public string songSaveFolder = "Songs/";

	[NonSerialized]
	public string projectSavePath;
	[NonSerialized]
	public string songSavePath;

	[Header("UI Resources")]

	[Tooltip("Font to use for UI.")]
	public Font font;

	public Sprite arrowIcon;
	public Sprite addIcon;
	public Sprite editIcon;
	public Sprite playIcon;
	public Sprite pauseIcon;
	public Sprite loadIcon;
	public Sprite removeIcon;
	public Sprite circleIcon;
	public Sprite volumeIcon;

	[Header("Menu Objects")]

	// Parent objects for all menu UI objects
	public GameObject mainMenu;
	public GameObject playlistMenu;
	public GameObject keySelectMenu;
	public GameObject songArrangeMenu;
	public GameObject riffEditMenu;
	public GameObject postPlayMenu;
	public GameObject pauseMenu;

	// Pop-up menu prompts
	public GameObject addRiffPrompt; // "Add Riff"
	public GameObject loadPrompt; // "Load Project"
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

	#endregion
	#region Unity Callbacks

	void Start () {
		if (instance) Debug.LogError ("GameManager: multiple instances! There should only be one.", gameObject);
		else instance = this;
			//Sounds.Load();

		// Initialize set of all menus
		menus = new Dictionary<Menu, GameObject>() {
			{ Menu.Main, mainMenu },
			{ Menu.KeySelect, keySelectMenu },
			{ Menu.SongArrange, songArrangeMenu },
			{ Menu.RiffEdit, riffEditMenu },
			{ Menu.PostPlay, postPlayMenu }
		};

		ShowAll ();
	}

	void Update () {
		if (!initialized) Initialize();

		// Fade exit icon
		if (currentMode == Mode.Live) {
			if (!paused) {
				Color temp = livePlayQuitPrompt.color;
				if (prevMouse != Input.mousePosition) {
					WakeLiveUI();
					prevMouse = Input.mousePosition;
				} else {
					if (fadeTimer <= 0f) temp.a -= fadeSpeed;
					else fadeTimer--;
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
		}

	}

	#endregion
	#region GameManager Methods

	void Initialize () {
		
		// Hide all menus and display default menu (main)
		HideAll ();
		Show (mainMenu);
		Load();
		initialized = true;
	}

	void Load () {
		startLoadTime = Time.realtimeSinceStartup;
		loadValue = Instrument.AllInstruments.Count+
			Sounds.soundsToLoad.Count + WorldManager.instance.decorationPaths.Count +
			WorldManager.instance.maxDecorations;


		loadingScreen.SetActive(true);
		LoadNext();

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
			CaseLibrary.initializecases ();
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
		currentMode = Mode.Setup;
	}

	public void ChangeLoadingMessage (string message) {
		loadingMessage.GetComponent<Text>().text = message;
	}

	public void GoToMainMenu () {
		HideAll ();
		Show (mainMenu);
	}

	public void GoToKeySelectMenu () {
		HideAll();
		Show (keySelectMenu);
		keySelectConfirmButton.GetComponent<Button>().interactable = 
			MusicManager.instance.currentSong.scale != -1 && MusicManager.instance.currentSong.key != Key.None;
		CameraControl.instance.LerpToPosition (CameraControl.instance.ViewDriving);
	}

	public void GoToSongArrangeMenu () {
		HideAll ();
		CameraControl.instance.LerpToPosition(CameraControl.instance.ViewRadio);
		Show (songArrangeMenu);
		SongArrangeSetup.instance.Refresh();
		SongTimeline.instance.RefreshTimeline();
	}

	public void GoToRiffEditor () {
		HideAll ();
		if (MusicManager.instance.currentSong.scale == -1) {
			GoToKeySelectMenu();
		} else {
			CameraControl.instance.LerpToPosition (CameraControl.instance.ViewDriving);
			Show (riffEditMenu);
			InstrumentSetup.instance.Initialize ();
		}
	}

	public void GoToPlaylistMenu () {
		HideAll ();
		Show (playlistMenu);
		PlaylistBrowser.instance.Refresh();
		PlaylistBrowser.instance.RefreshName();
	}

	public void GoToPostPlayMenu() {
		HideAll();
		Show (postPlayMenu);
	}

	public void Show (GameObject menu) {
		menu.SetActive(true);
	}

	public void ShowAll () {
		Show (playlistMenu);
		Show (songArrangeMenu);

		Show (addRiffPrompt);
		Show (loadPrompt);
		Show (prompt);
	}

	public void Hide (GameObject menu) {
		menu.SetActive(false);
	}

	public void HideAll () {
		Hide (mainMenu);
		Hide (playlistMenu);
		Hide (keySelectMenu);
		Hide (riffEditMenu);

		Hide (addRiffPrompt);
		Hide (loadPrompt);
		Hide (prompt);
		Hide (liveIcons);
	}
		
	public void Toggle (GameObject obj) {
		obj.SetActive (!obj.activeSelf);
	}

	// Swtich from setup to live mode
	public void SwitchToLive () {
		//Debug.Log (MusicManager.instance.currentSong.ToString ());
		MusicManager.instance.currentSong.CompileSong();
		if (MusicManager.instance.loopSong) Show(loopIcon);
		else Hide(loopIcon);

		HideAll ();
		Show (liveIcons);
		Show (songProgressBar);
		paused = false;

		InstrumentDisplay.instance.Refresh();
		MusicManager.instance.StartSong();
		CameraControl.instance.StartLiveMode();
		PlayerMovement.instance.StartMoving();

		currentMode = Mode.Live;
	}

	// Switch from live mode to postplay
	public void SwitchToPostplay () {
		MusicManager.instance.StopPlaying();
		PlayerMovement.instance.StopMoving();
		livePlayQuitPrompt.GetComponent<Image>().color = Color.white;
		paused = false;

		GoToPostPlayMenu();

		currentMode = Mode.Postplay;
	}

	// Returns to key selection
	public void NewSong () {
		MusicManager.instance.StopPlaying();
		paused = false;

		MusicManager.instance.currentSong = new Song();
		GoToKeySelectMenu();

		currentMode = Mode.Setup;
	}

	public void SaveCurrentProject () {
		SaveLoad.SaveCurrentProject();
	}

	public void ShowLoadPromptForProjects () {
		Show (loadPrompt);
		LoadPrompt.instance.SetLoadMode(LoadMode.Project);
		LoadPrompt.instance.Refresh();
	}

	public void ShowLoadPromptForSongs () {
		Show (loadPrompt);
		LoadPrompt.instance.SetLoadMode(LoadMode.Song);
		LoadPrompt.instance.Refresh();
	}
		
	// Toggle visibility of system buttons
	public void ToggleSystemButtons () {
		systemButtons.SetActive(!systemButtons.activeSelf);
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

	// Called when the user presses escape
	// If in setup, it will ask to confirm exit
	// If in live, it will pause the game
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

	// Enables visibility of the tooltip with the given message
	public void ShowTooltip (string message) {
		tooltip.SetActive(true);
		tooltip.GetComponent<Text>().text = message;
	}

	// Hides the tooltip
	public void HideTooltip () {
		tooltip.SetActive(false);
	}

	public void TogglePause () {
		if (paused) Unpause();
		else Pause();
	}

	public void Pause () {
		paused = true;
		pauseMenu.SetActive(true);
		PlayerMovement.instance.StopMoving();
	}

	public void Unpause () {
		paused = false;
		pauseMenu.SetActive(false);
		PlayerMovement.instance.StartMoving();
	}

	public void Exit () {
		Application.Quit();
	}

	#endregion
}
