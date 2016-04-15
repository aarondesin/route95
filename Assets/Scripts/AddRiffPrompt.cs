using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AddRiffPrompt : MonoBehaviour {
	public static AddRiffPrompt instance;

	public InputField inputField;
	public Dropdown dropdown;
	public Button confirmButton;

	void Start () {
		instance = this;
	}

	// Resets all selections on the prompt
	public void Refresh () {
		SetupDropdown();
		inputField.text = "";
		dropdown.value = 0;
		confirmButton.interactable = false;
		inputField.onEndEdit.RemoveAllListeners ();
		inputField.onEndEdit.AddListener (delegate { confirmButton.interactable = true; });
	}

	// Initializes the instrument selection dropdown
	void SetupDropdown () {
		dropdown.ClearOptions ();
		List<Dropdown.OptionData> options = new List<Dropdown.OptionData> ();
		for (int i=0; i<Instrument.AllInstruments.Count; i++) {
			Sprite sprite = Instrument.AllInstruments[i].icon;
			string instName = Instrument.AllInstruments[i].name;
			Dropdown.OptionData option = new Dropdown.OptionData (instName, sprite);
			options.Add(option);
		}
		dropdown.AddOptions(options);
	}
		
	// Creates a new riff with the filled in properties
	public void AddRiff () {
		Riff temp = MusicManager.instance.AddRiff ();
		temp.instrumentIndex = dropdown.value;
		temp.name = inputField.text;
		GameManager.instance.DisableAddRiffPrompt();
		GameManager.instance.SwitchToMenu ((int)Menu.RiffEdit);
		CameraControl.instance.MoveToPosition(GameObject.Find("CamView_Driving").GetComponent<Transform>());
	}
		
}
