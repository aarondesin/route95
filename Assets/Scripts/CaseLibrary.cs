using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CaseLibrary{

	public static List<Riff> cases = new List<Riff> ();

	////needs changing to strings for rhythm
	public static void initializecases(){
		cases.Add (new Riff () {
			name = "test",
			instrument = Instrument.ElectricGuitar,
			notes = new List<List<Note>> () {
				new List<Note> () {new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].root[0]) },
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () {new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].second[0]) },
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () {new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].seventh[0])},
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () {new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].fourth[0])},
				new List<Note> () ,
				new List<Note> (){new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].second[0])},
				new List<Note> ()
			}
		});
	}


}
