// UIManager.cs
// ©2016 Team 95

using Route95.Core;
using Route95.Music;
using Route95.World;

using UnityEngine;

namespace Route95.UI {

    public class UIManager : SingletonMonoBehaviour<UIManager> {

        #region UIManager Vars

        //-----------------------------------------------------------------------------------------------------------------
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

        //--------------------------------------------------------------------------
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

        //--------------------------------------------------------------------------
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

        #endregion
        #region Unity Callbacks

        new void Awake () {
            base.Awake();

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
            _percussionVolumeIcon = Resources.Load<Sprite>("Sprites/VolumeBar");
            _melodicVolumeIcon = Resources.Load<Sprite>("Sprites/VolumeBar_Melodic");
            _fillSprite = Resources.Load<Sprite>("Sprites/FillSprite");
            _scribbleCircle = Resources.Load<Sprite>("Sprites/ScribbleHighlight");
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

        /// <summary>
        /// Show the specified menu, fading if possible.
        /// </summary>
        /// <param name="menu">Menu to show.</param>
        public void ShowMenu<T>(MenuBase<T> menu) where T : MonoBehaviour {
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

            GameManager.Instance.CurrentState = GameManager.State.Setup;

            // Hide other menus
            HideAllMenus();
            Casette.Instance.MoveToBack();

            // Show main menu
            ShowMenu(MainMenu.Instance);

            // Move camera to outside view
            CameraControl.Instance.LerpToView(CameraControl.Instance.OutsideCar);
            CameraControl.Instance.doSway = true;
        }

        /// <summary>
        /// Goes to key select menu.
        /// </summary>
        public void GoToKeySelectMenu() {

            GameManager.Instance.CurrentState = GameManager.State.Setup;

            // Hide other menus
            Casette.Instance.MoveToBack();
            HideAllMenus();

            // Show key select menu
            ShowMenu(KeySelectMenu.Instance);

            // Move camera to driving view
            CameraControl.Instance.LerpToView(CameraControl.Instance.Driving);
            CameraControl.Instance.doSway = true;

            // Refresh radial menu
            RadialKeyMenu.Instance.Refresh();

            // Enable/disable confirmation button
            KeySelectConfirmButton.Instance.SetInteractable(
                MusicManager.Instance.CurrentSong.Scale != -1 && MusicManager.Instance.CurrentSong.Key != Key.None
            );
        }

        /// <summary>
        /// Goes to song arrange menu.
        /// </summary>
        public void GoToSongArrangeMenu() {

            GameManager.Instance.CurrentState = GameManager.State.Setup;

            // Hide other menus
            Casette.Instance.MoveToBack();
            HideAllMenus();

            // Show and refresh song arranger menu
            ShowMenu(SongArrangeMenu.Instance);
            SongArrangeMenu.Instance.Refresh();
            SongTimeline.Instance.RefreshTimeline();

            // Move camera to radio view
            CameraControl.Instance.LerpToView(CameraControl.Instance.Radio);
            CameraControl.Instance.doSway = false;
        }

        /// <summary>
        /// Goes to riff editor.
        /// </summary>
        public void GoToRiffEditor() {

            GameManager.Instance.CurrentState = GameManager.State.Setup;

            // Hide other menus
            Casette.Instance.MoveToBack();
            HideAllMenus();

            // If no scale selected, go to key select first
            if (MusicManager.Instance.CurrentSong.Scale == -1) GoToKeySelectMenu();

            else {
                SongArrangeMenu.Instance.UpdateValue();

                // Otherwise show riff editor
                ShowMenu(RiffEditor.Instance);
                RiffEditor.Instance.Initialize();


                // Move camera to driving view
                CameraControl.Instance.LerpToView(CameraControl.Instance.Driving);
                CameraControl.Instance.doSway = true;
            }
        }

        /// <summary>
        /// Goes to playlist menu.
        /// </summary>
        public void GoToPlaylistMenu() {

            // Switch modes
            GameManager.Instance.CurrentState = GameManager.State.Setup;

            // Stop music/live mode operations
            MusicManager.Instance.StopPlaying();
            PlayerMovement.Instance.StopMoving();
            CameraControl.Instance.StopLiveMode();
            GameManager.Instance.Paused = false;

            // Hide other menus
            HideAllMenus();
            HideMenu(LiveInstrumentIcons.Instance);
            HideMenu(SongProgressBar.Instance);

            // Show playlist menu
            ShowMenu(PlaylistBrowser.Instance);
            PlaylistBrowser.Instance.Refresh();
            PlaylistBrowser.Instance.RefreshName();

            // Queue casette to move when done moving camera
            Casette.Instance.WillMove = true;

            // Move camera to outside view
            CameraControl.Instance.LerpToView(CameraControl.Instance.OutsideCar);
            CameraControl.Instance.doSway = true;
        }

        /// <summary>
        /// Goes to post play menu.
        /// </summary>
        public void GoToPostPlayMenu() {

            // Switch mode
            GameManager.Instance.CurrentState = GameManager.State.Postplay;

            // Hide other menus
            Casette.Instance.MoveToBack();
            HideAllMenus();

            // Show postplay menu
            ShowMenu(PostPlayMenu.Instance);

            CameraControl.Instance.doSway = true;
        }

        #endregion
        #region Mode Switching Methods

        /// <summary>
        /// Switches to live mode.
        /// </summary>
        public void SwitchToLive() {

            // Switch mode
            GameManager.Instance.CurrentState = GameManager.State.Live;
            GameManager.Instance.Paused = false;

            // Hide menus
            Casette.Instance.SnapBack();
            HideAllMenus();

            // Show live menus
            ShowMenu(LiveInstrumentIcons.Instance);
            ShowMenu(SongProgressBar.Instance);
            if (MusicManager.Instance.LoopPlaylist) ShowMenu(LoopIcon.Instance);
            else HideMenu(LoopIcon.Instance);

            // Init music
            MusicManager.Instance.CurrentPlayingSong = 0;
            MusicManager.Instance.CurrentSong = (
                MusicManager.Instance.CurrentProject.Songs.Count > 0 ? MusicManager.Instance.CurrentProject.Songs[0] : null);
            if (MusicManager.Instance.CurrentSong != null) {
                MusicManager.Instance.StartPlaylist();
                MusicManager.Instance.StartSong();
            }

            // Start live operations
            InstrumentDisplay.Instance.Refresh();
            CameraControl.Instance.StartLiveMode();
            PlayerMovement.Instance.StartMoving();
        }

        /// <summary>
        /// Switches to postplay mode.
        /// </summary>
        public void SwitchToPostplay() {

            // Switch mode
            GameManager.Instance.CurrentState = GameManager.State.Postplay;
            GameManager.Instance.Paused = false;

            HideMenu(LiveInstrumentIcons.Instance);
            HideMenu(SongProgressBar.Instance);

            // Stop music/live operations
            MusicManager.Instance.StopPlaying();
            PlayerMovement.Instance.StopMoving();
            CameraControl.Instance.StopLiveMode();

            // Show prompt
            //livePlayQuitPrompt.GetComponent<Image>().color = Color.white;

            // Go to postplay menu
            GoToPostPlayMenu();
        }

        #endregion
        #region Sound Methods

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
