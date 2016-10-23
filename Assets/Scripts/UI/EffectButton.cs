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

        void Awake () {
            // Init vars
            _image = GetComponent<Image>();
        }

        #endregion
        #region Methods

        /// <summary>
        /// Toggles an effect on or off.
        /// </summary>
        /// <typeparam name="TEffect">Type of effect to toggle.</typeparam>
        public void Toggle<TEffect> () where TEffect : Behaviour {
            Riff riff = RiffEditor.CurrentRiff;
            Instrument inst = riff.Instrument;
            AudioSource source = MusicManager.Instance.GetAudioSource(inst);

            // Toggle effect
            TEffect effect = source.GetComponent<TEffect>();
            effect.enabled = !effect.enabled;

            // Update riff
            Type effectType = typeof(TEffect);
            if      (effectType == typeof(AudioDistortionFilter))    riff.DistortionEnabled = effect.enabled;
            else if (effectType == typeof(AudioEchoFilter))          riff.EchoEnabled       = effect.enabled;
            else if (effectType == typeof(AudioReverbFilter))        riff.ReverbEnabled     = effect.enabled;
            else if (effectType == typeof(AudioTremoloFilter))       riff.TremoloEnabled    = effect.enabled;
            else if (effectType == typeof(AudioChorusFilter))        riff.ChorusEnabled     = effect.enabled;
            else if (effectType == typeof(AudioFlangerFilter))       riff.FlangerEnabled    = effect.enabled;

            // Update button art
            _image.sprite = (effect.enabled ? RiffEditor.Instance.percussionFilled : RiffEditor.Instance.percussionEmpty);

            // Play sound
            if (effect.enabled) UIManager.Instance.PlayEnableEffectSound();
            else UIManager.Instance.PlayDisableEffectSound();
        }

        public void UpdateSetting<TEffect> (ref float effectSetting, ref float riffSetting, Slider slider) 
            where TEffect : Behaviour
        {
            Riff riff = RiffEditor.CurrentRiff;
            Instrument inst = riff.Instrument;
            AudioSource source = MusicManager.Instance.GetAudioSource (inst);

            // Update setting on both effect and riff
            riffSetting = effectSetting = slider.value;
            
            // Turn on effect if not already
            TEffect effect = source.GetComponent<TEffect>();
            if (!effect.enabled) Toggle<TEffect>();
        }

        public void UpdateDistortionLevel 

        public void UpdateDistortionLevel(Slider slider) {
            AudioSource source = MusicManager.Instance.GetAudioSource(RiffEditor.CurrentRiff.instrument);
            RiffEditor.CurrentRiff.distortionLevel = slider.value;
            source.gameObject.GetComponent<AudioDistortionFilter>().distortionLevel = RiffEditor.CurrentRiff.distortionLevel;

            Riff riff = RiffEditor.CurrentRiff;
            if (!riff.distortionEnabled && RiffEditor.Instance.initialized) ToggleDistortion();
        }

        public void UpdateEchoDecayRatio(Slider slider) {
            AudioSource source = MusicManager.Instance.GetAudioSource(RiffEditor.CurrentRiff.instrument);
            RiffEditor.CurrentRiff.echoDecayRatio = slider.value;
            source.gameObject.GetComponent<AudioEchoFilter>().decayRatio = RiffEditor.CurrentRiff.echoDecayRatio;

            Riff riff = RiffEditor.CurrentRiff;
            if (!riff.echoEnabled && RiffEditor.Instance.initialized) ToggleEcho();
        }


        public void UpdateEchoDelay(Slider slider) {
            AudioSource source = MusicManager.Instance.GetAudioSource(RiffEditor.CurrentRiff.instrument);
            RiffEditor.CurrentRiff.echoDelay = slider.value;
            source.gameObject.GetComponent<AudioEchoFilter>().delay = RiffEditor.CurrentRiff.echoDelay;

            Riff riff = RiffEditor.CurrentRiff;
            if (!riff.echoEnabled && RiffEditor.Instance.initialized) ToggleEcho();
        }


        public void UpdateEchoDryMix(Slider slider) {
            AudioSource source = MusicManager.Instance.GetAudioSource(RiffEditor.CurrentRiff.instrument);
            RiffEditor.CurrentRiff.echoDryMix = slider.value;
            source.gameObject.GetComponent<AudioEchoFilter>().dryMix = RiffEditor.CurrentRiff.echoDryMix;

            Riff riff = RiffEditor.CurrentRiff;
            if (!riff.echoEnabled && RiffEditor.Instance.initialized) ToggleEcho();
        }

        public void UpdateReverbDecayTime(Slider slider) {
            AudioSource source = MusicManager.Instance.GetAudioSource(RiffEditor.CurrentRiff.instrument);
            RiffEditor.CurrentRiff.reverbDecayTime = slider.value;
            source.gameObject.GetComponent<AudioReverbFilter>().decayTime = RiffEditor.CurrentRiff.reverbDecayTime;

            Riff riff = RiffEditor.CurrentRiff;
            if (!riff.reverbEnabled && RiffEditor.Instance.initialized) ToggleReverb();
        }


        public void UpdateReverbLevel(Slider slider) {
            AudioSource source = MusicManager.Instance.GetAudioSource(RiffEditor.CurrentRiff.instrument);
            RiffEditor.CurrentRiff.reverbLevel = slider.value;
            source.gameObject.GetComponent<AudioReverbFilter>().reverbLevel = RiffEditor.CurrentRiff.reverbLevel;

            Riff riff = RiffEditor.CurrentRiff;
            if (!riff.reverbEnabled && RiffEditor.Instance.initialized) ToggleReverb();
        }

        public void UpdateTremoloRate(Slider slider) {
            AudioSource source = MusicManager.Instance.GetAudioSource(RiffEditor.CurrentRiff.instrument);
            RiffEditor.CurrentRiff.tremoloRate = slider.value;
            source.gameObject.GetComponent<AudioTremoloFilter>().rate = RiffEditor.CurrentRiff.tremoloRate;

            Riff riff = RiffEditor.CurrentRiff;
            if (!riff.tremoloEnabled && RiffEditor.Instance.initialized) ToggleTremolo();
        }

        public void UpdateTremoloDepth(Slider slider) {
            AudioSource source = MusicManager.Instance.GetAudioSource(RiffEditor.CurrentRiff.instrument);
            RiffEditor.CurrentRiff.tremoloDepth = slider.value;
            source.gameObject.GetComponent<AudioTremoloFilter>().depth = RiffEditor.CurrentRiff.tremoloDepth;

            Riff riff = RiffEditor.CurrentRiff;
            if (!riff.tremoloEnabled && RiffEditor.Instance.initialized) ToggleTremolo();
        }

        public void UpdateChorusDryMix(Slider slider) {
            AudioSource source = MusicManager.Instance.GetAudioSource(RiffEditor.CurrentRiff.instrument);
            RiffEditor.CurrentRiff.chorusDryMix = slider.value;
            source.gameObject.GetComponent<AudioChorusFilter>().dryMix = RiffEditor.CurrentRiff.chorusDryMix;

            Riff riff = RiffEditor.CurrentRiff;
            if (!riff.chorusEnabled && RiffEditor.Instance.initialized) ToggleChorus();
        }

        public void UpdateChorusRate(Slider slider) {
            AudioSource source = MusicManager.Instance.GetAudioSource(RiffEditor.CurrentRiff.instrument);
            RiffEditor.CurrentRiff.chorusRate = slider.value;
            source.gameObject.GetComponent<AudioChorusFilter>().rate = RiffEditor.CurrentRiff.chorusRate;

            Riff riff = RiffEditor.CurrentRiff;
            if (!riff.chorusEnabled && RiffEditor.Instance.initialized) ToggleChorus();
        }

        public void UpdateChorusDepth(Slider slider) {
            AudioSource source = MusicManager.Instance.GetAudioSource(RiffEditor.CurrentRiff.instrument);
            RiffEditor.CurrentRiff.chorusDepth = slider.value;
            source.gameObject.GetComponent<AudioChorusFilter>().depth = RiffEditor.CurrentRiff.chorusDepth;

            Riff riff = RiffEditor.CurrentRiff;
            if (!riff.chorusEnabled && RiffEditor.Instance.initialized) ToggleChorus();
        }

        public void UpdateFlangerRate(Slider slider) {
            AudioSource source = MusicManager.Instance.GetAudioSource(RiffEditor.CurrentRiff.instrument);
            RiffEditor.CurrentRiff.flangerRate = Mathf.PI / 32f + Mathf.PI / 32f * slider.value;
            source.gameObject.GetComponent<AudioFlangerFilter>().rate = RiffEditor.CurrentRiff.flangerRate;

            Riff riff = RiffEditor.CurrentRiff;
            if (!riff.flangerEnabled && RiffEditor.Instance.initialized) ToggleFlanger();
        }


        public void UpdateFlangerDryMix(Slider slider) {
            AudioSource source = MusicManager.Instance.GetAudioSource(RiffEditor.CurrentRiff.instrument);
            RiffEditor.CurrentRiff.flangerDryMix = slider.value;
            source.gameObject.GetComponent<AudioFlangerFilter>().dryMix = RiffEditor.CurrentRiff.flangerDryMix;

            Riff riff = RiffEditor.CurrentRiff;
            if (!riff.flangerEnabled && RiffEditor.Instance.initialized) ToggleFlanger();
        }
    }

    #endregion
}

