using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

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

	// Parent objects for all menu UI objects
	public GameObject mainMenu;
	public GameObject keySelectMenu;
	public GameObject songArrangeMenu;
	public GameObject riffEditMenu;
	public GameObject postPlayMenu;
	public GameObject addRiffPrompt;
	public GameObject loadProjectPrompt;
	public GameObject prompt;

	// Parent objects for universal system buttons
	public GameObject systemButtons;

	// Key selection menu confirm button
	public GameObject keySelectConfirmButton;

	public Dictionary<Menu, GameObject> menus; 
		
	public Menu currentMenu = Menu.None;
	public Mode currentMode = Mode.Setup;

	public string savePath; 

	bool initialized = false;

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


	}

	void Initialize () {
		// Hide all menus and display default menu (main)
		DisableMenu(menus[Menu.KeySelect]);
		DisableMenu(menus[Menu.SongArrange]);
		DisableMenu(menus[Menu.RiffEdit]);
		DisableMenu (menus [Menu.PostPlay]);
		DisableMenu(addRiffPrompt);
		DisableMenu(loadProjectPrompt);
		DisableMenu(prompt);
		SwitchToMenu(Menu.Main);
		initialized = true;
	}

	void Update () {
		if (!initialized) Initialize();
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
		currentMode = Mode.Live;
		InputManager.instance.gameObject.SetActive(true);
		DisableMenu(menus[Menu.SongArrange]);
		InstrumentDisplay.instance.Refresh();
		MusicManager.instance.currentSong.CompileSong();
		Debug.Log (MusicManager.instance.currentSong.ToString ());

		//sets player to moving
		TESTPlayerMovement.moving = true;
	}

	// Switch from live mode to postplay
	public void SwitchToPostplay () {
		currentMode = Mode.Postplay;
		MusicManager.instance.StopPlaying();
		SwitchToMenu (Menu.PostPlay);
		//CameraControl.instance.MoveToPosition(CameraControl.instance.ViewRadio);
		//SwitchToMenu(Menu.SongArrange);
		TESTPlayerMovement.moving = false;
	}

	public void SwitchToSetup () {
		currentMode = Mode.Setup;
		SwitchToMenu (Menu.SongArrange);
		CameraControl.instance.MoveToPosition (CameraControl.instance.ViewRadio);
	}

	public void NewSong () {
		currentMode = Mode.Setup;
		SwitchToMenu (Menu.KeySelect);
		CameraControl.instance.MoveToPosition (CameraControl.instance.ViewOutsideCar);
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

	public void DisableAddRiffPrompt() {
		addRiffPrompt.SetActive(false);
	}

	void EnableMenu (GameObject menuObject) {
		menuObject.SetActive(true);
	}

	void DisableMenu (GameObject menuObject) {
		menuObject.SetActive(false);
	}
		
}
