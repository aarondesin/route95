// LoadPrompt.cs
// ©2016 Team 95

using Route95.Core;

using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

namespace Route95.UI {

    /// <summary>
    /// Class to handle the loading prompt.
    /// </summary>
    public class LoadPrompt : MenuBase<LoadPrompt> {

        #region LoadPrompt Enums

        /// <summary>
        /// Type of file to display and load.
        /// </summary>
        public enum Mode {
            Project,
            Song
        };

        #endregion
        #region LoadPrompt Vars

		/// <summary>
		/// Transform of the actual panel with all of the files listed.
		/// </summary>
        RectTransform _fileList;

		/// <summary>
		/// Size vector of the file list.
		/// </summary>
		static Vector2 _FileListSize = new Vector2(84f, 84f);

		/// <summary>
		/// Load button on prompt.
		/// </summary>
        GameObject _loadButton;

		/// <summary>
		/// Text on load button.
		/// </summary>
        Text _loadButtonText;

		/// <summary>
		/// Type of file to display and load.
		/// </summary>
        Mode _loadMode;

		/// <summary>
		/// Currently selected path.
		/// </summary>
        string _selectedPath;

		/// <summary>
		/// List of created buttons.
		/// </summary>
        List<GameObject> _fileButtons;

		/// <summary>
		/// Horizontal button padding.
		/// </summary>
        const float HORIZONTAL_PADDING = 8f;

		/// <summary>
		/// Vertical button padding.
		/// </summary>
        const float VERTICAL_PADDING = 4f;

		/// <summary>
		/// Size of a button.
		/// </summary>
        static Vector2 _ButtonSize = new Vector2(360f, 72f);

		/// <summary>
		/// Header text.
		/// </summary>
        Text _header;

        #endregion
        #region Unity Callbacks

        new void Awake() {
            base.Awake();

            // Init vars
            _fileButtons = new List<GameObject>();
        }

        #endregion
        #region LoadPrompt Methods

        /// <summary>
		/// Refreshes the load prompt based on the given mode.
		/// </summary>
        public void Refresh(Mode mode) {
			// Clear old buttons
            foreach (GameObject fileButton in _fileButtons)
                Destroy(fileButton);

            _fileButtons.Clear();

            _loadMode = mode;

            // Get list of files in save location
            List<string> files = new List<string>();
            switch (mode) {
                case LoadPrompt.Mode.Project:
                    files.AddRange(Directory.GetFiles(GameManager.Instance.ProjectSavePath, "*" + SaveLoad.PROJECT_SAVE_EXT).ToList<string>());
                    break;
                case LoadPrompt.Mode.Song:
                    files.AddRange(Directory.GetFiles(GameManager.Instance.SongSavePath, "*" + SaveLoad.SONG_SAVE_EXT).ToList<string>());
                    break;
            }
            for (int i = 0; i < files.Count; i++) {
                string path = files[i];
                string filename = Path.GetFileNameWithoutExtension(files[i]);

                GameObject button = UIHelpers.MakeButton(filename);

                RectTransform button_tr = button.GetComponent<RectTransform>();
                button_tr.SetParent(_fileList);
                float width = ((RectTransform)button_tr.parent.parent).rect.width;
                button_tr.sizeDelta = new Vector2(width, _ButtonSize.y);
                button_tr.AnchorAtPoint(0f, 1f);
                button_tr.anchoredPosition3D = new Vector3(
                    HORIZONTAL_PADDING + button_tr.sizeDelta.x / 2f,
                    ((i == 0 ? 0f : VERTICAL_PADDING) + button_tr.sizeDelta.y) * -(float)(i + 1),
                    0f
                );
                button_tr.ResetScaleRot();

                Image button_img = button.GetComponent<Image>();
                button_img.sprite = UIManager.Instance.FillSprite;
                button_img.color = new Color(1f, 1f, 1f, 0f);

                GameObject text = UIHelpers.MakeText(filename + "_Text");
                RectTransform text_tr = text.GetComponent<RectTransform>();
                text_tr.SetParent(button.transform);
                text_tr.sizeDelta = ((RectTransform)text_tr.parent).sizeDelta;
                text_tr.AnchorAtPoint(0.5f, 0.5f);
                text_tr.anchoredPosition3D = Vector3.zero;
                text_tr.ResetScaleRot();

                Text text_text = text.GetComponent<Text>();
                text_text.text = filename;
                text_text.fontSize = 36;
                text_text.color = Color.white;
                text_text.font = UIManager.Instance.Font;
                text_text.alignment = TextAnchor.MiddleLeft;

                Fadeable text_fade = text.AddComponent<Fadeable>();
                text_fade.StartFaded = false;

                button.GetComponent<Button>().onClick.AddListener(() => {
                    UIManager.Instance.PlayMenuClickSound();
                    ResetButtons();
                    _selectedPath = path;
                    _loadButton.GetComponent<Button>().interactable = true;
                    _loadButtonText.color = _loadButton.GetComponent<Button>().colors.normalColor;
                    button.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
                });

                _fileButtons.Add(button);

                /*GameObject highlight = UIHelpers.MakeImage(filename + "_Highlight");
                RectTransform highlight_tr = highlight.GetComponent<RectTransform>();
                highlight_tr.SetParent(button_tr);
                highlight_tr.sizeDelta = ((RectTransform)text_tr.parent).sizeDelta;
                highlight_tr.AnchorAtPoint(0.5f, 0.5f);
                highlight_tr.anchoredPosition3D = Vector3.zero;
                highlight_tr.ResetScaleRot();
                highlight.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);

                ShowHide sh = button.ShowHide();
                sh.objects = new List<GameObject>();
                sh.objects.Add(highlight);

                fileButtons.Add(highlight);
                highlight.SetActive(false);*/
            }

            // Update size of panel to fit all files
            _fileList.sizeDelta = new Vector2(_FileListSize.x, (float)(_fileButtons.Count + 1) * (VERTICAL_PADDING + _ButtonSize.y));

            // Update header
            _header.text = mode == Mode.Project ? "Load Project" : "Load Song";
        }



        // calls save_load to load the currently selected file
        public void LoadSelectedPath() {
            //string fullPath = GameManager.Instance.projectSavePath+"/"+selectedPath+SaveLoad.projectSaveExtension;
            Debug.Log("LoadPrompt.LoadSelectedPath(): loading " + _selectedPath);

            switch (_loadMode) {
                case LoadPrompt.Mode.Project:
                    try {
                        SaveLoad.LoadProject(_selectedPath);
                        Prompt.Instance.PromptMessage("Load Project", "Successfully loaded project!", "Nice");

                        // Refresh name field on playlist browser
                        PlaylistBrowser.Instance.RefreshName();

                        // Go to playlist menu if not there already
                        UIManager.Instance.GoToPlaylistMenu();
                        break;
                    }
                    catch (SaveLoad.FailedToLoadException) {
                        // Prompt
                        Prompt.Instance.PromptMessage("Failed to load project", "File is corrupted.", "Okay");
                        break;
                    }

                case LoadPrompt.Mode.Song:
                    try {
                        SaveLoad.LoadSongToProject(_selectedPath);
                        PlaylistBrowser.Instance.Refresh();
                        Prompt.Instance.PromptMessage("Load Song", "Successfully loaded song!", "Nice");
                        break;
                    }
                    catch (SaveLoad.FailedToLoadException) {
                        // Prompt
                        Prompt.Instance.PromptMessage("Failed to load song", "File is corrupted.", "Okay");
                        break;
                    }
            }
        }

        // Resets highlighting of all buttons
        void ResetButtons() {
            foreach (GameObject button in _fileButtons) button.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        }

        #endregion
    }
}
