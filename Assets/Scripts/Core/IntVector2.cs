// IntVector2.cs
// ©2016 Team 95

using Route95.World;

using UnityEngine;

namespace Route95.Core {

    /// <summary>
    /// Struct to hold an x,y coordinate pair of ints.
    /// </summary>
    public struct IntVector2 {

        #region IntVector2 Vars

        /// <summary>
        /// x coordinate.
        /// </summary>
        public int x;

        /// <summary>
        /// y coordinate.
        /// </summary>
        public int y;

        #endregion
        #region IntVector2 Methods

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="_x">Initial x value.</param>
        /// <param name="_y">Initial y value.</param>
        public IntVector2(int _x, int _y) {
            x = _x;
            y = _y;
        }

        /// <summary>
        /// Returns the distance between two IntVector2s.
        /// </summary>
        /// <param name="a">First IntVector2.</param>
        /// <param name="b">Second IntVector2.</param>
        public static float Distance(IntVector2 a, IntVector2 b) {
            return Vector2.Distance(a.ToVector2(), b.ToVector2());
        }

        /// <summary>
        /// Returns whether or not a coordinate is a corner.
        /// </summary>
        public bool IsCorner() {
            return
                (x == 0 || x == WorldManager.Instance.ChunkResolution - 1) &&
                (y == 0 || y == WorldManager.Instance.ChunkResolution - 1);
        }

        /// <summary>
        /// Vector2 conversion.
        /// </summary>
        public Vector2 ToVector2() {
            return new Vector2((float)x, (float)y);
        }

        /// <summary>
        /// String conversion.
        /// </summary>
        public override string ToString() {
            return "(" + x + "," + y + ")";
        }

        #endregion
    }
}