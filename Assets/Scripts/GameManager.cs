﻿using UnityEngine;
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

	public Font font;

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

	public Sprite arrowIcon;
	public Sprite addIcon;
	public Sprite editIcon;
	public Sprite playIcon;
	public Sprite pauseIcon;
	public Sprite loadIcon;
	public Sprite removeIcon;
	public Sprite circleIcon;
	public Sprite volumeIcon;

	public int loadingSpeed;
	public GameObject loadingScreen;
	public GameObject loadingBar;
	public GameObject loadingMessage;

	public GameObject tooltip;
	public float tooltipDistance;

	public Dictionary<Menu, GameObject> menus; 
		
	public Menu currentMenu = Menu.None;
	public Mode currentMode = Mode.Loading;

	// Global save path
	public string projectSavePath;
	public string songSavePath;

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
	bool hasShownSongArrangerHelp = false;
	//bool hasShownRiffEditorHelp = false;

	public GameObject shortSongWarningPrompt;
	int shortSongWarningThreshold = 6;

	void Start () {
		if (instance) Debug.LogError ("GameManager: multiple instances! There should only be one.", gameObject);
		else instance = this;
		projectSavePath = Application.persistentDataPath + "/Projects/";
		songSavePath = Application.persistentDataPath + "/Songs/";
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

	void Initialize () {
		SongTimeline.instance.MakeColumns();
		SongTimeline.instance.scrollbar.GetComponent<Scrollbar>().value = 0f;
		
		// Hide all menus and display default menu (main)
		HideAll ();
		Show (mainMenu);
		Load();
		//StartCoroutine("Load");
		initialized = true;
	}

	void Load () {
	//IEnumerator Load () {
		startLoadTime = Time.realtimeSinceStartup;
		loadValue = Instrument.AllInstruments.Count+
			Sounds.soundsToLoad.Count + WorldManager.instance.decorationPaths.Count +
			WorldManager.instance.maxDecorations;


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
		//currentTime = 0;
		currentMode = Mode.Setup;
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

	public void GoToMainMenu () {
		HideAll ();
		Show (mainMenu);
	}

	public void GoToKeySelectMenu () {
		HideAll();
		Show (keySelectMenu);
		DisableKeySelectConfirmButton();
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

	public void Show (GameObject menu) {
		menu.SetActive(true);
	}

	public void ShowAll () {
		Show (playlistMenu);
		Show (songArrangeMenu);

		Show (addRiffPrompt);
		Show (loadPrompt);
		Show (prompt);
		Show (shortSongWarningPrompt);
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
		Hide (shortSongWarningPrompt);
		Hide (liveIcons);
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

	public void AttemptSwitchToLive () {
		if (MusicManager.instance.currentSong.songPieces.Count <= shortSongWarningThreshold && !MusicManager.instance.loopSong) {
			EnableMenu(shortSongWarningPrompt);
		} else {
			SwitchToLive();
		}
	}

	public void Toggle (GameObject obj) {
		obj.SetActive (!obj.activeSelf);
	}

	// Swtich from setup to live mode
	public void SwitchToLive () {
		HideAll ();
		Hide (songArrangeMenu);
		Show (liveIcons);
		paused = false;
		currentMode = Mode.Live;
		InputManager.instance.gameObject.SetActive(true);
		InstrumentDisplay.instance.Refresh();
		MusicManager.instance.currentSong.CompileSong();
		Debug.Log (MusicManager.instance.currentSong.ToString ());
		EnableMenu(liveIcons);
		EnableMenu(songProgressBar);
		if (MusicManager.instance.loopSong) EnableMenu(loopIcon);
		else DisableMenu(loopIcon);
		//CameraControl.instance.MoveToPosition(CameraControl.instance.ViewChase);
		CameraControl.instance.StartLiveMode();
		MusicManager.instance.StartSong();

		if (!hasShownLiveHelp) {
			//ShowLiveHelp();
			hasShownLiveHelp = true;
		}

		//sets player to moving
		PlayerMovement.instance.StartMoving();
	}

	// Switch from live mode to postplay
	public void SwitchToPostplay () {
		paused = false;
		currentMode = Mode.Postplay;
		MusicManager.instance.StopPlaying();
		SwitchToMenu (Menu.PostPlay);
		//CameraControl.instance.MoveToPosition(CameraControl.instance.ViewRadio);
		//SwitchToMenu(Menu.SongArrange);
		PlayerMovement.instance.StopMoving();
		livePlayQuitPrompt.GetComponent<Image>().color = Color.white;
		DisableMenu(liveIcons);
	}

	// Returns to song arrangement
	public void SwitchToSetup () {
		paused = false;
		MusicManager.instance.StopPlaying();
		currentMode = Mode.Setup;
		SwitchToMenu (Menu.SongArrange);
		CameraControl.instance.LerpToPosition (CameraControl.instance.ViewRadio);
		livePlayQuitPrompt.GetComponent<Image>().color = Color.white;
		PlayerMovement.instance.StopMoving();
		DisableMenu(loopIcon);
		DisableMenu(liveIcons);
	}

	// Returns to key selection
	public void NewSong () {
		paused = false;
		MusicManager.instance.StopPlaying();
		currentMode = Mode.Setup;
		SwitchToMenu (Menu.KeySelect);
		CameraControl.instance.LerpToPosition (CameraControl.instance.ViewOutsideCar);
		livePlayQuitPrompt.GetComponent<Image>().color = Color.white;
		PlayerMovement.instance.StopMoving();
		DisableMenu(loopIcon);
		MusicManager.instance.currentSong = new Song();
		SongTimeline.instance.RefreshTimeline();
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

	public void ShowLiveHelp() {
		Prompt.instance.PromptMessage("Live Mode", "Use number keys 1-3 to switch instruments, and letter keys QWERT to play with those instruments (more in the final version!). You can increase your song's tempo by pressing the up or down arrow keys.", "Okay");
		Pause();
	}

	public void ShowSongArrangerHelp() {
		if (!hasShownSongArrangerHelp) {
			Prompt.instance.PromptMessage("Song Arranger", "Here is where you arrange your song. The timeline in the middle of the screen is for placing your riffs at certain positions in the song.", "Got it");
			hasShownSongArrangerHelp = true;
		}
	}
		
	public void ShowRiffEditorHelp() {
		/*if (!hasShownRiffEditorHelp) {
			Prompt.instance.PromptMessage("Riff Editor", "Here, you can edit your riff. Click a button to add note at that position in time.", "Alrighty");
			hasShownRiffEditorHelp = true;
		}*/
	}
}
