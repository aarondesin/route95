using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlaylistBrowser : MonoBehaviour {
	public static PlaylistBrowser instance;

	public InputField projectNameInputField;

	public RectTransform playlist;
	public float buttonHeight;
	public float horizontalPadding;
	public float verticalPadding;
	[Tooltip("Scale of edit/remove icons compared to button height.")]
	public float iconScale;
	public Sprite fillSprite;
	public int fontSize;
	public Font font;

	private List<GameObject> listings;

	#region Unity Callbacks

	void Awake () {
		instance = this;
		projectNameInputField.onEndEdit.AddListener( delegate {
			MusicManager.instance.currentProject.name = projectNameInputField.text;
		});
		listings = new List<GameObject>();
	}

	#endregion
	#region PlaylistBrowser Callbacks

	public void RefreshName () {
		projectNameInputField.text = MusicManager.instance.currentProject.name;
	}

	public void Refresh () {
		//
		// Clear all listings and buttons
		//
		foreach (GameObject listing in listings) Destroy (listing);
		listings.Clear();

		//
		// Resize browser panel
		//
		int numSongs = MusicManager.instance.currentProject.songs.Count;
		playlist.sizeDelta = new Vector2 (
			playlist.sizeDelta.x,
			verticalPadding * (numSongs + 2) + buttonHeight * (numSongs + 1)
		);

		//
		// Create song listings
		//
		for (int i = 0; i < numSongs; i++) {
			int num = i;
			Song song = MusicManager.instance.currentProject.songs[num];

			//
			// Create listing for song
			//
			GameObject listing = UI.MakeButton(song.name);
			listings.Add (listing);

			RectTransform listing_tr = listing.RectTransform();
			listing_tr.SetParent (playlist);
			listing_tr.sizeDelta = new Vector2 (playlist.GetComponent<RectTransform>().rect.width, buttonHeight);
			listing_tr.AnchorAtPoint(0f, 1f);
			listing_tr.anchoredPosition3D = new Vector3 (
				horizontalPadding + listing_tr.sizeDelta.x/2f,
				- verticalPadding - listing_tr.sizeDelta.y / 2f - (verticalPadding + listing_tr.sizeDelta.y) * (float)i,
				0f
			);
			listing_tr.ResetScaleRot();

			Image listing_img = listing.Image();
			listing_img.sprite = fillSprite;
			listing_img.color = new Color (0f, 0f, 1f, 0f);

			ShowHide listing_sh = listing.ShowHide();

			//
			// Create background for listing
			//
			GameObject listing_bg = UI.MakeImage (song.name + "_bg");
			RectTransform listing_bg_tr = listing_bg.RectTransform();
			listing_bg_tr.SetParent (playlist);
			listing_bg_tr.sizeDelta = new Vector2 (listing_tr.sizeDelta.x - 2f * horizontalPadding, listing_tr.sizeDelta.y);
			listing_bg_tr.AnchorAtPoint (0f ,1f);
			listing_bg_tr.anchoredPosition3D = new Vector3 (
				horizontalPadding + listing_tr.sizeDelta.x / 2f,
				listing_tr.anchoredPosition3D.y,
				0f
			);
			listing_bg_tr.SetSiblingIndex(-1);
			listing_bg_tr.ResetScaleRot();

			Image listing_bg_img = listing_bg.Image();
			listing_bg_img.raycastTarget = false;
			listing_bg_img.sprite = fillSprite;
			listing_bg_img.color = new Color (1f, 1f, 1f, 0.0f);

			listings.Add(listing_bg);

			//
			// Create song text
			//
			GameObject listing_text = UI.MakeText(song.name+"_text");

			RectTransform listing_text_tr = listing_text.RectTransform();
			listing_text_tr.SetParent(listing_tr);
			listing_text_tr.sizeDelta = listing_tr.sizeDelta;
			listing_text_tr.AnchorAtPoint (0.5f, 0.5f);
			listing_text_tr.anchoredPosition3D = new Vector3 (horizontalPadding*1.5f + listing_tr.sizeDelta.y, 0f, 0f);
			listing_text_tr.ResetScaleRot();

			Text listing_text_txt = listing_text.Text();
			listing_text_txt.text = (i+1).ToString() + ". " + song.name;
			listing_text_txt.fontSize = fontSize;
			listing_text_txt.color = Color.black;
			listing_text_txt.font = font;
			listing_text_txt.alignment = TextAnchor.MiddleLeft;


			//
			// Create remove song button
			//
			GameObject listing_remove = UI.MakeButton(song.name+"_remove");

			RectTransform listing_remove_tr = listing_remove.RectTransform();
			listing_remove_tr.SetParent(listing_tr);
			listing_remove_tr.sizeDelta = new Vector2 (iconScale * listing_tr.sizeDelta.y, iconScale * listing_tr.sizeDelta.y);
			listing_remove_tr.AnchorAtPoint (1f, 0.5f);
			listing_remove_tr.anchoredPosition3D = new Vector3 (-horizontalPadding - listing_remove_tr.sizeDelta.x, 0f, 0f);
			listing_remove_tr.ResetScaleRot();

			Image listing_remove_img = listing_remove.Image();
			listing_remove_img.color = Color.black;
			listing_remove_img.sprite = GameManager.instance.removeIcon;

			Button listing_remove_button = listing_remove.Button();
			listing_remove_button.onClick.AddListener(()=>{
				GameManager.instance.MenuClick();
				MusicManager.instance.currentProject.RemoveSong(num);
				Refresh();
			});

	
			//
			// Create edit song button
			//
			GameObject listing_edit = UI.MakeButton(song.name+"_edit");

			RectTransform listing_edit_tr = listing_edit.RectTransform();
			listing_edit_tr.SetParent(listing_tr);
			listing_edit_tr.sizeDelta = new Vector2 (iconScale * listing_tr.sizeDelta.y, iconScale * listing_tr.sizeDelta.y);
			listing_edit_tr.AnchorAtPoint (1f, 0.5f);
			listing_edit_tr.anchoredPosition3D = new Vector3 (
				listing_remove_tr.anchoredPosition3D.x -horizontalPadding - listing_edit_tr.sizeDelta.x , 
				0f, 
				0f
			);
			listing_edit_tr.ResetScaleRot();

			Image listing_edit_img = listing_edit.Image();
			listing_edit_img.color = Color.black;
			listing_edit_img.sprite = GameManager.instance.editIcon;

			Button listing_edit_button = listing_edit.Button();
			listing_edit_button.onClick.AddListener(()=>{
				GameManager.instance.MenuClick();
				MusicManager.instance.currentSong = song;
				GameManager.instance.GoToSongArrangeMenu();
			});
				
			listing_sh.objects = new List<GameObject>() {
				listing_edit,
				listing_remove,
				listing_bg
			};


			//
			// Create move song up button if not at top
			//
			if (num > 0) {
				Song prevSong = MusicManager.instance.currentProject.songs[i-1];
				GameObject listing_up = UI.MakeButton(song.name+"_up");

				RectTransform listing_up_tr = listing_up.RectTransform();
				listing_up_tr.SetParent(listing_tr);
				listing_up_tr.sizeDelta = new Vector2 (listing_tr.sizeDelta.y * iconScale / 2f, listing_tr.sizeDelta.y * iconScale / 2f);
				listing_up_tr.ResetScaleRot();
				listing_up_tr.localRotation = Quaternion.Euler(0f, 0f, 90f);
				listing_up_tr.AnchorAtPoint (0f, 0.5f);
				listing_up_tr.anchoredPosition3D = new Vector3 (
					horizontalPadding + listing_up_tr.sizeDelta.x / 2f, 
					listing_up_tr.sizeDelta.y/2f + verticalPadding / 2f, 
					0f
				);
					
				Image listing_up_img = listing_up.Image();
				listing_up_img.color = Color.black;
				listing_up_img.sprite = GameManager.instance.arrowIcon;

				Button listing_up_button = listing_up.Button();
				listing_up_button.onClick.AddListener(()=>{
					GameManager.instance.MenuClick();
					Song temp = song;
					MusicManager.instance.currentProject.songs[num] = prevSong;
					MusicManager.instance.currentProject.songs[num-1] = temp;
					Refresh();
				});

				listing_sh.objects.Add(listing_up);
			}

			//
			// Create move song down button if not at bottom
			//
			if (num < MusicManager.instance.currentProject.songs.Count-1) {
				Song nextSong = MusicManager.instance.currentProject.songs[i+1];
				GameObject listing_down = UI.MakeButton(song.name+"_down");

				RectTransform listing_down_tr = listing_down.RectTransform();
				listing_down_tr.SetParent(listing_tr);
				listing_down_tr.sizeDelta = new Vector2 (listing_tr.sizeDelta.y * iconScale / 2f, listing_tr.sizeDelta.y * iconScale / 2f);
				listing_down_tr.ResetScaleRot();
				listing_down_tr.localRotation = Quaternion.Euler(0f, 0f, -90f);
				listing_down_tr.AnchorAtPoint (0f, 0.5f);
				listing_down_tr.anchoredPosition3D = new Vector3 (
					horizontalPadding + listing_down_tr.sizeDelta.x / 2f, 
					- listing_down_tr.sizeDelta.y / 2f - verticalPadding / 2f, 
					0f
				);
					
				Image listing_down_img = listing_down.Image();
				listing_down_img.color = Color.black;
				listing_down_img.sprite = GameManager.instance.arrowIcon;

				Button listing_down_button = listing_down.Button();
				listing_down_button.onClick.AddListener(()=>{
					GameManager.instance.MenuClick();
					Song temp = song;
					MusicManager.instance.currentProject.songs[num] = nextSong;
					MusicManager.instance.currentProject.songs[num+1] = temp;
					Refresh();
				});

				listing_sh.objects.Add(listing_down);
			}

			foreach (GameObject obj in listing_sh.objects) obj.SetActive(false);
		}

		//
		// Create new song button
		//
		GameObject newSongButton = UI.MakeButton("New Song");
		listings.Add (newSongButton);

		RectTransform newSongButton_tr = newSongButton.RectTransform();
		newSongButton_tr.SetParent (playlist);
		newSongButton_tr.sizeDelta = new Vector2 (buttonHeight * iconScale, buttonHeight * iconScale);
		newSongButton_tr.AnchorAtPoint (0f, 1f);
		newSongButton_tr.anchoredPosition3D = new Vector3 (
			horizontalPadding + newSongButton_tr.sizeDelta.x/2f,
			-verticalPadding - 0.5f * buttonHeight - (verticalPadding + buttonHeight) * (float)(numSongs),
			0f
		);
		newSongButton_tr.ResetScaleRot();

		Image newSong_img = newSongButton.Image();
		newSong_img.sprite = GameManager.instance.addIcon;
		newSong_img.color = Color.black;

		newSongButton.Button().onClick.AddListener (delegate {
			GameManager.instance.MenuClick();
			MusicManager.instance.NewSong();
			GameManager.instance.GoToKeySelectMenu();
		});

		GameObject newSong_text = UI.MakeText("New Song_text");

		RectTransform newSong_text_tr = newSong_text.RectTransform();
		newSong_text_tr.SetParent(newSongButton_tr);
		newSong_text_tr.sizeDelta = new Vector2 (playlist.rect.width/2f - 2f*horizontalPadding, newSongButton_tr.sizeDelta.y);
		newSong_text_tr.AnchorAtPoint (0f, 0.5f);
		newSong_text_tr.anchoredPosition3D = new Vector3 (
			newSong_text_tr.sizeDelta.x/2f + horizontalPadding + newSongButton_tr.sizeDelta.x,
			0f, 
			0f
		);
		newSong_text_tr.ResetScaleRot();

		Text newSong_text_txt = newSong_text.Text();
		newSong_text_txt.text = "New Song...";
		newSong_text_txt.fontSize = (int)(fontSize * iconScale);
		newSong_text_txt.color = Color.black;
		newSong_text_txt.font = font;
		newSong_text_txt.alignment = TextAnchor.MiddleLeft;

		GameObject newSongButton_highlight = UI.MakeImage ("Highlight (New Song Button)");
		listings.Add (newSongButton_highlight);

		RectTransform newSongButton_highlight_tr = newSongButton_highlight.RectTransform();
		newSongButton_highlight_tr.SetParent (newSongButton_tr);
		newSongButton_highlight_tr.sizeDelta = new Vector2 (playlist.rect.width*0.5f - 2f*horizontalPadding, newSongButton_tr.sizeDelta.y*1.5f);
		newSongButton_highlight_tr.AnchorAtPoint (0f, 0.5f);
		newSongButton_highlight_tr.anchoredPosition3D = new Vector3 (newSongButton_highlight_tr.sizeDelta.x/3f, 0f, 0f);
		newSongButton_highlight_tr.ResetScaleRot();

		newSongButton_highlight_tr.SetSiblingIndex(-1);

		Image newSongButton_highlight_img = newSongButton_highlight.Image();
		newSongButton_highlight_img.raycastTarget = false;
		newSongButton_highlight_img.sprite = GameManager.instance.scribbleCircle;
		newSongButton_highlight_img.color = new Color (1f, 1f, 1f, 1f);

		ShowHide newSongButton_sh = newSongButton.ShowHide();
		newSongButton_sh.objects = new List<GameObject>() { newSongButton_highlight };
		newSongButton_sh.transitionType = TransitionType.Instant;
		newSongButton_sh.Hide();

		//
		// Create load song button
		//
		GameObject loadSongButton = UI.MakeButton("Load Song Button (Playlist Browser)");
		listings.Add (loadSongButton);

		RectTransform loadSongButton_tr = loadSongButton.RectTransform();
		loadSongButton_tr.SetParent (playlist);
		loadSongButton_tr.sizeDelta = new Vector2 (buttonHeight * iconScale, buttonHeight * iconScale);
		loadSongButton_tr.AnchorAtPoint (0.5f, 1f);
		loadSongButton_tr.anchoredPosition3D = new Vector3 (
			horizontalPadding + loadSongButton_tr.sizeDelta.x / 2f,
			-verticalPadding - 0.5f * buttonHeight - (verticalPadding + buttonHeight) * (float)(numSongs),
			0f
		);
		loadSongButton_tr.ResetScaleRot();

		Image loadSongButton_img = loadSongButton.Image();
		loadSongButton_img.sprite = GameManager.instance.loadIcon;
		loadSongButton_img.color = Color.black;

		loadSongButton.Button().onClick.AddListener (delegate {
			GameManager.instance.MenuClick();
			GameManager.instance.ShowLoadPromptForSongs();
		});

		GameObject loadSongButton_text = UI.MakeText("Load New Song_text");

		RectTransform loadSongButton_text_tr = loadSongButton_text.RectTransform();
		loadSongButton_text_tr.SetParent(loadSongButton_tr);
		loadSongButton_text_tr.sizeDelta = new Vector2 (playlist.rect.width/2f-2f*horizontalPadding, loadSongButton_tr.sizeDelta.y);
		loadSongButton_text_tr.AnchorAtPoint (0f, 0.5f);
		loadSongButton_text_tr.anchoredPosition3D = new Vector3 (
			loadSongButton_text_tr.sizeDelta.x/2f + horizontalPadding + loadSongButton_tr.sizeDelta.x,
			0f, 
			0f
		);
		loadSongButton_text_tr.ResetScaleRot();

		Text loadSongButton_text_txt = loadSongButton_text.Text();
		loadSongButton_text_txt.text = "Load Song...";
		loadSongButton_text_txt.fontSize = (int)(fontSize * iconScale);
		loadSongButton_text_txt.color = Color.black;
		loadSongButton_text_txt.font = font;
		loadSongButton_text_txt.alignment = TextAnchor.MiddleLeft;

		GameObject loadSongButton_highlight = UI.MakeImage ("Load New Song Highlight");
		listings.Add (loadSongButton_highlight);

		RectTransform loadSongButton_highlight_tr = loadSongButton_highlight.RectTransform();
		loadSongButton_highlight_tr.SetParent (loadSongButton_tr);
		loadSongButton_highlight_tr.sizeDelta = new Vector2 (playlist.rect.width*0.5f, loadSongButton_tr.sizeDelta.y*1.5f);
		loadSongButton_highlight_tr.AnchorAtPoint (0f, 0.5f);
		loadSongButton_highlight_tr.anchoredPosition3D = new Vector3 (loadSongButton_highlight_tr.sizeDelta.x/3f, 0f, 0f);
		loadSongButton_highlight_tr.ResetScaleRot();

		loadSongButton_highlight_tr.SetSiblingIndex(-1);

		Image loadSongButton_highlight_img = loadSongButton_highlight.Image();
		loadSongButton_highlight_img.raycastTarget = false;
		loadSongButton_highlight_img.sprite = GameManager.instance.scribbleCircle;
		loadSongButton_highlight_img.color = new Color (1f, 1f, 1f, 1f);

		ShowHide loadSongButton_sh = loadSongButton.ShowHide();
		loadSongButton_sh.objects = new List<GameObject>() { loadSongButton_highlight };
		loadSongButton_sh.transitionType = TransitionType.Instant;
		loadSongButton_sh.Hide();
	
	}

	#endregion
		
}
