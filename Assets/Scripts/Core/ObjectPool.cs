// ObjectPool.cs
// ©2016 Team 95

using System.Collections.Generic;

using UnityEngine;

namespace Route95.Core {

    /// <summary>
    /// Data storage class to handle poolable
    /// GameObjects.
    /// </summary>
    public class ObjectPool<T> where T : MonoBehaviour, IPoolable {

        #region Vars

        /// <summary>
        /// List of inactive GameObjects.
        /// </summary>
        List<T> _pool;

        #endregion
        #region Constructor

        /// <summary>
        /// Init this Instance.
        /// </summary>
        public ObjectPool() {
            _pool = new List<T>();
        }

        #endregion
        #region Properties

        /// <summary>
        /// Returns whether or not the pool is empty.
        /// </summary>
        public bool Empty { get { return _pool.Count == 0; } }

        #endregion
        #region Methods

        /// <summary>
        /// Adds an item to the pool, and deactivates it.
        /// </summary>
        /// <param name="item">Item to add to the pool.</param>
        public void Add(T item) {
            _pool.Add(item);
            item.OnPool();
        }

        /// <summary>
        /// Returns a reference to the object at the top
        /// of the pool.
        /// </summary>
        public T Peek() {
            if (Empty) return default(T);
            return _pool[0];
        }

        /// <summary>
        /// Removes and returns an item from the pool
        /// and activates it.
        /// </summary>
        public T Get() {

            // Return null if empty
            if (Empty) return default(T);

            T result = _pool[0];
            _pool.RemoveAt(0);
            result.OnDepool();
            return result;
        }

        #endregion
    }
}
