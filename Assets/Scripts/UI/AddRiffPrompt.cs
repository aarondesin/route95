using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// Class for "Add Riff Prompt"
public class AddRiffPrompt : MonoBehaviour {
	public static AddRiffPrompt instance;

	#region Inspector Vars

	[Tooltip("Riff name input field")]
	public InputField inputField;

	[Tooltip("Instrument select dropdown")]
	public Dropdown dropdown;

	[Tooltip("Confirm button")]
	public Button confirmButton;

	#endregion
	#region Unity Callbacks

	void Start () {
		instance = this;

		inputField.onEndEdit.RemoveAllListeners ();
		inputField.onEndEdit.AddListener (delegate { 
			confirmButton.interactable = true;
		});
	}

	#endregion
	#region AddRiffPrompt Callbacks

	//
	// Resets all selections on the prompt
	//
	public void Refresh () {
		SetupDropdown();
		inputField.text = "";
		dropdown.value = 0;
		confirmButton.interactable = false;
	}

	//
	// Initializes the instrument selection dropdown
	//
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
		
	//
	// Creates a new riff with the filled in properties
	//
	public void AddRiff () {
		Riff temp = new Riff();
		temp.instrumentIndex = dropdown.value;
		temp.instrument = Instrument.AllInstruments[dropdown.value];
		temp.name = inputField.text;
		MusicManager.instance.currentProject.RegisterRiff (temp);

		GameManager.instance.Hide (gameObject);
		GameManager.instance.GoToRiffEditor();
	}

	#endregion
		
}
