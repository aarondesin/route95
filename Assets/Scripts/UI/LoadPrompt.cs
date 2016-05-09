using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LoadPrompt : MonoBehaviour {

	public enum Mode {
		Project,
		Song
	};

	public static LoadPrompt instance;

	#region LoadPrompt Vars

	public RectTransform fileList; // Transform of the actual panel with all of the files listed
	public GameObject loadButton;

	string selectedPath; // Currently selected path

	List<GameObject> fileButtons;

	Mode loadMode;

	public Sprite fillSprite;

	static Vector2 fileListSize = new Vector2 (84f, 84f);

	static float horizontalPadding = 8f;
	static float verticalPadding = 4f;
	static Vector2 buttonSize = new Vector2 (360f, 72f);

	public Text header;

	#endregion
	#region Unity Callbacks

	void Awake () {
		fileButtons = new List<GameObject>();
		instance = this;
	}

	#endregion
	#region LoadPrompt Methods

	// 
	public void Refresh (Mode mode) {
		foreach (GameObject fileButton in fileButtons) {
			Destroy(fileButton);
		}
		fileButtons.Clear();

		loadMode = mode;

		// Get list of files in save location
		string[] files = new string[0];
		switch (mode) {
		case LoadPrompt.Mode.Project:
			files = Directory.GetFiles(GameManager.instance.projectSavePath, "*"+SaveLoad.projectSaveExtension);
			break;
		case LoadPrompt.Mode.Song:
			files = Directory.GetFiles(GameManager.instance.songSavePath, "*"+SaveLoad.songSaveExtension);
			break;
		}
		for (int i=0; i<files.Length; i++) {
			string path = files[i];
			string filename = Path.GetFileNameWithoutExtension (files[i]);

			GameObject button = UI.MakeButton(filename);

			RectTransform button_tr = button.RectTransform();
			button_tr.SetParent(fileList);
			float width = ((RectTransform)button_tr.parent.parent).rect.width;
			button_tr.sizeDelta = new Vector2(width, buttonSize.y);
			button_tr.AnchorAtPoint(0f, 1f);
			button_tr.anchoredPosition3D = new Vector3 (
				horizontalPadding + button_tr.sizeDelta.x/2f,
				((i == 0 ? 0f : verticalPadding) + button_tr.sizeDelta.y)*-(float)(i+1),
				0f
			);
			button_tr.ResetScaleRot();

			Image button_img = button.Image();
			button_img.sprite = fillSprite;
			button_img.color = new Color(1f,1f,1f,0f);

			GameObject text = UI.MakeText(filename+"_Text");
			RectTransform text_tr = text.RectTransform();
			text_tr.SetParent(button.transform);
			text_tr.sizeDelta = ((RectTransform)text_tr.parent).sizeDelta;
			text_tr.AnchorAtPoint (0.5f, 0.5f);
			text.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
			text_tr.ResetScaleRot();

			Text text_text = text.Text();
			text_text.text = filename;
			text_text.fontSize = 36;
			text_text.color = Color.white;
			text_text.font = GameManager.instance.font;
			text_text.alignment = TextAnchor.MiddleLeft;

			button.Button().onClick.AddListener(()=>{
				GameManager.instance.MenuClick();
				ResetButtons();
				selectedPath = path;
				loadButton.Button().interactable = true;
				button.Image().color = new Color(1f,1f,1f,0.5f);
			});
				
			fileButtons.Add(button);

			GameObject highlight = UI.MakeImage (filename+"_Highlight");
			RectTransform highlight_tr = highlight.RectTransform();
			highlight_tr.SetParent (button_tr);
			highlight_tr.sizeDelta = ((RectTransform)text_tr.parent).sizeDelta;
			highlight_tr.AnchorAtPoint(0.5f, 0.5f);
			highlight_tr.anchoredPosition3D = Vector3.zero;
			highlight_tr.ResetScaleRot();
			highlight.Image().color = new Color (1f, 1f, 1f, 0.5f);

			ShowHide sh = button.ShowHide();
			sh.objects = new List<GameObject>();
			sh.objects.Add (highlight);

			fileButtons.Add(highlight);
			highlight.SetActive (false);
		}

		// Update size of panel to fit all files
		fileList.sizeDelta = new Vector2 (fileListSize.x, (float)(fileButtons.Count+1)*(verticalPadding + buttonSize.y));

		// Update header
		header.text = mode == Mode.Project ? "Load Project" : "Load Song";
	}



	// calls save_load to load the currently selected file
	public void LoadSelectedPath () {
		//string fullPath = GameManager.instance.projectSavePath+"/"+selectedPath+SaveLoad.projectSaveExtension;
		Debug.Log("LoadPrompt.LoadSelectedPath(): loading "+selectedPath);
		switch (loadMode) {
		case LoadPrompt.Mode.Project:
			SaveLoad.LoadProject (selectedPath);
			break;
		case LoadPrompt.Mode.Song:
			SaveLoad.LoadSongToProject (selectedPath);
			break;
		}
	}

	// Resets highlighting of all buttons
	void ResetButtons () {
		foreach (GameObject button in fileButtons) button.Image().color = new Color (1f, 1f, 1f, 0f);
	}

	#endregion
}
