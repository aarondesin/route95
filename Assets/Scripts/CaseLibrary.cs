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
				new List<Note> () { new Note ("Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A2") },
				new List<Note> (),
				new List<Note> (),
				new List<Note> () { new Note ("Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A2") },
				new List<Note> () { new Note ("Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_G2") },
				new List<Note> () { new Note ("Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A2") },
				new List<Note> () { new Note ("Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A2") },
				new List<Note> () { new Note ("Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A2") },
				new List<Note> () { new Note ("Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_F#2") },
				new List<Note> () { new Note ("Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A2") },
				new List<Note> () { new Note ("Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A2") },
				new List<Note> () { new Note ("Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A2") },
				new List<Note> () { new Note ("Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A2") },
				new List<Note> () { new Note ("Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A2") },
				new List<Note> () { new Note ("Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A2") },
				new List<Note> () { new Note ("Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A2") }
			}
		});
	}


}
