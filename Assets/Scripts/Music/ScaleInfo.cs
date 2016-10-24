// ScaleInfo.cs
// ©2016 Team 95

using System.Collections.Generic;

namespace Route95.Music {

    /// <summary>
    /// Class to hold all types of scales.
    /// </summary>
    public class ScaleInfo {

        #region ScaleInfo Vars

        /// <summary>
        /// Name of scale.
        /// </summary>
        string _name;

        /// <summary>
        /// Index in list of all scales.
        /// </summary>
        int _scaleIndex;

        /// <summary>
        /// Interval to root.
        /// </summary>
        int _rootInterval;

        /// <summary>
        /// Interval to second.
        /// </summary>
        int _secondInterval;

        /// <summary>
        /// Interval to third.
        /// </summary>
        int _thirdInterval;

        /// <summary>
        /// Interval to fourth.
        /// </summary>
        int _fourthInterval;

        /// <summary>
        /// Interval to fifth.
        /// </summary>
        int _fifthInterval;

        /// <summary>
        /// Interval to sixth.
        /// </summary>
        int _sixthInterval;

        /// <summary>
        /// Interval to seventh.
        /// </summary>
        int _seventhInterval;

        /// <summary>
        /// Major scale.
        /// </summary>
        public static ScaleInfo Major = new ScaleInfo() {
            _name = "Major",
            _scaleIndex = 0,
            _secondInterval = 2,
            _thirdInterval = 2,
            _fourthInterval = 1,
            _fifthInterval = 2,
            _sixthInterval = 2,
            _seventhInterval = 2,
            _rootInterval = 1
        };

        /// <summary>
        /// Minor scale.
        /// </summary>
        public static ScaleInfo Minor = new ScaleInfo() {
            _name = "Minor",
            _scaleIndex = 1,
            _secondInterval = 2,
            _thirdInterval = 1,
            _fourthInterval = 2,
            _fifthInterval = 2,
            _sixthInterval = 1,
            _seventhInterval = 2,
            _rootInterval = 2
        };

        /// <summary>
        /// List of all scale types.
        /// </summary>
        public static List<ScaleInfo> AllScales = new List<ScaleInfo>() {
            Major,
            Minor
        };

        #endregion
        #region Properties

        /// <summary>
        /// Returns the name of this scale (read-only).
        /// </summary>
        public string Name { get { return _name; } }

        /// <summary>
        /// Returns the root interval (read-only).
        /// </summary>
        public int RootInterval { get { return _rootInterval; } }

        /// <summary>
        /// Returns the second interval (read-only).
        /// </summary>
        public int SecondInterval { get { return _secondInterval; } }

        /// <summary>
        /// Returns the third interval (read-only).
        /// </summary>
        public int ThirdInterval { get { return _thirdInterval; } }

        /// <summary>
        /// Returns the fourth interval (read-only).
        /// </summary>
        public int FourthInterval { get { return _fourthInterval; } }

        /// <summary>
        /// Returns the fifth interval (read-only).
        /// </summary>
        public int FifthInterval { get { return _fifthInterval; } }

        /// <summary>
        /// Returns the sixth interval (read-only).
        /// </summary>
        public int SixthInterval { get { return _sixthInterval; } }

        /// <summary>
        /// Returns the seventh interval (read-only).
        /// </summary>
        public int SeventhInterval { get { return _seventhInterval; } }

        /// <summary>
        /// Returns the index of this scale (read-only).
        /// </summary>
        public int ScaleIndex { get { return _scaleIndex; } }

        #endregion
    }
}