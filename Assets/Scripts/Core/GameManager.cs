// GameManager.cs
// ©2016 Team 95

using Route95.Music;
using Route95.UI;
using Route95.World;

using System.IO;

using UnityEngine;
using UnityEngine.Events;
using UnityStandardAssets.ImageEffects;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Route95.Core {

    /// <summary>
    /// Instanced class to handle all application functions,
    /// major state changes, and UI interactions.
    /// </summary>
    public class GameManager : SingletonMonoBehaviour<GameManager> {

        #region GameManager Enums

        /// <summary>
        /// Enum for various game states.
        /// </summary>
        public enum State {
            Loading,
            Setup,
            Live,
            Postplay
        };

        #endregion
        #region GameManager Nested Classes

        /// <summary>
        /// Class for game-related events.
        /// </summary>
        public class GameEvent : UnityEvent { }

        #endregion
        #region GameManager Vars

        [Header("Game Status")]

        /// <summary>
        /// Is the game paused?
        /// </summary>
	    [Tooltip("Is the game paused?")]
        [SerializeField]
        bool _paused = false;

        /// <summary>
        /// Current game state.
        /// </summary>
	    [Tooltip("Current game state.")]
        [SerializeField]
        State _currentState = State.Loading;

        /// <summary>
        /// Target FPS -- useful for coroutines.
        /// </summary>
	    public static float TargetDeltaTime;

        //----------------------------------------------------------------------
        [Header("Load Status")]

        /// <summary>
        /// Number of load operations performed.
        /// </summary>
	    int _loadOpsCompleted = 0;

        /// <summary>
        /// Total number of load operations to perform.
        /// </summary>
	    int _loadOpsToDo;

        /// <summary>
        /// Is the game loaded?
        /// </summary>
	    [Tooltip("Is the game loaded?")]
        [SerializeField]
        bool _isLoaded = false;

        /// <summary>
        /// Is the game currently loading?
        /// </summary>
        [Tooltip("Is the game currently loading?")]
        [SerializeField]
        bool _isLoading = false;

        /// <summary>
        /// Time at which loading started.
        /// </summary>
	    float _loadStartTime;

        //----------------------------------------------------------------------
        [Header("IO Settings")]

        /// <summary>
        /// Project save folder name.
        /// </summary>
        [Tooltip("Project save folder name.")]
        [SerializeField]
        string _projectSaveFolder = "Projects/";

        /// <summary>
        /// Song save folder name.
        /// </summary>
        [Tooltip("Project save folder name.")]
        [SerializeField]
        string _songSaveFolder = "Songs/";

        /// <summary>
        /// Full path to which to save projects.
        /// </summary>
	    string _projectSavePath;

        /// <summary>
        /// Full path to which to save songs.
        /// </summary>
        string _songSavePath;

        //----------------------------------------------------------------------

        /// <summary>
        /// Main game camera.
        /// </summary>
        Camera _mainCamera;

        /// <summary>
        /// Backup of main camera culling mask.
        /// </summary>
	    int _cullingMaskBackup = 0;

        /// <summary>
        /// Backup of main camera clear flags.
        /// </summary>
	    CameraClearFlags _clearFlagsBackup = CameraClearFlags.Nothing;

        //----------------------------------------------------------------------
        /// <summary>
        /// Invoked when loading begins.
        /// </summary>
        [HideInInspector]
        public GameEvent onStartLoading;

        /// <summary>
        /// Invoked when loading is completed.
        /// </summary>
        [HideInInspector]
        public GameEvent onFinishLoading;

        #endregion
        #region Unity Callbacks

        new void Awake() {
            base.Awake();

            // Init vars
            onStartLoading = new GameEvent();
            onFinishLoading = new GameEvent();

            // Remove profiler sample limit
            Profiler.maxNumberOfSamplesPerFrame = -1;

            // Set application target frame rate
            Application.targetFrameRate = 120;
            TargetDeltaTime = 1f / (float)Application.targetFrameRate;

            // Init save paths
            _projectSavePath = Application.dataPath + _projectSaveFolder;
            _songSavePath = Application.dataPath + _songSaveFolder;

            // Create folders if non-existent
            if (!Directory.Exists(_songSavePath))
                Directory.CreateDirectory(_songSavePath);

            if (!Directory.Exists(_projectSavePath))
                Directory.CreateDirectory(_projectSavePath);
        }

        void Start() {
            // Init camera ref
            _mainCamera = Camera.main;

            // Stop 3D rendering while loading
            StopRendering();

			UIManager.Instance.onSwitchToMainMenu.AddListener(()=> {
				_currentState = State.Setup;
			});

			UIManager.Instance.onSwitchToPlaylistMenu.AddListener(()=> {
				_currentState = State.Setup;
				_paused = false;
			});

			UIManager.Instance.onSwitchToKeySelectMenu.AddListener(()=> {
				_currentState = State.Setup;
			});

			UIManager.Instance.onSwitchToSongArrangeMenu.AddListener(()=> {
				_currentState = State.Setup;
			});

			UIManager.Instance.onSwitchToRiffEditor.AddListener(()=> {
				_currentState = State.Setup;
			});

			UIManager.Instance.onSwitchToPostPlayMenu.AddListener(()=> {
				_currentState = State.Postplay;
				_paused = false;
			});

			UIManager.Instance.onSwitchToLiveMode.AddListener(()=> {
				_currentState = State.Live;
				_paused = false;
			});
        }

        void Update() {
            // Don't update if not loaded
            if (_isLoading) return;

            // Start loading if not loaded
            if (!_isLoaded) {
                onStartLoading.Invoke();
                Load();
            }
        }

        #endregion
        #region GameManager Properties

        /// <summary>
        /// Returns true if the game is paused (read-only).
        /// </summary>
        public bool Paused {
            get { return _paused; }
            set { _paused = value; }
        }

        /// <summary>
        /// Returns GameManager's current state (read-only).
        /// </summary>
        public State CurrentState {
            get { return _currentState; }
            set { _currentState = value; }
        }

        /// <summary>
        /// Returns true if the game is loaded (read-only).
        /// </summary>
        public bool IsLoaded { get { return _isLoaded; } }

        /// <summary>
        /// Returns the folder to which to save songs (read-only).
        /// </summary>
        public string SongSaveFolder { get { return _songSaveFolder; } }

        /// <summary>
        /// Returns the path to which to save songs.
        /// </summary>
        public string SongSavePath { get { return _songSavePath; } }

        /// <summary>
        /// Returns the path to which to save projects.
        /// </summary>
        public string ProjectSavePath { get { return _projectSavePath; } }

        #endregion
        #region GameManager Loading Methods

        /// <summary>
        /// Load this Instance.
        /// </summary>
        void Load() {
            // Hide all menus
            UIManager.Instance.HideAllMenus();

            // Show loading screen
            UIManager.Instance.ShowMenu(LoadingScreen.Instance);

            // Calculate operations to do
            _loadOpsToDo = MusicManager.Instance.LoadOps +
                    WorldManager.Instance.LoadOpsToDo;

            // Init vars
            _loadStartTime = Time.realtimeSinceStartup;
            _isLoading = true;

            // Start by loading MusicManager
            MusicManager.Instance.Load();

            // Throw event
            onStartLoading.Invoke();
        }

        /// <summary>
        /// Used to tell GameManager how many items were just loaded.
        /// </summary>
        /// <param name="numLoaded">Number loaded.</param>
        public void ReportLoaded(int numLoaded) {
            _loadOpsCompleted += numLoaded;
            float loadProgress = (float)_loadOpsCompleted / (float)_loadOpsToDo;

            LoadingBar.Instance.UpdateProgress(loadProgress);
        }

        /// <summary>
        /// Performs all necessary actions after loading.
        /// </summary>
        public void FinishLoading() {

            float loadingTime = Time.realtimeSinceStartup - _loadStartTime;

            // Report to console
            Debug.Log("Fully loaded in " + loadingTime.ToString("0.0000") + 
                " seconds.");

            // Update vars
            _isLoading = false;
            _isLoaded = true;

            // Change state
            _currentState = State.Setup;

            // Move casette back
            Casette.Instance.SnapBack();

            // Hide all menus
            LoadingScreen.Instance.Hide();
            CameraBlocker.Instance.Hide();
            UIManager.Instance.HideAllMenus ();

            // Show main menu
            UIManager.Instance.ShowMenu(MainMenu.Instance);

            // Begin 3D rendering again
            StartRendering();

			onFinishLoading.Invoke();
        }

        #endregion
        #region Rendering Methods

        /// <summary>
        /// Stops 3D rendering on the main camera.
        /// </summary>
        public void StopRendering() {
            _cullingMaskBackup = _mainCamera.cullingMask;
            _clearFlagsBackup = _mainCamera.clearFlags;
            _mainCamera.cullingMask = 0;
            _mainCamera.clearFlags = CameraClearFlags.Nothing;
            _mainCamera.GetComponent<SunShafts>().enabled = false;
            _mainCamera.GetComponent<CameraMotionBlur>().enabled = false;
            _mainCamera.GetComponent<BloomOptimized>().enabled = false;
        }

        /// <summary>
        /// Starts 3D rendering on the main camera
        /// </summary>
        public void StartRendering() {
            _mainCamera.cullingMask = _cullingMaskBackup;
            _mainCamera.clearFlags = _clearFlagsBackup;
            _mainCamera.GetComponent<SunShafts>().enabled = true;
            _mainCamera.GetComponent<CameraMotionBlur>().enabled = true;
            _mainCamera.GetComponent<BloomOptimized>().enabled = true;
        }

        #endregion
        #region Save/Load Methods

        /// <summary>
        /// Saves the current project.
        /// </summary>
        public void SaveCurrentProject() {
            SaveLoad.SaveCurrentProject();
        }

        /// <summary>
        /// Shows the load prompt for projects.
        /// </summary>
        public void ShowLoadPromptForProjects() {
            LoadPrompt.Instance.Refresh(LoadPrompt.Mode.Project);
            UIManager.Instance.ShowMenu(LoadPrompt.Instance);
        }

        /// <summary>
        /// Shows the load prompt for songs.
        /// </summary>
        public void ShowLoadPromptForSongs() {
            LoadPrompt.Instance.Refresh(LoadPrompt.Mode.Song);
            UIManager.Instance.ShowMenu(LoadPrompt.Instance);
        }

        #endregion
        #region Application Methods

        /// <summary>
        /// Attempts to exit the GameManager.Instance.
        /// </summary>
        public void AttemptExit() {
            switch (_currentState) {
                case State.Loading:
                case State.Setup:
                case State.Postplay:
                    ConfirmExitPrompt.Instance.Show();
                    break;

                case State.Live:
                    if (_paused) UIManager.Instance.ShowMenu(ConfirmExitPrompt.Instance);
                    else Pause();
                    break;
            }
        }

        /// <summary>
        /// Toggles paused status.
        /// </summary>
        public void TogglePause() {
            if (_paused) Unpause();
            else Pause();
        }

        /// <summary>
        /// Pause this Instance.
        /// </summary>
        public void Pause() {
            _paused = true;
            PauseMenu.Instance.Show();
            PlayerMovement.Instance.StopMoving();
            CameraControl.Instance.Pause();
        }

        /// <summary>
        /// Unpause this Instance.
        /// </summary>
        public void Unpause() {
            _paused = false;
            PauseMenu.Instance.Hide();
            PlayerMovement.Instance.StartMoving();
            CameraControl.Instance.Unpause();
        }

        /// <summary>
        /// Use this to prevent debug statement spam.
        /// </summary>
        public static bool IsDebugFrame() {
            return (Time.frameCount % 100 == 1);
        }

        /// <summary>
        /// Exit this Instance.
        /// </summary>
        public void Exit() {
            Application.Quit();
        }

        #endregion
    }
}
