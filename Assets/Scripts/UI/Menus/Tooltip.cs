// Tooltip.cs
// ©2016 Team 95

using UnityEngine.UI;

namespace Route95.UI {

	/// <summary>
	/// Class to handle generic tooltips.
	/// </summary>
	public class Tooltip : MenuBase<Tooltip> {

		#region Tooltip Vars

		Text textObj;

		#endregion
		#region Unity Callbacks

		new void Awake() {
			base.Awake();

			// Init vars
			textObj = GetComponentInChildren<Text>();
		}

		#endregion
		#region Tooltip Methods

		/// <summary>
		/// Sets the text of the tooltip.
		/// </summary>
		public void SetText(string text) {
			textObj.text = text;
		}

		#endregion
	}
}
