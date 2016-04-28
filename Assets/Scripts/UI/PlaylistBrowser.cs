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

	void Start () {
		instance = this;
		projectNameInputField.onEndEdit.AddListener( delegate {
			MusicManager.instance.currentProject.name = projectNameInputField.text;
		});
	}

	public void RefreshName () {
		projectNameInputField.text = MusicManager.instance.currentProject.name;
	}

	public void Refresh () {

		if (listings == null) listings = new List<GameObject>();
		else {
			foreach (GameObject listing in listings) Destroy (listing);
			listings.Clear();
		}

		int numSongs = MusicManager.instance.currentProject.songs.Count;
		playlist.sizeDelta = new Vector2 (
			playlist.sizeDelta.x,
			verticalPadding * (numSongs + 2) + buttonHeight * (numSongs + 1)
		);

		// Create song listings
		for (int i=0; i<numSongs; i++) {
			int num = i;
			Song song = MusicManager.instance.currentProject.songs[num];

			//
			// Create button for song
			//
			GameObject listing = UI.MakeButton(song.name);
			listings.Add (listing);

			RectTransform tr = listing.GetComponent<RectTransform>();
			tr.SetParent (playlist);
			tr.sizeDelta = new Vector2 (playlist.GetComponent<RectTransform>().rect.width, buttonHeight);
			tr.localScale = new Vector3 (1f, 1f, 1f);
			tr.localRotation = Quaternion.Euler (0f, 0f, 0f);
			tr.anchorMin = new Vector2 (0f, 1f);
			tr.anchorMax = new Vector2 (0f, 1f);
			tr.anchoredPosition3D = new Vector3 (
				horizontalPadding + tr.sizeDelta.x/2f,
				//((i == 0 ? verticalPadding : verticalPadding + tr.sizeDelta.y)) * -(float)(i+1),
				- verticalPadding - tr.sizeDelta.y / 2f - (verticalPadding + tr.sizeDelta.y) * (float)i,
				0f
			);

			Image img = listing.GetComponent<Image>();
			img.sprite = fillSprite;
			img.color = new Color (0f, 0f, 1f, 0f);


			//
			// Create background for listing
			//
			GameObject listing_bg = UI.MakeImage (song.name + "_bg");
			RectTransform bgtr = listing_bg.GetComponent<RectTransform>();
			bgtr.SetParent (playlist);
			bgtr.sizeDelta = new Vector2 (tr.sizeDelta.x - 2f * horizontalPadding, tr.sizeDelta.y);
			bgtr.localScale = new Vector3 (1f, 1f, 1f);
			bgtr.localRotation = Quaternion.Euler (0f, 0f, 0f);
			bgtr.anchorMin = new Vector2 (0f, 1f);
			bgtr.anchorMax = new Vector2 (0f, 1f);
			bgtr.anchoredPosition3D = new Vector3 (
				horizontalPadding + bgtr.sizeDelta.x / 2f,
				tr.anchoredPosition3D.y,
				0f
			);
			bgtr.SetSiblingIndex(-1);

			Image bgimg = listing_bg.GetComponent<Image>();
			bgimg.raycastTarget = false;
			bgimg.sprite = fillSprite;
			bgimg.color = new Color (1f, 1f, 1f, 0.0f);

			listings.Add(listing_bg);

			//
			// Create song text
			//
			GameObject listing_text = UI.MakeText(song.name+"_text");

			RectTransform ttr = listing_text.GetComponent<RectTransform>();
			ttr.SetParent(tr);
			ttr.sizeDelta = tr.sizeDelta;
			ttr.localScale = new Vector3 (1f, 1f, 1f);
			ttr.localRotation = Quaternion.Euler (0f, 0f, 0f);
			ttr.anchorMin = new Vector2 (0.5f, 0.5f);
			ttr.anchorMin = new Vector2 (0.5f, 0.5f);
			ttr.anchoredPosition3D = new Vector3 (
				horizontalPadding*1.5f + tr.sizeDelta.y, 
				0f, 
				0f
			);

			Text ttxt = listing_text.GetComponent<Text>();
			//ttxt.text = (i+1).ToString() + ". " + song.name + " M: "+song.NumMeasures();
			ttxt.text = (i+1).ToString() + ". " + song.name;
			ttxt.fontSize = fontSize;
			ttxt.color = Color.black;
			ttxt.font = font;
			ttxt.fontStyle = FontStyle.Normal;
			ttxt.alignment = TextAnchor.MiddleLeft;


			//
			// Create remove song button
			//
			GameObject listing_remove = UI.MakeButton(song.name+"_remove");

			RectTransform rtr = listing_remove.GetComponent<RectTransform>();
			rtr.SetParent(tr);
			rtr.sizeDelta = new Vector2 (iconScale * tr.sizeDelta.y, iconScale * tr.sizeDelta.y);
			rtr.localScale = new Vector3 (1f, 1f, 1f);
			rtr.localRotation = Quaternion.Euler (1f, 1f, 1f);
			rtr.anchorMin = new Vector2 (1f, 0.5f);
			rtr.anchorMax = new Vector2 (1f, 0.5f);
			rtr.anchoredPosition3D = new Vector3 (
				-horizontalPadding - rtr.sizeDelta.x, 
				0f, 
				0f
			);

			Image rimg = listing_remove.GetComponent<Image>();
			rimg.color = Color.black;
			rimg.sprite = GameManager.instance.removeIcon;

			Button rbt = listing_remove.GetComponent<Button>();
			rbt.onClick.AddListener(()=>{
				MusicManager.instance.currentProject.songs.RemoveAt(num);
				Refresh();
			});

	
			//
			// Create edit song button
			//
			GameObject listing_edit = UI.MakeButton(song.name+"_edit");

			RectTransform etr = listing_edit.GetComponent<RectTransform>();
			etr.SetParent(tr);
			etr.sizeDelta = new Vector2 (iconScale * tr.sizeDelta.y, iconScale * tr.sizeDelta.y);
			etr.localRotation = Quaternion.Euler (1f, 1f, 1f);
			etr.localScale = new Vector3 (1f, 1f, 1f);
			etr.anchorMin = new Vector2 (1f, 0.5f);
			etr.anchorMax = new Vector2 (1f, 0.5f);
			etr.anchoredPosition3D = new Vector3 (
				rtr.anchoredPosition3D.x -horizontalPadding - etr.sizeDelta.x , 
				0f, 
				0f
			);

			Image eimg = listing_edit.GetComponent<Image>();
			eimg.color = Color.black;
			eimg.sprite = GameManager.instance.editIcon;

			Button ebt = listing_edit.GetComponent<Button>();
			ebt.onClick.AddListener(()=>{
				MusicManager.instance.currentSong = song;
				GameManager.instance.GoToSongArrangeMenu();
			});

			ShowHide sh = listing.GetComponent<ShowHide>(); 
			sh.objects = new List<GameObject>() {
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

				RectTransform utr = listing_up.GetComponent<RectTransform>();
				utr.SetParent(tr);
				utr.sizeDelta = new Vector2 (tr.sizeDelta.y * iconScale / 2f, tr.sizeDelta.y * iconScale / 2f);
				utr.localScale = new Vector3 (1f, 1f, 1f);
				utr.localRotation = Quaternion.Euler (1f, 1f, 1f);
				utr.anchorMin = new Vector2 (0f, 0.5f);
				utr.anchorMax = new Vector2 (0f, 0.5f);
				utr.anchoredPosition3D = new Vector3 (
					horizontalPadding + utr.sizeDelta.x / 2f, 
					utr.sizeDelta.y/2f + verticalPadding / 2f, 
					0f
				);
				utr.localRotation = Quaternion.Euler(0f, 0f, 90f);

				Image uimg = listing_up.GetComponent<Image>();
				uimg.color = Color.black;
				uimg.sprite = GameManager.instance.arrowIcon;

				Button ubt = listing_up.GetComponent<Button>();
				ubt.onClick.AddListener(()=>{
					Song temp = song;
					MusicManager.instance.currentProject.songs[num] = prevSong;
					MusicManager.instance.currentProject.songs[num-1] = temp;
					Refresh();
				});

				sh.objects.Add(listing_up);
			}

			//
			// Create move song down button if not at bottom
			//
			if (num < MusicManager.instance.currentProject.songs.Count-1) {
				Song nextSong = MusicManager.instance.currentProject.songs[i+1];
				GameObject listing_down = UI.MakeButton(song.name+"_down");

				RectTransform dtr = listing_down.GetComponent<RectTransform>();
				dtr.SetParent(tr);
				dtr.sizeDelta = new Vector2 (tr.sizeDelta.y * iconScale / 2f, tr.sizeDelta.y * iconScale / 2f);
				dtr.localScale = new Vector3 (1f, 1f, 1f);
				dtr.localRotation = Quaternion.Euler (1f, 1f, 1f);
				dtr.anchorMin = new Vector2 (0f, 0.5f);
				dtr.anchorMax = new Vector2 (0f, 0.5f);
				dtr.anchoredPosition3D = new Vector3 (
					horizontalPadding + dtr.sizeDelta.x / 2f, 
					 - dtr.sizeDelta.y / 2f - verticalPadding / 2f, 
					0f
				);
				dtr.localRotation = Quaternion.Euler(0f, 0f, -90f);

				Image dimg = listing_down.GetComponent<Image>();
				dimg.color = Color.black;
				dimg.sprite = GameManager.instance.arrowIcon;

				Button dbt = listing_down.GetComponent<Button>();
				dbt.onClick.AddListener(()=>{
					Song temp = song;
					MusicManager.instance.currentProject.songs[num] = nextSong;
					MusicManager.instance.currentProject.songs[num+1] = temp;
					Refresh();
				});

				sh.objects.Add(listing_down);
			}

			foreach (GameObject obj in sh.objects) {
				obj.SetActive(false);
			}
		}

		// Create new song button
		GameObject newSongButton = UI.MakeButton("New Song");
		listings.Add (newSongButton);

		RectTransform ntr = newSongButton.GetComponent<RectTransform>();
		ntr.SetParent (playlist);
		ntr.sizeDelta = new Vector2 (buttonHeight * iconScale, buttonHeight * iconScale);
		ntr.localScale = new Vector3 (1f, 1f, 1f);
		ntr.localRotation = Quaternion.Euler (0f, 0f, 0f);
		ntr.anchorMin = new Vector2 (0f, 1f);
		ntr.anchorMax = new Vector2 (0f, 1f);
		ntr.anchoredPosition3D = new Vector3 (
			horizontalPadding + ntr.sizeDelta.x/2f,
			-verticalPadding - 0.5f * buttonHeight - (verticalPadding + buttonHeight) * (float)(numSongs),
			0f
		);

		Image nimg = ntr.GetComponent<Image>();
		nimg.sprite = GameManager.instance.addIcon;
		nimg.color = new Color (0f, 0f, 0f, 1f);

		ntr.GetComponent<Button>().onClick.AddListener (delegate {
			MusicManager.instance.NewSong();
			GameManager.instance.GoToKeySelectMenu();
		});

		GameObject newSong_text = UI.MakeText("New Song_text");

		RectTransform nttr = newSong_text.GetComponent<RectTransform>();
		nttr.SetParent(ntr);
		nttr.sizeDelta = new Vector2 (playlist.rect.width, ntr.sizeDelta.y);
		nttr.localScale = new Vector3 (1f, 1f, 1f);
		nttr.localRotation = Quaternion.Euler (0f, 0f, 0f);
		nttr.anchorMin = new Vector2 (0f, 0.5f);
		nttr.anchorMin = new Vector2 (0f, 0.5f);
		nttr.anchoredPosition3D = new Vector3 (
			nttr.sizeDelta.x/2f + horizontalPadding + ntr.sizeDelta.x,
			0f, 
			0f
		);

		Text nttxt = newSong_text.GetComponent<Text>();
		nttxt.text = "New Song...";
		nttxt.resizeTextForBestFit = false;
		nttxt.fontSize = (int)(fontSize * iconScale);
		nttxt.color = Color.black;
		nttxt.font = font;
		nttxt.fontStyle = FontStyle.Normal;
		nttxt.alignment = TextAnchor.MiddleLeft;

		// Create load song button
		GameObject loadNewSongButton = UI.MakeButton("Load New Song");
		listings.Add (loadNewSongButton);

		RectTransform lntr = loadNewSongButton.GetComponent<RectTransform>();
		lntr.SetParent (playlist);
		lntr.sizeDelta = new Vector2 (buttonHeight * iconScale, buttonHeight * iconScale);
		lntr.localScale = new Vector3 (1f, 1f, 1f);
		lntr.localRotation = Quaternion.Euler (0f, 0f, 0f);
		lntr.anchorMin = new Vector2 (0.5f, 1f);
		lntr.anchorMax = new Vector2 (0.5f, 1f);
		lntr.anchoredPosition3D = new Vector3 (
			horizontalPadding + lntr.sizeDelta.x / 2f,
			-verticalPadding - 0.5f * buttonHeight - (verticalPadding + buttonHeight) * (float)(numSongs),
			0f
		);

		Image lnimg = lntr.GetComponent<Image>();
		lnimg.sprite = GameManager.instance.loadIcon;
		lnimg.color = new Color (0f, 0f, 0f, 1f);

		lntr.GetComponent<Button>().onClick.AddListener (delegate {
			GameManager.instance.ShowLoadPromptForSongs();
		});

		GameObject loadNewSong_text = UI.MakeText("Load New Song_text");

		RectTransform lnttr = loadNewSong_text.GetComponent<RectTransform>();
		lnttr.SetParent(lntr);
		lnttr.sizeDelta = new Vector2 (playlist.rect.width, ntr.sizeDelta.y);
		lnttr.localScale = new Vector3 (1f, 1f, 1f);
		lnttr.localRotation = Quaternion.Euler (0f, 0f, 0f);
		lnttr.anchorMin = new Vector2 (0f, 0.5f);
		lnttr.anchorMin = new Vector2 (0f, 0.5f);
		lnttr.anchoredPosition3D = new Vector3 (
			lnttr.sizeDelta.x/2f + horizontalPadding + ntr.sizeDelta.x,
			0f, 
			0f
		);

		Text lnttxt = loadNewSong_text.GetComponent<Text>();
		lnttxt.text = "Load Song...";
		lnttxt.resizeTextForBestFit = false;
		lnttxt.fontSize = (int)(fontSize * iconScale);
		lnttxt.color = Color.black;
		lnttxt.font = font;
		lnttxt.fontStyle = FontStyle.Normal;
		lnttxt.alignment = TextAnchor.MiddleLeft;

		GameObject loadNewSong_highlight = UI.MakeImage ("Load New Song Highlight");
		listings.Add (loadNewSong_highlight);

		RectTransform lnshtr = loadNewSong_highlight.GetComponent<RectTransform>();
		lnshtr.SetParent (lntr);
		lnshtr.sizeDelta = new Vector2 (playlist.rect.width/2f, ntr.sizeDelta.y);
		lnshtr.localScale = new Vector3 (1f, 1f, 1f);
		lnshtr.localRotation = Quaternion.Euler (1f, 1f, 1f);
		lnshtr.anchorMin = new Vector2 (0f, 0.5f);
		lnshtr.anchorMax = new Vector2 (0f, 0.5f);
		lnshtr.anchoredPosition3D = new Vector3 (
			0f, 0f, 0f
		);

		lnshtr.SetSiblingIndex(-1);

		Image lnshimg = lnshtr.GetComponent<Image>();
		lnshimg.raycastTarget = false;
		lnshimg.sprite = fillSprite;
		lnshimg.color = new Color (1f, 1f, 1f, 0.5f);

		Fadeable lnshf = loadNewSong_highlight.AddComponent<Fadeable>();
		lnshf.startFaded = true;
	
	}
		
}
