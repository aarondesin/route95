using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SongArrangeSetup : MonoBehaviour {

	public Dropdown dropdown;

	// Update the options in the dropdown to include all riffs
	public void Refresh () {
		dropdown.ClearOptions ();
		List<Dropdown.OptionData> options = new List<Dropdown.OptionData> ();
		foreach (Riff riff in MusicManager.instance.riffs) {
			//Dropdown.OptionData option = new Dropdown.OptionData (riff.name);
			Dropdown.OptionData option = new Dropdown.OptionData (MusicManager.instToString[riff.currentInstrument]);
			options.Add (option);
		}
		dropdown.AddOptions (options);
	}
}
