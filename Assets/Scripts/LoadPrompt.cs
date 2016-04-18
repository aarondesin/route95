using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public enum LoadMode {
	Project,
	Song
};

public class LoadPrompt : MonoBehaviour {
	public static LoadPrompt instance;

	public RectTransform fileList; // Transform of the actual panel with all of the files listed
	public GameObject loadButton;
	public GameObject appendButton;

	private LoadMode loadMode;

	string selectedPath; // Currently selected path

	List<GameObject> fileButtons;

	public Sprite fillSprite;

	static Vector2 fileListSize = new Vector2 (84f, 84f);

	static float horizontalPadding = 8f;
	static float verticalPadding = 4f;
	static Vector2 buttonSize = new Vector2 (360f, 72f);

	void Start () {
		fileButtons = new List<GameObject>();
		instance = this;
	}

	public void SetLoadMode (LoadMode newLoadMode) {
		loadMode = newLoadMode;
	}

	// 
	public void Refresh () {
		foreach (GameObject fileButton in fileButtons) {
			Destroy(fileButton);
		}
		fileButtons.Clear();

		// Get list of files in save location
		string[] files = new string[0];
		switch (loadMode) {
		case LoadMode.Project:
			files = Directory.GetFiles(GameManager.instance.projectSavePath, "*"+SaveLoad.projectSaveExtension);
			break;
		case LoadMode.Song:
			files = Directory.GetFiles(GameManager.instance.songSavePath, "*"+SaveLoad.songSaveExtension);
			break;
		}
		for (int i=0; i<files.Length; i++) {
			string path = files[i];
			string filename = Path.GetFileNameWithoutExtension (files[i]);

			GameObject button = UI.MakeButton(filename);

			button.GetComponent<RectTransform>().SetParent(fileList);
			float width = ((RectTransform)button.GetComponent<RectTransform>().parent.parent).rect.width;
			button.GetComponent<RectTransform>().sizeDelta = new Vector2(width, buttonSize.y);
			button.GetComponent<RectTransform>().localScale = new Vector3 (1f, 1f, 1f);
			button.GetComponent<RectTransform>().anchorMin = new Vector2 (0f, 1f);
			button.GetComponent<RectTransform>().anchorMax = new Vector2 (0f, 1f);
			button.GetComponent<RectTransform>().anchoredPosition3D = new Vector3 (
				horizontalPadding + button.GetComponent<RectTransform>().sizeDelta.x/2f,
				((i == 0 ? 0f : verticalPadding) + button.GetComponent<RectTransform>().sizeDelta.y)*-(float)(i+1),
				0f
			);

			button.GetComponent<Image>().sprite = fillSprite;
			button.GetComponent<Image>().color = new Color(1f,1f,1f,0f);

			GameObject text = UI.MakeText(filename+"_Text");
			text.GetComponent<RectTransform>().SetParent(button.transform);
			text.GetComponent<RectTransform>().sizeDelta = ((RectTransform)text.GetComponent<RectTransform>().parent).sizeDelta;
			text.GetComponent<RectTransform>().localScale = new Vector3 (1f, 1f, 1f);
			text.GetComponent<RectTransform>().anchorMin = new Vector2 (0.5f, 0.5f);
			text.GetComponent<RectTransform>().anchorMax = new Vector2 (0.5f, 0.5f);
			text.GetComponent<RectTransform>().anchoredPosition3D = new Vector3 (0f,0f,0f);
			text.GetComponent<Text>().text = filename;
			text.GetComponent<Text>().fontSize = 36;
			text.GetComponent<Text>().color = Color.white;
			text.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			text.GetComponent<Text>().fontStyle = FontStyle.Bold;
			text.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;

			button.GetComponent<Button>().onClick.AddListener(()=>{
				ResetButtons();
				selectedPath = path;
				loadButton.GetComponent<Button>().interactable = true;
				appendButton.GetComponent<Button>().interactable = true;
				button.GetComponent<Image>().color = new Color(1f,1f,1f,0.5f);
			});
			fileButtons.Add(button);
		}

		// Update size of panel to fit all files
		fileList.GetComponent<RectTransform>().sizeDelta = new Vector2 (fileListSize.x, (float)(fileButtons.Count+1)*(verticalPadding + buttonSize.y));
	}



	// calls save_load to load the currently selected file
	public void LoadSelectedPath () {
		//string fullPath = GameManager.instance.projectSavePath+"/"+selectedPath+SaveLoad.projectSaveExtension;
		Debug.Log("LoadPrompt.LoadSelectedPath(): loading "+selectedPath);
		switch (loadMode) {
		case LoadMode.Project:
			SaveLoad.LoadProject (selectedPath);
			break;
		case LoadMode.Song:
			SaveLoad.LoadSong (selectedPath);
			break;
		}
	}

	// Resets highlighting of all buttons
	void ResetButtons () {
		foreach (GameObject button in fileButtons) {
			//button.GetComponent<Image>().sprite = null;
			button.GetComponent<Image>().color = new Color (1f, 1f, 1f, 0f);
		}
	}
}
