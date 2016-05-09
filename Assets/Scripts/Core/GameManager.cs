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

	#region GameManager Enums

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

	#endregion
	#region GameManager Vars

	public static GameManager instance;

	[Header("Status")]

	public bool paused = false;
	public Menu currentMenu = Menu.None;
	public Mode currentMode = Mode.Loading;
	[NonSerialized]
	public Dictionary<Menu, GameObject> menus; 

	// Loading vars
	private int loadProgress = 0;
	private int loadsToDo;
	bool loading = false;
	bool initialized = false;
	public bool loaded = false;

	// Casette Vars
	bool casetteMoving = false;
	bool willMoveCasette = false;
	public Transform casetteFront;
	public Transform casetteBack;
	Transform casetteTarget;
	Transform casettePosition;
	public float casetteMoveSpeed = 1f;
	float sTime;


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
	public Sprite melodicVolumeIcon;
	public Sprite fillSprite;
	public Sprite scribbleCircle;

	public AudioClip menuClick;
	//public AudioClip

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

	float startLoadTime;
	public int loadingSpeed;
	public GameObject loadingScreen;
	public GameObject loadingBar;
	public GameObject loadingMessage;

	public GameObject tooltip;
	public float tooltipDistance;

	public GameObject casette;

	#endregion
	#region Unity Callbacks

	void Awake () {
		if (instance) Debug.LogError ("GameManager: multiple instances! There should only be one.", gameObject);
		else instance = this;

		Profiler.maxNumberOfSamplesPerFrame = -1;
		Application.targetFrameRate = 60;

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
	}

	void Start () {
		ShowAll();
	}

	void Update () {
		if (!initialized) {
			if (!loading) Load();
			else return;
		}

		switch (currentMode) {

		case Mode.Setup:
			
			// Check for tooltip
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
			break;

		case Mode.Live:
			if (paused) {

				// Wake/fade UI icons
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
			} else livePlayQuitPrompt.color = Color.white;
			break;
		}
			
		// Move casette
		if (casetteMoving) {
			float progress = (Time.time - sTime) * casetteMoveSpeed;
			float dist = Vector3.Distance (casetteTarget.position, casettePosition.position);
			if (dist == 0f) return;

			float journey = progress / dist;
			Vector3 pos = Vector3.Lerp (casettePosition.position, casetteTarget.position, journey);
			Quaternion rot = Quaternion.Lerp (casettePosition.rotation, casetteTarget.rotation, journey);
			casette.transform.position = pos;
			casette.transform.rotation = rot;

			if (journey >= 1f) {
				casetteMoving = false;
				casette.transform.position = casetteTarget.position;
				casette.transform.rotation = casetteTarget.rotation;
			}
		}

	}

	#endregion
	#region GameManager Loading Methods

	/// <summary>
	/// Load this instance.
	/// </summary>
	void Load () {

		// Hide all menus
		HideAll ();

		// Show loading screen
		loadingScreen.SetActive(true);

		// Calculate operations to do
		loadsToDo = MusicManager.instance.loadsToDo + WorldManager.instance.loadsToDo;

		// Init vars
		startLoadTime = Time.realtimeSinceStartup;
		loading = true;

		// Start by loading MusicManager
		MusicManager.instance.Load();
	}

	/// <summary>
	/// Used to tell GameManager how many items were just loaded.
	/// </summary>
	/// <param name="numLoaded">Number loaded.</param>
	public void ReportLoaded (int numLoaded) {
		loadProgress += numLoaded;
		loadingBar.GetComponent<Slider>().value = (float)loadProgress/(float)loadsToDo;
	}

	/// <summary>
	/// Changes the message on the loading screen.
	/// </summary>
	/// <param name="message">Message.</param>
	public void ChangeLoadingMessage (string message) {
		loadingMessage.GetComponent<Text>().text = message;
	}

	/// <summary>
	/// Performs all necessary actions after loading.
	/// </summary>
	public void FinishLoading () {

		// Report to console
		Debug.Log("Fully loaded in "+(Time.realtimeSinceStartup-startLoadTime).ToString("0.0000")+" seconds.");

		// Update vars
		loading = false;
		initialized = true;
		loaded = true;

		// Change mode
		currentMode = Mode.Setup;

		// Hide all menus
		loadingScreen.SetActive(false);
		HideAll ();

		// Show main menu
		Show (mainMenu);
	}

	#endregion
	#region GameManager Menu Methods

	/// <summary>
	/// Show the specified menu, fading if possible.
	/// </summary>
	/// <param name="menu">Menu to show.</param>
	public void Show (GameObject menu) {
		menu.SetActive(true);
		if (menu.GetComponent<Fadeable>() != null)
			menu.GetComponent<Fadeable>().UnFade();
	}

	/// <summary>
	/// Shows all menus, fading if possible.
	/// </summary>
	public void ShowAll () {
		Show (mainMenu);
		Show (playlistMenu);
		Show (keySelectMenu);
		Show (songArrangeMenu);
		Show (riffEditMenu);
		Show (postPlayMenu);

		Show (addRiffPrompt);
		Show (loadPrompt);
		Show (prompt);
		Show (liveIcons);
	}

	/// <summary>
	/// Hide the specified menu, fading if possible.
	/// </summary>
	/// <param name="menu">Menu to hide.</param>
	public void Hide (GameObject menu) {
		Fadeable fade = menu.GetComponent<Fadeable>();
		if (fade != null) {
			if (fade.disableAfterFading) {
				fade.Fade();
				return;
			}
		}
		menu.SetActive(false);
	}

	/// <summary>
	/// Hides all menus, fading if possible.
	/// </summary>
	public void HideAll () {
		Hide (mainMenu);
		Hide (playlistMenu);
		Hide (keySelectMenu);
		Hide (songArrangeMenu);
		Hide (riffEditMenu);
		Hide (postPlayMenu);

		Hide (addRiffPrompt);
		Hide (loadPrompt);
		Hide (prompt);
		Hide (liveIcons);
	}

	#endregion
	#region Menu Transition Methods
		
	/// <summary>
	/// Goes to main menu.
	/// </summary>
	public void GoToMainMenu () {
		
		// Hide other menus
		HideAll ();
		MoveCasetteBack();

		// Show main menu
		Show (mainMenu);

		// Move camera to outside view
		CameraControl.instance.LerpToPosition (CameraControl.instance.ViewOutsideCar);
	}

	/// <summary>
	/// Goes to key select menu.
	/// </summary>
	public void GoToKeySelectMenu () {

		// Hide other menus
		MoveCasetteBack();
		HideAll();

		// Show key select menu
		Show (keySelectMenu);

		// Move camera to driving view
		CameraControl.instance.LerpToPosition (CameraControl.instance.ViewDriving);

		// Refresh radial menu
		RadialKeyMenu.instance.Refresh();

		// Enable/disable confirmation button
		keySelectConfirmButton.GetComponent<Button>().interactable = 
			MusicManager.instance.currentSong.scale != -1 && MusicManager.instance.currentSong.key != Key.None;
		
	}

	/// <summary>
	/// Goes to song arrange menu.
	/// </summary>
	public void GoToSongArrangeMenu () {

		// Hide other menus
		MoveCasetteBack();
		HideAll ();

		// Show and refresh song arranger menu
		Show (songArrangeMenu);
		SongArrangeSetup.instance.Refresh();
		SongTimeline.instance.RefreshTimeline();

		// Move camera to radio view
		CameraControl.instance.LerpToPosition(CameraControl.instance.ViewRadio);
	}

	/// <summary>
	/// Goes to riff editor.
	/// </summary>
	public void GoToRiffEditor () {

		// Hide other menus
		MoveCasetteBack();
		HideAll ();

		// If no scale selected, go to key select first
		if (MusicManager.instance.currentSong.scale == -1) GoToKeySelectMenu();


		else {

			// Otherwise show riff editor
			Show (riffEditMenu);
			InstrumentSetup.instance.Initialize ();

			// Move camera to driving view
			CameraControl.instance.LerpToPosition (CameraControl.instance.ViewDriving);
		}
	}

	/// <summary>
	/// Goes to playlist menu.
	/// </summary>
	public void GoToPlaylistMenu () {

		// Switch modes
		currentMode = Mode.Setup;

		// Stop music/live mode operations
		MusicManager.instance.StopPlaying();
		PlayerMovement.instance.StopMoving();
		CameraControl.instance.StopLiveMode();

		// Hide other menus
		HideAll ();

		// Show playlist menu
		Show (playlistMenu);
		PlaylistBrowser.instance.Refresh();
		PlaylistBrowser.instance.RefreshName();

		// Move camera to outside view
		CameraControl.instance.LerpToPosition (CameraControl.instance.ViewOutsideCar);

		// Queue casette to move when done moving camera
		willMoveCasette = true;
	}

	/// <summary>
	/// Goes to post play menu.
	/// </summary>
	public void GoToPostPlayMenu() {

		// Switch mode
		currentMode = Mode.Postplay;

		// Hide other menus
		MoveCasetteBack();
		HideAll();

		// Show postplay menu
		Show (postPlayMenu);
	}
		
	#endregion
	#region Mode Switching Methods

	/// <summary>
	/// Switches to live mode.
	/// </summary>
	public void SwitchToLive () {

		// Switch mode
		currentMode = Mode.Live;
		paused = false;

		// Hide menus
		MoveCasetteBack();
		HideAll ();

		// Show live menus
		Show (liveIcons);
		Show (songProgressBar);
		if (MusicManager.instance.loopPlaylist) Show(loopIcon);
		else Hide(loopIcon);

		// Init music
		MusicManager.instance.currentPlayingSong = 0;
		if (MusicManager.instance.currentSong != null) {
			MusicManager.instance.StartSong();
			MusicManager.instance.currentSong.CompileSong();
		}

		// Start live operations
		InstrumentDisplay.instance.Refresh();
		CameraControl.instance.StartLiveMode();
		PlayerMovement.instance.StartMoving();
	}

	/// <summary>
	/// Switches to postplay mode.
	/// </summary>
	public void SwitchToPostplay () {

		// Switch mode
		currentMode = Mode.Postplay;
		paused = false;

		// Stop music/live operations
		MusicManager.instance.StopPlaying();
		PlayerMovement.instance.StopMoving();
		CameraControl.instance.StopLiveMode();

		// Show prompt
		livePlayQuitPrompt.GetComponent<Image>().color = Color.white;

		// Go to postplay menu
		GoToPostPlayMenu();
	}

	#endregion
	#region Save/Load Methods

	/// <summary>
	/// Saves the current project.
	/// </summary>
	public void SaveCurrentProject () {
		SaveLoad.SaveCurrentProject();
	}

	/// <summary>
	/// Shows the load prompt for projects.
	/// </summary>
	public void ShowLoadPromptForProjects () {
		Show (loadPrompt);
		LoadPrompt.instance.Refresh(LoadPrompt.Mode.Project);
	}

	/// <summary>
	/// Shows the load prompt for songs.
	/// </summary>
	public void ShowLoadPromptForSongs () {
		Show (loadPrompt);
		LoadPrompt.instance.Refresh(LoadPrompt.Mode.Song);
	}

	#endregion
	#region Utility Methods

	/// <summary>
	/// Moves the casette front.
	/// </summary>
	public void MoveCasetteFront () {
		casetteMoving = true;
		casettePosition = casette.transform;
		casetteTarget = casetteFront;
		sTime = Time.time;
		willMoveCasette = false;
	}

	/// <summary>
	/// Moves the casette back.
	/// </summary>
	public void MoveCasetteBack () {
		casetteMoving = true;
		casettePosition = casette.transform;
		casetteTarget = casetteBack;
		sTime = Time.time;
		willMoveCasette = false;
	}

	/// <summary>
	/// If set to move casette, will do so.
	/// </summary>
	public void AttemptMoveCasette () {
		if (willMoveCasette) MoveCasetteFront();
	}

	/// <summary>
	/// Toggles the visibility of an object.
	/// </summary>
	/// <param name="obj">Object.</param>
	public void Toggle (GameObject obj) {
		obj.SetActive (!obj.activeSelf);
	}
		
	/// <summary>
	/// Toggles the system buttons.
	/// </summary>
	public void ToggleSystemButtons () {
		systemButtons.SetActive(!systemButtons.activeSelf);
	}

	/// <summary>
	/// Wakes the live UI.
	/// </summary>
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

	/// <summary>
	/// Attempts to exit the game.
	/// </summary>
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

	/// <summary>
	/// Plays a click noise.
	/// </summary>
	public void MenuClick () {
		MusicManager.instance.GetComponent<AudioSource>().PlayOneShot(menuClick, 1f);
	}

	/// <summary>
	/// Shows the tooltip.
	/// </summary>
	/// <param name="message">Message.</param>
	public void ShowTooltip (string message) {
		tooltip.SetActive(true);
		tooltip.Text().text = message;
	}

	/// <summary>
	/// Hides the tooltip.
	/// </summary>
	public void HideTooltip () {
		tooltip.SetActive(false);
	}

	/// <summary>
	/// Toggles paused status.
	/// </summary>
	public void TogglePause () {
		if (paused) Unpause();
		else Pause();
	}

	/// <summary>
	/// Pause this instance.
	/// </summary>
	public void Pause () {
		paused = true;
		pauseMenu.SetActive(true);
		PlayerMovement.instance.StopMoving();
		CameraControl.instance.Pause();
	}

	/// <summary>
	/// Unpause this instance.
	/// </summary>
	public void Unpause () {
		paused = false;
		pauseMenu.SetActive(false);
		PlayerMovement.instance.StartMoving();
		CameraControl.instance.Unpause();
	}

	/// <summary>
	/// Exit this instance.
	/// </summary>
	public void Exit () {
		Application.Quit();
	}

	#endregion
}
