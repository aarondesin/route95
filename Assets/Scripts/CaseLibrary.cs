using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CaseLibrary{
	
	public static List<Riff> rhythmcases = new List<Riff> ();
	public static List<Riff> melodycases = new List<Riff> ();

	public void initializecases(){
		rhythmcases.Add (new Riff () {
			name = "test",
			instrument = Instrument.ElectricGuitar,
			notes = new List<List<Note>> () {
				new List<Note> () { new Note ("ElectricGuitar_E2") },
				new List<Note> () { new Note ("ElectricGuitar_A2") },
				new List<Note> () { new Note ("ElectricGuitar_A2") },
				new List<Note> () { new Note ("ElectricGuitar_A2") },
				new List<Note> () { new Note ("ElectricGuitar_G2") },
				new List<Note> () { new Note ("ElectricGuitar_A2") },
				new List<Note> () { new Note ("ElectricGuitar_A2") },
				new List<Note> () { new Note ("ElectricGuitar_A2") },
				new List<Note> () { new Note ("ElectricGuitar_F#2") },
				new List<Note> () { new Note ("ElectricGuitar_A2") },
				new List<Note> () { new Note ("ElectricGuitar_A2") },
				new List<Note> () { new Note ("ElectricGuitar_A2") },
				new List<Note> () { new Note ("ElectricGuitar_A2") },
				new List<Note> () { new Note ("ElectricGuitar_A2") },
				new List<Note> () { new Note ("ElectricGuitar_A2") },
				new List<Note> () { new Note ("ElectricGuitar_A2") }
			}
		});
	}
	

}
