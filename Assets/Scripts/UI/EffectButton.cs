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
            Riff riff = RiffEditor.currentRiff;
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
            else GameManager.Instance.EffectsOff();
        }

        /// <summary>
        ///  Toggles the active status of the echo filter.
        /// </summary>
        public void ToggleEcho() {
            Riff riff = RiffEditor.currentRiff;
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
            if (echo.enabled) UIManager.Instance.PlayEnableEffectSound
            else GameManager.Instance.EffectsOff();
        }

        /// <summary>
        /// Toggles the active status of the reverb filter.
        /// </summary>
        public void ToggleReverb() {
            Riff riff = RiffEditor.currentRiff;
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
            if (reverb.enabled) UIManager.Instance.PlayEnableEffectSound
            else GameManager.Instance.EffectsOff();
        }

        /// <summary>
        /// Toggles the active status of the tremolo filter.
        /// </summary>
        public void ToggleTremolo() {
            Riff riff = RiffEditor.currentRiff;
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
            if (tremolo.enabled) UIManager.Instance.PlayEnableEffectSound
            else GameManager.Instance.EffectsOff();
        }

        /// <summary>
        /// Toggles the active status of the chorus filter.
        /// </summary>
        public void ToggleChorus() {
            Riff riff = RiffEditor.currentRiff;
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
            if (chorus.enabled) UIManager.Instance.PlayEnableEffectSound
            else GameManager.Instance.EffectsOff();
        }

        /// <summary>
        /// Toggles the active status of the flanger filter.
        /// </summary>
        public void ToggleFlanger() {
            Riff riff = RiffEditor.currentRiff;
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
            if (flanger.enabled) UIManager.Instance.PlayEnableEffectSound
            else GameManager.Instance.EffectsOff();
        }

        public void updatedistortionLevel(Slider slider) {
            AudioSource source = MusicManager.Instance.instrumentAudioSources[RiffEditor.currentRiff.instrument];
            RiffEditor.currentRiff.distortionLevel = slider.value;
            source.gameObject.GetComponent<AudioDistortionFilter>().distortionLevel = RiffEditor.currentRiff.distortionLevel;
            //instrumentAudioSources[MelodicInstrument.ElectricGuitar].gameObject.GetComponent<AudioDistortionFilter>().distortionLevel = 0.9f;

            Riff riff = RiffEditor.currentRiff;
            if (!riff.distortionEnabled && RiffEditor.Instance.initialized) ToggleDistortion();
        }

        public void updateechoDecayRatio(Slider slider) {
            AudioSource source = MusicManager.Instance.instrumentAudioSources[RiffEditor.currentRiff.instrument];
            RiffEditor.currentRiff.echoDecayRatio = slider.value;
            source.gameObject.GetComponent<AudioEchoFilter>().decayRatio = RiffEditor.currentRiff.echoDecayRatio;

            Riff riff = RiffEditor.currentRiff;
            if (!riff.echoEnabled && RiffEditor.Instance.initialized) ToggleEcho();
        }


        public void updateechoDelay(Slider slider) {
            AudioSource source = MusicManager.Instance.instrumentAudioSources[RiffEditor.currentRiff.instrument];
            RiffEditor.currentRiff.echoDelay = slider.value;
            source.gameObject.GetComponent<AudioEchoFilter>().delay = RiffEditor.currentRiff.echoDelay;

            Riff riff = RiffEditor.currentRiff;
            if (!riff.echoEnabled && RiffEditor.Instance.initialized) ToggleEcho();
        }


        public void updateechoDryMix(Slider slider) {
            AudioSource source = MusicManager.Instance.instrumentAudioSources[RiffEditor.currentRiff.instrument];
            RiffEditor.currentRiff.echoDryMix = slider.value;
            source.gameObject.GetComponent<AudioEchoFilter>().dryMix = RiffEditor.currentRiff.echoDryMix;

            Riff riff = RiffEditor.currentRiff;
            if (!riff.echoEnabled && RiffEditor.Instance.initialized) ToggleEcho();
        }

        public void updatereverbDecayTime(Slider slider) {
            AudioSource source = MusicManager.Instance.instrumentAudioSources[RiffEditor.currentRiff.instrument];
            RiffEditor.currentRiff.reverbDecayTime = slider.value;
            source.gameObject.GetComponent<AudioReverbFilter>().decayTime = RiffEditor.currentRiff.reverbDecayTime;

            Riff riff = RiffEditor.currentRiff;
            if (!riff.reverbEnabled && RiffEditor.Instance.initialized) ToggleReverb();
        }


        public void updatereverbLevel(Slider slider) {
            AudioSource source = MusicManager.Instance.instrumentAudioSources[RiffEditor.currentRiff.instrument];
            RiffEditor.currentRiff.reverbLevel = slider.value;
            source.gameObject.GetComponent<AudioReverbFilter>().reverbLevel = RiffEditor.currentRiff.reverbLevel;

            Riff riff = RiffEditor.currentRiff;
            if (!riff.reverbEnabled && RiffEditor.Instance.initialized) ToggleReverb();
        }

        public void updatetremoloRate(Slider slider) {
            AudioSource source = MusicManager.Instance.instrumentAudioSources[RiffEditor.currentRiff.instrument];
            RiffEditor.currentRiff.tremoloRate = slider.value;
            source.gameObject.GetComponent<AudioTremoloFilter>().rate = RiffEditor.currentRiff.tremoloRate;

            Riff riff = RiffEditor.currentRiff;
            if (!riff.tremoloEnabled && RiffEditor.Instance.initialized) ToggleTremolo();
        }

        public void updatetremoloDepth(Slider slider) {
            AudioSource source = MusicManager.Instance.instrumentAudioSources[RiffEditor.currentRiff.instrument];
            RiffEditor.currentRiff.tremoloDepth = slider.value;
            source.gameObject.GetComponent<AudioTremoloFilter>().depth = RiffEditor.currentRiff.tremoloDepth;

            Riff riff = RiffEditor.currentRiff;
            if (!riff.tremoloEnabled && RiffEditor.Instance.initialized) ToggleTremolo();
        }

        public void updatechorusDryMix(Slider slider) {
            AudioSource source = MusicManager.Instance.instrumentAudioSources[RiffEditor.currentRiff.instrument];
            RiffEditor.currentRiff.chorusDryMix = slider.value;
            source.gameObject.GetComponent<AudioChorusFilter>().dryMix = RiffEditor.currentRiff.chorusDryMix;

            Riff riff = RiffEditor.currentRiff;
            if (!riff.chorusEnabled && RiffEditor.Instance.initialized) ToggleChorus();
        }

        public void updatechorusRate(Slider slider) {
            AudioSource source = MusicManager.Instance.instrumentAudioSources[RiffEditor.currentRiff.instrument];
            RiffEditor.currentRiff.chorusRate = slider.value;
            source.gameObject.GetComponent<AudioChorusFilter>().rate = RiffEditor.currentRiff.chorusRate;

            Riff riff = RiffEditor.currentRiff;
            if (!riff.chorusEnabled && RiffEditor.Instance.initialized) ToggleChorus();
        }

        public void updatechorusDepth(Slider slider) {
            AudioSource source = MusicManager.Instance.instrumentAudioSources[RiffEditor.currentRiff.instrument];
            RiffEditor.currentRiff.chorusDepth = slider.value;
            source.gameObject.GetComponent<AudioChorusFilter>().depth = RiffEditor.currentRiff.chorusDepth;

            Riff riff = RiffEditor.currentRiff;
            if (!riff.chorusEnabled && RiffEditor.Instance.initialized) ToggleChorus();
        }

        public void updateflangerRate(Slider slider) {
            AudioSource source = MusicManager.Instance.instrumentAudioSources[RiffEditor.currentRiff.instrument];
            RiffEditor.currentRiff.flangerRate = Mathf.PI / 32f + Mathf.PI / 32f * slider.value;
            source.gameObject.GetComponent<AudioFlangerFilter>().rate = RiffEditor.currentRiff.flangerRate;

            Riff riff = RiffEditor.currentRiff;
            if (!riff.flangerEnabled && RiffEditor.Instance.initialized) ToggleFlanger();
        }


        public void updateflangerDryMix(Slider slider) {
            AudioSource source = MusicManager.Instance.instrumentAudioSources[RiffEditor.currentRiff.instrument];
            RiffEditor.currentRiff.flangerDryMix = slider.value;
            source.gameObject.GetComponent<AudioFlangerFilter>().dryMix = RiffEditor.currentRiff.flangerDryMix;

            Riff riff = RiffEditor.currentRiff;
            if (!riff.flangerEnabled && RiffEditor.Instance.initialized) ToggleFlanger();
        }
    }
}

