// Measure.cs
// ©2016 Team 95

using System.Collections.Generic;

using UnityEngine;

namespace Route95.Music {

    /// <summary>
    /// Class to store riffs.
    /// </summary>
    [System.Serializable]
    public class Measure {

        #region Vars

        /// <summary>
        /// Project-specific index.
        /// </summary>
        [SerializeField]
        int _index;

        /// <summary>
        /// List of indices of riffs used.
        /// </summary>
        [SerializeField]
        List<int> _riffIndices;

        #endregion
        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Measure() {
            _riffIndices = new List<int>();
        }

        #endregion
    }
}
