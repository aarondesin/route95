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

	public enum LoadPhase {
		Classes,
		Sounds,
		Instruments,
		Scales,
		Chunks,
		Decorations
	};

	public static GameManager instance;

	#region GameManager Vars

	[Header("Status")]

	public int targetFrameRate = 120;

	private int loadSpeed = 1;
	private int loadProgress = 0;
	private int loadsToDo;
	bool loading = false;

	public LoadPhase loadPhase;
	private Dictionary <LoadPhase, bool> loadPhases;

	bool classesLoaded = false;
	bool soundsLoaded = false;
	bool scaleInfoLoaded = false;
	bool instrumentsLoaded = false;
	bool scalesLoaded = false;


	public bool paused = false;
	public Menu currentMenu = Menu.None;
	public Mode currentMode = Mode.Loading;


	bool initialized = false;
	float startLoadTime;
	int loadValue = 0;
	bool casetteMoving = false;
	public Transform casetteFront;
	public Transform casetteBack;
	Transform casetteTarget;
	Transform casettePosition;
	public float casetteMoveSpeed = 1f;
	float sTime;

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
	public Sprite fillSprite;

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

	public GameObject casette;

	#endregion
	#region Unity Callbacks

	void Start () {
		if (instance) Debug.LogError ("GameManager: multiple instances! There should only be one.", gameObject);
		else instance = this;

		// Initialize set of all menus
		menus = new Dictionary<Menu, GameObject>() {
			{ Menu.Main, mainMenu },
			{ Menu.KeySelect, keySelectMenu },
			{ Menu.SongArrange, songArrangeMenu },
			{ Menu.RiffEdit, riffEditMenu },
			{ Menu.PostPlay, postPlayMenu }
		};

		projectSavePath = Application.persistentDataPath + projectSaveFolder;
		songSavePath = Application.persistentDataPath + songSaveFolder;

		ShowAll ();
		//MoveCasetteBack();

		loadPhases = new Dictionary <LoadPhase, bool> () {
			{LoadPhase.Classes, false},
			{LoadPhase.Sounds, false},
		};

		loadingScreen.SetActive(true);

	}

	void Update () {
		if (!initialized) {
			if (!loading) {
				/*loadPhases [LoadPhase.Classes] = 
					GameManager.instance != null &&
					InputManager.instance != null &&
					KeyManager.instance != null &&
					MusicManager.instance != null &&
					AddRiffPrompt.instance != null &&
					InstrumentSetup.instance != null &&
					LoadPrompt.instance != null &&
					PlaylistBrowser.instance != null &&
					Prompt.instance != null &&
					RadialKeyMenu.instance != null &&
					SongArrangeSetup.instance != null &&
					SongTimeline.instance != null &&
					WorldManager.instance != null &&
					PlayerMovement.instance != null;*/

				

				//if (loadPhases [LoadPhase.Classes])  Load();
				Load();
			} else {
				return;
			}
		}

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
			if (casetteMoving) {
				float progress = (Time.time - sTime) * casetteMoveSpeed;
				float dist = progress / Vector3.Distance (casetteTarget.position, casettePosition.position);
				casette.transform.position = Vector3.Lerp (casettePosition.position, casetteTarget.position, dist);
				casette.transform.rotation = Quaternion.Lerp (casettePosition.rotation, casetteTarget.rotation, dist);
				if (dist >= 1f) {
					casetteMoving = false;
					casette.transform.position = casetteTarget.position;
					casette.transform.rotation = casetteTarget.rotation;
				}
			}
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

	void Load () {
		Debug.Log("GameManager.Load()");
		loadsToDo = MusicManager.instance.loadsToDo + WorldManager.instance.loadsToDo;
		loading = true;

		MusicManager.instance.Load();

	}

	public void FinishLoading () {
		// Hide all menus and display default menu (main)
		loadingScreen.SetActive(false);
		currentMode = Mode.Setup;
		HideAll ();
		Show (mainMenu);
		loading = false;
		initialized = true;
		Debug.Log("Completed initial load in "+(Time.realtimeSinceStartup-startLoadTime).ToString("0.0000")+" seconds.");
	}

	public void ReportLoaded (int numLoaded) {
		loadProgress += numLoaded;
		loadingBar.GetComponent<Slider>().value = (float)loadProgress/(float)loadsToDo;
	}

	public void ChangeLoadingMessage (string message) {
		loadingMessage.GetComponent<Text>().text = message;
	}

	public void GoToMainMenu () {
		HideAll ();
		Show (mainMenu);
		MoveCasetteBack();
		CameraControl.instance.LerpToPosition (CameraControl.instance.ViewOutsideCar);
	}

	public void GoToKeySelectMenu () {
		MoveCasetteBack();
		HideAll();
		Show (keySelectMenu);
		if (RadialKeyMenu.instance == null) {
			keySelectMenu.GetComponentInChildren<RadialKeyMenu>().Refresh();
		} else {
			RadialKeyMenu.instance.Refresh();
		}
		keySelectConfirmButton.GetComponent<Button>().interactable = 
			MusicManager.instance.currentSong.scale != -1 && MusicManager.instance.currentSong.key != Key.None;
		CameraControl.instance.LerpToPosition (CameraControl.instance.ViewDriving);
	}

	public void GoToSongArrangeMenu () {
		MoveCasetteBack();
		HideAll ();
		CameraControl.instance.LerpToPosition(CameraControl.instance.ViewRadio);
		Show (songArrangeMenu);
		SongArrangeSetup.instance.Refresh();
		SongTimeline.instance.RefreshTimeline();
	}

	public void GoToRiffEditor () {
		MoveCasetteBack();
		HideAll ();
		if (MusicManager.instance.currentSong.scale == -1) {
			GoToKeySelectMenu();
		} else {
			CameraControl.instance.LerpToPosition (CameraControl.instance.ViewDriving);
			Show (riffEditMenu);
			if (InstrumentSetup.instance == null) {
				riffEditMenu.GetComponentInChildren<InstrumentSetup>().Initialize();
			} else {
				InstrumentSetup.instance.Initialize ();
			}
		}
	}

	public void GoToPlaylistMenu () {
		currentMode = Mode.Setup;
		casette.SetActive(true);
		HideAll ();
		Show (playlistMenu);
		PlaylistBrowser.instance.Refresh();
		PlaylistBrowser.instance.RefreshName();
		MoveCasetteFront ();
		CameraControl.instance.LerpToPosition (CameraControl.instance.ViewOutsideCar);
	}

	public void GoToPostPlayMenu() {
		MoveCasetteBack();
		HideAll();
		Show (postPlayMenu);
		currentMode = Mode.Postplay;
	}

	public void Show (GameObject menu) {
		menu.SetActive(true);
		if (menu.GetComponent<Fadeable>() != null)
			menu.GetComponent<Fadeable>().UnFade();
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
		//if (mainMenu.activeSelf) mainMenu.GetComponent<Fadeable>().Fade();
		//if (playlistMenu.activeSelf) playlistMenu.GetComponent<Fadeable>().Fade();
		//if (keySelectMenu.activeSelf) keySelectMenu.GetComponent<Fadeable>().Fade();
		//if (riffEditMenu.activeSelf) riffEditMenu.GetComponent<Fadeable>().Fade();
		Hide (mainMenu);
		Hide (playlistMenu);
		Hide (keySelectMenu);
		Hide (riffEditMenu);
		Hide (postPlayMenu);

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
		casette.SetActive(false);
		//casetteMoving = false;
		//Debug.Log (MusicManager.instance.currentSong.ToString ());
		MusicManager.instance.currentSong.CompileSong();
		if (MusicManager.instance.loopSong) Show(loopIcon);
		else Hide(loopIcon);

		MoveCasetteBack();
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
		casette.SetActive(false);
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
		LoadPrompt.instance.SetLoadMode(LoadPrompt.Mode.Project);
		LoadPrompt.instance.Refresh();
	}

	public void ShowLoadPromptForSongs () {
		Show (loadPrompt);
		LoadPrompt.instance.SetLoadMode(LoadPrompt.Mode.Song);
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

	public void MoveCasetteFront () {
		casetteMoving = true;
		casettePosition = casette.transform;
		casetteTarget = casetteFront;
		sTime = Time.time;

	}

	public void MoveCasetteBack () {
		casetteMoving = true;
		casettePosition = casette.transform;
		casetteTarget = casetteBack;
		sTime = Time.time;

	}

	#endregion
}
