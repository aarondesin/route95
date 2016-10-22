using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace Route95.UI {

    /// <summary>
    /// Class to enable a UI object to have a tooltip displayed for it.
    /// </summary>
    public class Tooltippable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

        [Tooltip("Message to show when hovered.")]
        public string message;

        public void OnPointerEnter(PointerEventData e) {
            UIManager.Instance.ShowMenu(Tooltip.Instance);
            Tooltip.Instance.SetText(message);
        }

        public void OnPointerExit(PointerEventData e) {
            UIManager.Instance.HideMenu(Tooltip.Instance);
        }

        public void OnPointerClick(PointerEventData e) {
            UIManager.Instance.HideMenu(Tooltip.Instance);
        }
    }
}
