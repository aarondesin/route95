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
	public Sprite fillSprite;
	public int fontSize;
	public Font font;

	private List<GameObject> listings;

	void Start () {
		instance = this;
	}

	void Refresh () {
		if (listings == null) listings = new List<GameObject>();
		else {
			foreach (GameObject listing in listings) Destroy (listing);
			listings.Clear();
		}

		for (int i=0; i<MusicManager.instance.currentProject.songs.Count; i++) {
			int num = i;
			Song song = MusicManager.instance.currentProject.songs[num];


			//
			// Create button for song
			//
			GameObject listing = UI.MakeButton(song.name);
			listings.Add (listing);

			RectTransform tr = listing.GetComponent<RectTransform>();
			tr.SetParent (playlist);
			tr.sizeDelta = new Vector2 (GetComponent<RectTransform>().rect.width, buttonHeight);
			tr.localScale = new Vector3 (1f, 1f, 1f);
			tr.anchorMin = new Vector2 (0f, 1f);
			tr.anchorMax = new Vector2 (0f, 1f);
			tr.anchoredPosition3D = new Vector3 (
				horizontalPadding + tr.sizeDelta.x/2f,
				((i == 0 ? 0f : verticalPadding) + tr.sizeDelta.y) * -(float)(i+1),
				0f
			);

			Image img = listing.GetComponent<Image>();
			img.sprite = fillSprite;
			img.color = new Color (0f, 0f, 0f, 0f);


			//
			// Create song text
			//
			GameObject listing_text = UI.MakeText(song.name+"_text");

			RectTransform ttr = listing_text.GetComponent<RectTransform>();
			ttr.SetParent(tr);
			ttr.sizeDelta = tr.sizeDelta;
			ttr.localScale = new Vector3 (1f, 1f, 1f);
			ttr.anchorMin = new Vector2 (0.5f, 0.5f);
			ttr.anchorMin = new Vector2 (0.5f, 0.5f);
			ttr.anchoredPosition3D = new Vector3 (0f, 0f, 0f);

			Text ttxt = listing_text.GetComponent<Text>();
			ttxt.text = song.name;
			ttxt.fontSize = fontSize;
			ttxt.color = Color.white;
			ttxt.font = font;
			ttxt.fontStyle = FontStyle.Normal;
			ttxt.alignment = TextAnchor.MiddleLeft;


			//
			// Create edit song button
			//
			GameObject listing_edit = UI.MakeButton(song.name+"_edit");

			RectTransform etr = listing_edit.GetComponent<RectTransform>();
			etr.SetParent(tr);
			etr.sizeDelta = new Vector2 (tr.sizeDelta.y, tr.sizeDelta.y);
			etr.localScale = new Vector3 (1f, 1f, 1f);
			etr.anchorMin = new Vector2 (1f, 0.5f);
			etr.anchorMax = new Vector2 (1f, 0.5f);
			etr.anchoredPosition3D = new Vector3 (0f, 0f, 0f);

			Image eimg = listing_edit.GetComponent<Image>();
			eimg.sprite = GameManager.instance.editIcon;

			Button ebt = listing_edit.GetComponent<Button>();
			ebt.onClick.AddListener(()=>{
				MusicManager.instance.currentSong = song;
				GameManager.instance.GoToSongArrangeMenu();
			});

			//
			// Create remove song button
			//
			GameObject listing_remove = UI.MakeButton(song.name+"_remove");

			RectTransform rtr = listing_edit.GetComponent<RectTransform>();
			rtr.SetParent(tr);
			rtr.sizeDelta = new Vector2 (tr.sizeDelta.y, tr.sizeDelta.y);
			rtr.localScale = new Vector3 (1f, 1f, 1f);
			rtr.anchorMin = new Vector2 (1f, 0.5f);
			rtr.anchorMax = new Vector2 (1f, 0.5f);
			rtr.anchoredPosition3D = new Vector3 (-rtr.sizeDelta.x - horizontalPadding, 0f, 0f);

			Image rimg = listing_remove.GetComponent<Image>();
			rimg.sprite = GameManager.instance.removeIcon;

			Button rbt = listing_remove.GetComponent<Button>();
			rbt.onClick.AddListener(()=>{
				MusicManager.instance.currentProject.songs.RemoveAt(num);
				Refresh();
			});

			ShowHide sh = listing.GetComponent<ShowHide>(); 
			sh.objects = new List<GameObject>() {
				listing_edit,
				listing_remove,
			};

			//
			// Create move song up button if not at top
			//
			if (num > 0) {
				Song prevSong = MusicManager.instance.currentProject.songs[i-1];
				GameObject listing_up = UI.MakeButton(song.name+"_up");

				RectTransform utr = listing_up.GetComponent<RectTransform>();
				utr.SetParent(tr);
				utr.sizeDelta = new Vector2 (tr.sizeDelta.y*0.8f, tr.sizeDelta.y*0.8f);
				utr.localScale = new Vector3 (1f, 1f, 1f);
				utr.anchorMin = new Vector2 (0f, 0.5f);
				utr.anchorMax = new Vector2 (0f, 0.5f);
				utr.anchoredPosition3D = new Vector3 (
					-utr.sizeDelta.x - horizontalPadding, 
					utr.sizeDelta.y/2f + verticalPadding, 
					0f
				);
				utr.localRotation = Quaternion.Euler(0f, 0f, 90f);

				Image uimg = listing_up.GetComponent<Image>();
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
				dtr.sizeDelta = new Vector2 (tr.sizeDelta.y*0.8f, tr.sizeDelta.y*0.8f);
				dtr.localScale = new Vector3 (1f, 1f, 1f);
				dtr.anchorMin = new Vector2 (0f, 0.5f);
				dtr.anchorMax = new Vector2 (0f, 0.5f);
				dtr.anchoredPosition3D = new Vector3 (
					-dtr.sizeDelta.x - horizontalPadding, 
					dtr.sizeDelta.y/2f + verticalPadding, 
					0f
				);
				dtr.localRotation = Quaternion.Euler(0f, 0f, -90f);

				Image dimg = listing_down.GetComponent<Image>();
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
		}
	}
		
}
