// Instrument.cs
// ©2016 Team 95

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Route95.Music {

    /// <summary>
    /// Class to store all relevant instrument data and references.
    /// </summary>
    public abstract class Instrument : IComparable {

        #region Instrument Enums

        /// <summary>
        /// Base type of instrument.
        /// </summary>
        public enum Type {
            Percussion,
            Melodic
        };

        /// <summary>
        /// Instrument family.
        /// </summary>
        public enum Family {
            Percussion,
            Guitar,
            Bass,
            Keyboard,
            Brass
        };

        #endregion
        #region Instrument Vars

        /// <summary>
        /// User-friendly name.
        /// </summary>
        protected string _name;

        /// <summary>
        /// Name in code.
        /// </summary>
        protected string _codeName;

        /// <summary>
        /// Game index.
        /// </summary>
        protected int _index;

        /// <summary>
        /// Instrument type.
        /// </summary>
        protected Type _type;

        /// <summary>
        /// Instrument family.
        /// </summary>
        protected Family _family;

        /// <summary>
        /// Icon sprite.
        /// </summary>
        protected Sprite _icon;

        /// <summary>
        /// Path from which to load icon.
        /// </summary>
        protected string _iconPath;

        /// <summary>
        /// Glow sprite.
        /// </summary>
        protected Sprite _glow;

        /// <summary>
        /// Path from which to load glow sprite.
        /// </summary>
        protected string _glowPath;

        /// <summary>
        /// Sound to play when instrument is switched to.
        /// </summary>
        protected AudioClip _switchSound;

        /// <summary>
        /// Path from which to load switch sound.
        /// </summary>
        protected string _switchSoundPath;

        /// <summary>
        /// List of all instruments available.
        /// </summary>
        public static List<Instrument> AllInstruments;

        #endregion
        #region Properties

        /// <summary>
        /// Returns the user-friendly name of this instrument (read-only).
        /// </summary>
        public string Name { get { return _name; } }

        /// <summary>
        /// Returns the in-code name of this instrument (read-only).
        /// </summary>
        public string CodeName { get { return _codeName; } }

        /// <summary>
        /// Returns the icon for this instrument (read-only).
        /// </summary>
        public Sprite Icon { get { return _icon; } }

        /// <summary>
        /// Returns the index of this instrument (read-only).
        /// </summary>
        public int Index { get { return _index; } }

        /// <summary>
        /// Returns the type of this instrument (read-only).
        /// </summary>
        public Instrument.Type InstrumentType { get { return _type; } }

        /// <summary>
        /// Returns the family of this instrument (read-only).
        /// </summary>
        public Family InstrumentFamily { get { return _family; } }

        /// <summary>
        /// Returns the switch sound for this instrument (read-only).
        /// </summary>
        public AudioClip SwitchSound { get { return _switchSound; } }

        #endregion
        #region IComparable Implementation

        public int CompareTo(object obj) {
            if (obj == null) return 1;

            Instrument other = obj as Instrument;
            if (other != null) return this._index.CompareTo(other.Index);
            else throw new ArgumentException("Argument is not an instrument.");

        }

        #endregion
        #region Instrument Methods

        /// <summary>
        /// Loads all resources associated with an instrument.
        /// </summary>
        public virtual void Load() {
            _icon = Resources.Load<Sprite>(_iconPath);
            _glow = Resources.Load<Sprite>(_glowPath);
            _switchSound = Resources.Load<AudioClip>(_switchSoundPath);
        }

        /// <summary>
        /// Loads all instruments.
        /// </summary>
        public static void LoadInstruments() {

            // Populate list of instruments
            AllInstruments = new List<Instrument>() {
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

            // Load all instruments
            foreach (Instrument instrument in AllInstruments)
                instrument.Load();
        }

        #endregion
    }
}
