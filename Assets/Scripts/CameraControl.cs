using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum View {
	OutsideCar,
	Driving,
	Radio,
	Chase
}

public enum CameraControlMode {
	Manual,
	Random
}

public enum CameraFollowMode {
	Lead, // Points in front of target
	Static,
	Shaky
}

public class CameraView {
	public string name;
	public Transform transform;
	public float fov; // field of view
	public CameraFollowMode followMode; // type of camera following
	public Vector3 pos; // current position of camera
	public Quaternion rot; // current rotation of camera

	public Vector3 targetPos; // target transform
	public Quaternion targetRot;

	public float lag; // how tightly camera follows (lower = tighter)
	public float shake;
}

public class CameraControl : MonoBehaviour {
	public static CameraControl instance;

	public Transform initialPosition;

	Transform startPosition;
	Transform targetPosition;
	public float speed;
	float sTime;

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

	CameraView currentAngle;
	List<CameraView> angles;
	public float transitionFreq;
	float transitionTimer;
	public bool liveMode = false;

	bool moving = false;

	public void StartLiveMode () {
		liveMode = true;
		ChangeAngle();
	}

	public void SetSpeed (float newSpeed) {
		speed = newSpeed;
	}

	public void SnapToPosition (Transform newPosition) {
		startPosition = newPosition;
		targetPosition = newPosition;
		GetComponent<Transform>().position = newPosition.position;
		GetComponent<Transform>().rotation = newPosition.rotation;
	}

	public void MoveToPosition (Transform newPosition) {
		startPosition = GetComponent<Transform>();
		sTime = Time.time;
		targetPosition = newPosition;
		moving = true;
	}

	public void ChangeAngle () {
		currentAngle = angles[Random.Range(0, angles.Count)];
		//SnapToPosition (currentAngle.tr);
		GetComponent<Camera>().fieldOfView = currentAngle.fov;
		Debug.Log(currentAngle.name);
	}

	void Start() {
		instance = this;
		SnapToPosition (initialPosition);
		transitionTimer = transitionFreq;
		angles  = new List<CameraView> () {

			// On the hood, forwards
			new CameraView () {
				name = "HoodForward",
				transform = HoodForward,
				targetPos = HoodForward.position,
				targetRot = HoodForward.rotation,
				fov = 75f,
				followMode = CameraFollowMode.Static
			},

			// On the hood, backwards
			/*new CameraView () {
				name = "HoodBackward",
				tr = HoodBackward,
				fov = 90f,
				followMode = CameraFollowMode.Static
			},*/

			// Near chase
			new CameraView () {
				name = "NearChase",
				transform = NearChase,
				targetPos = NearChase.position,
				targetRot = NearChase.rotation,
				fov = 75f,
				followMode = CameraFollowMode.Lead,
				lag = 0.04f
			},

			// Far chase
			new CameraView () {
				name = "FarChase",
				transform = FarChase,
				targetPos = FarChase.position,
				targetRot = FarChase.rotation,
				fov = 75f,
				followMode = CameraFollowMode.Lead,
				lag = 0.05f
			},

			// Front right wheel
			new CameraView () {
				name = "FrontRightWheel",
				transform = FrontRightWheel,
				targetPos = FrontRightWheel.position,
				targetRot = FrontRightWheel.rotation,
				fov = 75f,
				followMode = CameraFollowMode.Static
			},

			// Front left wheel
			new CameraView () {
				name = "FrontLeftWheel",
				transform = FrontLeftWheel,
				targetPos = FrontLeftWheel.position,
				targetRot = FrontLeftWheel.rotation,
				fov = 75f,
				followMode = CameraFollowMode.Static
			},

			// Rear right wheel
			new CameraView () {
				name = "RearRightWheel",
				transform = RearRightWheel,
				targetPos = RearRightWheel.position,
				targetRot = RearRightWheel.rotation,
				fov = 75f,
				followMode = CameraFollowMode.Static
			},

			// Rear left wheel
			new CameraView () {
				name = "RearLeftWheel",
				transform = RearLeftWheel,
				targetPos = RearLeftWheel.position,
				targetRot = RearLeftWheel.rotation,
				fov = 75f,
				followMode = CameraFollowMode.Static
			},

			// Rear left wheel
			new CameraView () {
				name = "RearLeftWheel",
				transform = RearLeftWheel,
				targetPos = RearLeftWheel.position,
				targetRot = RearLeftWheel.rotation,
				fov = 75f,
				followMode = CameraFollowMode.Static
			},

			// Wide rear
			new CameraView () {
				name = "WideRear",
				transform = WideRear,
				targetPos = WideRear.position,
				targetRot = WideRear.rotation,
				fov = 75f,
				followMode = CameraFollowMode.Static
			},

			// Wide front
			new CameraView () {
				name = "WideFront",
				transform = WideFront,
				targetPos = WideFront.position,
				targetRot = WideFront.rotation,
				fov = 75f,
				followMode = CameraFollowMode.Static
			},

			// Far top
			new CameraView () {
				name = "FarTop",
				targetPos = PickRandom (25f, 50f),
				fov = 60f,
				followMode = CameraFollowMode.Shaky
			},

			// Distant
			new CameraView () {
				name = "Distant",
				targetPos = PickRandom (10f, 20f),
				fov = 60f,
				followMode = CameraFollowMode.Shaky
			}
		};

		foreach (CameraView angle in angles) {
			angle.pos = angle.targetPos;
			angle.rot = angle.targetRot;
		}
	}

	Vector3 PickRandom (float minHeight, float maxHeight) {
		float chunkSize = WorldManager.instance.CHUNK_SIZE;
		Vector3 result = new Vector3 (
			WorldManager.instance.player.transform.position.x + Random.Range (-chunkSize / 2f, chunkSize / 2f),
			WorldManager.instance.player.transform.position.y + Random.Range (minHeight, maxHeight),
			WorldManager.instance.player.transform.position.z + Random.Range (-chunkSize / 2f, chunkSize / 2f)
		);
		return result;
	}

	void UpdateAngles () {
		foreach (CameraView angle in angles) {
			if (angle.transform != null) {
				angle.targetPos = angle.transform.position;
				angle.targetRot = angle.transform.rotation;
			}
			switch (angle.followMode) {
			case CameraFollowMode.Lead:
				//angle.pos = (angle.tr.position - angle.pos) * angle.lag * Time.deltaTime;
				//angle.pos =Vector3.MoveTowards (angle.pos, angle.tr.position, Vector3.Distance(angle.pos, angle.tr.position) * angle.lag);
				Vector3 velocity = Vector3.zero;
				angle.pos = Vector3.SmoothDamp (angle.pos, angle.targetPos, ref velocity, angle.lag);
				//transform.rotation = Quaternion.Euler (transform.rotation.eulerAngles + (currentAngle.tr.rotation.eulerAngles - transform.rotation.eulerAngles) * currentAngle.lag * Time.deltaTime);
				angle.rot = Quaternion.LookRotation (WorldManager.instance.player.transform.position + WorldManager.instance.player.transform.forward *20f - angle.pos, Vector3.up);
				break;
			case CameraFollowMode.Static:
				angle.pos = angle.targetPos;
				angle.rot = angle.targetRot;
				break;
			case CameraFollowMode.Shaky:
				angle.pos = angle.targetPos;
				float x = Mathf.PerlinNoise (angle.shake, angle.shake);
				float y = Mathf.PerlinNoise (angle.shake, angle.shake);
				//angle.rot = Quaternion.FromToRotation (angle.pos, WorldManager.instance.player.transform.position);
				transform.LookAt (WorldManager.instance.player.transform.position, Vector3.up);
				angle.rot = transform.rotation;
				//angle.rot = angle.targetRot * Quaternion.Euler (x, y, 0f);
				break;
			}
		}
	}

	void Update() {
		if (liveMode) {
			UpdateAngles();
			//Debug.Log (currentAngle.pos);
			transform.position = currentAngle.pos;
			transform.rotation = currentAngle.rot;
			if (transitionTimer <= 0f) {
				ChangeAngle();
				transitionTimer = transitionFreq;
			} else {
				transitionTimer--;
			}
		} else {
			if (moving) {
				if (Vector3.Distance(startPosition.GetComponent<Transform>().position, targetPosition.GetComponent<Transform>().position) != 0f) {
					GetComponent<Transform>().position = Vector3.Lerp(startPosition.position, targetPosition.position, 
						(Time.time-sTime)*speed*Time.deltaTime/Vector3.Distance(startPosition.GetComponent<Transform>().position, targetPosition.GetComponent<Transform>().position));
					GetComponent<Transform>().rotation = Quaternion.Lerp(startPosition.rotation, targetPosition.rotation, 
						(Time.time-sTime)*speed*Time.deltaTime/Vector3.Distance(startPosition.GetComponent<Transform>().position, targetPosition.GetComponent<Transform>().position));
				} else {
					startPosition = targetPosition;
				}
			}
		}
	}
		
}
