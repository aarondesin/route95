using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LoadProjectPrompt : MonoBehaviour {
	public static LoadProjectPrompt instance;

	public RectTransform fileList; // Transform of the actual panel with all of the files listed
	public GameObject loadButton;
	public GameObject appendButton;

	string selectedPath; // Currently selected path

	List<GameObject> fileButtons;

	public Sprite fillSprite;

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
			button.AddComponent<Image>();
			button.GetComponent<Image>().sprite = fillSprite;
			button.GetComponent<Image>().color = new Color(1f,1f,1f,0f);

			GameObject text = new GameObject();
			text.name = "Text";
			text.AddComponent<CanvasRenderer>();
			text.AddComponent<RectTransform>();
			text.GetComponent<RectTransform>().SetParent(button.transform);
			text.GetComponent<RectTransform>().sizeDelta = ((RectTransform)text.GetComponent<RectTransform>().parent).sizeDelta;
			text.GetComponent<RectTransform>().localScale = new Vector3 (1f, 1f, 1f);
			text.GetComponent<RectTransform>().anchorMin = new Vector2 (0.5f, 0.5f);
			text.GetComponent<RectTransform>().anchorMax = new Vector2 (0.5f, 0.5f);
			text.GetComponent<RectTransform>().anchoredPosition3D = new Vector3 (0f,0f,0f);
			text.AddComponent<Text>();
			text.GetComponent<Text>().text = filename;
			text.GetComponent<Text>().fontSize = 10;
			text.GetComponent<Text>().color = Color.white;
			text.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			text.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;

			button.GetComponent<Button>().onClick.AddListener(()=>{
				ResetButtons();
				selectedPath = button.name;
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
		string fullPath = GameManager.instance.savePath+"/"+selectedPath+save_load.saveExtension;
		Debug.Log("LoadProjectPrompt.LoadSelectedPath(): loading "+fullPath);
		save_load.LoadFile (fullPath);
	}

	// Resets highlighting of all buttons
	void ResetButtons () {
		foreach (GameObject button in fileButtons) {
			//button.GetComponent<Image>().sprite = null;
			button.GetComponent<Image>().color = new Color (1f, 1f, 1f, 0f);
		}
	}
}
