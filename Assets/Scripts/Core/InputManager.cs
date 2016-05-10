using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class InputManager : MonoBehaviour {

	#region InputManager Vars

	public static InputManager instance;

	[Tooltip("Current selected object.")]
	public GameObject selected;

	[Tooltip("List of scroll views to disable while dragging.")]
	public List<ScrollRect> scrollviews;

	Vector3 clickPosition;
	List<AudioSource> audioSources;


	public int framesDragged = 0;

	public bool IsDragging {
		get {
			return framesDragged >= 30;
		}
	}

	#endregion
	#region Key Mappings

	public static Dictionary<KeyCode, Instrument> keyToInstrument = new Dictionary<KeyCode, Instrument>() {
		{ KeyCode.Alpha1, PercussionInstrument.RockDrums },
		{ KeyCode.Alpha2, PercussionInstrument.ExoticPercussion },
		{ KeyCode.Alpha3, MelodicInstrument.ElectricGuitar },
		{ KeyCode.Alpha4, MelodicInstrument.ElectricBass },
		{ KeyCode.Alpha5, MelodicInstrument.AcousticGuitar },
		{ KeyCode.Alpha6, MelodicInstrument.ClassicalGuitar },
		{ KeyCode.Alpha7, MelodicInstrument.PipeOrgan },
		{ KeyCode.Alpha8, MelodicInstrument.Keyboard },
		{ KeyCode.Alpha9, MelodicInstrument.Trumpet }
	};
		
	public static Dictionary<KeyCode, int> keyToLick = new Dictionary<KeyCode, int>() {
		{ KeyCode.Q, 0 },
		{ KeyCode.W, 1 },
		{ KeyCode.E, 2 },
		{ KeyCode.R, 3 },
		{ KeyCode.T, 4 }
	};

	public static Dictionary<KeyCode, int> keyToNote = new Dictionary<KeyCode, int>() {
		{ KeyCode.P, 0 },
		{ KeyCode.O, 1 },
		{ KeyCode.I, 2 },
		{ KeyCode.U, 3 },
		{ KeyCode.Y, 4 },
		{ KeyCode.T, 5 },
		{ KeyCode.R, 6 },
		{ KeyCode.E, 7 },
		{ KeyCode.W, 8 },
		{ KeyCode.Q, 9 },
		{ KeyCode.L, 10 },
		{ KeyCode.K, 11 },
		{ KeyCode.J, 12 },
		{ KeyCode.H, 13 },
		{ KeyCode.G, 14 },
		{ KeyCode.F, 15 },
		{ KeyCode.D, 16 },
		{ KeyCode.S, 17 },
		{ KeyCode.A, 18 },
		{ KeyCode.M, 19 },
		{ KeyCode.N, 20 },
		{ KeyCode.B, 21 },
		{ KeyCode.V, 22 },
		{ KeyCode.C, 23 },
		{ KeyCode.X, 24 },
		{ KeyCode.Z, 25 }
	};

	#endregion
	#region Unity Callbacks

	void Awake () {
		instance = this;
		audioSources = new List<AudioSource>();
		for (int i=0; i<26; i++) {
			GameObject obj = new GameObject();
			AudioSource temp = obj.AddComponent<AudioSource>();
			temp.volume = 1.0f;
			audioSources.Add(temp);
		}
	}

	void Update () {
		
		if (GameManager.instance.currentMode == GameManager.Mode.Live) {
			Song song = null;
			if (MusicManager.instance.currentProject.songs.Count > 0) 
				song = MusicManager.instance.currentProject.songs[MusicManager.instance.currentPlayingSong];

			// Check for pause
			if (Input.GetKeyDown (KeyCode.Escape)) GameManager.instance.TogglePause ();

			// Check for tempo up/down
			else if (Input.GetKeyDown (KeyCode.UpArrow)) MusicManager.instance.IncreaseTempo ();
			else if (Input.GetKeyDown (KeyCode.DownArrow))  MusicManager.instance.DecreaseTempo ();

			// Check for instrument volume up/down
			else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				AudioSource source = MusicManager.instance.instrumentAudioSources[MusicManager.instance.currentInstrument];
				if (source.volume >= 0.1f) source.volume -= 0.1f;
			} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
				AudioSource source = MusicManager.instance.instrumentAudioSources[MusicManager.instance.currentInstrument];
				if (source.volume <= 0.9f) source.volume += 0.1f;

			} else if (Input.GetKeyDown (KeyCode.Escape)) {
				GameManager.instance.Pause();
			} else {
				
				// Check for instruments switch
				foreach (KeyValuePair<KeyCode, Instrument> key in keyToInstrument) {
					if (Input.GetKeyDown(key.Key)) {
						SwitchInstrument(keyToInstrument[key.Key]);
						GameManager.instance.WakeLiveUI();
					}
				}
					
				// Check for note presses
				foreach (KeyCode keyPress in keyToNote.Keys.ToList()) {
					Instrument inst = MusicManager.instance.currentInstrument;
					if (Input.GetKeyDown(keyPress)) {
						int noteIndex;
						AudioSource source = audioSources[keyToNote[keyPress]];
						if (inst.type == Instrument.Type.Percussion) {
							noteIndex = KeyManager.instance.percussionSets[inst].Count-1-keyToNote[keyPress];
							if (noteIndex >= 0) {
								Note note = new Note(KeyManager.instance.percussionSets[inst][noteIndex]);
								note.PlayNote(source, false);

								if (inst.type == Instrument.Type.Percussion) {
									if (note.IsSnare()) WorldManager.instance.LightningStrike(note.volume * source.volume);
									else if (note.IsKick()) WorldManager.instance.LightningFlash(note.volume * source.volume);
									else if (note.IsTom()) WorldManager.instance.LightningFlash(0.75f * note.volume * source.volume);
									else if (note.IsShaker()) WorldManager.instance.shakers++;
									else if (note.IsHat()) WorldManager.instance.StarBurst();
									else if (note.IsCymbal()) WorldManager.instance.ShootingStar();
									else if (note.IsWood()) WorldManager.instance.ExhaustPuff();
								} else {
									if (inst.codeName == "ElectricBass") {
										//WorldManager.instance.StartWave();
										//WorldManager.instance.MakeWave(PlayerMovement.instance.transform.position, 1f, 1f);
										WorldManager.instance.DeformRandom();
									} else {
										
									}
								}

							}
						} else if (song != null && song.scale != -1 && song.key != Key.None) {

							Key key = MusicManager.instance.currentSong.key;
							ScaleInfo scale = ScaleInfo.AllScales[MusicManager.instance.currentSong.scale];

							noteIndex = KeyManager.instance.scales[key][scale][(MelodicInstrument)inst].allNotes.Count-1-keyToNote[keyPress];
							if (noteIndex >= 0) {
								Note note = new Note(KeyManager.instance.scales[key][scale][(MelodicInstrument)inst].allNotes[noteIndex]);
								if (note != null)
									note.PlayNote(source, true);
								}
						}
						break;
					} else if (Input.GetKeyUp(keyPress) && inst.type != Instrument.Type.Percussion)
						audioSources[keyToNote[keyPress]].Stop();
					
				}
						
			}
		} else if (GameManager.instance.currentMode == GameManager.Mode.Setup) {
			if (Input.GetMouseButtonUp(0)) {
				if (selected != null) {
					if (selected.GetComponent<DraggableButton>() != null) {
						selected.GetComponent<DraggableButton>().OnMouseUp();
					}
				}	
				selected = null;
				UnfreezeAllScrollviews();
				clickPosition = Vector3.zero;
			} else if (Input.GetMouseButtonDown(0)) {
				selected = EventSystem.current.currentSelectedGameObject;
				if (selected != null) {
					if (selected.tag == "StopScrolling") FreezeAllScrollviews();
						clickPosition = Input.mousePosition;
						if (selected.GetComponent<DraggableButton>() != null) {
							selected.GetComponent<DraggableButton>().OnMouseDown();
					}
				}
			} else {
				if (selected) {
					framesDragged++;
					if (selected.GetComponent<DraggableButton>() != null) {
						selected.GetComponent<DraggableButton>().Drag(Input.mousePosition - clickPosition);
					}
				} else {
					framesDragged = 0;
				}
			}
		}
	}

	#endregion
	#region InputManager Methods

	/// <summary>
	/// Freezes all selected scrollviews.
	/// </summary>
	void FreezeAllScrollviews () {
		foreach (ScrollRect scrollview in scrollviews)
			scrollview.enabled = false;
	}

	/// <summary>
	/// Unfreezes all selected scrollviews.
	/// </summary>
	void UnfreezeAllScrollviews () {
		foreach (ScrollRect scrollview in scrollviews)
			scrollview.enabled = true;
	}

	/// <summary>
	/// Switches the instrument.
	/// </summary>
	/// <param name="instrument">Instrument.</param>
	void SwitchInstrument (Instrument instrument) {
		if (instrument != MusicManager.instance.currentInstrument) {
			MusicManager.instance.currentInstrument = instrument;
			MusicManager.instance.GetComponent<AudioSource>().PlayOneShot(instrument.switchSound);
			InstrumentDisplay.instance.Refresh();
		}
	}

	#endregion

}
