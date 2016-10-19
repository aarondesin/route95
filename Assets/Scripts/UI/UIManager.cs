// UIManager.cs
// ©2016 Team 95

using UnityEngine;
using System.Collections;

using Route95.Core;

namespace Route95.UI {

    public class UIManager : SingletonMonoBehaviour<UIManager> {

        #region UIManager Vars

        //-----------------------------------------------------------------------------------------------------------------
        [Header("UI Resources")]

        /// <summary>
        /// Font to use for UI.
        /// </summary>
        [Tooltip("Font to use for UI.")]
        [SerializeField]
        Font _font;

        /// <summary>
        /// Handwritten-style font to use for UI.
        /// </summary>
        [Tooltip("Handwritten-style font to use for UI.")]
        [SerializeField]
        Font _handwrittenFont;

        /// <summary>
        /// Arrow icon.
        /// </summary>
        [Tooltip("Arrow icon.")]
        [SerializeField]
        Sprite _arrowIcon;

        /// <summary>
        /// Add icon.
        /// </summary>
        [Tooltip("Add icon.")]
        [SerializeField]
        Sprite _addIcon;

        /// <summary>
        /// Edit icon.
        /// </summary>
        [Tooltip("Edit icon.")]
        [SerializeField]
        Sprite _editIcon;

        /// <summary>
        /// Play icon.
        /// </summary>
        [Tooltip("Play icon.")]
        [SerializeField]
        Sprite _playIcon;

        /// <summary>
        /// Pause icon.
        /// </summary>
        [Tooltip("Pause icon.")]
        [SerializeField]
        Sprite _pauseIcon;

        /// <summary>
        /// Load file icon.
        /// </summary>
        [Tooltip("Load file icon.")]
        [SerializeField]
        Sprite _loadIcon;

        /// <summary>
        /// Remove icon.
        /// </summary>
        [Tooltip("Remove icon.")]
        [SerializeField]
        Sprite _removeIcon;

        /// <summary>
        /// Circle icon.
        /// </summary>
        [Tooltip("Circle icon.")]
        [SerializeField]
        Sprite _circleIcon;

        /// <summary>
        /// Percussion note volume icon.
        /// </summary>
        [Tooltip("Percussion note volume icon.")]
        [SerializeField]
        Sprite _percussionVolumeIcon;

        /// <summary>
        /// Melodic note volume icon.
        /// </summary>
        [Tooltip("Melodic note volume icon.")]
        [SerializeField]
        Sprite _melodicVolumeIcon;

        /// <summary>
        /// Fill sprite.
        /// </summary>
        [Tooltip("Fill sprite.")]
        [SerializeField]
        Sprite _fillSprite;

        /// <summary>
        /// Scribble circle sprite.
        /// </summary>
        [Tooltip("Scribble circle sprite.")]
        [SerializeField]
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
        /// GameObject to use for tooltip.
        /// </summary>
        [Tooltip("GameObject to use for tooltip.")]
        [SerializeField]
        GameObject _tooltipObj;

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

        void Start() {
            // Init vars
            _tooltipTR = _tooltipObj.RectTransform();
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
            ShowAll();

            // Hide menus
            HideAll();
            Hide(Prompt.Instance.gameObject);

            GameManager.Instance.onStartLoading.AddListener(() => {
                HideAll();
            });
        }

        // Update is called once per frame
        void Update() {
            switch (GameManager.Instance.CurrentState) {

                case GameManager.State.Setup:

                    // Check for tooltip
                    if (_tooltipObj.activeSelf) {

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

                case GameManager.State.Live:
                    if (!_paused) {

                        // Wake/fade UI icons
                        Color temp = livePlayQuitPrompt.color;
                        if (_prevMouse != Input.mousePosition) {
                            WakeLiveUI();
                            _prevMouse = Input.mousePosition;
                        }
                        else {
                            if (_liveInstrumentIconFadeTimer <= 0f)
                                temp.a -= _liveInstrumentIconFadeSpeed;
                            else _liveInstrumentIconFadeTimer--;

                            livePlayQuitPrompt.color = temp;
                            foreach (Image image in liveIcons.GetComponentsInChildren<Image>()) {
                                image.color = temp;
                                if (image.GetComponentInChildren<Text>())
                                    image.GetComponentInChildren<Text>().color = temp;
                            }
                        }
                    }
                    else livePlayQuitPrompt.color = Color.white;
                    break;
            }
        }

        #endregion
        #region UIManager Menu Methods

        /// <summary>
        /// Show the specified menu, fading if possible.
        /// </summary>
        /// <param name="menu">Menu to show.</param>
        public void ShowMenu(MenuBase menu) {
            menu.SetActive(true);
            Fadeable fade = menu.GetComponent<Fadeable>();
            if (fade != null) fade.UnFade();
        }

        /// <summary>
        /// Shows all menus, fading if possible.
        /// </summary>
        public void ShowAllMenus() {
            Show(mainMenu);
            Show(playlistMenu);
            Show(keySelectMenu);
            Show(songArrangeMenu);
            Show(riffEditMenu);
            Show(postPlayMenu);

            Show(addRiffPrompt);
            Show(loadPrompt);
            Show(prompt);
            Show(liveIcons);
        }

        /// <summary>
        /// Hide the specified menu, fading if possible.
        /// </summary>
        /// <param name="menu">Menu to hide.</param>
        public void HideMenu(MenuBase menu) {
            Fadeable fade = menu.GetComponent<Fadeable>();
            if (fade != null) fade.Fade();
        }

        /// <summary>
        /// Hides all menus, fading if possible.
        /// </summary>
        public void HideAllMenus() {
            Hide(mainMenu);
            Hide(playlistMenu);
            Hide(keySelectMenu);
            Hide(songArrangeMenu);
            Hide(riffEditMenu);
            Hide(postPlayMenu);

            Hide(addRiffPrompt);
            Hide(loadPrompt);
            Hide(liveIcons);
            Hide(pauseMenu);
        }

        /// <summary>
        /// Goes to main menu.
        /// </summary>
        public void GoToMainMenu() {

            currentState = State.Setup;

            // Hide other menus
            HideAll();
            MoveCasetteBack();

            // Show main menu
            Show(mainMenu);

            // Move camera to outside view
            CameraControl.instance.LerpToView(CameraControl.instance.OutsideCar);
            CameraControl.instance.doSway = true;
        }

        /// <summary>
        /// Goes to key select menu.
        /// </summary>
        public void GoToKeySelectMenu() {

            currentState = State.Setup;

            // Hide other menus
            MoveCasetteBack();
            HideAll();

            // Show key select menu
            Show(keySelectMenu);

            // Move camera to driving view
            CameraControl.instance.LerpToView(CameraControl.instance.Driving);
            CameraControl.instance.doSway = true;

            // Refresh radial menu
            RadialKeyMenu.instance.Refresh();

            // Enable/disable confirmation button
            keySelectConfirmButton.GetComponent<Button>().interactable =
                MusicManager.instance.currentSong.scale != -1 && MusicManager.instance.currentSong.key != Key.None;

        }

        /// <summary>
        /// Goes to song arrange menu.
        /// </summary>
        public void GoToSongArrangeMenu() {

            currentState = State.Setup;

            // Hide other menus
            MoveCasetteBack();
            HideAll();

            // Show and refresh song arranger menu
            Show(songArrangeMenu);
            SongArrangeSetup.instance.Refresh();
            SongTimeline.instance.RefreshTimeline();

            // Move camera to radio view
            CameraControl.instance.LerpToView(CameraControl.instance.Radio);
            CameraControl.instance.doSway = false;
        }

        /// <summary>
        /// Goes to riff editor.
        /// </summary>
        public void GoToRiffEditor() {

            currentState = State.Setup;

            // Hide other menus
            MoveCasetteBack();
            HideAll();

            // If no scale selected, go to key select first
            if (MusicManager.instance.currentSong.scale == -1) GoToKeySelectMenu();

            else {
                SongArrangeSetup.instance.UpdateValue();

                // Otherwise show riff editor
                Show(riffEditMenu);
                InstrumentSetup.instance.Initialize();


                // Move camera to driving view
                CameraControl.instance.LerpToView(CameraControl.instance.Driving);
                CameraControl.instance.doSway = true;
            }
        }

        /// <summary>
        /// Goes to playlist menu.
        /// </summary>
        public void GoToPlaylistMenu() {

            // Switch modes
            currentState = State.Setup;

            // Stop music/live mode operations
            MusicManager.instance.StopPlaying();
            PlayerMovement.instance.StopMoving();
            CameraControl.instance.StopLiveMode();
            GameManager.instance.paused = false;

            // Hide other menus
            HideAll();
            Hide(liveIcons);
            Hide(songProgressBar);

            // Show playlist menu
            Show(playlistMenu);
            PlaylistBrowser.instance.Refresh();
            PlaylistBrowser.instance.RefreshName();

            // Queue casette to move when done moving camera
            willMoveCasette = true;

            // Move camera to outside view
            CameraControl.instance.LerpToView(CameraControl.instance.OutsideCar);
            CameraControl.instance.doSway = true;
        }

        /// <summary>
        /// Goes to post play menu.
        /// </summary>
        public void GoToPostPlayMenu() {

            // Switch mode
            currentState = State.Postplay;

            // Hide other menus
            MoveCasetteBack();
            HideAll();

            // Show postplay menu
            Show(postPlayMenu);

            CameraControl.instance.doSway = true;
        }

        #endregion
        #region Mode Switching Methods

        /// <summary>
        /// Switches to live mode.
        /// </summary>
        public void SwitchToLive() {

            // Switch mode
            currentState = State.Live;
            paused = false;

            // Hide menus
            SnapCasetteBack();
            HideAll();

            // Show live menus
            Show(liveIcons);
            Show(songProgressBar);
            if (MusicManager.instance.loopPlaylist) Show(loopIcon);
            else Hide(loopIcon);

            // Init music
            MusicManager.instance.currentPlayingSong = 0;
            MusicManager.instance.currentSong = (
                MusicManager.instance.currentProject.songs.Count > 0 ? MusicManager.instance.currentProject.songs[0] : null);
            if (MusicManager.instance.currentSong != null) {
                MusicManager.instance.StartPlaylist();
                MusicManager.instance.StartSong();
            }

            // Start live operations
            InstrumentDisplay.instance.Refresh();
            CameraControl.instance.StartLiveMode();
            PlayerMovement.instance.StartMoving();
        }

        /// <summary>
        /// Switches to postplay mode.
        /// </summary>
        public void SwitchToPostplay() {

            // Switch mode
            currentState = State.Postplay;
            paused = false;

            Hide(liveIcons);
            Hide(songProgressBar);

            // Stop music/live operations
            MusicManager.instance.StopPlaying();
            PlayerMovement.instance.StopMoving();
            CameraControl.instance.StopLiveMode();

            // Show prompt
            livePlayQuitPrompt.GetComponent<Image>().color = Color.white;

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

        #endregion
    }
}
