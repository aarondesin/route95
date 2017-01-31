using Route95.Core;
using Route95.Music;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Route95.UI {

    /// <summary>
    /// Class to handle the song arrange menu
    /// </summary>
    public class SongArrangeMenu : MenuBase<SongArrangeMenu> {

        #region SongArrangeMenu Vars

        [Tooltip("Index of currently selected riff.")]
        int _selectedRiffIndex;

        [Tooltip("Riff selection dropdown.")]
        Dropdown _dropdown;

        [Tooltip("Song name input field.")]
        InputField _songNameInputField;

        [Tooltip("Play riff button.")]
        Button _playRiffButton;

        [Tooltip("Edit riff button.")]
        Button _editRiffButton;

        [Tooltip("Add riff reminder prompt.")]
        GameObject _addRiffReminder;

        Button _previewSongButton;

		AudioSource _radioThemeEmitter;

        #endregion
        #region Unity Callbacks

        new void Awake() {
			base.Awake();

			// Init vars
			_radioThemeEmitter = GameObject.FindGameObjectWithTag("RadioThemeEmitter").GetComponent<AudioSource>();
			_songNameInputField = GetComponentInChildren<InputField>();
			_dropdown = GetComponentInChildren<Dropdown>();
			_editRiffButton = GameObject.FindGameObjectWithTag("SongArrangeEditRiffButton").GetComponent<Button>();
			_playRiffButton = GameObject.FindGameObjectWithTag("SongArrangePlayRiffButton").GetComponent<Button>();
			_previewSongButton = GameObject.FindGameObjectWithTag("SongArrangePreviewSongButton").GetComponent<Button>();
			_addRiffReminder = GameObject.FindGameObjectWithTag("SongArrangeAddRiffReminder");

            // Set song name input field functionality
            _songNameInputField.onEndEdit.AddListener(delegate { MusicManager.Instance.CurrentSong.Name = _songNameInputField.text; });
        }

		void Start () {
			UIManager.Instance.onSwitchToSongArrangeMenu.AddListener(Refresh);

			UIManager.Instance.onSwitchToRiffEditor.AddListener(UpdateValue);
		}

		#endregion
		#region Properties

		public int SelectedRiffIndex {
			get { return _selectedRiffIndex; }
			set { _selectedRiffIndex = value; }
		}

		#endregion
		#region SongArrangeMenu Methods

		/// <summary>
		/// Refreshes all elements on the song arranger UI.
		/// </summary>
		public void Refresh() {

            // Update the options in the dropdown to include all riffs
            _dropdown.ClearOptions();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

            foreach (Riff riff in MusicManager.Instance.CurrentSong.Riffs) {
                Sprite sprite = riff.Instrument.Icon;
                Dropdown.OptionData option = new Dropdown.OptionData(riff.Name, sprite);
                options.Add(option);
            }
            _dropdown.AddOptions(options);

            if (MusicManager.Instance.CurrentSong.Riffs.Count == 0) {
                _dropdown.interactable = false;
                _editRiffButton.GetComponent<Button>().interactable = false;
                _playRiffButton.GetComponent<Button>().interactable = false;
                _previewSongButton.GetComponent<Button>().interactable = false;
            }
            else {
                _dropdown.interactable = true;
                _editRiffButton.GetComponent<Button>().interactable = true;
                _playRiffButton.GetComponent<Button>().interactable = true;
                _previewSongButton.GetComponent<Button>().interactable = true;
                if (RiffEditor.CurrentRiff == null)
                    RiffEditor.CurrentRiff = MusicManager.Instance.CurrentSong.Riffs[0];
            }

            _dropdown.value = _selectedRiffIndex;

            // Refresh song name input field
            _songNameInputField.text = MusicManager.Instance.CurrentSong.Name;

            // Update play riff button art
            _playRiffButton.GetComponent<Image>().sprite = UIManager.Instance.PlayIcon;

            bool hasRiffs = MusicManager.Instance.CurrentSong.Riffs.Count != 0;
            SongTimeline.Instance.SetInteractable(hasRiffs);
            _addRiffReminder.SetActive(!hasRiffs);
        }

        /// <summary>
        /// Sets the selected riff from the dropdown.
        /// </summary>
        public void UpdateValue() {
            _selectedRiffIndex = _dropdown.value;
            RiffEditor.CurrentRiff = MusicManager.Instance.CurrentSong.Riffs[_selectedRiffIndex];
        }

        /// <summary>
        /// Sets the dropdown value from the selected riff.
        /// </summary>
        public void SetValue() {
            SetValue(_selectedRiffIndex);
        }

        public void SetValue(int i) {
            _dropdown.value = i;
        }

        /// <summary>
        /// Hide the dropdown.
        /// </summary>
        public void HideDropdown() {
            _dropdown.Hide();
        }

        /// <summary>
        /// Updates the play riff button art.
        /// </summary>
        public void TogglePlayRiffGetComponent<Button>() {
            if (MusicManager.Instance.IsPlaying && MusicManager.Instance.RiffMode) _playRiffButton.GetComponent<Image>().sprite = UIManager.Instance.PauseIcon;
            else _playRiffButton.GetComponent<Image>().sprite = UIManager.Instance.PlayIcon;
        }

        public void TogglePlaySongGetComponent<Button>() {
            if (MusicManager.Instance.IsPlaying && !MusicManager.Instance.RiffMode) _previewSongButton.GetComponent<Image>().sprite = UIManager.Instance.PauseIcon;
            else _previewSongButton.GetComponent<Image>().sprite = UIManager.Instance.PlayIcon;
        }

        #endregion
    }
}
