using Route95.Core;
using Route95.Music;

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Audio;

namespace Route95.UI {

    /// <summary>
    /// Class to handle effect sliders and buttons.
    /// </summary>
    public class EffectButton : MonoBehaviour {

        public Image image;

        /// <summary>
        /// Toggles the active status of the distortion filter.
        /// </summary>
        public void ToggleDistortion() {
            Riff riff = RiffEditor.CurrentRiff;
            Instrument inst = riff.instrument;
            GameObject source = MusicManager.Instance.GetAudioSource(inst).gameObject;

            // Toggle distortion
            AudioDistortionFilter distortion = source.GetComponent<AudioDistortionFilter>();
            distortion.enabled = !distortion.enabled;
            riff.distortionEnabled = distortion.enabled;

            // Update button art
            if (image == null) Debug.Log("shit");
            image.sprite = (distortion.enabled ?
                RiffEditor.Instance.percussionFilled : RiffEditor.Instance.percussionEmpty);

            // Play sound
            if (distortion.enabled) UIManager.Instance.PlayEnableEffectSound();
            else UIManager.Instance.PlayDisableEffectSound();
        }

        /// <summary>
        ///  Toggles the active status of the echo filter.
        /// </summary>
        public void ToggleEcho() {
            Riff riff = RiffEditor.CurrentRiff;
            Instrument inst = riff.instrument;
            GameObject source = MusicManager.Instance.GetAudioSource(inst).gameObject;

            // Toggle echo
            AudioEchoFilter echo = source.GetComponent<AudioEchoFilter>();
            echo.enabled = !echo.enabled;
            riff.echoEnabled = echo.enabled;

            // Update button art
            image.sprite = (echo.enabled ?
                RiffEditor.Instance.percussionFilled : RiffEditor.Instance.percussionEmpty);

            // Play sound
            if (echo.enabled) UIManager.Instance.PlayEnableEffectSound();
            else UIManager.Instance.PlayDisableEffectSound();
        }

        /// <summary>
        /// Toggles the active status of the reverb filter.
        /// </summary>
        public void ToggleReverb() {
            Riff riff = RiffEditor.CurrentRiff;
            Instrument inst = riff.instrument;
            GameObject source = MusicManager.Instance.GetAudioSource(inst).gameObject;

            // Toggle reverb
            AudioReverbFilter reverb = source.GetComponent<AudioReverbFilter>();
            reverb.enabled = !reverb.enabled;
            riff.reverbEnabled = reverb.enabled;

            // Update button art
            image.sprite = (reverb.enabled ?
                RiffEditor.Instance.percussionFilled : RiffEditor.Instance.percussionEmpty);

            // Play sound
            if (reverb.enabled) UIManager.Instance.PlayEnableEffectSound();
            else UIManager.Instance.PlayDisableEffectSound();
        }

        /// <summary>
        /// Toggles the active status of the tremolo filter.
        /// </summary>
        public void ToggleTremolo() {
            Riff riff = RiffEditor.CurrentRiff;
            Instrument inst = riff.instrument;
            GameObject source = MusicManager.Instance.GetAudioSource(inst).gameObject;

            // Toggle tremolo
            AudioTremoloFilter tremolo = source.GetComponent<AudioTremoloFilter>();
            tremolo.enabled = !tremolo.enabled;
            riff.tremoloEnabled = tremolo.enabled;

            // Update button art
            image.sprite = (tremolo.enabled ?
                RiffEditor.Instance.percussionFilled : RiffEditor.Instance.percussionEmpty);

            // Play sound
            if (tremolo.enabled) UIManager.Instance.PlayEnableEffectSound();
            else UIManager.Instance.PlayDisableEffectSound();
        }

        /// <summary>
        /// Toggles the active status of the chorus filter.
        /// </summary>
        public void ToggleChorus() {
            Riff riff = RiffEditor.CurrentRiff;
            Instrument inst = riff.instrument;
            GameObject source = MusicManager.Instance.GetAudioSource(inst).gameObject;

            // Toggle chorus
            AudioChorusFilter chorus = source.GetComponent<AudioChorusFilter>();
            chorus.enabled = !chorus.enabled;
            riff.chorusEnabled = chorus.enabled;

            // Update button art
            image.sprite = (chorus.enabled ?
                RiffEditor.Instance.percussionFilled : RiffEditor.Instance.percussionEmpty);

            // Play sound
            if (chorus.enabled) UIManager.Instance.PlayEnableEffectSound();
            else UIManager.Instance.PlayDisableEffectSound();
        }

        /// <summary>
        /// Toggles the active status of the flanger filter.
        /// </summary>
        public void ToggleFlanger() {
            Riff riff = RiffEditor.CurrentRiff;
            Instrument inst = riff.instrument;
            GameObject source = MusicManager.Instance.GetAudioSource(inst).gameObject;

            // Toggle flanger
            AudioFlangerFilter flanger = source.GetComponent<AudioFlangerFilter>();
            flanger.enabled = !flanger.enabled;
            riff.flangerEnabled = flanger.enabled;

            // Update button art
            image.sprite = (flanger.enabled ?
                RiffEditor.Instance.percussionFilled : RiffEditor.Instance.percussionEmpty);

            // Play sound
            if (flanger.enabled) UIManager.Instance.PlayEnableEffectSound();
            else UIManager.Instance.PlayDisableEffectSound();
        }

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
}

