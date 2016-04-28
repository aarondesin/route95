using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Instrument {

	public enum Type {
		Percussion,
		Melodic
	};

	public enum Family {
		Percussion,
		Guitar,
		Bass,
		Keyboard,
		Brass
	};

	#region Instrument Vars

	public string name; // User-friendly name
	public string codeName; // Name in code
	public int index;
	public Type type;
	public Family family;

	public Sprite icon;
	protected string iconPath;

	public Sprite glow;
	protected string glowPath;

	public AudioClip switchSound;
	protected string switchSoundPath;

	public static List<Instrument> AllInstruments;

	#endregion
	#region Instrument Methods

	public virtual void Load () {
		icon = Resources.Load<Sprite>(iconPath);
		glow = Resources.Load<Sprite>(glowPath);
		switchSound = Resources.Load<AudioClip>(switchSoundPath);
	}

	public static void LoadInstruments () {
		AllInstruments = new List<Instrument> () {
			PercussionInstrument.RockDrums,
			PercussionInstrument.ExoticPercussion,
			MelodicInstrument.ElectricGuitar,
			MelodicInstrument.ElectricBass,
			MelodicInstrument.AcousticGuitar,
			MelodicInstrument.ClassicalGuitar,
			MelodicInstrument.PipeOrgan,
			MelodicInstrument.Keyboard,
			MelodicInstrument.Trumpet
		};
		foreach (Instrument instrument in AllInstruments)
			instrument.Load();
	}

	#endregion
}
	
public class PercussionInstrument : Instrument {

	#region Percussioninstrument Vars

	public Dictionary <string, Sprite> icons;
	Dictionary <string, string> iconPaths;

	#endregion
	#region PercussionInstrument Methods

	public override void Load () {
		base.Load();
		icons = new Dictionary<string, Sprite>();
		foreach (string path in iconPaths.Keys) {
			Sprite sprite = Resources.Load<Sprite>(iconPaths[path]);
			if (sprite == null) {
				Debug.LogError ("PercussionInstrument.Load(): failed to load icon "+iconPaths[path]);
				continue;
			}
			icons.Add (path, sprite);
		}
	}

	#endregion
	#region PercussionInstrument Instruments

	public static PercussionInstrument RockDrums = new PercussionInstrument {
		name = "Rock Drums",
		codeName = "RockDrums",
		index = 0,
		type = Type.Percussion,
		family = Family.Percussion,
		iconPath = "UI/Instrument_RockDrums",
		glowPath = "UI/Instrument_RockDrums_Glow",
		switchSoundPath = "Audio/Gameplay/Instruments/RockDrums",
		iconPaths = new Dictionary <string, string> () {
			{ "Audio/Instruments/Percussion/RockDrums/RockDrums_Kick", "UI/Percussion_Kick" },
			{ "Audio/Instruments/Percussion/RockDrums/RockDrums_Snare", "UI/Percussion_Snare" },
			{ "Audio/Instruments/Percussion/RockDrums/RockDrums_Tom", "UI/Percussion_Tom" },
			{ "Audio/Instruments/Percussion/RockDrums/RockDrums_Hat", "UI/Percussion_Hat" },
			{ "Audio/Instruments/Percussion/RockDrums/RockDrums_Crash", "UI/Percussion_Hat" }
		}
	};

	public static PercussionInstrument ExoticPercussion = new PercussionInstrument {
		name = "Exotic Percussion",
		codeName = "ExoticPercussion",
		index = 1,
		type = Type.Percussion,
		family = Family.Percussion,
		iconPath = "UI/Instrument_ExoticPercussion",
		glowPath = "UI/Instrument_ExoticPercussion",
		switchSoundPath = "Audio/Gameplay/Instruments/RockDrums",
		iconPaths = new Dictionary<string, string> () {
			{ "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Castinets","UI/Percussion_Castinets" },
			{ "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Claves", "UI/Percussion_Claves" },
			{ "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Cowbell", "UI/Percussion_Cowbell" },
			{ "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Cowbell2", "UI/Percussion_Cowbell" },
			{ "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_JamBlock", "UI/Percussion_JamBlock" },
			{ "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Maracas1", "UI/Percussion_Maracas" },
			{ "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Maracas2", "UI/Percussion_Maracas" },
			{ "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Tambourine", "UI/Percussion_Tambourine" }
		}
	};

	#endregion
			
}

public class MelodicInstrument : Instrument {

	#region MelodicInstrument Vars

	public Dictionary<Key, int> startingNote;

	#endregion
	#region MelodicInstrument Methods

	public static MelodicInstrument ElectricGuitar = new MelodicInstrument {
		name = "Electric Guitar",
		codeName = "ElectricGuitar",
		index = 2,
		type = Type.Melodic,
		family = Family.Guitar,
		iconPath = "UI/Instrument_ElectricGuitar",
		glowPath = "UI/Instrument_ElectricGuitar_Glow",
		switchSoundPath = "Audio/Gameplay/Instruments/ElectricGuitar",
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

	public static MelodicInstrument ElectricBass = new MelodicInstrument {
		name = "Electric Bass",
		codeName = "ElectricBass",
		index = 3,
		type = Type.Melodic,
		family = Family.Bass,
		iconPath = "UI/Instrument_ElectricBass",
		glowPath = "UI/Instrument_ElectricBass_Glow",
		switchSoundPath = "Audio/Gameplay/Instruments/ElectricBass",
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

	public static MelodicInstrument AcousticGuitar = new MelodicInstrument {
		name = "Acoustic Guitar",
		codeName = "AcousticGuitar",
		index = 4,
		type = Type.Melodic,
		family = Family.Guitar,
		iconPath = "UI/Instrument_AcousticGuitar",
		glowPath = "UI/Instrument_AcousticGuitar_Glow",
		switchSoundPath = "Audio/Gameplay/Instruments/ElectricGuitar",
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

	public static MelodicInstrument ClassicalGuitar = new MelodicInstrument {
		name = "Classical Guitar",
		codeName = "ClassicalGuitar",
		index = 5,
		type = Type.Melodic,
		family = Family.Guitar,
		iconPath = "UI/Instrument_ClassicalGuitar",
		glowPath = "UI/Instrument_ClassicalGuitar_Glow",
		switchSoundPath = "Audio/Gameplay/Instruments/ElectricGuitar",
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

	public static MelodicInstrument PipeOrgan = new MelodicInstrument {
		name = "Pipe Organ",
		codeName = "PipeOrgan",
		index = 6,
		type = Type.Melodic,
		family = Family.Keyboard,
		iconPath = "UI/Instrument_PipeOrgan",
		glowPath = "UI/Instrument_PipeOrgan_Glow",
		switchSoundPath = "Audio/Gameplay/Instruments/ElectricGuitar",
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

	public static MelodicInstrument Keyboard = new MelodicInstrument {
		name = "Keyboard",
		codeName = "Keyboard",
		index = 7,
		type = Type.Melodic,
		family = Family.Keyboard,
		iconPath = "UI/Instrument_Keyboard",
		glowPath = "UI/Instrument_Keyboard_Glow",
		switchSoundPath = "Audio/Gameplay/Instruments/ElectricGuitar",
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

	public static MelodicInstrument Trumpet = new MelodicInstrument {
		name = "Trumpet",
		codeName = "Trumpet",
		index = 8,
		type = Type.Melodic,
		family = Family.Brass,
		iconPath = "UI/Instrument_Trumpet",
		glowPath = "UI/Instrument_Trumpet_Glow",
		switchSoundPath = "Audio/Gameplay/Instruments/ElectricGuitar",
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

	#endregion

}
