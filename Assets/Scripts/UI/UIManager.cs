﻿// UIManager.cs
// ©2016 Team 95

using Route95.Core;
using Route95.Music;
using Route95.World;

using UnityEngine;
using UnityEngine.Events;

namespace Route95.UI {

    public class UIManager : SingletonMonoBehaviour<UIManager> {

		#region Nested Classes

		public class UIEvent : UnityEvent { }

		#endregion
		#region Vars

		//----------------------------------------------------------------------
		[Header("UI Resources")]

        /// <summary>
        /// Font to use for UI.
        /// </summary>
        [Tooltip("Font to use for UI.")]
        Font _font;

        /// <summary>
        /// Handwritten-style font to use for UI.
        /// </summary>
        [Tooltip("Handwritten-style font to use for UI.")]
        Font _handwrittenFont;

        /// <summary>
        /// Arrow icon.
        /// </summary>
        [Tooltip("Arrow icon.")]
        Sprite _arrowIcon;

        /// <summary>
        /// Add icon.
        /// </summary>
        [Tooltip("Add icon.")]
        Sprite _addIcon;

        /// <summary>
        /// Edit icon.
        /// </summary>
        [Tooltip("Edit icon.")]
        Sprite _editIcon;

        /// <summary>
        /// Play icon.
        /// </summary>
        [Tooltip("Play icon.")]
        Sprite _playIcon;

        /// <summary>
        /// Pause icon.
        /// </summary>
        [Tooltip("Pause icon.")]
        Sprite _pauseIcon;

        /// <summary>
        /// Load file icon.
        /// </summary>
        [Tooltip("Load file icon.")]
        Sprite _loadIcon;

        /// <summary>
        /// Remove icon.
        /// </summary>
        [Tooltip("Remove icon.")]
        Sprite _removeIcon;

        /// <summary>
        /// Circle icon.
        /// </summary>
        [Tooltip("Circle icon.")]
        Sprite _circleIcon;

		Sprite _emptyPercussionNoteIcon;

		Sprite _filledPercussionNoteIcon;

		Sprite _emptyMelodicNoteIcon;

		Sprite _filledMelodicNoteIcon;

		Sprite _minorSuggestionIcon;

		Sprite _majorSuggestionIcon;

		Sprite _powerSuggestionIcon;

		Sprite _octaveSuggestionIcon;

        /// <summary>
        /// Percussion note volume icon.
        /// </summary>
        [Tooltip("Percussion note volume icon.")]
        Sprite _percussionVolumeIcon;

        /// <summary>
        /// Melodic note volume icon.
        /// </summary>
        [Tooltip("Melodic note volume icon.")]
        Sprite _melodicVolumeIcon;

        /// <summary>
        /// Fill sprite.
        /// </summary>
        [Tooltip("Fill sprite.")]
        Sprite _fillSprite;

        /// <summary>
        /// Scribble circle sprite.
        /// </summary>
        [Tooltip("Scribble circle sprite.")]
        Sprite _scribbleCircle;

        /// <summary>
        /// Sound to use when clicking a menu.
        /// </summary>
        [Tooltip("Sound to use when clicking a menu.")]
        [SerializeField]
        AudioClip _menuClickSound;

        /// <summary>
        /// Alternate sound to use when clicking a menu.
        /// </summary>
        [Tooltip("Alternate sound to use when clicking a menu.")]
        [SerializeField]
        AudioClip _altMenuClickSound;

        /// <summary>
        /// Sound to play when enabling an instrument effect.
        /// </summary>
        [Tooltip("Sound to play when enabling an instrument effect.")]
        [SerializeField]
        AudioClip _enableEffectSound;

        /// <summary>
        /// Sound to play when disabling an instrument effect.
        /// </summary>
        [Tooltip("Sound to play when disabling an instrument effect.")]
        [SerializeField]
        AudioClip _disableEffectSound;

		AudioSource _menuThemeSource;

        //----------------------------------------------------------------------
        [Header("Menu Objects")]

        /// <summary>
        /// RectTransform of the tooltip.
        /// </summary>
        RectTransform _tooltipTR;

        /// <summary>
        /// RectTransform of the 2D canvas.
        /// </summary>
        RectTransform _canvasTR;

        /// <summary>
        /// Dimensions of the 2D canvas.
        /// </summary>
        Vector2 _canvasDimensions;

        //----------------------------------------------------------------------
        [Header("Tooltip Settings")]

        /// <summary>
        /// Distance to show tooltip.
        /// </summary>
        [Tooltip("Distance to show tooltip.")]
        [SerializeField]
        float _tooltipDistance;

        /// <summary>
        /// Dimensions of the tooltip object.
        /// </summary>
        Vector2 _tooltipDimensions;

		[HideInInspector]
		public UIEvent onSwitchMenus;

		[HideInInspector]
		public UIEvent onSwitchModes;

		[HideInInspector]
		public UIEvent onSwitchToMainMenu;

		[HideInInspector]
		public UIEvent onSwitchToCreditsMenu;

		[HideInInspector]
		public UIEvent onSwitchToPlaylistMenu;

		[HideInInspector]
		public UIEvent onSwitchToKeySelectMenu;

		[HideInInspector]
		public UIEvent onSwitchToSongArrangeMenu;

		[HideInInspector]
		public UIEvent onSwitchToRiffEditor;

		[HideInInspector]
		public UIEvent onSwitchToPostPlayMenu;

		[HideInInspector]
		public UIEvent onSwitchToLiveMode;

		[HideInInspector]
		public UIEvent onSwitchToPostplayMode;

        #endregion
        #region Unity Callbacks

        new void Awake () {
            base.Awake();

			// Init events
			onSwitchMenus = new UIEvent();
			onSwitchModes = new UIEvent();
			onSwitchToMainMenu = new UIEvent();
			onSwitchToCreditsMenu = new UIEvent();
			onSwitchToPlaylistMenu = new UIEvent();
			onSwitchToKeySelectMenu = new UIEvent();
			onSwitchToSongArrangeMenu = new UIEvent();
			onSwitchToRiffEditor = new UIEvent();
			onSwitchToPostPlayMenu = new UIEvent();
			onSwitchToLiveMode = new UIEvent();
			onSwitchToPostplayMode = new UIEvent();
		

            // Load UI resources
            _font = Resources.Load<Font>("Fonts/LemonMilk");
            _handwrittenFont = Resources.Load<Font>("Fonts/Handwritten");

            _arrowIcon = Resources.Load<Sprite>("Sprites/Arrow");
            _addIcon = Resources.Load<Sprite>("Sprites/PlusRiff");
            _editIcon = Resources.Load<Sprite>("Sprites/EditRiff");
            _playIcon = Resources.Load<Sprite>("Sprites/StartProject");
            _pauseIcon = Resources.Load<Sprite>("Sprites/Pause");
            _loadIcon = Resources.Load<Sprite>("Sprites/LoadProject");
            _removeIcon = Resources.Load<Sprite>("Sprites/X");
            _circleIcon = Resources.Load<Sprite>("Sprites/Circle");
			_filledPercussionNoteIcon = Resources.Load<Sprite>("Sprites/FilledPercBeat");
			_emptyPercussionNoteIcon = Resources.Load<Sprite>("Sprites/EmptyPercBeat");
			_filledMelodicNoteIcon = Resources.Load<Sprite>("Sprites/FilledMeloBeat");
			_emptyMelodicNoteIcon = Resources.Load<Sprite>("Sprites/EmptyMeloBeat");
			_minorSuggestionIcon = Resources.Load<Sprite>("Sprites/MinorChord");
			_majorSuggestionIcon = Resources.Load<Sprite>("Sprites/MajorChord");
			_powerSuggestionIcon = Resources.Load<Sprite>("Sprites/PowerChord");
			_octaveSuggestionIcon = Resources.Load<Sprite>("Sprites/Octave");
            _percussionVolumeIcon = Resources.Load<Sprite>("Sprites/VolumeBar");
            _melodicVolumeIcon = Resources.Load<Sprite>("Sprites/VolumeBar_Melodic");
            _fillSprite = Resources.Load<Sprite>("Sprites/FillSprite");
            _scribbleCircle = Resources.Load<Sprite>("Sprites/ScribbleHighlight");

			_menuThemeSource = GameObject.FindGameObjectWithTag("MenuThemeSource").GetComponent<AudioSource>();

			onSwitchToLiveMode = new UIEvent();
			onSwitchToPostplayMode = new UIEvent();
        }

        void Start() {
            // Init vars
            _tooltipTR = Tooltip.Instance.GetComponent<RectTransform>();
            _canvasTR = _tooltipTR.parent as RectTransform;

            // Get 2D canvas dimensions
            _canvasDimensions = new Vector2(
                _canvasTR.rect.width,
                _canvasTR.rect.height
            );

            // Get tooltip object dimensions
            _tooltipDimensions = new Vector2(
                _tooltipTR.rect.width,
                _tooltipTR.rect.height
            );

            // Show/init all menus
            ShowAllMenus();

            // Hide menus
            HideAllMenus();
            HideMenu(Prompt.Instance);

            GameManager.Instance.onStartLoading.AddListener(() => {
                HideAllMenus();
            });
        }

        // Update is called once per frame
        void Update() {
            switch (GameManager.Instance.CurrentState) {

                case GameManager.State.Setup:

                    // Check for tooltip
                    if (Tooltip.Instance.gameObject.activeSelf) {

                        Vector2 realPosition = new Vector2(
                            Input.mousePosition.x / Screen.width * _canvasDimensions.x,
                            Input.mousePosition.y / Screen.height * _canvasDimensions.y
                        );

                        float w = _tooltipDimensions.x / 2f;
                        float h = _tooltipDimensions.y / 2f;

                        Vector3 pos = Vector3.zero;
                        pos.z = Input.mousePosition.z;

                        if (Input.mousePosition.x > Screen.width - w)
                            pos.x = realPosition.x - w;
                        else pos.x = realPosition.x + w;

                        if (Input.mousePosition.y > Screen.height - w)
                            pos.y = realPosition.y - h;
                        else pos.y = realPosition.y + h;

                        _tooltipTR.anchoredPosition3D = pos;
                    }
                    break;
            }
        }

        #endregion
        #region UIManager Properties

        /// <summary>
        /// Returns the main UI font (read-only).
        /// </summary>
        public Font Font { get { return _font; } }

        /// <summary>
        /// Returns the handwritten UI font (read-only).
        /// </summary>
        public Font HandwrittenFont { get { return _handwrittenFont; } }

        /// <summary>
        /// Returns the fill sprite (read-only).
        /// </summary>
        public Sprite FillSprite { get { return _fillSprite; } }

        /// <summary>
        /// Returns the play icon (read-only).
        /// </summary>
        public Sprite PlayIcon { get { return _playIcon; } }

        /// <summary>
        /// Returns the pause icon (read-only).
        /// </summary>
        public Sprite PauseIcon { get { return _pauseIcon; } }

        /// <summary>
        /// Returns the add icon (read-only).
        /// </summary>
        public Sprite AddIcon { get { return _addIcon; } }

        /// <summary>
        /// Returns the edit icon (read-only).
        /// </summary>
        public Sprite EditIcon { get { return _editIcon; } }

        public Sprite RemoveIcon { get { return _removeIcon; } }

        /// <summary>
        /// Returns the load file icon (read-only).
        /// </summary>
        public Sprite LoadIcon { get { return _loadIcon; } }

        /// <summary>
        /// Returns the arrow icon (read-only).
        /// </summary>
        public Sprite ArrowIcon { get { return _arrowIcon; } }

        /// <summary>
        /// Returns the circle sprite (read-only).
        /// </summary>
        public Sprite CircleIcon { get { return _circleIcon; } }

        /// <summary>
        /// Returns the scribble sprite (read-only).
        /// </summary>
        public Sprite ScribbleIcon { get { return _scribbleCircle; } }

		public Sprite EmptyPercussionNoteIcon { get { return _emptyPercussionNoteIcon; } }

		public Sprite FilledPercussionNoteIcon { get { return _filledPercussionNoteIcon; } }

		public Sprite EmptyMelodicNoteIcon { get { return _emptyMelodicNoteIcon; } }

		public Sprite FilledMelodicNoteIcon { get { return _filledPercussionNoteIcon; } }

		public Sprite MinorSuggestionIcon { get { return _minorSuggestionIcon; } }

		public Sprite MajorSuggestionIcon { get { return _majorSuggestionIcon; } }

		public Sprite PowerSuggestionIcon { get { return _powerSuggestionIcon; } }

		public Sprite OctaveSuggestionIcon { get { return _octaveSuggestionIcon; } }

        /// <summary>
        /// Returns the melodic note volume icon (read-only).
        /// </summary>
        public Sprite MelodicVolumeIcon { get { return _melodicVolumeIcon; } }

        /// <summary>
        /// Returns the percussion note volume icon (read-only).
        /// </summary>
        public Sprite PercussionVolumeIcon { get { return _percussionVolumeIcon; } }

        #endregion
        #region UIManager Menu Methods

		public void InitEvents () {
			onSwitchMenus = new UIEvent();

			onSwitchModes = new UIEvent();
		}

        /// <summary>
        /// Show the specified menu, fading if possible.
        /// </summary>
        /// <param name="menu">Menu to show.</param>
        public void ShowMenu<T>(MenuBase<T> menu) where T: MonoBehaviour {
            menu.gameObject.SetActive(true);
            Fadeable fade = menu.GetComponent<Fadeable>();
            if (fade != null) fade.UnFade();
        }

        /// <summary>
        /// Shows all menus, fading if possible.
        /// </summary>
        public void ShowAllMenus() {
            ShowMenu(MainMenu.Instance);
            ShowMenu(PlaylistBrowser.Instance);
            ShowMenu(KeySelectMenu.Instance);
            ShowMenu(SongArrangeMenu.Instance);
            ShowMenu(RiffEditor.Instance);
            ShowMenu(PostPlayMenu.Instance);

            ShowMenu(AddRiffPrompt.Instance);
            ShowMenu(LoadPrompt.Instance);
            ShowMenu(Prompt.Instance);
            ShowMenu(LiveInstrumentIcons.Instance);
        }

        /// <summary>
        /// Hide the specified menu, fading if possible.
        /// </summary>
        /// <param name="menu">Menu to hide.</param>
        public void HideMenu<T>(MenuBase<T> menu) where T : MonoBehaviour {
            Fadeable fade = menu.GetComponent<Fadeable>();
            if (fade != null) fade.Fade();
        }

        /// <summary>
        /// Hides all menus, fading if possible.
        /// </summary>
        public void HideAllMenus() {
            HideMenu(MainMenu.Instance);
			HideMenu(CreditsMenu.Instance);
            HideMenu(PlaylistBrowser.Instance);
            HideMenu(KeySelectMenu.Instance);
            HideMenu(SongArrangeMenu.Instance);
            HideMenu(RiffEditor.Instance);
            HideMenu(PostPlayMenu.Instance);

            HideMenu(AddRiffPrompt.Instance);
            HideMenu(LoadPrompt.Instance);
            HideMenu(LiveInstrumentIcons.Instance);
            HideMenu(PauseMenu.Instance);
        }

        /// <summary>
        /// Goes to main menu.
        /// </summary>
        public void GoToMainMenu() {
            // Hide other menus
            HideAllMenus();

            // Show main menu
            ShowMenu(MainMenu.Instance);

			onSwitchToMainMenu.Invoke();
        }

		public void GoToCreditsMenu () {
			// Hide other menus
			HideAllMenus();

			// Show credits menu
			ShowMenu(CreditsMenu.Instance);

			onSwitchToCreditsMenu.Invoke();
		}

        /// <summary>
        /// Goes to key select menu.
        /// </summary>
        public void GoToKeySelectMenu() {
			// Hide other menus
            HideAllMenus();

            // Show key select menu
            ShowMenu(KeySelectMenu.Instance);

			onSwitchToKeySelectMenu.Invoke();
        }

        /// <summary>
        /// Goes to song arrange menu.
        /// </summary>
        public void GoToSongArrangeMenu() {
            // Hide other menus
            HideAllMenus();

            // Show and refresh song arranger menu
            ShowMenu(SongArrangeMenu.Instance);

			onSwitchToSongArrangeMenu.Invoke();
        }

        /// <summary>
        /// Goes to riff editor.
        /// </summary>
        public void GoToRiffEditor() {

            // Hide other menus
            HideAllMenus();

            // If no scale selected, go to key select first
            if (MusicManager.Instance.CurrentSong.Scale == -1) GoToKeySelectMenu();

            else {
                // Otherwise show riff editor
                ShowMenu(RiffEditor.Instance);

				// Stop playing theme music
				if (_menuThemeSource.isPlaying) _menuThemeSource.Stop();

				onSwitchToRiffEditor.Invoke();
            }
        }

        /// <summary>
        /// Goes to playlist menu.
        /// </summary>
        public void GoToPlaylistMenu() {
            // Hide other menus
            HideAllMenus();
            HideMenu(LiveInstrumentIcons.Instance);
            HideMenu(SongProgressBar.Instance);

            // Show playlist menu
            ShowMenu(PlaylistBrowser.Instance);

			onSwitchToPlaylistMenu.Invoke();
        }

        /// <summary>
        /// Goes to post play menu.
        /// </summary>
        public void GoToPostPlayMenu() {
            // Hide other menus
            HideAllMenus();

            // Show postplay menu
            ShowMenu(PostPlayMenu.Instance);

			onSwitchToPostPlayMenu.Invoke();
        }

        #endregion
        #region Mode Switching Methods

        /// <summary>
        /// Switches to live mode.
        /// </summary>
        public void SwitchToLive() {

            // Hide menus
            HideAllMenus();

            // Show live menus
            ShowMenu(LiveInstrumentIcons.Instance);
            ShowMenu(SongProgressBar.Instance);
            if (MusicManager.Instance.LoopPlaylist) ShowMenu(LoopIcon.Instance);
            else HideMenu(LoopIcon.Instance);

			onSwitchToLiveMode.Invoke();
        }

        /// <summary>
        /// Switches to postplay mode.
        /// </summary>
        public void SwitchToPostplay() {
            HideMenu(LiveInstrumentIcons.Instance);
            HideMenu(SongProgressBar.Instance);

            // Go to postplay menu
            GoToPostPlayMenu();
        }

        #endregion
        #region Sound Methods

		public void PlayInstrumentSound (int instrumentIndex) {
			var inst = Instrument.AllInstruments[instrumentIndex];
			MusicManager.PlayMenuSound (inst.SwitchSound);
		}

        /// <summary>
        /// Plays a click noise.
        /// </summary>
        public void PlayMenuClickSound() {
            MusicManager.PlayMenuSound(_menuClickSound);
        }

        /// <summary>
        /// Plays an alternate click noise.
        /// </summary>
        public void PlayAltMenuClickSound() {
            MusicManager.PlayMenuSound(_altMenuClickSound);
        }

		/// <summary>
		/// Plays a sound for enabling effects.
		/// </summary>
        public void PlayEnableEffectSound () {
            MusicManager.PlayMenuSound(_enableEffectSound);
        }

		/// <summary>
		/// Plays a spound for disabling effects.
		/// </summary>
        public void PlayDisableEffectSound () {
            MusicManager.PlayMenuSound(_disableEffectSound);
        }

        #endregion
    }
}
