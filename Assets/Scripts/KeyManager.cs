using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KeyManager{

	public Key selectedkey = MusicManager.instance.currentKey;
	public class Scale{
		public Note root1;
		public Note second1;
		public Note third1;
		public Note fourth1;
		public Note fifth1;
		public Note sixth1;
		public Note seventh1;

		public Note root2;
		public Note second2;
		public Note third2;
		public Note fourth2;
		public Note fifth2;
		public Note sixth2;
		public Note seventh2;

		public Note root3;
		public Note second3;
		public Note third3;
		public Note fourth3;
		public Note fifth3;
		public Note sixth3;
		public Note seventh3;


		public Note root4;
		public Note second4;
		public Note third4;
		public Note fourth4;
		public Note fifth4;
		public Note sixth4;
		public Note seventh4;

	}

	public static Scale ElectricGuitarEminor = new Scale () {
		
		root1 = new Note ("ElectricGuitar_E2"),
		second1 = new Note ("ElectricGuitar_F#2"),
		third1 = new Note("ElectricGuitar_G2"),
		fourth1 = new Note ("ElectricGuitar_A2"),
		fifth1 = new Note ("ElectricGuitar_B2"),
		sixth1 = new Note("ElectricGuitar_C3"),
		seventh1 = new Note ("ElectricGuitar_D3"),

		root2 = new Note ("ElectricGuitar_E3"),
		second2 = new Note ("ElectricGuitar_F#3"),
		third2 = new Note("ElectricGuitar_G3"),
		fourth2 = new Note ("ElectricGuitar_A3"),
		fifth2 = new Note ("ElectricGuitar_B3"),
		sixth2 = new Note("ElectricGuitar_C4"),
		seventh2 = new Note ("ElectricGuitar_D4"),

		root3 = new Note ("ElectricGuitar_E4"),
		second3 = new Note ("ElectricGuitar_F#4"),
		third3 = new Note("ElectricGuitar_G4"),
		fourth3 = new Note ("ElectricGuitar_A4"),
		fifth3 = new Note ("ElectricGuitar_B4"),
		sixth3 = new Note("ElectricGuitar_C5"),
		seventh3 = new Note ("ElectricGuitar_D5"),

		root4 = new Note ("ElectricGuitar_E5"),
		second4 = new Note ("ElectricGuitar_F#5"),
		third4 = new Note("ElectricGuitar_G5"),
		fourth4 = new Note ("ElectricGuitar_A5"),
		fifth4 = new Note ("ElectricGuitar_B5"),
		sixth4 = new Note("ElectricGuitar_C6")

			
	};
			

	Dictionary<Key, Dictionary<Instrument, Scale>> scales = new Dictionary<Key, Dictionary<Instrument, Scale>> () { 
		{ Key.Eminor, new Dictionary<Instrument,Scale> () {
				{ Instrument.ElectricGuitar, ElectricGuitarEminor }
			}
		}
	};
	  
	  	  
	 //scale = scales[MM.currentKey][riff.currentInstrument];

}
