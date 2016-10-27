// CameraControl.cs
// ©2016 Team 95

using Route95.Core;
using Route95.UI;

using System.Collections.Generic;

using UnityEngine;

namespace Route95.World {

    /// <summary>
    /// Class to manage the game camera.
    /// </summary>
    public class CameraControl : SingletonMonoBehaviour<CameraControl> {

        #region CameraControl Enums

        /// <summary>
        /// Current state of the game camera.
        /// </summary>
        public enum State {
            Setup,
            Live,
            Free
        }

        /// <summary>
        /// Current type of camera control.
        /// </summary>
        public enum CameraControlMode {
            Manual, // Angles only change on user input
            Random  // Angles cycle randomly through all available angles
        }

        #endregion
        #region CameraControl Vars

        [Header("General camera settings")]

		/// <summary>
		/// Current camera state.
		/// </summary>
        [Tooltip("Current camera state.")]
		[SerializeField]
        State _state = State.Setup;

		/// <summary>
		/// Free camera rotate sensitivity.
		/// </summary>
        [Tooltip("Free camera rotate sensitivity.")]
		[SerializeField]
        float _rotateSensitivity = 0.25f;

		/// <summary>
		/// Free camera movement sensitivity.
		/// </summary>
        [Tooltip("Free camera movement sensitivity.")]
		[SerializeField]
        float _moveSensitivity = 0.4f;

		/// <summary>
		/// Camera sway speed.
		/// </summary>
        [Tooltip("Camera sway speed.")]
		[SerializeField]
        float _swaySpeed = 1f;

		/// <summary>
		/// Camera base sway amount.
		/// </summary>
        [Tooltip("Camera base sway amount.")]
		[SerializeField]
        float _baseSway = 1f;

		/// <summary>
		/// Camera sway enabled.
		/// </summary>
        bool _doSway = true;

		/// <summary>
		/// Default camera speed.
		/// </summary>
        const float DEFAULT_SPEED = 1f;

		/// <summary>
		/// Default camera FOV.
		/// </summary>
        const float DEFAULT_FOV = 75f;

        /// <summary>
		/// Initial camera view.
		/// </summary>
        CameraView _initialView;

		/// <summary>
		/// Camera lerp start transform.
		/// </summary>
        Transform _startTransform;

		/// <summary>
		/// Camera lerp start FOV.
		/// </summary>
        float _startFOV;

		/// <summary>
		/// Camera lerp target view.
		/// </summary>
        CameraView _targetView;

		/// <summary>
		/// Current camera speed.
		/// </summary>
        float _speed;

		/// <summary>
		/// Is the camera currently lerping?
		/// </summary>
        bool _moving = false;

		/// <summary>
		/// Lerp progress.
		/// </summary>
        [SerializeField]
        float _progress = 0f;

        /// <summary>
		/// Current live mode angle.
		/// </summary>
        CameraView _currentAngle;

		/// <summary>
		/// List of all live mode angles.
		/// </summary>
        List<CameraView> _liveAngles;

		/// <summary>
		/// Timer to next angle change.
		/// </summary>
        [SerializeField]
        float _transitionTimer;

		/// <summary>
		/// Is live mode paused?
		/// </summary>
        bool _paused = false;

		/// <summary>
		/// Distance to car for camera to reset.
		/// </summary>
        float _resetDistance;

        Transform _viewOutsideCar;
        Transform _viewDriving;
        Transform _viewRadio;
        Transform _viewChase;

        CameraView _outsideCar;
        CameraView _driving;
        CameraView _radio;

        Transform _hoodForward;
        Transform _nearChase;
        Transform _farChase;
        Transform _frontLeftWheel;
        Transform _frontRightWheel;
        Transform _rearLeftWheel;
        Transform _rearRightWheel;
        Transform _rideRear;
        Transform _rideFront;

		/// <summary>
		/// Frequency with which to automatically change camera angle in live mode.
		/// </summary>
        [Tooltip("Frequency with which to automatically change camera angle in live mode.")]
        [Range(1f, 600f)]
		[SerializeField]
        float _liveModeTransitionFreq;

		/// <summary>
		/// Initial control mode.
		/// </summary>
        [Tooltip("Initial control mode.")]
        CameraControlMode _controlMode = CameraControlMode.Random;

		/// <summary>
		/// Camera lerp target angle.
		/// </summary>
        int _targetAngle = -1;

        // Mappings of keys to camera angle indices
        static Dictionary<KeyCode, int> _KeyToView = new Dictionary<KeyCode, int>() {
			{ KeyCode.F1, 0 },
			{ KeyCode.F2, 1 },
			{ KeyCode.F3, 2 },
			{ KeyCode.F4, 3 },
			{ KeyCode.F5, 4 },
			{ KeyCode.F6, 5 },
			{ KeyCode.F7, 6 },
			{ KeyCode.F8, 7 },
			{ KeyCode.F9, 8 },
			{ KeyCode.F10, 9 }
		};

        #endregion
        #region Unity Callbacks

        new void Awake() {
            base.Awake();

			// Init vars
            _transitionTimer = _liveModeTransitionFreq;
            _speed = DEFAULT_SPEED;

            // Outside car
			_viewOutsideCar = GameObject.FindGameObjectWithTag("ViewOutsideCar").transform;
            _outsideCar = new CameraView() {
                name = "OutsideCar",
                transform = _viewOutsideCar,
                targetPos = _viewOutsideCar.position,
                targetRot = _viewOutsideCar.rotation,
                fov = DEFAULT_FOV,
                followMode = CameraView.CameraFollowMode.Static
            };

            // Driving
			_viewDriving = GameObject.FindGameObjectWithTag("ViewDriving").transform;
            _driving = new CameraView() {
                name = "Driving",
                transform = _viewDriving,
                targetPos = _viewDriving.position,
                targetRot = _viewDriving.rotation,
                fov = DEFAULT_FOV,
                followMode = CameraView.CameraFollowMode.Static
            };

            // Radio
			_viewRadio = GameObject.FindGameObjectWithTag("ViewRadio").transform;
            _radio = new CameraView() {
                name = "Radio",
                transform = _viewRadio,
                targetPos = _viewRadio.position,
                targetRot = _viewRadio.rotation,
                fov = 20f,
                followMode = CameraView.CameraFollowMode.Static
            };

            _initialView = _outsideCar;
            SnapToView(_initialView);

			_hoodForward = GameObject.FindGameObjectWithTag("ViewHoodForward").transform;
			_nearChase = GameObject.FindGameObjectWithTag("ViewNearChase").transform;
			_farChase = GameObject.FindGameObjectWithTag("ViewFarChase").transform;
			_frontRightWheel = GameObject.FindGameObjectWithTag("ViewFrontRightWheel").transform;
			_frontLeftWheel = GameObject.FindGameObjectWithTag("ViewFrontLeftWheel").transform;
			_rearRightWheel = GameObject.FindGameObjectWithTag("ViewRearRightWheel").transform;
			_rearLeftWheel = GameObject.FindGameObjectWithTag("ViewRearLeftWheel").transform;

            // Init live mode angles
            _liveAngles = new List<CameraView>() {

				// On the hood, forwards
				new CameraView () {
					name = "HoodForward",
					transform = _hoodForward,
					targetPos = _hoodForward.position,
					targetRot = _hoodForward.rotation,
					fov = DEFAULT_FOV,
					followMode = CameraView.CameraFollowMode.Static
				},

				// Near chase
				new CameraView () {
					name = "NearChase",
					transform = _nearChase,
					targetPos = _nearChase.position,
					targetRot = _nearChase.rotation,
					fov = DEFAULT_FOV,
					followMode = CameraView.CameraFollowMode.Lead,
					placementMode = CameraView.CameraPlacementMode.Fixed,
					lag = 0.04f
				},

				// Far chase
				new CameraView () {
					name = "FarChase",
					transform = _farChase,
					targetPos = _farChase.position,
					targetRot = _farChase.rotation,
					fov = DEFAULT_FOV,
					followMode = CameraView.CameraFollowMode.Lead,
					placementMode = CameraView.CameraPlacementMode.Fixed,
					lag = 0.2f
				},

				// Front right wheel
				new CameraView () {
					name = "FrontRightWheel",
					transform = _frontRightWheel,
					targetPos = _frontRightWheel.position,
					targetRot = _frontRightWheel.rotation,
					fov = DEFAULT_FOV,
					followMode = CameraView.CameraFollowMode.Static,
					placementMode = CameraView.CameraPlacementMode.Fixed
				},

				// Front left wheel
				new CameraView () {
					name = "FrontLeftWheel",
					transform = _frontLeftWheel,
					targetPos = _frontLeftWheel.position,
					targetRot = _frontLeftWheel.rotation,
					fov = DEFAULT_FOV,
					followMode = CameraView.CameraFollowMode.Static,
					placementMode = CameraView.CameraPlacementMode.Fixed
				},

				// Rear right wheel
				new CameraView () {
					name = "RearRightWheel",
					transform = _rearRightWheel,
					targetPos = _rearRightWheel.position,
					targetRot = _rearRightWheel.rotation,
					fov = DEFAULT_FOV,
					followMode = CameraView.CameraFollowMode.Static,
					placementMode = CameraView.CameraPlacementMode.Fixed
				},

				// Rear left wheel
				new CameraView () {
					name = "RearLeftWheel",
					transform = _rearLeftWheel,
					targetPos = _rearLeftWheel.position,
					targetRot = _rearLeftWheel.rotation,
					fov = DEFAULT_FOV,
					followMode = CameraView.CameraFollowMode.Static,
					placementMode = CameraView.CameraPlacementMode.Fixed
				},

				// Rear left wheel
				new CameraView () {
					name = "RearLeftWheel",
					transform = _rearLeftWheel,
					targetPos = _rearLeftWheel.position,
					targetRot = _rearLeftWheel.rotation,
					fov = DEFAULT_FOV,
					followMode = CameraView.CameraFollowMode.Static,
					placementMode = CameraView.CameraPlacementMode.Fixed
				},

				// Far top
				new CameraView () {
					name = "FarTop",
					targetPos = PickRandomPosition (25f, 50f),
					fov = 60f,
					followMode = CameraView.CameraFollowMode.Shaky,
					placementMode = CameraView.CameraPlacementMode.RandomSky
				},

				// Distant
				new CameraView () {
					name = "Distant",
					targetPos = PickRandomPosition (10f, 20f),
					fov = 60f,
					followMode = CameraView.CameraFollowMode.Shaky,
					placementMode = CameraView.CameraPlacementMode.RandomGround
				}
			};

            foreach (CameraView angle in _liveAngles) {
                angle.pos = angle.targetPos;
                angle.rot = angle.targetRot;
            }
        }

        void Start() {
            _resetDistance = WorldManager.Instance.chunkLoadRadius *
                0.5f * WorldManager.Instance.chunkSize;
            _currentAngle = _outsideCar;
        }

        void Update() {

            switch (_state) {
                case State.Setup:
                    if (_moving) {

                        if (_progress < 1f) {

                            _progress += _speed * Time.deltaTime;

                            // Lerp position
                            transform.position = Vector3.Slerp(_startTransform.position, _targetView.transform.position, _progress);

                            // Lerp Rotation
                            transform.rotation = Quaternion.Slerp(_startTransform.rotation, _targetView.transform.rotation, _progress);

                            Camera.main.fieldOfView = Mathf.Lerp(_startFOV, _targetView.fov, Mathf.Sqrt(_progress));

                        }
                        else {
                            _moving = false;
                            _startTransform = _targetView.transform;
                            transform.position = _startTransform.position;
                            transform.rotation = _startTransform.rotation;
                            OnCompleteLerp();
                        }
                    }
                    break;


                case State.Live:
                    if (!_paused) {

                        // Check each mapped key for input
                        foreach (KeyCode key in _KeyToView.Keys) {
                            if (Input.GetKeyDown(key) && CameraBlocker.Instance.GetComponent<Fadeable>().NotFading) {
                                if (_controlMode != CameraControlMode.Manual)
                                    _controlMode = CameraControlMode.Manual;
                                StartFade();
                                _targetAngle = _KeyToView[key];
                            }
                        }

                        UpdateAllAngles();

                        // Move camera to current angle position
                        transform.position = _currentAngle.pos;
                        transform.rotation = _currentAngle.rot;

                        // Update transition timer
                        if (_controlMode == CameraControlMode.Random) {
                            if (_transitionTimer <= 0f) {
                                _transitionTimer = _liveModeTransitionFreq;
                                _targetAngle = -1;
                                StartFade();
                            }
                            else _transitionTimer--;
                        }

                        if (CameraBlocker.Instance.GetComponent<Fadeable>().DoneUnfading) {
                            UIManager.Instance.HideMenu(CameraBlocker.Instance);
                            if (_targetAngle == -1) ChangeAngle();
                            else ChangeAngle(_targetAngle);
                        }

                        // Check if camera is out of range
                        float distToPlayer = Vector3.Distance(transform.position, PlayerMovement.Instance.transform.position);
                        if (distToPlayer > _resetDistance && !CameraBlocker.Instance.GetComponent<Fadeable>().Busy)
                            StartFade();
                    }
                    break;

                case State.Free:

                    // Rotate camera
                    Vector3 d = InputManager.Instance.MouseDelta;
                    Vector3 old = transform.rotation.eulerAngles;
                    bool slow = Input.GetAxisRaw("Slow") != 0f;
                    old.z = 0f;
                    old.x += -d.y * _rotateSensitivity * (slow ? 0.05f : 1f);
                    old.y += d.x * _rotateSensitivity * (slow ? 0.05f : 1f);
                    transform.rotation = Quaternion.Euler(old);

                    // Translate camera
                    float forward = Input.GetAxisRaw("Forward") * _moveSensitivity * (slow ? 0.05f : 1f);
                    float up = Input.GetAxisRaw("Up") * _moveSensitivity * (slow ? 0.05f : 1f);
                    float right = Input.GetAxisRaw("Right") * _moveSensitivity * (slow ? 0.05f : 1f);
                    transform.Translate(new Vector3(right, up, forward));

                    break;

            }

            if (_doSway) {

                // Calculate sway
                float bx = ((Mathf.PerlinNoise(0f, Time.time * _swaySpeed) - 0.5f)) * _baseSway * Camera.main.fieldOfView / DEFAULT_FOV;
                float by = ((Mathf.PerlinNoise(0f, (Time.time * _swaySpeed) + 100f)) - 0.5f) * _baseSway * Camera.main.fieldOfView / DEFAULT_FOV;

                // Do sway
                transform.Rotate(bx, by, 0f);
            }
        }

        #endregion
        #region CameraControl Live Mode Methods

        /// <summary>
        /// Start camera live mode.
        /// </summary>
        public void StartLiveMode() {
            if (_state == State.Free) return;
            _state = State.Live;
            ChangeAngle();
            _paused = false;
        }

        /// <summary>
        /// Pause camera live mode.
        /// </summary>
        public void StopLiveMode() {
            _state = State.Setup;
            _paused = true;
        }

        /// <summary>
        /// Start camera free mode.
        /// </summary>
        public void StartFreeMode() {
            _state = State.Free;
            transform.rotation = Quaternion.identity;
            Casette.Instance.MoveToBack();
            UIManager.Instance.HideAllMenus();
            UIManager.Instance.HideMenu(SystemButtons.Instance);
            UIManager.Instance.HideMenu(ExitButton.Instance);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }

        /// <summary>
        /// Stop camera free mode.
        /// </summary>
        public void StopFreeMode() {
            _state = State.Setup;
        }

        public void StartFade() {
            UIManager.Instance.ShowMenu(CameraBlocker.Instance);
        }

        /// <summary>
        /// Pause camera movement.
        /// </summary>
        public void Pause() {
            _paused = true;
        }

        /// <summary>
        /// Unpause camera movement.
        /// </summary>
        public void Unpause() {
            _paused = false;
        }

        /// <summary>
        /// Pick a random live camera angle.
        /// </summary>
        public void ChangeAngle() {
            ChangeAngle(Random.Range(0, _liveAngles.Count));
        }

        /// <summary>
        /// Pick a specific live camera angle.
        /// </summary>
        /// <param name="camView"></param>
        public void ChangeAngle(int camView) {
            _currentAngle = _liveAngles[camView];
            GetComponent<Camera>().fieldOfView = _currentAngle.fov;
            //if (Debug.isDebugBuild) 
            //Debug.Log("CameraControl.ChangeAngle(): switch to view \"" + currentAngle.name +".\"");

            switch (_currentAngle.placementMode) {

                case CameraView.CameraPlacementMode.Fixed:
                    break;

                case CameraView.CameraPlacementMode.RandomGround:
                    _currentAngle.targetPos = PickRandomPosition(10f, 20f);
                    break;
                case CameraView.CameraPlacementMode.RandomSky:
                    _currentAngle.targetPos = PickRandomPosition(25f, 50f);
                    break;
            }
        }

        /// <summary>
        /// Warms all live mode camera angles.
        /// </summary>
        void UpdateAllAngles() {
            foreach (CameraView angle in _liveAngles) {
                if (angle.transform != null) {
                    angle.targetPos = angle.transform.position;
                    angle.targetRot = angle.transform.rotation;
                }

                switch (angle.followMode) {

                    case CameraView.CameraFollowMode.Lead:
                        //angle.pos = Vector3.Lerp(angle.pos, angle.targetPos, angle.lag);
                        //angle.pos = angle.pos + (angle.targetPos - angle.pos) * angle.lag;
                        angle.pos = angle.targetPos;
                        angle.rot = Quaternion.LookRotation(PlayerMovement.Instance.transform.position + PlayerMovement.Instance.transform.forward * 20f - angle.pos, Vector3.up);
                        break;

                    case CameraView.CameraFollowMode.Static:
                        angle.pos = angle.targetPos;
                        angle.rot = angle.targetRot;
                        break;

                    case CameraView.CameraFollowMode.Shaky:
                        angle.pos = angle.targetPos;
                        transform.LookAt(PlayerMovement.Instance.transform.position, Vector3.up);
                        angle.rot = transform.rotation;
                        break;
                }

            }
        }

        /// <summary>
        /// Picks a random position.
        /// </summary>
        /// <param name="minHeight"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        Vector3 PickRandomPosition(float minHeight, float maxHeight) {
            float chunkSize = WorldManager.Instance.chunkSize;

            Vector3 point = new Vector3(
                PlayerMovement.Instance.transform.position.x + Random.Range(-chunkSize / 2f, chunkSize / 2f),
                0f,
                PlayerMovement.Instance.transform.position.z + Random.Range(-chunkSize / 2f, chunkSize / 2f)
            );

            Vector3 rayOrigin = new Vector3(point.x, WorldManager.Instance.heightScale, point.z);

            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity))
                point.y = hit.point.y;

            point.y += Random.Range(minHeight, maxHeight);

            return point;
        }

        #endregion
        #region CameraControl Non-Live Mode Methods

        /// <summary>
        /// Sets the camera lerp speed.
        /// </summary>
        /// <param name="newSpeed"></param>
        public void SetSpeed(float newSpeed) {
            _speed = newSpeed;
        }

        /// <summary>
        /// Teleports the camera to a position.
        /// </summary>
        /// <param name="newPosition"></param>
        public void SnapToView(CameraView newView) {
            _targetView = newView;
            _startTransform = newView.transform;
            transform.position = newView.transform.position;
            transform.rotation = newView.transform.rotation;
            Camera.main.fieldOfView = newView.fov;
        }

        public void LerpToView(CameraView newView, float newSpeed = DEFAULT_SPEED) {
            if (_targetView == newView) {
                OnCompleteLerp();
                return;
            }

            CameraView oldView = _targetView;
            _startFOV = oldView.fov;
            _startTransform = oldView.transform;

            _targetView = newView;
            _moving = true;
            _speed = newSpeed;
            _progress = 0f;
        }

        void OnCompleteLerp() {
            Casette.Instance.AttemptMove();
        }

        #endregion

    }
}
