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
        public int selectedRiffIndex;

        [Tooltip("Riff selection dropdown.")]
        public Dropdown dropdown;

        [Tooltip("Song name input field.")]
        public InputField songNameInputField;

        [Tooltip("Play riff button.")]
        public GameObject playRiffButton;

        [Tooltip("Edit riff button.")]
        public GameObject editRiffButton;

        [Tooltip("Add riff reminder prompt.")]
        public GameObject addRiffReminder;

        public GameObject previewSongButton;

        #endregion
        #region Unity Callbacks

        new void Awake() {
            // Set song name input field functionality
            songNameInputField.onEndEdit.AddListener(delegate { MusicManager.Instance.CurrentSong.Name = songNameInputField.text; });
        }

        #endregion
        #region SongArrangeMenu Methods

        /// <summary>
        /// Refreshes all elements on the song arranger UI.
        /// </summary>
        public void Refresh() {

            // Update the options in the dropdown to include all riffs
            dropdown.ClearOptions();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

            foreach (Riff riff in MusicManager.Instance.CurrentSong.Riffs) {
                Sprite sprite = riff.Instrument.Icon;
                Dropdown.OptionData option = new Dropdown.OptionData(riff.Name, sprite);
                options.Add(option);
            }
            dropdown.AddOptions(options);

            if (MusicManager.Instance.CurrentSong.Riffs.Count == 0) {
                dropdown.interactable = false;
                editRiffButton.GetComponent<Button>().interactable = false;
                playRiffButton.GetComponent<Button>().interactable = false;
                previewSongButton.GetComponent<Button>().interactable = false;
            }
            else {
                dropdown.interactable = true;
                editRiffButton.GetComponent<Button>().interactable = true;
                playRiffButton.GetComponent<Button>().interactable = true;
                previewSongButton.GetComponent<Button>().interactable = true;
                if (RiffEditor.CurrentRiff == null)
                    RiffEditor.CurrentRiff = MusicManager.Instance.CurrentSong.Riffs[0];
            }

            dropdown.value = selectedRiffIndex;

            // Refresh song name input field
            songNameInputField.text = MusicManager.Instance.CurrentSong.Name;

            // Update play riff button art
            playRiffButton.GetComponent<Image>().sprite = UIManager.Instance.PlayIcon;

            bool hasRiffs = MusicManager.Instance.CurrentSong.Riffs.Count != 0;
            SongTimeline.Instance.SetInteractable(hasRiffs);
            addRiffReminder.SetActive(!hasRiffs);

        }

        /// <summary>
        /// Sets the selected riff from the dropdown.
        /// </summary>
        public void UpdateValue() {
            selectedRiffIndex = dropdown.value;
            RiffEditor.CurrentRiff = MusicManager.Instance.CurrentSong.Riffs[selectedRiffIndex];
        }

        /// <summary>
        /// Sets the dropdown value from the selected riff.
        /// </summary>
        public void SetValue() {
            SetValue(selectedRiffIndex);
        }

        public void SetValue(int i) {
            dropdown.value = i;
        }

        /// <summary>
        /// Hide the dropdown.
        /// </summary>
        public void HideDropdown() {
            dropdown.Hide();
        }

        /// <summary>
        /// Updates the play riff button art.
        /// </summary>
        public void TogglePlayRiffGetComponent<Button>() {
            if (MusicManager.Instance.IsPlaying && MusicManager.Instance.RiffMode) playRiffButton.GetComponent<Image>().sprite = UIManager.Instance.PauseIcon;
            else playRiffButton.GetComponent<Image>().sprite = UIManager.Instance.PlayIcon;
        }

        public void TogglePlaySongGetComponent<Button>() {
            if (MusicManager.Instance.IsPlaying && !MusicManager.Instance.RiffMode) previewSongButton.GetComponent<Image>().sprite = UIManager.Instance.PauseIcon;
            else previewSongButton.GetComponent<Image>().sprite = UIManager.Instance.PlayIcon;
        }

        #endregion
    }
}
