// LoadingScreen.cs
// ©2016 Team 95

using UnityEngine.UI;

namespace Route95.UI {

    public class LoadingScreen : MenuBase<LoadingScreen> {

        #region LoadingScreen Vars

        /// <summary>
        /// Loading screen message.
        /// </summary>
        Text _loadingMessage;

        #endregion
        #region Unity Callbacks

        new void Awake () {
            base.Awake();

            // Init vars
            _loadingMessage = GetComponentInChildren<Text>();
        }

        #endregion
        #region Methods

        /// <summary>
        /// Sets the loading message.
        /// </summary>
        /// <param name="message">New loading message.</param>
        public void SetLoadingMessage (string message) {
            _loadingMessage.text = message;
        }

        #endregion
    }
}
