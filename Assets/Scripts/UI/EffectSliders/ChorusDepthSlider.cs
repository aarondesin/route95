// ChorusDepthSlider.cs
// ©2016 Team 95

namespace Route95.UI {

    /// <summary>
    /// Class to handle the chorus effect depth slider.
    /// </summary>
    public class ChorusDepthSlider : EffectSlider {

        /// <summary>
        /// Inits the slider.
        /// </summary>
	    public override void Initialize () {
		    UpdateSlider (RiffEditor.CurrentRiff.chorusDepth);
	    }

        /// <summary>
        /// Changes the current riff's chorus effect depth.
        /// </summary>
	    public override void ChangeValue () {
		    RiffEditor.CurrentRiff.chorusDepth = slider.value;
	    }

    }
}