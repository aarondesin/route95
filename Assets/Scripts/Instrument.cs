using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum InstrumentType {
	Percussion,
	Melodic
};
	
public class Instrument {

	public string name; // User-friendly name
	public string codeName; // Name in code
	public int index;
	public InstrumentType type;

	public Sprite icon;
	private string iconPath;

	public Sprite glow;
	private string glowPath;

	public Dictionary<Key, int> startingNote;

	public static Instrument RockDrums = new Instrument {
		name = "Rock Drums",
		codeName = "RockDrums",
		index = 0,
		type = InstrumentType.Percussion,
		iconPath = "UI/Instrument_RockDrums",
		glowPath = "UI/Instrument_RockDrums_Glow",
		startingNote = null
	};

	public static Instrument ExoticPercussion = new Instrument {
		name = "Exotic Percussion",
		codeName = "ExoticPercussion",
		index = 1,
		type = InstrumentType.Percussion,
		iconPath = "UI/Instrument_ExoticPercussion",
		glowPath = "UI/Instrument_ExoticPercussion",
		startingNote = null
	};

	public static Instrument ElectricGuitar = new Instrument {
		name = "Electric Guitar",
		codeName = "ElectricGuitar",
		index = 2,
		type = InstrumentType.Melodic,
		iconPath = "UI/Instrument_ElectricGuitar",
		glowPath = "UI/Instrument_ElectricGuitar_Glow",
		startingNote = new Dictionary<Key, int> () {
			{ Key.C, 8 },
			{ Key.CSharp, 9 },
			{ Key.D, 10 },
			{ Key.DSharp, 11 },
			{ Key.E, 0 },
			{ Key.F, 1 },
			{ Key.FSharp, 2 },
			{ Key.G, 3 },
			{ Key.GSharp, 4 },
			{ Key.A, 5 },
			{ Key.ASharp, 6 },
			{ Key.B, 7 }
		}
	};

	public static Instrument ElectricBass = new Instrument {
		name = "Electric Bass",
		codeName = "ElectricBass",
		index = 3,
		type = InstrumentType.Melodic,
		iconPath = "UI/Instrument_ElectricBass",
		glowPath = "UI/Instrument_ElectricBass_Glow",
		startingNote = new Dictionary<Key, int> () {
			{ Key.C, 8 },
			{ Key.CSharp, 9 },
			{ Key.D, 10 },
			{ Key.DSharp, 11 },
			{ Key.E, 0 },
			{ Key.F, 1 },
			{ Key.FSharp, 2 },
			{ Key.G, 3 },
			{ Key.GSharp, 4 },
			{ Key.A, 5 },
			{ Key.ASharp, 6 },
			{ Key.B, 7 }
		}
	};

	public static Instrument AcousticGuitar = new Instrument {
		name = "Acoustic Guitar",
		codeName = "AcousticGuitar",
		index = 4,
		type = InstrumentType.Melodic,
		iconPath = "UI/Instrument_AcousticGuitar",
		glowPath = "UI/Instrument_AcousticGuitar_Glow",
		startingNote = new Dictionary<Key, int> () {
			{ Key.C, 8 },
			{ Key.CSharp, 9 },
			{ Key.D, 10 },
			{ Key.DSharp, 11 },
			{ Key.E, 0 },
			{ Key.F, 1 },
			{ Key.FSharp, 2 },
			{ Key.G, 3 },
			{ Key.GSharp, 4 },
			{ Key.A, 5 },
			{ Key.ASharp, 6 },
			{ Key.B, 7 }
		}
	};

	public static Instrument ClassicalGuitar = new Instrument {
		name = "Classical Guitar",
		codeName = "ClassicalGuitar",
		index = 5,
		type = InstrumentType.Melodic,
		iconPath = "UI/Instrument_ClassicalGuitar",
		glowPath = "UI/Instrument_ClassicalGuitar_Glow",
		startingNote = new Dictionary<Key, int> () {
			{ Key.C, 8 },
			{ Key.CSharp, 9 },
			{ Key.D, 10 },
			{ Key.DSharp, 11 },
			{ Key.E, 0 },
			{ Key.F, 1 },
			{ Key.FSharp, 2 },
			{ Key.G, 3 },
			{ Key.GSharp, 4 },
			{ Key.A, 5 },
			{ Key.ASharp, 6 },
			{ Key.B, 7 }
		}
	};

	public static Instrument PipeOrgan = new Instrument {
		name = "Pipe Organ",
		codeName = "PipeOrgan",
		index = 6,
		type = InstrumentType.Melodic,
		iconPath = "UI/Instrument_PipeOrgan",
		glowPath = "UI/Instrument_PipeOrgan_Glow",
		startingNote = new Dictionary<Key, int> () {
			{ Key.C, 3 },
			{ Key.CSharp, 4 },
			{ Key.D, 5 },
			{ Key.DSharp, 6 },
			{ Key.E, 7 },
			{ Key.F, 8 },
			{ Key.FSharp, 9 },
			{ Key.G, 10 },
			{ Key.GSharp, 11 },
			{ Key.A, 0 },
			{ Key.ASharp, 1 },
			{ Key.B, 2 }
		}
	};

	public static Instrument Keyboard = new Instrument {
		name = "Keyboard",
		codeName = "Keyboard",
		index = 7,
		type = InstrumentType.Melodic,
		iconPath = "UI/Instrument_Trumpet",
		glowPath = "UI/Instrument_Keyboard_Glow",
		startingNote = new Dictionary<Key, int> () {
			{ Key.C, 3 },
			{ Key.CSharp, 4 },
			{ Key.D, 5 },
			{ Key.DSharp, 6 },
			{ Key.E, 7 },
			{ Key.F, 8 },
			{ Key.FSharp, 9 },
			{ Key.G, 10 },
			{ Key.GSharp, 11 },
			{ Key.A, 0 },
			{ Key.ASharp, 1 },
			{ Key.B, 2 }
		}
	};

	public static Instrument Trumpet = new Instrument {
		name = "Trumpet",
		codeName = "Trumpet",
		index = 8,
		type = InstrumentType.Melodic,
		iconPath = "UI/Instrument_Trumpet",
		glowPath = "UI/Instrument_Trumpet_Glow",
		startingNote = new Dictionary<Key, int> () {
			{ Key.C, 3 },
			{ Key.CSharp, 4 },
			{ Key.D, 5 },
			{ Key.DSharp, 6 },
			{ Key.E, 7 },
			{ Key.F, 8 },
			{ Key.FSharp, 9 },
			{ Key.G, 10 },
			{ Key.GSharp, 11 },
			{ Key.A, 0 },
			{ Key.ASharp, 1 },
			{ Key.B, 2 }
		}
	};

	public static List<Instrument> AllInstruments = new List<Instrument> () {
		RockDrums,
		ExoticPercussion,
		ElectricGuitar,
		ElectricBass,
		AcousticGuitar,
		ClassicalGuitar,
		PipeOrgan,
		Keyboard,
		Trumpet
	};

	public static void LoadInstruments () {
		foreach (Instrument instrument in AllInstruments) {
			instrument.icon = Resources.Load<Sprite>(instrument.iconPath);
			instrument.glow = Resources.Load<Sprite>(instrument.glowPath);
		}
	}
}
