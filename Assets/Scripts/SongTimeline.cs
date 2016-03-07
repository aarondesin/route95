using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SongTimeline : MonoBehaviour {

	public static SongTimeline instance;

	static int NUM_COLUMNS = 4; // number of columns shown on timeline

	public Sprite graphic;
	public Sprite addGraphic;

	List<GameObject> columns;

	public GameObject scrollbar;

	void Start () {
		instance = this;
		columns = new List<GameObject>();
	}

	public void MakeColumns () {
		columns.Clear();

		float columnWidth = GetComponent<RectTransform>().rect.width/(float)NUM_COLUMNS;
		float columnHeight = GetComponent<RectTransform>().rect.height;

		GetComponent<RectTransform>().sizeDelta = new Vector2 (
			(MusicManager.instance.currentSong.songPieces.Count+1)*columnWidth,
			columnHeight
		);
		for (int i=0; i<MusicManager.instance.currentSong.songPieces.Count; i++) {
			GameObject column = new GameObject();
			column.name = "Column"+i;

			column.AddComponent<CanvasRenderer>();

			column.AddComponent<RectTransform>();
			column.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());
			column.GetComponent<RectTransform>().sizeDelta = new Vector2 (columnWidth, columnHeight);
			column.GetComponent<RectTransform>().localScale = new Vector3 (1f, 1f, 1f);
			column.GetComponent<RectTransform>().anchorMin = new Vector2 (0f, 0f);
			column.GetComponent<RectTransform>().anchorMax = new Vector2 (0f, 0f);
			//column.GetComponent<RectTransform>().anchoredPosition = new Vector2 (
			//	GetComponent<RectTransform>().offsetMin.x+(float)(i/NUM_COLUMNS)*columnWidth, 0f
			//);
			column.GetComponent<RectTransform>().anchoredPosition3D = new Vector3 (
				columnWidth/2f+i*columnWidth,
				//GetComponent<RectTransform>().offsetMin.x+((float)i/(float)NUM_COLUMNS)*4f*columnWidth,
				columnHeight/2f,
				0f
			);

			int num = i; // avoid pointer problems
			column.AddComponent<Button>();
			column.GetComponent<Button>().onClick.AddListener(()=>{
				MusicManager.instance.currentSong.ToggleRiff(MusicManager.instance.riffs[SongArrangeSetup.instance.selectedRiffIndex], num);
				RefreshColumn (column, MusicManager.instance.currentSong.songPieces[num]);
				Debug.Log(MusicManager.instance.currentSong.songPieces[num].ToString());
			});

			column.AddComponent<Image>();
			//column.GetComponent<Image>().sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
			column.GetComponent<Image>().sprite = graphic;
			columns.Add(column);
		}
		for (int i=0; i<columns.Count; i++) RefreshColumn (columns[i], MusicManager.instance.currentSong.songPieces[i]);

		GameObject addColumnButton = new GameObject();
		addColumnButton.name = "AddColumnButton";
		addColumnButton.AddComponent<CanvasRenderer>();

		addColumnButton.AddComponent<RectTransform>();
		addColumnButton.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());
		addColumnButton.GetComponent<RectTransform>().sizeDelta = new Vector2 (columnWidth/2f, columnHeight/2f);
		addColumnButton.GetComponent<RectTransform>().localScale = new Vector3 (1f, 1f, 1f);
		addColumnButton.GetComponent<RectTransform>().anchorMin = new Vector2 (1f, 0f);
		addColumnButton.GetComponent<RectTransform>().anchorMax = new Vector2 (1f, 0f);
		addColumnButton.GetComponent<RectTransform>().anchoredPosition3D = new Vector3 (
			-columnWidth/2f,
			//GetComponent<RectTransform>().offsetMin.x+((float)i/(float)NUM_COLUMNS)*4f*columnWidth,
			columnHeight/2f,
			0f
		);
			
		addColumnButton.AddComponent<Button>();
		addColumnButton.GetComponent<Button>().onClick.AddListener(()=>{
			MusicManager.instance.AddSongPieceToSong();
			RefreshTimeline();
		});

		addColumnButton.AddComponent<Image>();
		//column.GetComponent<Image>().sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
		addColumnButton.GetComponent<Image>().sprite = addGraphic;
		columns.Add(addColumnButton);
	}

	void RefreshTimeline () {
		while (columns.Count != 0) {
			GameObject temp = columns[0];
			Destroy(temp);
			columns.RemoveAt(0);
		}
		MakeColumns();
	}

	void RefreshColumn (GameObject column, SongPiece songpiece) {
		foreach (RectTransform child in column.GetComponent<RectTransform>()) {
			Destroy(child.gameObject);
		}
		int i = 0;
		foreach (Riff riff in songpiece.riffs[0]) {
			GameObject label = new GameObject();
			label.name = riff.name;
			label.AddComponent<RectTransform>();
			label.GetComponent<RectTransform>().SetParent(column.GetComponent<RectTransform>());
			label.GetComponent<RectTransform>().sizeDelta = new Vector2 (
				column.GetComponent<RectTransform>().rect.width,
				column.GetComponent<RectTransform>().rect.height/(float)songpiece.riffs[0].Count
			);
			label.GetComponent<RectTransform>().localScale = new Vector3 (1f, 1f, 1f);
			label.AddComponent<CanvasRenderer>();
			label.AddComponent<Text>();
			label.GetComponent<Text>().text = riff.name;
			label.GetComponent<Text>().color = Color.white;
			label.GetComponent<Text>().fontStyle = FontStyle.Bold;
			label.GetComponent<Text>().fontSize = 4;
			label.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			label.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
			label.GetComponent<RectTransform>().anchorMin = new Vector2 (0.5f, 0.0f);
			label.GetComponent<RectTransform>().anchorMax = new Vector2 (0.5f, 0.0f);
			label.GetComponent<RectTransform>().anchoredPosition3D = new Vector3 (
				0,
				(float)(songpiece.riffs[0].Count-1-i)*label.GetComponent<RectTransform>().sizeDelta.y+0.5f*label.GetComponent<RectTransform>().sizeDelta.y,
				0f
			);
			i++;
		}
	}
}
