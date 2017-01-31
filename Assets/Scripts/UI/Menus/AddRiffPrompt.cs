// AddRiffPrompt.cs
// ©2016 Team 95

using Route95.Music;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Route95.UI {

    /// <summary>
    /// Class to control "Add Riff" prompt.
    /// </summary>
    public class AddRiffPrompt : MenuBase<AddRiffPrompt> {

        #region AddRiffPrompt Vars

        /// <summary>
        /// Riff name input field.
        /// </summary>
        InputField _inputField;

        /// <summary>
        /// Instrument select dropdown.
        /// </summary>
        Dropdown _dropdown;

        /// <summary>
        /// Confirm button.
        /// </summary>
        Button _confirmButton;

        #endregion
        #region Unity Callbacks

        new void Awake() {
			base.Awake();

            // Init vars
            _inputField = GetComponentInChildren<InputField>();
            _dropdown = GetComponentInChildren<Dropdown>();
            _confirmButton = GetComponentInChildren<Button>();

            // Reset listeners
            _inputField.onEndEdit.AddListener(EnableConfirmButton);
			_dropdown.onValueChanged.AddListener(PlayInstrumentSound);
        }

        void Start() {
            Refresh();
        }

        #endregion
        #region AddRiffPrompt Callbacks

        /// <summary>
        /// Refreshes the "Add Riff" prompt.
        /// </summary>
        public void Refresh() {

            // Set riff name input field to be blank
            _inputField.text = "";

            // Refresh dropdown
            SetupDropdown();

            // Set confirm button to be non-interactable
            _confirmButton.interactable = false;
        }

        /// <summary>
        /// Refreshes the instrument selection dropdown.
        /// </summary>
        void SetupDropdown() {

            // Clear old options
            _dropdown.ClearOptions();

            // Reset options
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            foreach (Instrument inst in Instrument.AllInstruments)
                options.Add(new Dropdown.OptionData(inst.Name, inst.Icon));

            _dropdown.AddOptions(options);
            _dropdown.value = 0;
        }

		void EnableConfirmButton (string riffName) {
			if (riffName != "" && riffName != default(string))
				_confirmButton.interactable = true;
		}

		void PlayInstrumentSound (int instrumentIndex) {
			UIManager.Instance.PlayInstrumentSound (instrumentIndex);
		}

        /// <summary>
        /// Adds a riff from the prompt.
        /// </summary>
        public void AddRiff() {

            // Create riff
            Riff temp = new Riff(_dropdown.value);

            // Name riff
            temp.Name = _inputField.text;

            // Register riff with song
            MusicManager.Instance.CurrentSong.RegisterRiff(temp);

            // Go to riff editor
            RiffEditor.CurrentRiff = temp;
            SongArrangeMenu.Instance.SelectedRiffIndex = temp.Index;
            SongArrangeMenu.Instance.SetValue(temp.Index);
            SongArrangeMenu.Instance.Refresh();
        }

        #endregion
    }
}
