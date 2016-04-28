using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SongArrangeSetup : MonoBehaviour {

	public static SongArrangeSetup instance;

	public int selectedRiffIndex; // index of currently selected riff

	public Dropdown dropdown;
	public InputField songNameInputField;
	public GameObject playRiffButton;
	public GameObject editRiffButton;
	public Sprite play;
	public Sprite pause;

	void Start () {
		instance = this;
		dropdown.onValueChanged.AddListener (delegate { UpdateValue(); });
		songNameInputField.onEndEdit.AddListener (delegate { MusicManager.instance.currentSong.name = songNameInputField.text;});
	}

	// Refresh all elements on the Song Arrangement UI
	public void Refresh () {

		// Update the options in the dropdown to include all riffs
		dropdown.ClearOptions ();
		List<Dropdown.OptionData> options = new List<Dropdown.OptionData> ();
		foreach (Riff riff in MusicManager.instance.currentProject.riffs) {
			Sprite sprite = riff.instrument.icon;
			Dropdown.OptionData option = new Dropdown.OptionData (riff.name + (riff.copy !=0 ? " ("+riff.copy.ToString()+")" : ""), sprite);
			//Dropdown.OptionData option = new Dropdown.OptionData (MusicManager.instToString[riff.currentInstrument]);
			options.Add (option);
		}
		dropdown.AddOptions (options);

		if (MusicManager.instance.currentProject.riffs.Count == 0) {
			InstrumentSetup.instance.currentRiff = null;
			dropdown.interactable = false;
			editRiffButton.GetComponent<Button>().interactable = false;
			playRiffButton.GetComponent<Button> ().interactable = false;
		} else {
			dropdown.interactable = true;
			if (InstrumentSetup.instance.currentRiff == null)
				InstrumentSetup.instance.currentRiff = MusicManager.instance.currentProject.riffs[0];
			editRiffButton.GetComponent<Button>().interactable = true;
			playRiffButton.GetComponent<Button> ().interactable = true;
		}

		// Refresh song name input field
		songNameInputField.text = MusicManager.instance.currentSong.name;

		playRiffButton.GetComponent<Image>().sprite = play;

		SetValue();
	}

	public void UpdateValue () {
		selectedRiffIndex = dropdown.value;
		InstrumentSetup.instance.currentRiff = MusicManager.instance.currentProject.riffs[selectedRiffIndex];
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

				
}
