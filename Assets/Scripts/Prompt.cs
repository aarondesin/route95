using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Prompt : MonoBehaviour {

	public static Prompt instance;

	public GameObject dialog;
	public Text titleText;
	public Text messageText;
	public Text buttonText;

	void Start () {
		instance = this;
	}

	public void PromptMessage (string title, string message, string button) {
		titleText.text = title;
		messageText.text = message;
		buttonText.text = button;
		dialog.SetActive(true);
	}

	public void HideDialog () {
		dialog.SetActive(false);
	}
}
