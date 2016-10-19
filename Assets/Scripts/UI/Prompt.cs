using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Class for generic modal popups.
/// </summary>
public class Prompt : SingletonMonoBehaviour<Prompt> {

	#region Prompt Vars

	public Text titleText;         // Reference to title text
	public Text messageText;       // Reference to message body text
	public Text buttonText;        // Reference to button text

	#endregion
	#region Unity Callbacks

	void Awake () {
		instance = this;
	}

	#endregion
	#region Prompt Methods

	/// <summary>
	/// Pops up the dialog with the given messages.
	/// </summary>
	/// <param name="title">Title of prompt.</param>
	/// <param name="message">Message.</param>
	/// <param name="button">Button text.</param>
	public void PromptMessage (string title, string message, string button) {
		titleText.text = title;
		messageText.text = message;
		buttonText.text = button;
		GameManager.instance.Show(gameObject);
	}

	/// <summary>
	/// Hides the dialog.
	/// </summary>
	public void HideDialog () {
		GameManager.instance.Hide(gameObject);
	}

	#endregion
}
