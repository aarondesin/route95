// PercussionInstrument.cs
// ©2016 Team 95

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Route95.Music {

    /// <summary>
    /// Class to hold percussion instrument data.
    /// </summary>
    public class PercussionInstrument : Instrument {

        #region Percussioninstrument Vars

        /// <summary>
        /// Dictionary of individual drums to icons.
        /// </summary>
        Dictionary<string, Sprite> _icons;

        /// <summary>
        /// Paths from which to load icons.
        /// </summary>
        Dictionary<string, string> _iconPaths;

        #endregion
        #region PercussionInstrument Methods

        /// <summary>
        /// Loads this percussion instrument.
        /// </summary>
        public override void Load() {

            // Load instrument data
            base.Load();

            // Load drum icons
            _icons = new Dictionary<string, Sprite>();
            foreach (string path in _iconPaths.Keys) {
                Sprite sprite = Resources.Load<Sprite>(_iconPaths[path]);

                if (sprite == null) {
                    Debug.LogError("PercussionInstrument.Load(): failed to load icon " + _iconPaths[path]);
                    continue;
                }

                _icons.Add(path, sprite);
            }
        }

        #endregion
        #region PercussionInstrument Instruments

        /// <summary>
        /// Rock drums.
        /// </summary>
        public static PercussionInstrument RockDrums = new PercussionInstrument {
            _name = "Rock Drums",
            _codeName = "RockDrums",
            _index = 0,
            _type = Type.Percussion,
            _family = Family.Percussion,
            _iconPath = "UI/Instrument_RockDrums",
            _glowPath = "UI/Instrument_RockDrums_Glow",
            _switchSoundPath = "Audio/Gameplay/Instruments/RockDrums",
            _iconPaths = new Dictionary<string, string>() {
            { "Audio/Instruments/Percussion/RockDrums/RockDrums_Kick", "UI/Percussion_Kick" },
            { "Audio/Instruments/Percussion/RockDrums/RockDrums_Snare", "UI/Percussion_Snare" },
            { "Audio/Instruments/Percussion/RockDrums/RockDrums_LowTom", "UI/Percussion_Tom" },
            { "Audio/Instruments/Percussion/RockDrums/RockDrums_MidTom", "UI/Percussion_Tom" },
            { "Audio/Instruments/Percussion/RockDrums/RockDrums_HiTom", "UI/Percussion_Tom" },
            { "Audio/Instruments/Percussion/RockDrums/RockDrums_Hat", "UI/Percussion_Hat" },
            { "Audio/Instruments/Percussion/RockDrums/RockDrums_Crash", "UI/Percussion_Hat" }
        }
        };

        /// <summary>
        /// Exotic percussion.
        /// </summary>
        public static PercussionInstrument ExoticPercussion = new PercussionInstrument {
            _name = "Exotic Percussion",
            _codeName = "ExoticPercussion",
            _index = 1,
            _type = Type.Percussion,
            _family = Family.Percussion,
            _iconPath = "UI/Instrument_ExoticPercussion",
            _glowPath = "UI/Instrument_ExoticPercussion",
            _switchSoundPath = "Audio/Gameplay/Instruments/RockDrums",
            _iconPaths = new Dictionary<string, string>() {
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
        #region Methods

        /// <summary>
        /// Returns the note icon for a note.
        /// </summary>
        public Sprite GetNoteIcon (string note) {
            return _icons[note];
        }

        #endregion
    }
}
