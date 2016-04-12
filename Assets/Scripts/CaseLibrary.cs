using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CaseLibrary{

	public static List<Riff> cases = new List<Riff> ();

	////needs changing to strings for rhythm
	/// make this load after we know what the key is and then give initiaiize case a parameter that takes in a key
	/// and replace that with what we have therefore we can use this regardless of key. can also do that with instruments.
	public static void initializecases(){
		/*cases.Add (new Riff () {
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
				new List<Note> () {new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].third[0]) },
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () {new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].fifth[0])},
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () {new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].seventh[0])},
				new List<Note> () ,
				new List<Note> (){new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].third[0])},
				new List<Note> ()
			}
		});*/
		cases.Add( new Riff () {
			name = "Guitarcase1",
			instrument = Instrument.ElectricGuitar,
			notes = new List<List<Note>>() {
				new List<Note> () {new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].root[0]) },
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () {new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].fifth[0]) },
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () {new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].fourth[0])},
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () {new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].seventh[0])},
				new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].second[0])},
				new List<Note> ()
			}
		});
		cases.Add( new Riff () {
			name = "Guitarcase2",
			instrument = Instrument.ElectricGuitar,
			notes = new List<List<Note>>() {
				new List<Note> () {new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].root[0]) },
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () {new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].fifth[0]) },
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () {new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].fourth[0])},
				new List<Note> (),
				new List<Note> () {new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].fifth[0])},
				new List<Note> (),
				new List<Note> () {new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].sixth[0])},
				new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].seventh[0])},
				new List<Note> () {new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].root[1])}
			}
		});
		cases.Add( new Riff () {
			name = "Guitarcase3",
			instrument = Instrument.ElectricGuitar,
			notes = new List<List<Note>>() {
				new List<Note> () {new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].third[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].third[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].third[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].third[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].fifth[0]) },
				new List<Note> (),
				new List<Note> (){new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].sixth[0])},
				new List<Note> (),
				new List<Note> () ,
				new List<Note> (),
				new List<Note> (){new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].fifth[0]) },
				new List<Note> (),
				new List<Note> () {new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].fourth[0])},
				new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricGuitar].sixth[0])},
				new List<Note> ()
			}
		});
		cases.Add( new Riff () {
			name = "basecase1",
			instrument = Instrument.ElectricBass,
			notes = new List<List<Note>>() {
				new List<Note> () { new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricBass].root[0]) },
				new List<Note> () ,
				new List<Note> () { new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricBass].root[0]) },
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> (){ new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricBass].root[0]) },
				new List<Note> (),
				new List<Note> (){ new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricBass].root[0]) },
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> (){ new Note(KeyManager.instance.scales[Key.Eminor][Instrument.ElectricBass].third[0]) },
				new List<Note> (),
				new List<Note> (),
				new List<Note> ()
			}
		});
	}


}
