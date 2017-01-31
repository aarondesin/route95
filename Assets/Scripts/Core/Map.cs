// Map.cs
// ©2016 Team 95

using UnityEngine;

namespace Route95.Core {

    /// <summary>
    /// Generic data storage class with a 2D array that automatically
    /// resizes itselft when given out-of-range data. Centered at (0,0).
    /// </summary>
    /// <typeparam name="T">Type of element.</typeparam>
    public class Map<T> where T : class {

        #region Vars

        /// <summary>
        /// 2D array of values.
        /// </summary>
        T[,] _values;

        /// <summary>
        /// Current width of array.
        /// </summary>
        int _width;

        #endregion
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="w">Initial width.</param>
        public Map(int w) {
            _values = new T[w, w];
            _width = w;
        }

        #endregion
        #region Properties

        /// <summary>
        /// Returns the current width of the array (read-only).
        /// </summary>
        public int Width { get { return _width; } }

        #endregion
        #region Methods

        /// <summary>
        /// Doubles the width of the array and remaps
        /// all stored values.
        /// </summary>
        void Resize() {

            // Error if invalid width
            if (_width == 0) {
                Debug.LogError("Map.Resize(): width 0!");
                return;
            }

            // Save old width
            int oldWidth = _width;

            // Create new array
            T[,] newValues = new T[oldWidth * 2, oldWidth * 2];

            // Remap old values
            for (int x = 0; x < oldWidth; x++) for (int y = 0; y < oldWidth; y++)
                    newValues[x + oldWidth / 2, y + oldWidth / 2] = _values[x, y];

            // Assign new array
            _values = newValues;

            // Double width
            _width *= 2;
        }

        /// <summary>
        /// Returns the value at i.
        /// </summary>
        /// <param name="i">Coordinates.</param>
        public T At(IntVector2 i) {
            return At(i.x, i.y);
        }

        /// <summary>
        /// Returns the value at x,y.
        /// </summary>
        public T At(int x, int y) {

            // Return null if values uninitialized or empty
            if (_values == null || _width == 0) return null;

            // Return null if values out of bounds
            if (x + _width / 2 < 0 || x + _width / 2 >= _width ||
                y + _width / 2 < 0 || y + _width / 2 >= _width) return null;

            return _values[x + _width / 2, y + _width / 2];
        }

        /// <summary>
        /// Sets the value at x,y.
        /// </summary>
        /// <param name="item"></param>
        public void Set(int x, int y, T item) {

            // While value is out of bounds, resize array
            while (x + _width / 2 >= _width || y + _width / 2 >= _width ||
            x + _width / 2 < 0 || y + _width / 2 < 0) Resize();

            _values[x + _width / 2, y + _width / 2] = item;
        }

        /// <summary>
        /// Conversion to string.
        /// </summary>
        public override string ToString() {
            return "Width: " + _width;
        }

        #endregion
    }
}
