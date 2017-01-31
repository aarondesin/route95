// KeySelectConfirmButton.cs
// ©2016 Team95

using Route95.Music;

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
			base.Awake();

            // Init vars
            _button = GetComponent<Button>();
        }

		void Start () {
			UIManager.Instance.onSwitchToKeySelectMenu.AddListener(()=> {
				SetInteractable(MusicManager.Instance.CurrentSong.Scale != -1 &&
					MusicManager.Instance.CurrentSong.Key != Key.None);
			});
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
