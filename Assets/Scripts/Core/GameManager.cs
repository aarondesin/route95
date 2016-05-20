﻿using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Instanced class to handle all application functions,
/// major state changes, and UI interactions.
/// </summary>
public class GameManager : MonoBehaviour {

	#region GameManager Enums

	/// <summary>
	/// Enum for various game states.
	/// </summary>
	public enum State {
		Loading,
		Setup,
		Live,
		Postplay
	};

	#endregion
	#region GameManager Vars

	public static GameManager instance;            // Quick reference to the Game Manager

	//-----------------------------------------------------------------------------------------------------------------
	[Header("Game Status")]

	[Tooltip("Is the game paused?")]
	public bool paused = false;

	[Tooltip("Current game state.")]
	public State currentState = State.Loading;

	[NonSerialized]
	public float targetDeltaTime;                  // 1 / Target FPS -- useful for coroutines

	//-----------------------------------------------------------------------------------------------------------------
	[Header("Load Status")]

	private int loadProgress = 0;                  // Number of load operations performed
	private int loadsToDo;                         // Number of load operations to perform

	[Tooltip("Is the game loaded?")]
	public bool loaded = false;

	bool loading = false;                          // Is the game currently loading?

	float startLoadTime;                           // Time at which loading started
	public GameObject loadingScreen;               // Parent loading screen object
	public GameObject loadingBar;                  // Loading bar
	public GameObject loadingMessage;              // Message above loading bar

	//-----------------------------------------------------------------------------------------------------------------
	[Header("Casette Settings")]

	public GameObject casette;                     // Casette GameObject
	bool casetteMoving = false;                    // Is the casette currently moving?
	bool willMoveCasette = false;                  // Will the casette move after camera lerp?

	[Tooltip("Speed at which the casette moves.")]
	[Range(0.5f,2f)]
	public float casetteMoveSpeed = 1f;

	[Tooltip("Position for casette to move in front of camera.")]
	public Transform casetteFront;  

	[Tooltip("Position for casette to move behind camera.")]
	public Transform casetteBack;

	Transform casetteTarget;                       // Current casette lerp target
	Transform casettePosition;                     // Current casette lerp start position

	float sTime;                                   // Start time of casette lerp

	//-----------------------------------------------------------------------------------------------------------------
	[Header("UI Settings")]

	[Tooltip("(Live Mode) How long to wait before fading the instrument icons.")]
	public float fadeWaitTime;

	[Tooltip("(Live Mode) How quickly to fade the instrument icons.")]
	public float fadeSpeed;

	float fadeTimer;                               // Current fade timer
	Vector3 prevMouse = Vector3.zero;              // Position of mouse during last frame

	//-----------------------------------------------------------------------------------------------------------------
	[Header("IO Settings")]
	public string projectSaveFolder = "Projects/"; // Name of the folder in which to save projects
	public string songSaveFolder = "Songs/";       // Name of the folder in which to save songs

	[NonSerialized]
	public string projectSavePath;                 // Full path to which to save projects

	[NonSerialized]
	public string songSavePath;                    // Full path to which to save songs

	//-----------------------------------------------------------------------------------------------------------------
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

	[Tooltip("Sound to use when clicking a menu.")]
	public AudioClip menuClick;
	public AudioClip menuClick2;
	public AudioClip effectsOn;
	public AudioClip effectsOff;
	public List<AudioClip> scribbles;

	//-----------------------------------------------------------------------------------------------------------------
	[Header("Menu Objects")]

	public GameObject mainMenu;
	public GameObject playlistMenu;
	public GameObject keySelectMenu;
	public GameObject songArrangeMenu;
	public GameObject riffEditMenu;
	public GameObject postPlayMenu;
	public GameObject pauseMenu;

	public GameObject addRiffPrompt;            // "Add Riff"
	public GameObject loadPrompt;               // "Load Project"
	public GameObject prompt;                   // Generic pop-up prompt
	public GameObject confirmExitPrompt;        // "Would you like to exit..."

	public GameObject keySelectConfirmButton;

	public GameObject systemButtons;            // "Settings" and "Exit"
	public Image livePlayQuitPrompt;            // "Exit"
	public GameObject liveIcons;                // Parent of instrument icons
	public GameObject songProgressBar;
	public GameObject loopIcon;

	//-----------------------------------------------------------------------------------------------------------------
	[Header("Tooltip Settings")]

	[Tooltip("GameObject to use for tooltip.")]
	public GameObject tooltip;  
	
	[Tooltip("Distance to show tooltip.")]                
	public float tooltipDistance;

	//-----------------------------------------------------------------------------------------------------------------
	Camera mainCamera;
	int cullingMaskBackup = 0;
	CameraClearFlags clearFlagsBackup = CameraClearFlags.Nothing;

	#endregion
	#region Unity Callbacks

	void Awake () {

		// Check if already initialized
		if (instance) Debug.LogError ("GameManager: multiple instances! There should only be one.", gameObject);
		else instance = this;

		// Remove profiler sample limit
		Profiler.maxNumberOfSamplesPerFrame = -1;

		// Set application target frame rate
		Application.targetFrameRate = 120;
		targetDeltaTime = 1f / (float)Application.targetFrameRate;

		// Init save paths
		//projectSavePath = Application.persistentDataPath + projectSaveFolder;
		projectSavePath = Application.dataPath + projectSaveFolder;
		//songSavePath = Application.persistentDataPath + songSaveFolder;
		songSavePath = Application.dataPath + songSaveFolder;

		// Create save folders
		//if (!Directory.Exists(GameManager.instance.projectSavePath)) 
		//	Directory.CreateDirectory (GameManager.instance.projectSavePath);

		if (!Directory.Exists(projectSavePath))
			Directory.CreateDirectory (projectSavePath);

		//if (!Directory.Exists(GameManager.instance.songSavePath)) 
		//	Directory.CreateDirectory (GameManager.instance.songSavePath);

		if (!Directory.Exists(projectSaveFolder))
			Directory.CreateDirectory (projectSaveFolder);
	}

	void Start () {

		// Init camera ref
		mainCamera = Camera.main;

		// Stop 3D rendering while loading
		StopRendering ();

		// Hide menus
		HideAll();
		Hide (prompt);
	}

	void Update () {

		// Don't update if not loaded
		if (loading) return;

		// Start loading if not laoded
		if (!loaded) Load();

		switch (currentState) {

		case State.Setup:
			
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

		case State.Live:
			if (!paused) {

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
		loaded = true;

		// Change state
		currentState = State.Setup;

		// Hide all menus
		loadingScreen.SetActive(false);
		HideAll ();

		// Show main menu
		Show (mainMenu);

		// Begin 3D rendering again
		StartRendering ();
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
		currentState = State.Setup;

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
		currentState = State.Postplay;

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
		currentState = State.Live;
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
			MusicManager.instance.StartPlaylist();
			MusicManager.instance.StartSong();
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
		currentState = State.Postplay;
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

	/// <summary>
	/// Stops 3D rendering on the main camera.
	/// </summary>
	public void StopRendering () {
		cullingMaskBackup = mainCamera.cullingMask;
		clearFlagsBackup = mainCamera.clearFlags;
		mainCamera.cullingMask = 0;
		mainCamera.clearFlags = CameraClearFlags.Nothing;
		mainCamera.GetComponent<SunShafts> ().enabled = false;
		mainCamera.GetComponent<CameraMotionBlur> ().enabled = false;
		mainCamera.GetComponent<BloomOptimized> ().enabled = false;
	}

	/// <summary>
	/// Starts 3D rendering on the main camera
	/// </summary>
	public void StartRendering() {
		mainCamera.cullingMask = cullingMaskBackup;
		mainCamera.clearFlags = clearFlagsBackup;
		mainCamera.GetComponent<SunShafts> ().enabled = true;
		mainCamera.GetComponent<CameraMotionBlur> ().enabled = true;
		mainCamera.GetComponent<BloomOptimized> ().enabled = true;
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
		switch (currentState) {
		case State.Setup: case State.Postplay:
			confirmExitPrompt.SetActive(true);
			break;
		case State.Live:
			Pause();
			break;
		}
	}

	/// <summary>
	/// Plays a click noise.
	/// </summary>
	public void MenuClick () {
		MusicManager.PlayMenuSound (menuClick);
	}

	public void MenuClick2 () {
		MusicManager.PlayMenuSound (menuClick2);
	}

	public void EffectsOn () {
		MusicManager.PlayMenuSound (effectsOn);
	}

	public void EffectsOff () {
		MusicManager.PlayMenuSound (effectsOff);
	}

	public void Scribble () {
		MusicManager.PlayMenuSound (scribbles[UnityEngine.Random.Range(0,3)]);
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
