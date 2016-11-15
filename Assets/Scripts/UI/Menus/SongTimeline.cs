using Route95.Core;
using Route95.Music;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Route95.UI {

    /// <summary>
    /// Class to handle the song arranger timeline.
    /// </summary>
    public class SongTimeline : MonoBehaviour {

        #region SongTimeline Vars

        public static SongTimeline Instance; // Quick reference to the song timeline
        RectTransform timeline_tr;           // Transform of the timeline

        static int NUM_COLUMNS = 4;          // Number of columns shown on timeline

        [Tooltip("Sprite to use for columns.")]
        public Sprite graphic;

        List<GameObject> columns;            // List of active columns

        [Tooltip("Reference to scrollbar.")]
        public GameObject scrollbar;

        float columnWidth;                   // Calculated width of columns
        float columnHeight;                  // Calculated height of columns

        #endregion
        #region Unity Callbacks

        void Awake() {
            Instance = this;

            // Init lists
            columns = new List<GameObject>();

            // Init vars
            timeline_tr = gameObject.GetComponent<RectTransform>();
            columnWidth = timeline_tr.rect.width / (float)NUM_COLUMNS;
            columnHeight = ((RectTransform)timeline_tr.parent).rect.height;
        }

		void Start () {
			UIManager.Instance.onSwitchToSongArrangeMenu.AddListener(RefreshTimeline);
		}

        #endregion
        #region SongTimeline Methods

        /// <summary>
        /// Inits all song piece columns on the timeline.
        /// </summary>
        public void MakeColumns() {
            Song song = MusicManager.Instance.CurrentSong;
            int numSongPieces = MusicManager.Instance.CurrentSong.SongPieceIndices.Count;

            // Clear current columns
            columns.Clear();

            // Resize timeline
            timeline_tr.sizeDelta = new Vector2(
                (numSongPieces + 1) * columnWidth, columnHeight
            );

            // Make columns
            for (int i = 0; i < numSongPieces; i++) {
                GameObject column = new GameObject("Column" + i,
                    typeof(RectTransform),
                    typeof(CanvasRenderer),
                    typeof(Button),
                    typeof(Image)
                );

                RectTransform tr = column.GetComponent<RectTransform>();
                column.SetParent(timeline_tr);
                tr.sizeDelta = new Vector2(columnWidth, columnHeight);
                tr.AnchorAtPoint(0f, 0f);
                tr.anchoredPosition3D = new Vector3((i + 0.5f) * columnWidth, columnHeight / 2f, 0f);
                tr.ResetScaleRot();

                int num = i; // avoid pointer problems

                // Setup column button properties
                column.GetComponent<Button>().onClick.AddListener(() => {
                    SongArrangeMenu.Instance.UpdateValue();
                    Riff riff = MusicManager.Instance.CurrentSong.Riffs[SongArrangeMenu.Instance.SelectedRiffIndex];
                    song.ToggleRiff(riff, num);
                    RefreshColumn(column, song.SongPieces[song.SongPieceIndices[num]]);
                });

                column.GetComponent<Image>().sprite = UIManager.Instance.FillSprite;
                Color color = Color.white;
                color.a = (i % 2 == 1) ? 0.2f : 0.4f;
                column.GetComponent<Image>().color = color;
                columns.Add(column);
            }

            // Refresh all columns
            for (int i = 0; i < columns.Count; i++)
                RefreshColumn(columns[i], song.SongPieces[song.SongPieceIndices[i]]);

            // Create add columns button
            GameObject addColumnButton = new GameObject("AddColumnButton",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Button),
                typeof(Image)
            );

            RectTransform atr = addColumnButton.GetComponent<RectTransform>();
            addColumnButton.SetParent(gameObject.GetComponent<RectTransform>());
            float width = Mathf.Min(columnWidth, columnHeight) / 3f;
            atr.SetSideWidth(width);
            atr.AnchorAtPoint(1f, 0f);
            atr.anchoredPosition3D = new Vector3(-columnWidth / 2f, columnHeight / 2f, 0f);
            atr.ResetScaleRot();

            addColumnButton.GetComponent<Button>().onClick.AddListener(() => {
                MusicManager.Instance.CurrentSong.NewSongPiece();
                RefreshTimeline();
            });

            addColumnButton.GetComponent<Image>().sprite = UIManager.Instance.AddIcon;
            columns.Add(addColumnButton);
        }

        /// <summary>
        /// Clears and remakes all columns.
        /// </summary>
        public void RefreshTimeline() {
            while (columns.Count != 0) {
                GameObject temp = columns[0];
                Destroy(temp);
                columns.RemoveAt(0);
            }
            MakeColumns();
        }

        /// <summary>
        /// Refreshes a column.
        /// </summary>
        /// <param name="column">Column to refresh.</param>
        /// <param name="songpiece">Song piece to use for column.</param>
        void RefreshColumn(GameObject column, SongPiece songpiece) {
            Song song = MusicManager.Instance.CurrentSong;
            RectTransform column_tr = column.GetComponent<RectTransform>();

            // Clear all chuldren from column
            foreach (RectTransform child in column_tr)
                Destroy(child.gameObject);

            int i = 0;
            float height = columnHeight / Instrument.AllInstruments.Count;
            Measure measure = song.Measures[songpiece.MeasureIndices[0]];

            foreach (int r in measure.RiffIndices) {
                Riff riff = song.Riffs[r];

                float y = (float)(Instrument.AllInstruments.Count - 1 - i);

                // Riff name label
                GameObject label = UIHelpers.MakeText(riff.Name);
                RectTransform label_tr = label.GetComponent<RectTransform>();
                label.SetParent(column_tr);
                label_tr.sizeDelta = new Vector2(columnWidth, height);
                label_tr.AnchorAtPoint(0f, 0f);
                label_tr.anchoredPosition3D = new Vector3(2f * height, height * y, 0f);
                label_tr.ResetScaleRot();

                // Instrument icon
                GameObject icon = UIHelpers.MakeImage(riff.Name + "Icon", riff.Instrument.Icon);
                RectTransform icon_tr = icon.GetComponent<RectTransform>();
                icon_tr.SetParent(column_tr);
                icon_tr.SetSideWidth(height);
                icon_tr.AnchorAtPoint(0f, 0f);
                icon_tr.anchoredPosition3D = new Vector3(height, height * y, 0f);
                icon_tr.ResetScaleRot();

                Text label_text = label.GetComponent<Text>();
                label_text.text = riff.Name;
                label_text.color = Color.white;
                label_text.fontStyle = FontStyle.Normal;
                label_text.fontSize = 4;
                label_text.font = UIManager.Instance.Font;
                label_text.alignment = TextAnchor.MiddleCenter;

                i++;
            }
        }

        /// <summary>
        /// Toggles interactivity of the timeline.
        /// </summary>
        /// <param name="inter"></param>
        public void SetInteractable(bool inter) {
            foreach (Selectable selectable in GetComponentsInChildren<Selectable>())
                selectable.interactable = inter;
        }

        #endregion
    }
}
