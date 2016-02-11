using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public enum Menu {
	None,
	Main,
	KeySelect,
	SongArrange,
	RiffEdit
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

	// Parent objects for universal system buttons
	public GameObject systemButtons;

	// Key selection menu confirm button
	public GameObject keySelectConfirmButton;

	public Dictionary<Menu, GameObject> menus; 
		
	public Menu currentMenu = Menu.None;
	public Mode currentMode = Mode.Setup;

	void Start () {
		if (instance) Debug.LogError ("GameManager: multiple instances! There should only be one.", gameObject);
		else instance = this;
		//Sounds.Load();

		// Initialize set of all menus
		menus = new Dictionary<Menu, GameObject>() {
			{ Menu.Main, mainMenu },
			{ Menu.KeySelect, keySelectMenu },
			{ Menu.SongArrange, songArrangeMenu },
			{ Menu.RiffEdit, riffEditMenu }
		};

		// Hide all menus and display default menu (main)
		DisableMenu(menus[Menu.KeySelect]);
		DisableMenu(menus[Menu.SongArrange]);
		DisableMenu(menus[Menu.RiffEdit]);
		SwitchToMenu(Menu.Main);
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

		//sets player to moving
		TESTPlayerMovement.moving = true;
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

	void EnableMenu (GameObject menuObject) {
		menuObject.SetActive(true);
	}

	void DisableMenu (GameObject menuObject) {
		menuObject.SetActive(false);
	}
		
}
