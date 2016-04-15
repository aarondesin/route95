using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum InstrumentType {
	Percussion,
	Melodic
};

public class Instrument {

	public string name;
	public InstrumentType type;

	public Sprite icon;
	private string iconPath;

	public Sprite glow;
	private string glowPath;

	public static Instrument RockDrums = new Instrument {
		name = "Rock Drums",
		type = InstrumentType.Percussion,
		iconPath = "UI/Instrument_RockDrums",
		glowPath = "UI/Instrument_RockDrums_Glow"
	};

	public static Instrument ElectricGuitar = new Instrument {
		name = "Electric Guitar",
		type = InstrumentType.Melodic,
		iconPath = "UI/Instrument_ElectricGuitar",
		glowPath = "UI/Instrument_ElectricGuitar_Glow"
	};

	public static Instrument ElectricBass = new Instrument {
		name = "Electric Bass",
		type = InstrumentType.Melodic,
		iconPath = "UI/Instrument_ElectricBass",
		glowPath = "UI/Instrument_ElectricBass_Glow"
	};

	public static Instrument AcousticGuitar = new Instrument {
		name = "Acoustic Guitar",
		type = InstrumentType.Melodic,
		iconPath = "UI/Instrument_AcousticGuitar",
		glowPath = "UI/Instrument_AcousticGuitar_Glow"
	};

	public static Instrument ClassicalGuitar = new Instrument {
		name = "Classical Guitar",
		type = InstrumentType.Melodic,
		iconPath = "UI/Instrument_ClassicalGuitar",
		glowPath = "UI/Instrument_ClassicalGuitar_Glow"
	};

	public static Instrument PipeOrgan = new Instrument {
		name = "Pipe Organ",
		type = InstrumentType.Melodic,
		iconPath = "UI/Instrument_PipeOrgan",
		glowPath = "UI/Instrument_PipeOrgan_Glow"
	};

	public static Instrument Keyboard = new Instrument {
		name = "Keyboard",
		type = InstrumentType.Melodic,
		iconPath = "UI/Instrument_Keyboard",
		glowPath = "UI/Instrument_Keyboard_Glow"
	};

	public static List<Instrument> AllInstruments = new List<Instrument> () {
		RockDrums,
		ElectricGuitar,
		ElectricBass,
		AcousticGuitar,
		ClassicalGuitar,
		PipeOrgan,
		Keyboard
	};

	public static void LoadInstruments () {
		foreach (Instrument instrument in AllInstruments) {
			instrument.icon = Resources.Load<Sprite>(instrument.iconPath);
			instrument.glow = Resources.Load<Sprite>(instrument.glowPath);
		}
	}
}
