// MenuBase.cs
// ©2016 Team 95

using UnityEngine;

namespace Route95.UI {

	[RequireComponent(typeof(Fadeable))]
	[RequireComponent(typeof(CanvasGroup))]
	public abstract class MenuBase<T> : SingletonMonoBehaviour<T>
		where T : MonoBehaviour {
		Fadeable _fade;

		new protected void Awake() {
			base.Awake();

			// Init vars
			_fade = GetComponent<Fadeable>();
		}

		public void Show() {
			_fade.UnFade();
		}

		public void Hide() {
			_fade.Fade();
		}
	}
}
