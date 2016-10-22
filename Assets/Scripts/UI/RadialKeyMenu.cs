// RadialKeyMenu.cs
// ©2016 Team 95

using Route95.Core;
using Route95.Music;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Route95.UI {

    /// <summary>
    /// Class to handle the radial key selection menu.
    /// </summary>
    public class RadialKeyMenu : MenuBase<RadialKeyMenu> {

        #region RadialKeyMenu Vars

        /// <summary>
        /// All active buttons.
        /// </summary>
        List<GameObject> _objects;

        [Tooltip("Base button scale.")]
        [SerializeField]
        float _baseScale;

        [Tooltip("Base button scale factor.")]
        [SerializeField]
        float _scaleFactor;

        /// <summary>
        /// Current button placement radius.
        /// </summary>
        float _radius;
        /// <summary>
        /// Current button scale.
        /// </summary>
        float _scale;

        /// <summary>
        /// Gray color constant.
        /// </summary>
        Color _GRAY = new Color(0.8f, 0.8f, 0.8f, 0.8f);

        /// <summary>
        /// Parent RectTransform;
        /// </summary>
        RectTransform _tr;

        #endregion
        #region Unity Callbacks

        new void Awake() {
            // Init vars
            _tr = transform as RectTransform();
            _objects = new List<GameObject>();
        }

        void Start() {
            Refresh();
        }

        #endregion
        #region RadialKeyMenu Vars

        /// <summary>
        /// Refreshes the radial key menu.
        /// </summary>
        public void Refresh() {

            // Clear old buttons
            foreach (GameObject obj in _objects) Destroy(obj);
            _objects.Clear();

            // Init radius and scale
            _radius = (_tr.rect.width - _baseScale) / 2f;
            _scale = _baseScale;

            // Layer one -- keys
            int numKeys = Enum.GetValues(typeof(Key)).Length;
            for (int i = 1; i < numKeys; i++) { // i=1 so that it skips Key.None
                Key key = (Key)i;
                float angle = (float)i / (float)(numKeys - 1) * 2f * Mathf.PI;

                // Create button
                GameObject button = UIHelpers.MakeTextButton(key.ToString());
                RectTransform tr = button.RectTransform();
                tr.SetParent(gameObject.RectTransform());
                tr.SetSideWidth(scale);
                tr.AnchorAtPoint(0.5f, 0.5f);
                tr.anchoredPosition3D = new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0f);
                tr.ResetScaleRot();

                // Set button text
                Text text = button.GetComponentInChildren<Text>();
                if (key.ToString().Contains("Sharp")) text.text = key.ToString()[0] + "#";
                text.font = GameManager.Instance.font;
                text.fontSize = (int)(scale / 2f);
                text.color = gray;

                // Set button image
                Image img = button.Image();
                img.sprite = GameManager.Instance.circleIcon;
                img.color = gray;

                // Highlight if selected key
                if (key == MusicManager.Instance.CurrentSong.Key) {
                    text.color = Color.white;
                    img.color = Color.white;

                    GameObject hl = UIHelpers.MakeImage(key.ToString() + "_SelectedHighlight");
                    tr = hl.RectTransform();
                    tr.SetParent(button.RectTransform());
                    tr.sizeDelta = ((RectTransform)(tr.parent)).sizeDelta;
                    tr.AnchorAtPoint(0.5f, 0.5f);
                    tr.anchoredPosition3D = Vector3.zero;
                    tr.ResetScaleRot();

                    img = hl.Image();
                    img.sprite = GameManager.Instance.circleIcon;
                    img.color = Color.white;
                }

                // Set button functionality
                button.Button().onClick.AddListener(delegate {
                    UIManager.Instance.PlayMenuClickSound();
                    MusicManager.Instance.CurrentSong.key = key;
                    Refresh();
                });

                // Set button show/hide
                ShowHide sh = button.AddComponent<ShowHide>();
                GameObject highlight = UIHelpers.MakeImage(key.ToString() + "_Highlight");
                tr = highlight.RectTransform();
                tr.SetParent(button.RectTransform());
                tr.sizeDelta = ((RectTransform)(tr.parent)).sizeDelta;
                tr.AnchorAtPoint(0.5f, 0.5f);
                tr.ResetScaleRot();
                tr.anchoredPosition3D = Vector3.zero;
                highlight.Image().sprite = GameManager.Instance.volumeIcon;
                highlight.Image().color = Color.white;

                sh.objects = new List<GameObject>();
                sh.objects.Add(highlight);

                highlight.SetActive(false);

                objects.Add(button);
            }

            // Layer two -- scales
            radius *= scaleFactor;
            scale *= scaleFactor;
            int numScales = ScaleInfo.AllScales.Count;
            for (int i = 0; i < numScales; i++) {
                ScaleInfo scalei = ScaleInfo.AllScales[i];
                float angle = (float)i / (float)numScales * 2f * Mathf.PI;

                // Make scale button
                GameObject button = UIHelpers.MakeTextButton(scalei.name);
                RectTransform tr = button.RectTransform();
                tr.SetParent(gameObject.RectTransform());
                tr.SetSideWidth(scale);
                tr.AnchorAtPoint(0.5f, 0.5f);
                tr.ResetScaleRot();
                tr.anchoredPosition3D = new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0f);

                // Set button text
                Text text = button.GetComponentInChildren<Text>();
                text.font = GameManager.Instance.font;
                text.fontSize = (int)(baseScale / 6f);
                text.color = gray;

                // Set button image
                Image img = button.Image();
                img.sprite = GameManager.Instance.circleIcon;
                img.color = gray;

                // Set highlighted button
                if (i == MusicManager.Instance.currentSong.scale) {
                    text.color = Color.white;
                    img.color = Color.white;
                }

                // Set button functionality
                button.Button().onClick.AddListener(delegate {
                    GameManager.Instance.MenuClick();
                    MusicManager.Instance.currentSong.scale = scalei.scaleIndex;
                    Refresh();
                });

                // Set show/hide
                ShowHide sh = button.AddComponent<ShowHide>();
                GameObject highlight = UIHelpers.MakeImage(scalei.name + "_Highlight");
                tr = highlight.RectTransform();
                tr.SetParent(button.RectTransform());
                tr.sizeDelta = ((RectTransform)(tr.parent)).sizeDelta;
                tr.AnchorAtPoint(0.5f, 0.5f);
                tr.ResetScaleRot();
                tr.anchoredPosition3D = Vector3.zero;
                highlight.Image().sprite = GameManager.Instance.volumeIcon;
                highlight.Image().color = Color.white;

                sh.objects = new List<GameObject>();
                sh.objects.Add(highlight);

                highlight.SetActive(false);

                objects.Add(button);

            }

            // Confirm button
            if (MusicManager.Instance.currentSong.key != Key.None &&
                MusicManager.Instance.currentSong.scale != -1) {
                confirmButton.Button().interactable = true;
                confirmButton.SetActive(true);
            }
            else confirmButton.SetActive(false);
        }

        #endregion
    }
}
