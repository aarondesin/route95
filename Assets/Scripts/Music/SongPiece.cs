// SongPiece.cs
// ©2016 Team 95

using System.Collections.Generic;

using UnityEngine;

namespace Route95.Music {

    /// <summary>
    /// Class to store all measure data.
    /// </summary>
    [System.Serializable]
    public class SongPiece {

        #region NonSerialized Song Piece Vars

        /// <summary>
        /// Default number of measures per song piece.
        /// </summary>
        const int DEFAULT_MEASURES = 1;

        #endregion
        #region Serialized Song Piece Vars

        /// <summary>
        /// Name of song piece.
        /// </summary>
        [SerializeField]
        string _name;

        /// <summary>
        /// Project-assigned index.
        /// </summary>
        [SerializeField]
        int _index;

        /// <summary>
        /// List of indices of measures used.
        /// </summary>
        [SerializeField]
        List<int> _measureIndices;

        #endregion
        #region Properties

        /// <summary>
        /// Gets/sets the index of this song piece.
        /// </summary>
        public int Index {
            get { return _index; }
            set { _index = value; }
        }

        /// <summary>
        /// Returns a list of the indices of the measures used in this song piece (read-only).
        /// </summary>
        public List<int> MeasureIndices { get { return _measureIndices; } }

        #endregion
        #region Song Piece Methods

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SongPiece() {
            _measureIndices = new List<int>();
        }

        /// <summary>
        /// Plays all notes at the given position.
        /// </summary>
        /// <param name="pos">Beat at which to play notes.</param>
        public void PlaySongPiece(int pos) {
            int measureNum = pos / 4;
            Song song = MusicManager.Instance.CurrentSong;
            Measure measure = song.Measures[measureNum];

            // Play all riffs
            foreach (int r in measure.RiffIndices) {
                Riff riff = song.Riffs[r];
                riff.PlayRiff(pos % 4);
            }
        }

        #endregion
    }
}
