using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class to manage the game camera.
/// </summary>
public class CameraControl : MonoBehaviour {

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

	public static CameraControl instance; // Quick reference to this instance

	[Tooltip("Current camera state.")]
	public State state = State.Setup;

	[Tooltip("Free camera rotate sensitivity.")]
	public float rotateSensitivity = 0.25f;

	[Tooltip("Free camera movement sensitivity.")]
	public float moveSensitivity = 0.4f;

	[Tooltip("Camera sway speed.")]
	public float swaySpeed = 1f;

	[Tooltip("Camera base sway amount.")]
	public float baseSway = 1f;

	const float DEFAULT_SPEED = 0.2f;     // Default camera speed

	public GameObject CameraBlocker;

	// Camera interp vars
	Transform start;                      // Camera lerp start transform
	Transform target;                     // Camera lerp target transform
	float speed = DEFAULT_SPEED;          // Camera speed
	bool moving = false;                  // Is the camera currently lerping?
	float progress = 0f;                  // Lerp progress

	// Live mode camera angle vars
	CameraView currentAngle;              // Current live mode angle
	List<CameraView> angles;              // List of all live mode angles
	float transitionTimer;                // Timer to next angle change
	bool paused = false;                  // Is live mode paused?

	[Tooltip("Initial position for camera")]
	public Transform initialPosition;

	public Transform ViewOutsideCar;
	public Transform ViewDriving;
	public Transform ViewRadio;
	public Transform ViewChase;

	public Transform HoodForward;
	public Transform HoodBackward;
	public Transform NearChase;
	public Transform FarChase;
	public Transform FrontLeftWheel;
	public Transform FrontRightWheel;
	public Transform RearLeftWheel;
	public Transform RearRightWheel;
	public Transform WideRear;
	public Transform WideFront;

	[Tooltip("Frequency with which to automatically change camera angle in live mode.")]
	[Range(1f, 600f)]
	public float liveModeTransitionFreq;

	[Tooltip("Initial control mode.")]
	public CameraControlMode controlMode = CameraControlMode.Random;

	// Mappings of keys to camera angle indices
	static Dictionary <KeyCode, int> keyToView = new Dictionary<KeyCode, int> () {
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

	void Awake () {
		instance = this;
		SnapToPosition (initialPosition);
		transitionTimer = liveModeTransitionFreq;

		// Init live mode angles
		angles  = new List<CameraView> () {

			// On the hood, forwards
			new CameraView () {
				name = "HoodForward",
				transform = HoodForward,
				targetPos = HoodForward.position,
				targetRot = HoodForward.rotation,
				fov = 75f,
				followMode = CameraView.CameraFollowMode.Static
			},

			// Near chase
			new CameraView () {
				name = "NearChase",
				transform = NearChase,
				targetPos = NearChase.position,
				targetRot = NearChase.rotation,
				fov = 75f,
				followMode = CameraView.CameraFollowMode.Lead,
				placementMode = CameraView.CameraPlacementMode.Fixed,
				lag = 0.04f
			},

			// Far chase
			new CameraView () {
				name = "FarChase",
				transform = FarChase,
				targetPos = FarChase.position,
				targetRot = FarChase.rotation,
				fov = 75f,
				followMode = CameraView.CameraFollowMode.Lead,
				placementMode = CameraView.CameraPlacementMode.Fixed,
				lag = 0.2f
			},

			// Front right wheel
			new CameraView () {
				name = "FrontRightWheel",
				transform = FrontRightWheel,
				targetPos = FrontRightWheel.position,
				targetRot = FrontRightWheel.rotation,
				fov = 75f,
				followMode = CameraView.CameraFollowMode.Static,
				placementMode = CameraView.CameraPlacementMode.Fixed
			},

			// Front left wheel
			new CameraView () {
				name = "FrontLeftWheel",
				transform = FrontLeftWheel,
				targetPos = FrontLeftWheel.position,
				targetRot = FrontLeftWheel.rotation,
				fov = 75f,
				followMode = CameraView.CameraFollowMode.Static,
				placementMode = CameraView.CameraPlacementMode.Fixed
			},

			// Rear right wheel
			new CameraView () {
				name = "RearRightWheel",
				transform = RearRightWheel,
				targetPos = RearRightWheel.position,
				targetRot = RearRightWheel.rotation,
				fov = 75f,
				followMode = CameraView.CameraFollowMode.Static,
				placementMode = CameraView.CameraPlacementMode.Fixed
			},

			// Rear left wheel
			new CameraView () {
				name = "RearLeftWheel",
				transform = RearLeftWheel,
				targetPos = RearLeftWheel.position,
				targetRot = RearLeftWheel.rotation,
				fov = 75f,
				followMode = CameraView.CameraFollowMode.Static,
				placementMode = CameraView.CameraPlacementMode.Fixed
			},

			// Rear left wheel
			new CameraView () {
				name = "RearLeftWheel",
				transform = RearLeftWheel,
				targetPos = RearLeftWheel.position,
				targetRot = RearLeftWheel.rotation,
				fov = 75f,
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

		foreach (CameraView angle in angles) {
			angle.pos = angle.targetPos;
			angle.rot = angle.targetRot;
		}
	}

	void Update() {

		switch (state) {
		case State.Setup:
			if (moving) {

				if (Vector3.Distance(start.position, target.position) != 0f) {

					progress += speed * Time.deltaTime / 
						Vector3.Distance(start.position, target.transform.position);

					// Lerp position
					transform.position = Vector3.Lerp(start.position, target.position, progress);

					// Lerp Rotation
					transform.rotation = Quaternion.Lerp(start.rotation, target.rotation, progress);

				} else {
					GameManager.instance.AttemptMoveCasette();
					start = target;
					moving = false;
				}
			}
			break;


		case State.Live:
			if (!paused) {

				// Check each mapped key for input
				foreach (KeyCode key in keyToView.Keys) {
					if (Input.GetKeyDown (key)) {
						if (controlMode != CameraControlMode.Manual)
							controlMode = CameraControlMode.Manual;
						ChangeAngle (keyToView [key]);
					}
				}
					
				UpdateAllAngles();

				// Move camera to current angle position
				transform.position = currentAngle.pos;
				transform.rotation = currentAngle.rot;

				// Update transition timer
				if (controlMode == CameraControlMode.Random) {
					if (transitionTimer <= 0f) {
						StartFade();
						transitionTimer = liveModeTransitionFreq;
					} else {
						transitionTimer--;
						if (CameraBlocker.GetComponent<Fadeable>().DoneUnfading) {
							GameManager.instance.Hide(CameraBlocker);
							ChangeAngle();
						}
					}
				}
			}
			break;

		case State.Free:

			// Rotate camera
			Vector3 d = InputManager.instance.mouseDelta;
			Vector3 old = transform.rotation.eulerAngles;
			bool slow = Input.GetAxisRaw("Slow") != 0f;
			old.z = 0f;
			old.x += -d.y * rotateSensitivity * (slow ? 0.05f : 1f);
			old.y += d.x * rotateSensitivity * (slow ? 0.05f : 1f);
			transform.rotation = Quaternion.Euler(old);

			// Translate camera
			float forward = Input.GetAxisRaw("Forward") * moveSensitivity * (slow ? 0.05f : 1f);
			float up = Input.GetAxisRaw("Up") * moveSensitivity * (slow ? 0.05f : 1f);
			float right = Input.GetAxisRaw("Right") * moveSensitivity * (slow ? 0.05f : 1f);
			transform.Translate(new Vector3 (right, up, forward));

			break;

		}

		// Calculate sway
		float bx = ((Mathf.PerlinNoise (0f, Time.time*swaySpeed)-0.5f)) * baseSway;
		float by = ((Mathf.PerlinNoise (0f, (Time.time*swaySpeed)+100f))-0.5f) * baseSway;

		// Do sway
		transform.Rotate (bx, by, 0f);
	}

	#endregion
	#region CameraControl Live Mode Methods

	/// <summary>
	/// Start camera live mode.
	/// </summary>
	public void StartLiveMode () {
		if (state == State.Free) return;
		state = State.Live;
		ChangeAngle();
		paused = false;
	}

	/// <summary>
	/// Pause camera live mode.
	/// </summary>
	public void StopLiveMode () {
		state = State.Setup;
		LerpToPosition(ViewChase);
		paused = true;
	}

	/// <summary>
	/// Start camera free mode.
	/// </summary>
	public void StartFreeMode () {
		state = State.Free;
		transform.rotation = Quaternion.identity;
		GameManager.instance.MoveCasetteBack();
		GameManager.instance.HideAll();
		GameManager.instance.Hide(GameManager.instance.systemButtons);
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = false;
	}

	/// <summary>
	/// Stop camera free mode.
	/// </summary>
	public void StopFreeMode () {
		state = State.Setup;
		LerpToPosition(ViewChase);
	}

	public void StartFade () {
		GameManager.instance.Show(CameraBlocker);
	}

	/// <summary>
	/// Pause camera movement.
	/// </summary>
	public void Pause () {
		paused = true;
	}

	/// <summary>
	/// Unpause camera movement.
	/// </summary>
	public void Unpause () {
		paused = false;
	}

	/// <summary>
	/// Pick a random live camera angle.
	/// </summary>
	public void ChangeAngle () {
		ChangeAngle (Random.Range (0, angles.Count));
	}

	/// <summary>
	/// Pick a specific live camera angle.
	/// </summary>
	/// <param name="camView"></param>
	public void ChangeAngle (int camView) {
		if (currentAngle == angles[camView]) return;
		currentAngle = angles[camView];
		GetComponent<Camera>().fieldOfView = currentAngle.fov;
		if (Debug.isDebugBuild) 
			Debug.Log("CameraControl.ChangeAngle(): switch to view \"" + currentAngle.name +".\"");
		
		switch (currentAngle.placementMode) {

			case CameraView.CameraPlacementMode.Fixed:
				break;

			case CameraView.CameraPlacementMode.RandomGround:
				currentAngle.targetPos = PickRandomPosition (10f, 20f);
				break;
			case CameraView.CameraPlacementMode.RandomSky:
				currentAngle.targetPos = PickRandomPosition (25f, 50f);
				break;
		}
	}

	/// <summary>
	/// Warms all live mode camera angles.
	/// </summary>
	void UpdateAllAngles () {
		foreach (CameraView angle in angles) {
			if (angle.transform != null) {
				angle.targetPos = angle.transform.position;
				angle.targetRot = angle.transform.rotation;
			}

			switch (angle.followMode) {

				case CameraView.CameraFollowMode.Lead:
					//angle.pos = Vector3.Lerp(angle.pos, angle.targetPos, angle.lag);
					//angle.pos = angle.pos + (angle.targetPos - angle.pos) * angle.lag;
					angle.pos = angle.targetPos;
					angle.rot = Quaternion.LookRotation (PlayerMovement.instance.transform.position + PlayerMovement.instance.transform.forward *20f - angle.pos, Vector3.up);
					break;

				case CameraView.CameraFollowMode.Static:
					angle.pos = angle.targetPos;
					angle.rot = angle.targetRot;
					break;

				case CameraView.CameraFollowMode.Shaky:
					angle.pos = angle.targetPos;
				transform.LookAt (PlayerMovement.instance.transform.position, Vector3.up);
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
	Vector3 PickRandomPosition (float minHeight, float maxHeight) {
		float chunkSize = WorldManager.instance.chunkSize;

		

		Vector3 point = new Vector3 (
			PlayerMovement.instance.transform.position.x + Random.Range (-chunkSize / 2f, chunkSize / 2f),
			0f,
			PlayerMovement.instance.transform.position.z + Random.Range (-chunkSize / 2f, chunkSize / 2f)
		);

		Vector3 rayOrigin = new Vector3 (point.x, WorldManager.instance.heightScale, point.z);

		RaycastHit hit;
		if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity))
			point.y = hit.point.y;

		point.y += Random.Range (minHeight, maxHeight);

		return point;
	}

	#endregion
	#region CameraControl Non-Live Mode Methods

	/// <summary>
	/// Sets the camera lerp speed.
	/// </summary>
	/// <param name="newSpeed"></param>
	public void SetSpeed (float newSpeed) {
		speed = newSpeed;
	}

	/// <summary>
	/// Teleports the camera to a position.
	/// </summary>
	/// <param name="newPosition"></param>
	public void SnapToPosition (Transform newPosition) {
		start = newPosition;
		target = newPosition;
		transform.position = newPosition.position;
		transform.rotation = newPosition.rotation;
	}

	/// <summary>
	/// Linearly interpolates the camera to a position.
	/// </summary>
	/// <param name="newPosition"></param>
	/// <param name="newSpeed"></param>
	public void LerpToPosition (Transform newPosition, float newSpeed=DEFAULT_SPEED) {
		Camera.main.fieldOfView = 75f;
		start = transform;
		target = newPosition;
		moving = true;
		speed = newSpeed;
		progress = 0f;
	}
		
	#endregion
		
}
