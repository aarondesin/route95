using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SongArrangeSetup : MonoBehaviour {

	#region SongArrangeSetup Vars

	public static SongArrangeSetup instance;

	public int selectedRiffIndex; // index of currently selected riff

	public Dropdown dropdown;
	public InputField songNameInputField;
	public GameObject playRiffButton;
	public GameObject editRiffButton;
	public GameObject addRiffReminder;
	public Sprite play;
	public Sprite pause;

	#endregion
	#region Unity Callbacks

	void Awake () {
		instance = this;
		dropdown.onValueChanged.AddListener (delegate { UpdateValue(); });
		songNameInputField.onEndEdit.AddListener (delegate { MusicManager.instance.currentSong.name = songNameInputField.text;});
	}

	#endregion
	#region SongArrangeSetup Methods

	// Refresh all elements on the Song Arrangement UI
	public void Refresh () {

		// Update the options in the dropdown to include all riffs
		dropdown.ClearOptions ();
		List<Dropdown.OptionData> options = new List<Dropdown.OptionData> ();
		foreach (Riff riff in MusicManager.instance.currentSong.riffs) {
			Sprite sprite = riff.instrument.icon;
			Dropdown.OptionData option = new Dropdown.OptionData (riff.name, sprite);
			//Dropdown.OptionData option = new Dropdown.OptionData (MusicManager.instToString[riff.currentInstrument]);
			options.Add (option);
		}
		dropdown.AddOptions (options);

		if (MusicManager.instance.currentSong.riffs.Count == 0) {
			//InstrumentSetup.instance.currentRiff = null;
			dropdown.interactable = false;
			editRiffButton.GetComponent<Button>().interactable = false;
			playRiffButton.GetComponent<Button> ().interactable = false;
		} else {
			dropdown.interactable = true;
			editRiffButton.GetComponent<Button>().interactable = true;
			playRiffButton.GetComponent<Button> ().interactable = true;
			if (InstrumentSetup.currentRiff == null)
				InstrumentSetup.currentRiff = MusicManager.instance.currentSong.riffs [0];
		}

		// Refresh song name input field
		songNameInputField.text = MusicManager.instance.currentSong.name;

		playRiffButton.GetComponent<Image>().sprite = play;

		SetValue();

		bool hasRiffs = MusicManager.instance.currentSong.riffs.Count != 0;
		SongTimeline.instance.SetInteractable (hasRiffs);
		addRiffReminder.SetActive(!hasRiffs);

	}

	public void UpdateValue () {
		selectedRiffIndex = dropdown.value;
		InstrumentSetup.currentRiff = MusicManager.instance.currentSong.riffs[selectedRiffIndex];
	}

	public void SetValue () {
		//Debug.Log(selectedRiffIndex);
		dropdown.value = selectedRiffIndex;
	}

	public void HideDropdown () {
		dropdown.Hide();
	}

	public void TogglePlayRiffButton () {
		if (playRiffButton.GetComponent<Image>().sprite == play) playRiffButton.GetComponent<Image>().sprite = pause;
		else playRiffButton.GetComponent<Image>().sprite = play;
	}

	#endregion
}
