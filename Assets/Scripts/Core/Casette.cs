// Casette.cs
// ©2016 Team 95

using Route95.UI;

using UnityEngine;

namespace Route95.Core {

    /// <summary>
    /// Class to handle the physical casette object.
    /// </summary>
    public class Casette : SingletonMonoBehaviour<Casette> {

        #region Casette Vars

        /// <summary>
        /// Is the casette currently moving?
        /// </summary>
        bool _isMoving = false;

        /// <summary>
        /// Will the casette move after camera lerp?
        /// </summary>
        bool _willMoveCasette = false;

        /// <summary>
        /// Speed at which the casette moves.
        /// </summary>
        [Tooltip("Speed at which the casette moves.")]
        [Range(0.5f, 2f)]
        [SerializeField]
        float _moveSpeed = 1f;

        /// <summary>
        /// Position for casette to move in front of camera.
        /// </summary>
        [Tooltip("Position for casette to move in front of camera.")]
        [SerializeField]
        Transform _frontTransform;

        /// <summary>
        /// Position for casette to move in front of camera.
        /// </summary>
        [Tooltip("Position for casette to move behind camera.")]
        [SerializeField]
        Transform _backTransform;

        /// <summary>
        /// Current casette lerp target.
        /// </summary>
        Transform _lerpTargetTransform;

        /// <summary>
        /// Current casette lerp start position.
        /// </summary>
        Transform _lerpStartTransform;

        /// <summary>
        /// Progress of casette lerp.
        /// </summary>
        [Tooltip("Progress of casette lerp.")]
        [SerializeField]
        float _lerpProgress;

        #endregion
        #region Unity Callbacks

        new void Awake () {
            base.Awake();

            // Init vars
            _lerpProgress = 0f;
        }

		void Start () {

			UIManager.Instance.onSwitchToMainMenu.AddListener(()=> {
				MoveToBack();
			});

			UIManager.Instance.onSwitchToKeySelectMenu.AddListener(()=> {
				MoveToBack();
			});

			UIManager.Instance.onSwitchToSongArrangeMenu.AddListener(()=> {
				MoveToBack();
			});

			UIManager.Instance.onSwitchToRiffEditor.AddListener(()=> {
				MoveToBack();
			});

			UIManager.Instance.onSwitchToPlaylistMenu.AddListener(()=> {
				_willMoveCasette = true;
			});

			UIManager.Instance.onSwitchToPostPlayMenu.AddListener(()=> {
				MoveToBack();
			});

			UIManager.Instance.onSwitchToLiveMode.AddListener(()=> {
				SnapBack();
			});
		}

        void Update() {
            // Move casette
            if (_isMoving) {

                Vector3 endPos = _lerpTargetTransform.position;
                Quaternion endRot = _lerpTargetTransform.rotation;

                if (_lerpProgress < 1f) {

                    Vector3 startPos = _lerpStartTransform.position;
                    Quaternion startRot = _lerpStartTransform.rotation;

                    // Increment lerp progress
                    _lerpProgress += _moveSpeed * Time.deltaTime;

                    // Calculate new position/rotation
                    Vector3 pos = Vector3.Lerp(startPos, endPos, _lerpProgress);
                    Quaternion rot = Quaternion.Lerp(startRot, endRot, _lerpProgress);

                    // Assign new position/rotation
                    transform.position = pos;
                    transform.rotation = rot;

                } else {
                    // Finish moving
                    _isMoving = false;
                     transform.position = endPos;
                     transform.rotation = endRot;
                }
            }
        }

        #endregion
        #region Properties

        public bool WillMove {
            get { return _willMoveCasette; }
            set { _willMoveCasette = value; }
        }

        #endregion
        #region Methods

        /// <summary>
        /// Moves the casette front.
        /// </summary>
        public void MoveToFront() {
            _isMoving = true;
            _lerpStartTransform = transform;
            _lerpTargetTransform = _frontTransform;
            _lerpProgress = 0f;
            _willMoveCasette = false;
        }

        /// <summary>
        /// Moves the casette back.
        /// </summary>
        public void MoveToBack() {
            _isMoving = true;
            _lerpStartTransform = transform;
            _lerpTargetTransform = _backTransform;
            _lerpProgress = 0f;
            _willMoveCasette = false;
        }

        /// <summary>
        /// Instantly moves the casette back.
        /// </summary>
        public void SnapBack() {
            _isMoving = false;
            _lerpStartTransform = _lerpTargetTransform = _backTransform;
            _willMoveCasette = false;
            SnapToTarget();
        }

        /// <summary>
        /// Snaps the casette to its current target transform.
        /// </summary>
        void SnapToTarget () {
            transform.position = _lerpTargetTransform.position;
            transform.rotation = _lerpTargetTransform.rotation;
        }

        /// <summary>
        /// If set to move casette, will do so.
        /// </summary>
        public void AttemptMove() {
            if (_willMoveCasette) MoveToFront();
        }

        #endregion
    }
}
