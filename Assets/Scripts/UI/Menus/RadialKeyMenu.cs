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

        /// <summary>
        /// Number of musical keys.
        /// </summary>
        int _numKeys;

        /// <summary>
        /// Number of musical scales.
        /// </summary>
        int _numScales;

        /// <summary>
        /// Two pi constant.
        /// </summary>
        float _TWOPI = Mathf.PI * 2f;

        #endregion
        #region Unity Callbacks

        new void Awake() {
            // Init vars
            _tr = transform as RectTransform;
            _objects = new List<GameObject>();
            _numKeys = Enum.GetValues(typeof(Key)).Length;
            _numScales = ScaleInfo.AllScales.Count;
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
            for (int i = 1; i < _numKeys; i++) { // i=1 so that it skips Key.None
                Key key = (Key)i;
                float angle = (float)i / (float)(_numKeys - 1) * _TWOPI;

                // Create button
                GameObject button = UIHelpers.MakeTextButton(key.ToString());
                RectTransform tr = button.GetComponent<RectTransform>();
                tr.SetParent(gameObject.GetComponent<RectTransform>());
                tr.SetSideWidth(_scale);
                tr.AnchorAtPoint(0.5f, 0.5f);
                tr.anchoredPosition3D = new Vector3(
                    _radius * Mathf.Cos(angle), 
                    _radius * Mathf.Sin(angle), 
                    0f);
                tr.ResetScaleRot();

                // Set button text
                Text text = button.GetComponentInChildren<Text>();
                if (key.ToString().Contains("Sharp"))
                    text.text = key.ToString()[0] + "#";
                text.font = UIManager.Instance.Font;
                text.fontSize = (int)(_scale / 2f);
                text.color = _GRAY;

                // Set button image
                Image img = button.GetComponent<Image>();
                img.sprite = UIManager.Instance.CircleIcon;
                img.color = _GRAY;

                // Highlight if selected key
                if (key == MusicManager.Instance.CurrentSong.Key) {
                    text.color = Color.white;
                    img.color = Color.white;

                    GameObject hl = UIHelpers.MakeImage(
                        key.ToString() + "_SelectedHighlight");
                    tr = hl.GetComponent<RectTransform>();
                    tr.SetParent(button.GetComponent<RectTransform>());
                    tr.sizeDelta = ((RectTransform)(tr.parent)).sizeDelta;
                    tr.AnchorAtPoint(0.5f, 0.5f);
                    tr.anchoredPosition3D = Vector3.zero;
                    tr.ResetScaleRot();

                    img = hl.GetComponent<Image>();
                    img.sprite = UIManager.Instance.CircleIcon;
                    img.color = Color.white;
                }

                // Set button functionality
                button.GetComponent<Button>().onClick.AddListener(delegate {
                    UIManager.Instance.PlayMenuClickSound();
                    MusicManager.Instance.CurrentSong.Key = key;
                    Refresh();
                });

                // Set button show/hide
                //ShowHide sh = button.AddComponent<ShowHide>();
                GameObject highlight = UIHelpers.MakeImage(
                    key.ToString() + "_Highlight");
                tr = highlight.GetComponent<RectTransform>();
                tr.SetParent(button.GetComponent<RectTransform>());
                tr.sizeDelta = ((RectTransform)(tr.parent)).sizeDelta;
                tr.AnchorAtPoint(0.5f, 0.5f);
                tr.ResetScaleRot();
                tr.anchoredPosition3D = Vector3.zero;
                highlight.GetComponent<Image>().sprite = 
                    UIManager.Instance.PercussionVolumeIcon;
                highlight.GetComponent<Image>().color = Color.white;

                //sh.objects = new List<GameObject>();
                //sh.objects.Add(highlight);

                highlight.SetActive(false);

                _objects.Add(button);
            }

            // Layer two -- scales
            _radius *= _scaleFactor;
            _scale *= _scaleFactor;
            for (int i = 0; i < _numScales; i++) {
                ScaleInfo scalei = ScaleInfo.AllScales[i];
                float angle = (float)i / (float)_numScales * _TWOPI;

                // Make scale button
                GameObject button = UIHelpers.MakeTextButton(scalei.Name);
                RectTransform tr = button.GetComponent<RectTransform>();
                tr.SetParent(gameObject.GetComponent<RectTransform>());
                tr.SetSideWidth(_scale);
                tr.AnchorAtPoint(0.5f, 0.5f);
                tr.ResetScaleRot();
                tr.anchoredPosition3D = new Vector3(
                    _radius * Mathf.Cos(angle), 
                    _radius * Mathf.Sin(angle), 
                    0f);

                // Set button text
                Text text = button.GetComponentInChildren<Text>();
                text.font = UIManager.Instance.Font;
                text.fontSize = (int)(_baseScale / 6f);
                text.color = _GRAY;

                // Set button image
                Image img = button.GetComponent<Image>();
                img.sprite = UIManager.Instance.CircleIcon;
                img.color = _GRAY;

                // Set highlighted button
                if (i == MusicManager.Instance.CurrentSong.Scale) {
                    text.color = Color.white;
                    img.color = Color.white;
                }

                // Set button functionality
                button.GetComponent<Button>().onClick.AddListener(delegate {
                    UIManager.Instance.PlayMenuClickSound();
                    MusicManager.Instance.CurrentSong.Scale = scalei.ScaleIndex;
                    Refresh();
                });

                // Set show/hide
                //ShowHide sh = button.AddComponent<ShowHide>();
                GameObject highlight = UIHelpers.MakeImage(
                    scalei.Name + "_Highlight");
                tr = highlight.GetComponent<RectTransform>();
                tr.SetParent(button.GetComponent<RectTransform>());
                tr.sizeDelta = ((RectTransform)(tr.parent)).sizeDelta;
                tr.AnchorAtPoint(0.5f, 0.5f);
                tr.ResetScaleRot();
                tr.anchoredPosition3D = Vector3.zero;
                highlight.GetComponent<Image>().sprite = 
                    UIManager.Instance.PercussionVolumeIcon;
                highlight.GetComponent<Image>().color = Color.white;

                //sh.objects = new List<GameObject>();
                //sh.objects.Add(highlight);

                highlight.SetActive(false);

                _objects.Add(button);

            }

            GameObject confirmButton = 
                KeySelectConfirmButton.Instance.gameObject;

            // Confirm button
            if (MusicManager.Instance.CurrentSong.Key != Key.None &&
                MusicManager.Instance.CurrentSong.Scale != -1) {
                confirmButton.GetComponent<Button>().interactable = true;
                confirmButton.SetActive(true);
            }
            else confirmButton.SetActive(false);
        }

        #endregion
    }
}
