// Prompt.cs
// ©2016 Team 95

using UnityEngine;
using UnityEngine.UI;

namespace Route95.UI {

    /// <summary>
    /// Class for generic modal popups.
    /// </summary>
    public class Prompt : MenuBase<Prompt> {

        #region Prompt Vars

		/// <summary>
		/// Reference to title text.
		/// </summary>
		[SerializeField]
        Text _titleText;

		/// <summary>
		/// Reference to message body text.
		/// </summary>
		[SerializeField]
        Text _messageText;

		/// <summary>
		/// Reference to button text.
		/// </summary>
		[SerializeField]
        Text _buttonText;

        #endregion
        #region Prompt Methods

        /// <summary>
        /// Pops up the dialog with the given messages.
        /// </summary>
        /// <param name="title">Title of prompt.</param>
        /// <param name="message">Message.</param>
        /// <param name="button">Button text.</param>
        public void PromptMessage(string title, string message, string button) {
            _titleText.text = title;
            _messageText.text = message;
            _buttonText.text = button;
            UIManager.Instance.ShowMenu(this);
        }

        #endregion
    }
}