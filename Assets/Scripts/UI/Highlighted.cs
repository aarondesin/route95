// Highlighted.cs
// ©2016 Team 95

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Route95.UI {

	public class Highlighted : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

		[SerializeField]
		Fadeable _highlightObject;

		Selectable _selectable;

		void Awake () {
			_selectable = GetComponent<Selectable>();
		}

		void Start () {
			_highlightObject.FadeInstant();
		}

		public void OnPointerEnter (PointerEventData eventData) {
			if (!_selectable || _selectable.interactable)
				_highlightObject.UnFade();
		}

		public void OnPointerExit (PointerEventData eventData) {
			_highlightObject.Fade();
		}

		public Fadeable HighlightObject { 
			get { return _highlightObject; }
			set { _highlightObject = value; }
		}
	}
}
