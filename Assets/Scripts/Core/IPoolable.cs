// IPoolable.cs
// ©2016 Team 95

namespace Route95.Core {

    /// <summary>
    /// Interface for GameObjects that can be pooled and reused.
    /// </summary>
    public interface IPoolable {

        /// <summary>
        /// Called when this object is pooled.
        /// </summary>
        void OnPool();

        /// <summary>
        /// Called when this object is reused.
        /// </summary>
        void OnDepool();

    }
}