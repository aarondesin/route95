// EffectButton.cs
// ©2016 Team 95

using Route95.Music;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Route95.UI {

    /// <summary>
    /// Class to handle effect sliders and buttons.
    /// </summary>
    public class EffectButton : MonoBehaviour {

		#region Vars

		/// <summary>
		/// This effect button's sprite.
		/// </summary>
		Image _image;

        #endregion
        #region Unity Callbacks

        void Awake() {
            // Init vars
            _image = GetComponent<Image>();
        }

        #endregion
        #region Methods

        /// <summary>
        /// Toggles an effect on or off.
        /// </summary>
        public void Toggle<TEffect> () where TEffect: Behaviour {
            Riff riff = RiffEditor.CurrentRiff;
            Instrument inst = riff.Instrument;
            AudioSource source = MusicManager.Instance.GetAudioSource(inst);

            // Toggle effect
			TEffect effect = source.GetComponent<TEffect>();
			effect.enabled = ! effect.enabled;

            // Update riff
            Type effectType = typeof(TEffect);
            if (effectType == typeof(AudioDistortionFilter)) riff.DistortionEnabled = effect.enabled;
            else if (effectType == typeof(AudioEchoFilter)) riff.EchoEnabled = effect.enabled;
            else if (effectType == typeof(AudioReverbFilter)) riff.ReverbEnabled = effect.enabled;
            else if (effectType == typeof(AudioTremoloFilter)) riff.TremoloEnabled = effect.enabled;
            else if (effectType == typeof(AudioChorusFilter)) riff.ChorusEnabled = effect.enabled;
            else if (effectType == typeof(AudioFlangerFilter)) riff.FlangerEnabled = effect.enabled;

            // Update button art
            _image.sprite = (effect.enabled ? RiffEditor.Instance.percussionFilled : RiffEditor.Instance.percussionEmpty);

            // Play sound
            if (effect.enabled) UIManager.Instance.PlayEnableEffectSound();
            else UIManager.Instance.PlayDisableEffectSound();
        }

        public void UpdateSetting<TEffect>(ref float effectSetting, ref float riffSetting, Slider slider)
            where TEffect : Behaviour {
            Riff riff = RiffEditor.CurrentRiff;
            Instrument inst = riff.Instrument;
            AudioSource source = MusicManager.Instance.GetAudioSource(inst);

            // Update setting on both effect and riff
            riffSetting = effectSetting = slider.value;

            // Turn on effect if not already
            TEffect effect = source.GetComponent<TEffect>();
            if (!effect.enabled) Toggle<TEffect>();
        }

		public void ToggleDistortion () { Toggle<AudioDistortionFilter>(); }

		public void ToggleTremolo () { Toggle<AudioTremoloFilter>(); }

		public void ToggleChorus () { Toggle<AudioChorusFilter>(); }

		public void ToggleFlanger () { Toggle<AudioFlangerFilter>(); }

		public void ToggleEcho () { Toggle<AudioEchoFilter>(); }

		public void ToggleReverb () { Toggle<AudioReverbFilter>(); }

        #endregion
    }
}

