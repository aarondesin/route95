using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SongTimeline : MonoBehaviour {

	public static SongTimeline instance;

	static int NUM_COLUMNS = 4; // number of columns shown on timeline

	public Sprite graphic;
	public Sprite addGraphic;

	List<GameObject> columns;

	public GameObject scrollbar;

	float columnWidth;
	float columnHeight;

	void Start () {
		instance = this;
		columns = new List<GameObject>();
		columnWidth = GetComponent<RectTransform>().rect.width/(float)NUM_COLUMNS;
		columnHeight = GetComponent<RectTransform>().rect.height;
	}

	public void MakeColumns () {
		Song song = MusicManager.instance.currentSong;

		columns.Clear();

		GetComponent<RectTransform>().sizeDelta = new Vector2 (
			(MusicManager.instance.currentSong.songPieceIndices.Count+1)*columnWidth,
			columnHeight
		);
			
		for (int i=0; i<MusicManager.instance.currentSong.songPieceIndices.Count; i++) {
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
			column.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3 (0f, 0f, 0f));

			int num = i; // avoid pointer problems
			column.AddComponent<Button>();
			column.GetComponent<Button>().onClick.AddListener(()=>{
				Riff riff = MusicManager.instance.currentSong.riffs[SongArrangeSetup.instance.selectedRiffIndex];
				song.ToggleRiff (riff, num);
				RefreshColumn (column, song.songPieces[song.songPieceIndices[num]]);
				//Debug.Log(MusicManager.instance.currentSong.songPieces[num].ToString());
			});

			column.AddComponent<Image>();
			//column.GetComponent<Image>().sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
			column.GetComponent<Image>().sprite = graphic;
			columns.Add(column);
		}
		for (int i=0; i<columns.Count; i++)
			RefreshColumn (columns[i], song.songPieces[song.songPieceIndices[i]]);

		GameObject addColumnButton = new GameObject();
		addColumnButton.name = "AddColumnButton";
		addColumnButton.AddComponent<CanvasRenderer>();

		addColumnButton.AddComponent<RectTransform>();
		addColumnButton.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());
		addColumnButton.GetComponent<RectTransform>().sizeDelta = new Vector2 (columnWidth/3f, columnHeight/3f);
		addColumnButton.GetComponent<RectTransform>().localScale = new Vector3 (1f, 1f, 1f);
		addColumnButton.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3 (0f, 0f, 0f));
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
			MusicManager.instance.currentSong.NewSongPiece();
			RefreshTimeline();
		});

		addColumnButton.AddComponent<Image>();
		//column.GetComponent<Image>().sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
		addColumnButton.GetComponent<Image>().sprite = addGraphic;
		columns.Add(addColumnButton);
	}

	public void RefreshTimeline () {
		while (columns.Count != 0) {
			GameObject temp = columns[0];
			Destroy(temp);
			columns.RemoveAt(0);
		}
		MakeColumns();
	}

	void RefreshColumn (GameObject column, SongPiece songpiece) {
		Song song = MusicManager.instance.currentSong;
		foreach (RectTransform child in column.GetComponent<RectTransform>()) {
			Destroy(child.gameObject);
		}
		int i = 0;
		Measure measure = song.measures[songpiece.measureIndices[0]];
		foreach (int r in measure.riffIndices) {
			Riff riff = song.riffs[r];
			GameObject label = new GameObject();
			label.name = riff.name;
			label.AddComponent<RectTransform>();
			label.GetComponent<RectTransform>().SetParent(column.GetComponent<RectTransform>());
			label.GetComponent<RectTransform>().sizeDelta = new Vector2 (
				column.GetComponent<RectTransform>().rect.width,
				column.GetComponent<RectTransform>().rect.height/(float)measure.riffIndices.Count
			);
			label.GetComponent<RectTransform>().localScale = new Vector3 (1f, 1f, 1f);
			label.AddComponent<CanvasRenderer>();
			label.AddComponent<Text>();
			label.GetComponent<Text>().text = riff.name;
			label.GetComponent<Text>().color = Color.white;
			label.GetComponent<Text>().fontStyle = FontStyle.Normal;
			label.GetComponent<Text>().fontSize = 5;
			label.GetComponent<Text>().font = GameManager.instance.font;
			label.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
			label.GetComponent<RectTransform>().anchorMin = new Vector2 (0.5f, 0.0f);
			label.GetComponent<RectTransform>().anchorMax = new Vector2 (0.5f, 0.0f);
			label.GetComponent<RectTransform>().anchoredPosition3D = new Vector3 (
				0,
				(float)(measure.riffIndices.Count-1-i)*label.GetComponent<RectTransform>().sizeDelta.y+0.5f*label.GetComponent<RectTransform>().sizeDelta.y,
				0f
			);
			label.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, 0f, 0f);
			i++;
		}
	}
}
