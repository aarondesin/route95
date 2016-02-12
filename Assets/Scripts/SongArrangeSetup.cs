using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SongArrangeSetup : MonoBehaviour {

	public static SongArrangeSetup instance;

	public int selectedRiffIndex; // index of currently selected riff

	public Dropdown dropdown;

	void Start () {
		instance = this;
		dropdown.onValueChanged.AddListener (delegate { UpdateValue(); });
	}

	// Update the options in the dropdown to include all riffs
	public void Refresh () {
		dropdown.ClearOptions ();
		List<Dropdown.OptionData> options = new List<Dropdown.OptionData> ();
		foreach (Riff riff in MusicManager.instance.riffs) {
			Sprite sprite = InstrumentDisplay.instrumentIcons[riff.currentInstrument];
			Dropdown.OptionData option = new Dropdown.OptionData (riff.name, sprite);
			//Dropdown.OptionData option = new Dropdown.OptionData (MusicManager.instToString[riff.currentInstrument]);
			options.Add (option);
		}
		dropdown.AddOptions (options);
	}

	public void UpdateValue () {
		selectedRiffIndex = dropdown.value;
		InstrumentSetup.currentRiff = MusicManager.instance.riffs[selectedRiffIndex];
	}

	public void SetValue () {
		//Debug.Log(selectedRiffIndex);
		dropdown.value = selectedRiffIndex;
	}

	public void HideDropdown () {
		dropdown.Hide();
	}
				
}
