// MelodicInstrument.cs
// ©2016 Team 95

using System.Collections.Generic;

namespace Route95.Music {

    /// <summary>
    /// Class to store all melodic instrument data.
    /// </summary>
    public class MelodicInstrument : Instrument {

        #region MelodicInstrument Vars

        /// <summary>
        /// Mapping of musical key to index of starting note.
        /// </summary>
        Dictionary<Key, int> _startingNote;

        #endregion
        #region MelodicInstrument Methods

        /// <summary>
        /// Electric guitar.
        /// </summary>
        public static MelodicInstrument ElectricGuitar = new MelodicInstrument {
            _name = "Electric Guitar",
            _codeName = "ElectricGuitar",
            _index = 2,
            _type = Type.Melodic,
            _family = Family.Guitar,
            _iconPath = "UI/Instrument_ElectricGuitar",
            _glowPath = "UI/Instrument_ElectricGuitar_Glow",
            _switchSoundPath = "Audio/Gameplay/Instruments/ElectricGuitar",
            _startingNote = new Dictionary<Key, int>() {
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

        /// <summary>
        /// Electric bass.
        /// </summary>
        public static MelodicInstrument ElectricBass = new MelodicInstrument {
            _name = "Electric Bass",
            _codeName = "ElectricBass",
            _index = 3,
            _type = Type.Melodic,
            _family = Family.Bass,
            _iconPath = "UI/Instrument_ElectricBass",
            _glowPath = "UI/Instrument_ElectricBass_Glow",
            _switchSoundPath = "Audio/Gameplay/Instruments/ElectricBass",
            _startingNote = new Dictionary<Key, int>() {
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

        /// <summary>
        /// Acoustic guitar.
        /// </summary>
        public static MelodicInstrument AcousticGuitar = new MelodicInstrument {
            _name = "Acoustic Guitar",
            _codeName = "AcousticGuitar",
            _index = 4,
            _type = Type.Melodic,
            _family = Family.Guitar,
            _iconPath = "UI/Instrument_AcousticGuitar",
            _glowPath = "UI/Instrument_AcousticGuitar_Glow",
            _switchSoundPath = "Audio/Gameplay/Instruments/ElectricGuitar",
            _startingNote = new Dictionary<Key, int>() {
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

        /// <summary>
        /// Classical guitar.
        /// </summary>
        public static MelodicInstrument ClassicalGuitar = new MelodicInstrument {
            _name = "Classical Guitar",
            _codeName = "ClassicalGuitar",
            _index = 5,
            _type = Type.Melodic,
            _family = Family.Guitar,
            _iconPath = "UI/Instrument_ClassicalGuitar",
            _glowPath = "UI/Instrument_ClassicalGuitar_Glow",
            _switchSoundPath = "Audio/Gameplay/Instruments/ElectricGuitar",
            _startingNote = new Dictionary<Key, int>() {
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

        /// <summary>
        /// Pipe organ.
        /// </summary>
        public static MelodicInstrument PipeOrgan = new MelodicInstrument {
            _name = "Pipe Organ",
            _codeName = "PipeOrgan",
            _index = 6,
            _type = Type.Melodic,
            _family = Family.Keyboard,
            _iconPath = "UI/Instrument_PipeOrgan",
            _glowPath = "UI/Instrument_PipeOrgan_Glow",
            _switchSoundPath = "Audio/Gameplay/Instruments/ElectricGuitar",
            _startingNote = new Dictionary<Key, int>() {
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

        /// <summary>
        /// Keyboard.
        /// </summary>
        public static MelodicInstrument Keyboard = new MelodicInstrument {
            _name = "Keyboard",
            _codeName = "Keyboard",
            _index = 7,
            _type = Type.Melodic,
            _family = Family.Keyboard,
            _iconPath = "UI/Instrument_Keyboard",
            _glowPath = "UI/Instrument_Keyboard_Glow",
            _switchSoundPath = "Audio/Gameplay/Instruments/ElectricGuitar",
            _startingNote = new Dictionary<Key, int>() {
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

        /// <summary>
        /// Trumpet.
        /// </summary>
        public static MelodicInstrument Trumpet = new MelodicInstrument {
            _name = "Trumpet",
            _codeName = "Trumpet",
            _index = 8,
            _type = Type.Melodic,
            _family = Family.Brass,
            _iconPath = "UI/Instrument_Trumpet",
            _glowPath = "UI/Instrument_Trumpet_Glow",
            _switchSoundPath = "Audio/Gameplay/Instruments/ElectricGuitar",
            _startingNote = new Dictionary<Key, int>() {
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
}