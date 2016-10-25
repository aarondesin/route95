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
            _iconPath = "Sprites/Instrument_RockDrums",
            _glowPath = "Sprites/Instrument_RockDrums_Glow",
            _switchSoundPath = "Audio/Gameplay/Instruments/RockDrums",
            _iconPaths = new Dictionary<string, string>() {
            { "Audio/Instruments/Percussion/RockDrums/RockDrums_Kick", "Sprites/Percussion_Kick" },
            { "Audio/Instruments/Percussion/RockDrums/RockDrums_Snare", "Sprites/Percussion_Snare" },
            { "Audio/Instruments/Percussion/RockDrums/RockDrums_LowTom", "Sprites/Percussion_Tom" },
            { "Audio/Instruments/Percussion/RockDrums/RockDrums_MidTom", "Sprites/Percussion_Tom" },
            { "Audio/Instruments/Percussion/RockDrums/RockDrums_HiTom", "Sprites/Percussion_Tom" },
            { "Audio/Instruments/Percussion/RockDrums/RockDrums_Hat", "Sprites/Percussion_Hat" },
            { "Audio/Instruments/Percussion/RockDrums/RockDrums_Crash", "Sprites/Percussion_Hat" }
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
            _iconPath = "Sprites/Instrument_ExoticPercussion",
            _glowPath = "Sprites/Instrument_ExoticPercussion",
            _switchSoundPath = "Audio/Gameplay/Instruments/RockDrums",
            _iconPaths = new Dictionary<string, string>() {
            { "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Castinets","Sprites/Percussion_Castinets" },
            { "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Claves", "Sprites/Percussion_Claves" },
            { "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Cowbell", "Sprites/Percussion_Cowbell" },
            { "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Cowbell2", "Sprites/Percussion_Cowbell" },
            { "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_JamBlock", "Sprites/Percussion_JamBlock" },
            { "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Maracas1", "Sprites/Percussion_Maracas" },
            { "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Maracas2", "Sprites/Percussion_Maracas" },
            { "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Tambourine", "Sprites/Percussion_Tambourine" }
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
