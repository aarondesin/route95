using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Prompt : MonoBehaviour {

	#region Prompt Vars

	public static Prompt instance;

	public GameObject dialog;
	public Text titleText;
	public Text messageText;
	public Text buttonText;

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
		dialog.SetActive(true);
	}

	/// <summary>
	/// Hides the dialog.
	/// </summary>
	public void HideDialog () {
		dialog.SetActive(false);
	}

	#endregion
}
