// PlaylistBrowser.cs
// ©2016 Team 95

using Route95.Core;
using Route95.Music;
using Route95.World;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Route95.UI {

    /// <summary>
    /// Class to handle the playlist menu.
    /// </summary>
    public class PlaylistBrowser : MenuBase<PlaylistBrowser> {

        #region PlaylistBrowser Vars

        /// <summary>
        /// List of sounds to use when writing playlist name.
        /// </summary>
        [Tooltip("List of sounds to use when writing playlist name.")]
        [SerializeField]
        List<AudioClip> _scribbleSounds;

		/// <summary>
		/// Playlist name input field.
		/// </summary>
        InputField _projectNameInputField;

		/// <summary>
		/// Panel to use for playlist entries.
		/// </summary>
        RectTransform _playlistTR;

		/// <summary>
		/// Base height of buttons.
		/// </summary>
		[SerializeField]
        float _buttonHeight;

		/// <summary>
		/// Horizontal padding between buttons.
		/// </summary>
		[SerializeField]
        float _horizontalPadding;

		/// <summary>
		/// Vertical padding between buttons.
		/// </summary>
		[SerializeField]
        float _verticalPadding;

		/// <summary>
		/// Scale of edit/remove icons compared to button height.
		/// </summary>
		[SerializeField]
        float _iconScale;

		/// <summary>
		/// Base font size.
		/// </summary>
		[SerializeField]
        int _fontSize;

		/// <summary>
		/// List of active buttons.
		/// </summary>
        List<GameObject> _listings;

        #endregion
        #region Unity Callbacks

        new void Awake() {
            base.Awake();

			// Init vars
			_projectNameInputField = GetComponentInChildren<InputField>();

            // Init lists
            _listings = new List<GameObject>();

            // Add listeners to playlist name input field
            _projectNameInputField.onEndEdit.AddListener(delegate {
                MusicManager.Instance.CurrentProject.SetName(_projectNameInputField.text);

                // "devnull" -> free camera mode
                if (_projectNameInputField.text == "devnull") CameraControl.Instance.StartFreeMode();
            });
        }

        #endregion
        #region PlaylistBrowser Callbacks

        /// <summary>
        /// Plays a random pen scribble sound.
        /// </summary>
        public void PlayScribbleSound() {
            MusicManager.PlayMenuSound(_scribbleSounds.Random(), 0.75f);
        }

        /// <summary>
        /// Gets playlist name from MM.
        /// </summary>
        public void RefreshName() {
            _projectNameInputField.text = MusicManager.Instance.CurrentProject.Name;
        }

        /// <summary>
        /// Refreshes the entire playlist menu.
        /// </summary>
        public void Refresh() {

            // Clear all listings and buttons
            foreach (GameObject listing in _listings) Destroy(listing);
            _listings.Clear();


            // Resize browser panel
            int numSongs = MusicManager.Instance.CurrentProject.Songs.Count;
            _playlistTR.sizeDelta = new Vector2(
                _playlistTR.sizeDelta.x,
                _verticalPadding * (numSongs + 2) + _buttonHeight * (numSongs + 1)
            );

            // Create song listings
            for (int i = 0; i < numSongs; i++) {
                int num = i;
                Song song = MusicManager.Instance.CurrentProject.Songs[num];

                // Create listing for song
                GameObject listing = UIHelpers.MakeButton(song.Name);
                _listings.Add(listing);

                RectTransform listing_tr = listing.GetComponent<RectTransform>();
                listing_tr.SetParent(_playlistTR);
                listing_tr.sizeDelta = new Vector2(_playlistTR.rect.width, _buttonHeight);
                listing_tr.AnchorAtPoint(0f, 1f);
                listing_tr.anchoredPosition3D = new Vector3(
                    _horizontalPadding + listing_tr.sizeDelta.x / 2f,
                    -_verticalPadding - listing_tr.sizeDelta.y / 2f - (_verticalPadding + listing_tr.sizeDelta.y) * (float)i,
                    0f
                );
                listing_tr.ResetScaleRot();

                Image listing_img = listing.GetComponent<Image>();
                listing_img.sprite = UIManager.Instance.FillSprite;
                listing_img.color = new Color(0f, 0f, 1f, 0f);

                /*ShowHide listing_sh = listing.ShowHide();

                // Create background for listing
                GameObject listing_bg = UIHelpers.MakeImage(song.Name + "_bg");
                RectTransform listing_bg_tr = listing_bg.GetComponent<RectTransform>();
                listing_bg_tr.SetParent(playlist);
                listing_bg_tr.sizeDelta = new Vector2(listing_tr.sizeDelta.x - 2f * horizontalPadding, listing_tr.sizeDelta.y);
                listing_bg_tr.AnchorAtPoint(0f, 1f);
                listing_bg_tr.anchoredPosition3D = new Vector3(
                    horizontalPadding + listing_tr.sizeDelta.x / 2f,
                    listing_tr.anchoredPosition3D.y,
                    0f
                );
                listing_bg_tr.SetSiblingIndex(-1);
                listing_bg_tr.ResetScaleRot();

                Image listing_bg_img = listing_bg.GetComponent<Image>();
                listing_bg_img.raycastTarget = false;
                listing_bg_img.sprite = UIManager.Instance.FillSprite;
                listing_bg_img.color = new Color(1f, 1f, 1f, 0.0f);

                listings.Add(listing_bg);*/

                // Create song text
                GameObject listing_text = UIHelpers.MakeText(song.Name + "_text");

                RectTransform listing_text_tr = listing_text.GetComponent<RectTransform>();
                listing_text_tr.SetParent(listing_tr);
                listing_text_tr.sizeDelta = listing_tr.sizeDelta;
                listing_text_tr.AnchorAtPoint(0.5f, 0.5f);
                listing_text_tr.anchoredPosition3D = new Vector3(_horizontalPadding * 1.5f + listing_tr.sizeDelta.y, 0f, 0f);
                listing_text_tr.ResetScaleRot();

                Text listing_text_txt = listing_text.GetComponent<Text>();
                listing_text_txt.text = (i + 1).ToString() + ". " + song.Name;
                listing_text_txt.fontSize = _fontSize;
                listing_text_txt.color = Color.black;
                listing_text_txt.font = UIManager.Instance.HandwrittenFont;
                listing_text_txt.alignment = TextAnchor.MiddleLeft;


                // Create remove song button
                GameObject listing_remove = UIHelpers.MakeButton(song.Name + "_remove");

                RectTransform listing_remove_tr = listing_remove.GetComponent<RectTransform>();
                listing_remove_tr.SetParent(listing_tr);
                listing_remove_tr.sizeDelta = new Vector2(_iconScale * listing_tr.sizeDelta.y, _iconScale * listing_tr.sizeDelta.y);
                listing_remove_tr.AnchorAtPoint(1f, 0.5f);
                listing_remove_tr.anchoredPosition3D = new Vector3(-_horizontalPadding - listing_remove_tr.sizeDelta.x, 0f, 0f);
                listing_remove_tr.ResetScaleRot();

                Image listing_remove_img = listing_remove.GetComponent<Image>();
                listing_remove_img.color = Color.black;
                listing_remove_img.sprite = UIManager.Instance.RemoveIcon;

                Button listing_remove_button = listing_remove.GetComponent<Button>();
                listing_remove_button.onClick.AddListener(() => {
                    UIManager.Instance.PlayMenuClickSound();
                    MusicManager.Instance.CurrentProject.RemoveSong(num);
                    Refresh();
                });

                listing_remove.AddComponent<Tooltippable>().Message = "Remove \"" + song.Name + "\".";

                // Create edit song button
                GameObject listing_edit = UIHelpers.MakeButton(song.Name + "_edit");

                RectTransform listing_edit_tr = listing_edit.GetComponent<RectTransform>();
                listing_edit_tr.SetParent(listing_tr);
                listing_edit_tr.sizeDelta = new Vector2(_iconScale * listing_tr.sizeDelta.y, _iconScale * listing_tr.sizeDelta.y);
                listing_edit_tr.AnchorAtPoint(1f, 0.5f);
                listing_edit_tr.anchoredPosition3D = new Vector3(
                    listing_remove_tr.anchoredPosition3D.x - _horizontalPadding - listing_edit_tr.sizeDelta.x,
                    0f,
                    0f
                );
                listing_edit_tr.ResetScaleRot();

                Image listing_edit_img = listing_edit.GetComponent<Image>();
                listing_edit_img.color = Color.black;
                listing_edit_img.sprite = UIManager.Instance.EditIcon;

                Button listing_edit_button = listing_edit.GetComponent<Button>();
                listing_edit_button.onClick.AddListener(() => {
                    UIManager.Instance.PlayMenuClickSound();
                    MusicManager.Instance.CurrentSong = song;
                    if (song.Key == Key.None || song.Scale == -1)
                        UIManager.Instance.GoToKeySelectMenu();
                    else UIManager.Instance.GoToSongArrangeMenu();
                });

                /*listing_sh.objects = new List<GameObject>() {
                listing_edit,
                listing_remove,
                listing_bg
            };*/

                listing_edit.AddComponent<Tooltippable>().Message = "Edit \"" + song.Name + "\".";

                // Create move song up button if not at top
                if (num > 0) {
                    Song prevSong = MusicManager.Instance.CurrentProject.Songs[i - 1];
                    GameObject listing_up = UIHelpers.MakeButton(song.Name + "_up");

                    RectTransform listing_up_tr = listing_up.GetComponent<RectTransform>();
                    listing_up_tr.SetParent(listing_tr);
                    listing_up_tr.sizeDelta = new Vector2(listing_tr.sizeDelta.y * _iconScale / 2f, listing_tr.sizeDelta.y * _iconScale / 2f);
                    listing_up_tr.ResetScaleRot();
                    listing_up_tr.localRotation = Quaternion.Euler(0f, 0f, 90f);
                    listing_up_tr.AnchorAtPoint(0f, 0.5f);
                    listing_up_tr.anchoredPosition3D = new Vector3(
                        _horizontalPadding + listing_up_tr.sizeDelta.x / 2f,
                        listing_up_tr.sizeDelta.y / 2f + _verticalPadding / 2f,
                        0f
                    );

                    Image listing_up_img = listing_up.GetComponent<Image>();
                    listing_up_img.color = Color.black;
                    listing_up_img.sprite = UIManager.Instance.ArrowIcon;

                    Button listing_up_button = listing_up.GetComponent<Button>();
                    listing_up_button.onClick.AddListener(() => {
                        UIManager.Instance.PlayMenuClickSound();
                        Song temp = song;
                        MusicManager.Instance.CurrentProject.Songs[num] = prevSong;
                        MusicManager.Instance.CurrentProject.Songs[num - 1] = temp;
                        Refresh();
                    });

                    //listing_sh.objects.Add(listing_up);
                }

                // Create move song down button if not at bottom
                if (num < MusicManager.Instance.CurrentProject.Songs.Count - 1) {
                    Song nextSong = MusicManager.Instance.CurrentProject.Songs[i + 1];
                    GameObject listing_down = UIHelpers.MakeButton(song.Name + "_down");

                    RectTransform listing_down_tr = listing_down.GetComponent<RectTransform>();
                    listing_down_tr.SetParent(listing_tr);
                    listing_down_tr.sizeDelta = new Vector2(listing_tr.sizeDelta.y * _iconScale / 2f, listing_tr.sizeDelta.y * _iconScale / 2f);
                    listing_down_tr.ResetScaleRot();
                    listing_down_tr.localRotation = Quaternion.Euler(0f, 0f, -90f);
                    listing_down_tr.AnchorAtPoint(0f, 0.5f);
                    listing_down_tr.anchoredPosition3D = new Vector3(
                        _horizontalPadding + listing_down_tr.sizeDelta.x / 2f,
                        -listing_down_tr.sizeDelta.y / 2f - _verticalPadding / 2f,
                        0f
                    );

                    Image listing_down_img = listing_down.GetComponent<Image>();
                    listing_down_img.color = Color.black;
                    listing_down_img.sprite = UIManager.Instance.ArrowIcon;

                    Button listing_down_button = listing_down.GetComponent<Button>();
                    listing_down_button.onClick.AddListener(() => {
                        UIManager.Instance.PlayMenuClickSound();
                        Song temp = song;
                        MusicManager.Instance.CurrentProject.Songs[num] = nextSong;
                        MusicManager.Instance.CurrentProject.Songs[num + 1] = temp;
                        Refresh();
                    });

                    //listing_sh.objects.Add(listing_down);
                }

               // foreach (GameObject obj in listing_sh.objects) obj.SetActive(false);
            }

            // Create new song button
            GameObject newSongButton = UIHelpers.MakeButton("New Song");
            _listings.Add(newSongButton);

            RectTransform newSongButton_tr = newSongButton.GetComponent<RectTransform>();
            newSongButton_tr.SetParent(_playlistTR);
            newSongButton_tr.sizeDelta = new Vector2(_buttonHeight * _iconScale, _buttonHeight * _iconScale);
            newSongButton_tr.AnchorAtPoint(0.15f, 1f);
            newSongButton_tr.anchoredPosition3D = new Vector3(
                //horizontalPadding + newSongButton_tr.sizeDelta.x/2f,
                0f,
                -_verticalPadding - 0.5f * _buttonHeight - (_verticalPadding + _buttonHeight) * (float)(numSongs),
                0f
            );
            newSongButton_tr.ResetScaleRot();

            Image newSong_img = newSongButton.GetComponent<Image>();
            newSong_img.sprite = UIManager.Instance.AddIcon;
            newSong_img.color = Color.black;

            newSongButton.GetComponent<Button>().onClick.AddListener(delegate {
                UIManager.Instance.PlayMenuClickSound();
                MusicManager.Instance.NewSong();
                UIManager.Instance.GoToKeySelectMenu();
            });

            GameObject newSong_text = UIHelpers.MakeText("New Song_text");

            RectTransform newSong_text_tr = newSong_text.GetComponent<RectTransform>();
            newSong_text_tr.SetParent(newSongButton_tr);
            newSong_text_tr.sizeDelta = new Vector2(_playlistTR.rect.width / 2f - 2f * _horizontalPadding, newSongButton_tr.sizeDelta.y);
            newSong_text_tr.AnchorAtPoint(0f, 0.5f);
            newSong_text_tr.anchoredPosition3D = new Vector3(
                newSong_text_tr.sizeDelta.x / 2f + _horizontalPadding + newSongButton_tr.sizeDelta.x,
                0f,
                0f
            );
            newSong_text_tr.ResetScaleRot();

            Text newSong_text_txt = newSong_text.GetComponent<Text>();
            newSong_text_txt.text = "New Song...";
            newSong_text_txt.fontSize = (int)(_fontSize * _iconScale);
            newSong_text_txt.color = Color.black;
            newSong_text_txt.font = UIManager.Instance.HandwrittenFont;
            newSong_text_txt.alignment = TextAnchor.MiddleLeft;

            GameObject newSongButton_highlight = UIHelpers.MakeImage("Highlight (New Song Button)");
            _listings.Add(newSongButton_highlight);

            RectTransform newSongButton_highlight_tr = newSongButton_highlight.GetComponent<RectTransform>();
            newSongButton_highlight_tr.SetParent(newSongButton_tr);
            newSongButton_highlight_tr.sizeDelta = new Vector2(_playlistTR.rect.width * 0.5f - 2f * _horizontalPadding, newSongButton_tr.sizeDelta.y * 1.5f);
            newSongButton_highlight_tr.AnchorAtPoint(0f, 0.5f);
            newSongButton_highlight_tr.anchoredPosition3D = new Vector3(newSongButton_highlight_tr.sizeDelta.x / 3f, 0f, 0f);
            newSongButton_highlight_tr.ResetScaleRot();

            newSongButton_highlight_tr.SetSiblingIndex(-1);

            Image newSongButton_highlight_img = newSongButton_highlight.GetComponent<Image>();
            newSongButton_highlight_img.raycastTarget = false;
            newSongButton_highlight_img.sprite = UIManager.Instance.ScribbleIcon;
            newSongButton_highlight_img.color = new Color(1f, 1f, 1f, 1f);

            //ShowHide newSongButton_sh = newSongButton.ShowHide();
            //newSongButton_sh.objects = new List<GameObject>() { newSongButton_highlight };
            //newSongButton_sh.Hide();

            // Create load song button
            GameObject loadSongButton = UIHelpers.MakeButton("Load Song Button (Playlist Browser)");
            _listings.Add(loadSongButton);

            RectTransform loadSongButton_tr = loadSongButton.GetComponent<RectTransform>();
            loadSongButton_tr.SetParent(_playlistTR);
            loadSongButton_tr.sizeDelta = new Vector2(_buttonHeight * _iconScale, _buttonHeight * _iconScale);
            loadSongButton_tr.AnchorAtPoint(0.6f, 1f);
            loadSongButton_tr.anchoredPosition3D = new Vector3(
                //horizontalPadding + loadSongButton_tr.sizeDelta.x / 2f,
                0f,
                -_verticalPadding - 0.5f * _buttonHeight - (_verticalPadding + _buttonHeight) * (float)(numSongs),
                0f
            );
            loadSongButton_tr.ResetScaleRot();

            Image loadSongButton_img = loadSongButton.GetComponent<Image>();
            loadSongButton_img.sprite = UIManager.Instance.LoadIcon;
            loadSongButton_img.color = Color.black;

            loadSongButton.GetComponent<Button>().onClick.AddListener(delegate {
                UIManager.Instance.PlayMenuClickSound();
                GameManager.Instance.ShowLoadPromptForSongs();
            });

            GameObject loadSongButton_text = UIHelpers.MakeText("Load New Song_text");

            RectTransform loadSongButton_text_tr = loadSongButton_text.GetComponent<RectTransform>();
            loadSongButton_text_tr.SetParent(loadSongButton_tr);
            loadSongButton_text_tr.sizeDelta = new Vector2(_playlistTR.rect.width / 2f - 2f * _horizontalPadding, loadSongButton_tr.sizeDelta.y);
            loadSongButton_text_tr.AnchorAtPoint(0f, 0.5f);
            loadSongButton_text_tr.anchoredPosition3D = new Vector3(
                loadSongButton_text_tr.sizeDelta.x / 2f + _horizontalPadding + loadSongButton_tr.sizeDelta.x,
                0f,
                0f
            );
            loadSongButton_text_tr.ResetScaleRot();

            Text loadSongButton_text_txt = loadSongButton_text.GetComponent<Text>();
            loadSongButton_text_txt.text = "Load Song...";
            loadSongButton_text_txt.fontSize = (int)(_fontSize * _iconScale);
            loadSongButton_text_txt.color = Color.black;
            loadSongButton_text_txt.font = UIManager.Instance.HandwrittenFont;
            loadSongButton_text_txt.alignment = TextAnchor.MiddleLeft;

            GameObject loadSongButton_highlight = UIHelpers.MakeImage("Load New Song Highlight");
            _listings.Add(loadSongButton_highlight);

            RectTransform loadSongButton_highlight_tr = loadSongButton_highlight.GetComponent<RectTransform>();
            loadSongButton_highlight_tr.SetParent(loadSongButton_tr);
            loadSongButton_highlight_tr.sizeDelta = new Vector2(_playlistTR.rect.width * 0.5f, loadSongButton_tr.sizeDelta.y * 1.5f);
            loadSongButton_highlight_tr.AnchorAtPoint(0f, 0.5f);
            loadSongButton_highlight_tr.anchoredPosition3D = new Vector3(loadSongButton_highlight_tr.sizeDelta.x / 3f, 0f, 0f);
            loadSongButton_highlight_tr.ResetScaleRot();

            loadSongButton_highlight_tr.SetSiblingIndex(-1);

            Image loadSongButton_highlight_img = loadSongButton_highlight.GetComponent<Image>();
            loadSongButton_highlight_img.raycastTarget = false;
            loadSongButton_highlight_img.sprite = UIManager.Instance.ScribbleIcon;
            loadSongButton_highlight_img.color = new Color(1f, 1f, 1f, 1f);

            //ShowHide loadSongButton_sh = loadSongButton.ShowHide();
            //loadSongButton_sh.objects = new List<GameObject>() { loadSongButton_highlight };
            //loadSongButton_sh.Hide();
        }

        #endregion

    }
}
