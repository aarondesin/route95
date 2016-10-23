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
        /// Interval in list of all scales.
        /// </summary>
        int _scaleInterval;

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
            _scaleInterval = 0,
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
            _scaleInterval = 1,
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
        /// Returns the root interval.
        /// </summary>
        public int RootInterval { get { return _rootInterval; } }

        /// <summary>
        /// Returns the second interval.
        /// </summary>
        public int SecondInterval { get { return _secondInterval; } }

        /// <summary>
        /// Returns the third interval.
        /// </summary>
        public int ThirdInterval { get { return _thirdInterval; } }

        /// <summary>
        /// Returns the fourth interval.
        /// </summary>
        public int FourthInterval { get { return _fourthInterval; } }

        /// <summary>
        /// Returns the fifth interval.
        /// </summary>
        public int FifthInterval { get { return _fifthInterval; } }

        /// <summary>
        /// Returns the sixth interval.
        /// </summary>
        public int SixthInterval { get { return _sixthInterval; } }

        /// <summary>
        /// Returns the seventh interval.
        /// </summary>
        public int SeventhInterval { get { return _seventhInterval; } }

        #endregion
    }
}