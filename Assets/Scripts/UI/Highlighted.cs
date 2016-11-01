// Highlighted.cs
// ©2016 Team 95

using UnityEngine;
using UnityEngine.EventSystems;

namespace Route95.UI {

	public class Highlighted : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

		[SerializeField]
		Fadeable _highlight;

		void Start () {
			_highlight.FadeInstant();
		}

		public void OnPointerEnter (PointerEventData eventData) {
			_highlight.UnFade();
		}

		public void OnPointerExit (PointerEventData eventData) {
			_highlight.Fade();
		}
	}
}
