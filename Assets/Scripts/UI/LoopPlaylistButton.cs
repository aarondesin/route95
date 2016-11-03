// LoopPlaylistButton.cs
// ©2016 Team 95

using Route95.Music;

using UnityEngine;
using UnityEngine.UI;

namespace Route95.UI {

	public class LoopPlaylistButton : MonoBehaviour {

		Button _button;

		Image _buttonSprite;

		void Awake () {
			_button = GetComponent<Button>();
			_buttonSprite = GetComponent<Image>();

			_button.onClick.AddListener(()=> {
				if (MusicManager.Instance.LoopPlaylist)
					_buttonSprite.sprite = UIManager.Instance.FilledPercussionNoteIcon;
				else _buttonSprite.sprite = UIManager.Instance.CircleIcon;
			});
		}
	}
}