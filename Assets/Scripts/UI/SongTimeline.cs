using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SongTimeline : MonoBehaviour {

	#region SongTimeline Vars

	public static SongTimeline instance;

	static int NUM_COLUMNS = 4; // number of columns shown on timeline

	public Sprite graphic;
	public Sprite addGraphic;

	List<GameObject> columns;

	public GameObject scrollbar;

	float columnWidth;
	float columnHeight;

	#endregion
	#region Unity Callbacks

	void Awake () {
		instance = this;
		columns = new List<GameObject>();
		columnWidth = GetComponent<RectTransform>().rect.width/(float)NUM_COLUMNS;
		columnHeight = GetComponent<RectTransform>().rect.height;
	}

	#endregion
	#region SongTimeline Methods

	public void MakeColumns () {
		Song song = MusicManager.instance.currentSong;

		columns.Clear();

		GetComponent<RectTransform>().sizeDelta = new Vector2 (
			(MusicManager.instance.currentSong.songPieceIndices.Count+1)*columnWidth,
			columnHeight
		);
			
		for (int i=0; i<MusicManager.instance.currentSong.songPieceIndices.Count; i++) {
			GameObject column = new GameObject("Column"+i,
				typeof (RectTransform),
				typeof (CanvasRenderer),
				typeof (Button),
				typeof (Image)
			);

			RectTransform tr = column.RectTransform();
			column.SetParent(gameObject.RectTransform());
			tr.sizeDelta = new Vector2 (columnWidth, columnHeight);
			tr.localScale = new Vector3 (1f, 1f, 1f);
			tr.AnchorAtPoint (0f, 0f);
			tr.anchoredPosition3D = new Vector3 (columnWidth/2f+i*columnWidth, columnHeight/2f, 0f);
			tr.ResetScaleRot();

			int num = i; // avoid pointer problems
			column.Button().onClick.AddListener(()=>{
				Riff riff = MusicManager.instance.currentSong.riffs[SongArrangeSetup.instance.selectedRiffIndex];
				song.ToggleRiff (riff, num);
				RefreshColumn (column, song.songPieces[song.songPieceIndices[num]]);
			});

			column.Image().sprite = graphic;
			columns.Add(column);
		}

		for (int i=0; i<columns.Count; i++)
			RefreshColumn (columns[i], song.songPieces[song.songPieceIndices[i]]);

		GameObject addColumnButton = new GameObject("AddColumnButton",
			typeof (RectTransform),
			typeof (CanvasRenderer),
			typeof (Button),
			typeof (Image)
		);

		RectTransform atr = addColumnButton.RectTransform();
		addColumnButton.SetParent(gameObject.RectTransform());
		atr.sizeDelta = new Vector2 (columnWidth/3f, columnHeight/3f);
		atr.AnchorAtPoint (1f, 0f);
		atr.anchoredPosition3D = new Vector3 (-columnWidth/2f, columnHeight/2f, 0f);
		atr.ResetScaleRot();
			
		addColumnButton.Button().onClick.AddListener(()=>{
			MusicManager.instance.currentSong.NewSongPiece();
			RefreshTimeline();
		});

		addColumnButton.Image().sprite = addGraphic;
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
			RectTransform column_tr = column.RectTransform();

			GameObject icon = UIHelpers.MakeImage (riff.name + "Icon", riff.instrument.icon);
			RectTransform icon_tr = icon.RectTransform();
			icon_tr.SetParent (column_tr);
			icon_tr.sizeDelta = new Vector2 (column_tr.rect.height/9f, column_tr.rect.height/9f);
			icon_tr.AnchorAtPoint (0f, 0.5f);
			icon_tr.anchoredPosition3D = new Vector3 (icon_tr.sizeDelta.x/2f, (float)(8-i)*icon_tr.sizeDelta.y, 0f);
			icon_tr.ResetScaleRot();

			GameObject label = UIHelpers.MakeText (riff.name);
			RectTransform label_tr = label.RectTransform();
			label.SetParent(column_tr);
			label_tr.sizeDelta = new Vector2 (column_tr.rect.width, column_tr.rect.height/(float)measure.riffIndices.Count);
			label_tr.AnchorAtPoint (0.5f, 0f);
			label_tr.anchoredPosition3D = new Vector3 (0f, (float)(measure.riffIndices.Count-1-i)*label_tr.sizeDelta.y+0.5f*label_tr.sizeDelta.y, 0f);
			label_tr.ResetScaleRot();

			Text label_text = label.Text();
			label_text.text = riff.name;
			label_text.color = Color.white;
			label_text.fontStyle = FontStyle.Normal;
			label_text.fontSize = 6;
			label_text.font = GameManager.instance.font;
			label_text.alignment = TextAnchor.MiddleCenter;

			

			i++;
		}
	}

	public void SetInteractable (bool inter) {
		foreach (Selectable selectable in GetComponentsInChildren<Selectable>())
			selectable.interactable = inter;
	}

	#endregion
}
