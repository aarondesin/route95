// MusicVideoText.cs
// ©2016 Team 95

using Route95.Music;

using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace Route95.UI {

	public class MusicVideoText : MenuBase<MusicVideoText> {

		#region Vars

		[SerializeField]
		float _duration;

		Text _text;

		#endregion
		#region Unity Callbacks

		new void Awake() {
			base.Awake();

			// Init vars
			_text = GetComponentInChildren<Text>();
		}

		void Start() {
			MusicManager.Instance.onStartSong.AddListener(ShowText);
		}

		#endregion
		#region Methods

		void SetText (Project project, Song song) {
			_text.text = song.ArtistName + "\n\"" +
				song.Name + "\"\n" +
				project.Name + "\nTeam 95 Studios";
		}
 
		public void ShowText () {
			SetText (
				MusicManager.Instance.CurrentProject,
				MusicManager.Instance.CurrentSong
				);
			StartCoroutine (DoShowText());
		}

		IEnumerator DoShowText () {

			Show ();
			
			while (!IsFullyVisible) yield return null;

			yield return new WaitForSeconds (_duration);

			Hide ();

			yield break;
		}

		#endregion
	}
}
