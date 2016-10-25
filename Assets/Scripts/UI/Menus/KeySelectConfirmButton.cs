// KeySelectConfirmButton.cs
// ©2016 Team95

using UnityEngine;
using UnityEngine.UI;

namespace Route95.UI {

    [RequireComponent(typeof(Button))]
    public class KeySelectConfirmButton : MenuBase<KeySelectConfirmButton> {

		#region Vars

		/// <summary>
		/// Button component.
		/// </summary>
		Button _button;

		#endregion
		#region Unity Callbacks

		new void Awake () {
            // Init vars
            _button = GetComponent<Button>();
        }

		#endregion
		#region Methods

		/// <summary>
		/// Sets the interactable status of this button.
		/// </summary>
		public void SetInteractable (bool val) {
            _button.interactable = val;
        }

		#endregion
	}
}
