﻿using Route95.Music;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Route95.UI {

    /// <summary>
    /// Class to control "Add Riff" prompt.
    /// </summary>
    public class AddRiffPrompt : MenuBase<AddRiffPrompt> {

        #region AddRiffPrompt Vars

        [Tooltip("Riff name input field.")]
        public InputField inputField;

        [Tooltip("Instrument select dropdown.")]
        public Dropdown dropdown;

        [Tooltip("Confirm button.")]
        public Button confirmButton;

        #endregion
        #region Unity Callbacks

        new void Awake() {
            // Reset listeners
            inputField.onEndEdit.AddListener(delegate {
                confirmButton.interactable = true;
            });
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
            inputField.text = "";

            // Refresh dropdown
            SetupDropdown();

            // Set confirm button to be non-interactable
            confirmButton.interactable = false;
        }

        /// <summary>
        /// Refreshes the instrument selection dropdown.
        /// </summary>
        void SetupDropdown() {

            // Clear old options
            dropdown.ClearOptions();

            // Reset options
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            foreach (Instrument inst in Instrument.AllInstruments)
                options.Add(new Dropdown.OptionData(inst.name, inst.icon));

            dropdown.AddOptions(options);
            dropdown.value = 0;
        }

        /// <summary>
        /// Adds a riff from the prompt.
        /// </summary>
        public void AddRiff() {

            // Create riff
            Riff temp = new Riff(dropdown.value);

            // Name riff
            temp.name = inputField.text;

            // Register riff with song
            MusicManager.Instance.CurrentSong.RegisterRiff(temp);

            // Go to riff editor
            RiffEditor.CurrentRiff = temp;
            SongArrangeMenu.Instance.selectedRiffIndex = temp.index;
            SongArrangeMenu.Instance.SetValue(temp.index);
            SongArrangeMenu.Instance.Refresh();
        }

        #endregion

    }
}
