using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Route95.Core;

namespace Route95.UI {

    /// <summary>
    /// Class for generic modal popups.
    /// </summary>
    public class Prompt : MenuBase<Prompt> {

        #region Prompt Vars

        public Text titleText;         // Reference to title text
        public Text messageText;       // Reference to message body text
        public Text buttonText;        // Reference to button text

        #endregion
        #region Prompt Methods

        /// <summary>
        /// Pops up the dialog with the given messages.
        /// </summary>
        /// <param name="title">Title of prompt.</param>
        /// <param name="message">Message.</param>
        /// <param name="button">Button text.</param>
        public void PromptMessage(string title, string message, string button) {
            titleText.text = title;
            messageText.text = message;
            buttonText.text = button;
            UIManager.Instance.ShowMenu(this);
        }

        /// <summary>
        /// Hides the dialog.
        /// </summary>
        public void HideDialog() {
            UIManager.Instance.HideMenu(this);
        }

        #endregion
    }
}