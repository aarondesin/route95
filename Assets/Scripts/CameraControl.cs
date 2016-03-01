using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum View {
	OutsideCar,
	Driving,
	Radio,
	Chase
};

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
	}

	void Update() {
		if (Vector3.Distance(startPosition.GetComponent<Transform>().position, targetPosition.GetComponent<Transform>().position) != 0f) {
			GetComponent<Transform>().position = Vector3.Lerp(startPosition.position, targetPosition.position, 
				(Time.time-sTime)*speed*Time.deltaTime/Vector3.Distance(startPosition.GetComponent<Transform>().position, targetPosition.GetComponent<Transform>().position));
			GetComponent<Transform>().rotation = Quaternion.Lerp(startPosition.rotation, targetPosition.rotation, 
				(Time.time-sTime)*speed*Time.deltaTime/Vector3.Distance(startPosition.GetComponent<Transform>().position, targetPosition.GetComponent<Transform>().position));
		} else {
			startPosition = targetPosition;
		}
	}

	void Start() {
		instance = this;
		SnapToPosition (initialPosition);
	}
}
