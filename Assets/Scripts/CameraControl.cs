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
	Static
}

public class CameraView {
	public string name;
	public Transform tr;
	public float fov;
	public CameraFollowMode followMode;
	public float lag;
	public Vector3 pos;
	public Quaternion rot;
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
				tr = HoodForward,
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
				tr = NearChase,
				fov = 75f,
				followMode = CameraFollowMode.Lead,
				lag = 0.04f
			},

			// Far chase
			new CameraView () {
				name = "FarChase",
				tr = FarChase,
				fov = 75f,
				followMode = CameraFollowMode.Lead,
				lag = 0.05f
			},

			// Front right wheel
			new CameraView () {
				name = "FrontRightWheel",
				tr = FrontRightWheel,
				fov = 75f,
				followMode = CameraFollowMode.Static
			},

			// Front left wheel
			new CameraView () {
				name = "FrontLeftWheel",
				tr = FrontLeftWheel,
				fov = 75f,
				followMode = CameraFollowMode.Static
			},

			// Rear right wheel
			new CameraView () {
				name = "RearRightWheel",
				tr = RearRightWheel,
				fov = 75f,
				followMode = CameraFollowMode.Static
			},

			// Rear left wheel
			new CameraView () {
				name = "RearLeftWheel",
				tr = RearLeftWheel,
				fov = 75f,
				followMode = CameraFollowMode.Static
			},

			// Rear left wheel
			new CameraView () {
				name = "RearLeftWheel",
				tr = RearLeftWheel,
				fov = 75f,
				followMode = CameraFollowMode.Static
			},

			// Wide rear
			new CameraView () {
				name = "WideRear",
				tr = WideRear,
				fov = 75f,
				followMode = CameraFollowMode.Static
			},

			// Wide front
			new CameraView () {
				name = "WideFront",
				tr = WideFront,
				fov = 75f,
				followMode = CameraFollowMode.Static
			}
		};

		foreach (CameraView angle in angles) {
			angle.pos = angle.tr.position;
			angle.rot = angle.tr.rotation;
		}
	}

	void UpdateAngles () {
		foreach (CameraView angle in angles) {
			switch (angle.followMode) {
			case CameraFollowMode.Lead:
				//angle.pos = (angle.tr.position - angle.pos) * angle.lag * Time.deltaTime;
				//angle.pos =Vector3.MoveTowards (angle.pos, angle.tr.position, Vector3.Distance(angle.pos, angle.tr.position) * angle.lag);
				Vector3 velocity = Vector3.zero;
				angle.pos = Vector3.SmoothDamp (angle.pos, angle.tr.position, ref velocity, angle.lag);
				//transform.rotation = Quaternion.Euler (transform.rotation.eulerAngles + (currentAngle.tr.rotation.eulerAngles - transform.rotation.eulerAngles) * currentAngle.lag * Time.deltaTime);
				angle.rot = Quaternion.LookRotation (WorldManager.instance.player.transform.position + WorldManager.instance.player.transform.forward *20f - angle.pos, Vector3.up);
				break;
			case CameraFollowMode.Static:
				angle.pos = angle.tr.position;
				angle.rot = angle.tr.rotation;
				break;
			}
		}
	}

	void Update() {
		if (liveMode) {
			UpdateAngles();
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
