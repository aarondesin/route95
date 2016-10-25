// Tooltippable.cs
// ©2016 Team 95

using UnityEngine;
using UnityEngine.EventSystems;

namespace Route95.UI {

    /// <summary>
    /// Class to enable a UI object to have a tooltip displayed for it.
    /// </summary>
    public class Tooltippable : MonoBehaviour, IPointerEnterHandler, 
		IPointerExitHandler, IPointerClickHandler 
	{

		#region Vars

		/// <summary>
		/// Message to show when hovered.
		/// </summary>
		[Tooltip("Message to show when hovered.")]
		[SerializeField]
        string _message;

		#endregion
		#region Unity Callbacks

		public void OnPointerEnter(PointerEventData e) {
            UIManager.Instance.ShowMenu(Tooltip.Instance);
            Tooltip.Instance.SetText(_message);
        }

        public void OnPointerExit(PointerEventData e) {
            UIManager.Instance.HideMenu(Tooltip.Instance);
        }

        public void OnPointerClick(PointerEventData e) {
            UIManager.Instance.HideMenu(Tooltip.Instance);
        }

		#endregion
		#region Properties

		/// <summary>
		/// Gets/sets the tooltip message for this object.
		/// </summary>
		public string Message {
			get { return _message; }
			set { _message = value; }
		}

		#endregion
	}
}
