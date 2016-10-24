// KeySelectConfirmButton.cs
// ©2016 Team95

using UnityEngine;
using UnityEngine.UI;

namespace Route95.UI {

    [RequireComponent(typeof(Button))]
    public class KeySelectConfirmButton : MenuBase<KeySelectConfirmButton> {

        /// <summary>
        /// Button component.
        /// </summary>
        Button _button;

        new void Awake () {
            // Init vars
            _button = GetComponent<Button>();
        }

        public void SetInteractable (bool val) {
            _button.interactable = val;
        }
       
    }
}
