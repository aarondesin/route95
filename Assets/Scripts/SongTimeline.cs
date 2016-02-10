using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

public class SongTimeline : MonoBehaviour {

	static int NUM_COLUMNS = 4; // number of columns shown on timeline

	void Start () {
		MakeColumns();
	}

	void MakeColumns () {
		for (int i=0; i<NUM_COLUMNS; i++) {
			GameObject column = new GameObject();
			column.name = "Column"+i;

			column.AddComponent<CanvasRenderer>();

			float columnWidth = GetComponent<RectTransform>().rect.width/(float)NUM_COLUMNS;
			float columnHeight = GetComponent<RectTransform>().rect.height;
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
				columnWidth/2f+(float)i*GetComponent<RectTransform>().sizeDelta.x/(float)NUM_COLUMNS,
				//GetComponent<RectTransform>().offsetMin.x+((float)i/(float)NUM_COLUMNS)*4f*columnWidth,
				columnHeight/2f,
				0f
			);

			int num = i; // avoid pointer problems
			column.AddComponent<Button>();
			column.GetComponent<Button>().onClick.AddListener(()=>{
				MusicManager.currentSong.ToggleRiff(MusicManager.instance.riffs[SongArrangeSetup.instance.selectedRiffIndex], num);
				RefreshColumn (column, MusicManager.currentSong.songPieces[num]);
			});

			column.AddComponent<Image>();
			column.GetComponent<Image>().sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
		}
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
			label.GetComponent<RectTransform>().sizeDelta = new Vector2 (column.GetComponent<RectTransform>().rect.width,
				column.GetComponent<RectTransform>().rect.height/4f
			);
			label.GetComponent<RectTransform>().localScale = new Vector3 (1f, 1f, 1f);
			label.AddComponent<CanvasRenderer>();
			label.AddComponent<Text>();
			label.GetComponent<Text>().text = riff.name;
			label.GetComponent<Text>().color = Color.black;
			label.GetComponent<Text>().fontSize = 3;
			label.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			label.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
			label.GetComponent<RectTransform>().anchorMin = new Vector2 (0f, 0f);
			label.GetComponent<RectTransform>().anchorMax = new Vector2 (0f, 0f);
			label.GetComponent<RectTransform>().anchoredPosition3D = new Vector3 (
				label.GetComponent<RectTransform>().rect.width/2f,
				(float)(4-i)*label.GetComponent<RectTransform>().rect.height/4f,
				0f
			);
			i++;
		}
	}
}
