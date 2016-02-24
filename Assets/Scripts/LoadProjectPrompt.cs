using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LoadProjectPrompt : MonoBehaviour {
	public static LoadProjectPrompt instance;

	public RectTransform fileList; // Transform of the actual panel with all of the files listed
	public GameObject loadButton;

	string selectedPath; // Currently selected path

	List<GameObject> fileButtons;

	static Vector2 fileListSize = new Vector2 (84f, 84f);

	static float horizontalPadding = 8f;
	static float verticalPadding = 4f;
	static Vector2 buttonSize = new Vector2 (64f, 16f);

	void Start () {
		fileButtons = new List<GameObject>();
		instance = this;
	}

	// 
	public void PopulateList () {
		foreach (GameObject fileButton in fileButtons) {
			Destroy(fileButton);
		}
		fileButtons.Clear();

		// Get list of files in save location
		//DirectoryInfo saveDirectory = new DirectoryInfo (@GameManager.instance.savePath);
		string[] files = Directory.GetFiles(GameManager.instance.savePath, "*"+save_load.saveExtension);
		for (int i=0; i<files.Length; i++) {
			string filename = Path.GetFileNameWithoutExtension (files[i]);

			GameObject button = new GameObject();
			button.name = filename;

			button.AddComponent<Button>();
			button.AddComponent<CanvasRenderer>();
			button.AddComponent<RectTransform>();
			button.GetComponent<RectTransform>().SetParent(fileList);
			button.GetComponent<RectTransform>().sizeDelta = buttonSize;
			button.GetComponent<RectTransform>().localScale = new Vector3 (1f, 1f, 1f);
			button.GetComponent<RectTransform>().anchorMin = new Vector2 (0f, 1f);
			button.GetComponent<RectTransform>().anchorMax = new Vector2 (0f, 1f);
			button.GetComponent<RectTransform>().anchoredPosition3D = new Vector3 (
				horizontalPadding + button.GetComponent<RectTransform>().sizeDelta.x/2f,
				((i == 0 ? 0f : verticalPadding) + button.GetComponent<RectTransform>().sizeDelta.y)*-(float)(i+1),
				0f
			);
			button.GetComponent<Button>().onClick.AddListener(()=>{selectedPath = button.name; loadButton.GetComponent<Button>().interactable = true;});

			button.AddComponent<Text>();
			button.GetComponent<Text>().text = filename;
			button.GetComponent<Text>().fontSize = 8;
			button.GetComponent<Text>().color = Color.white;
			button.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			button.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
			fileButtons.Add(button);
		}

		// Update size of panel to fit all files
		fileList.GetComponent<RectTransform>().sizeDelta = new Vector2 (fileListSize.x, (float)(fileButtons.Count+1)*(verticalPadding + buttonSize.y));
	}

	// calls save_load to load the currently selected file
	public void LoadSelectedPath () {
		string fullPath = GameManager.instance.savePath+"/"+selectedPath+save_load.saveExtension;
		Debug.Log("LoadProjectPrompt.LoadSelectedPath(): loading "+fullPath);
		save_load.LoadFile (fullPath);
	}
}
