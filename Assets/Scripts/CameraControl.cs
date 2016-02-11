using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public Transform _initialPosition;

	Transform _startPosition;
	Transform _targetPosition;
	public float _speed;
	float _sTime;

	public void SetSpeed (float newSpeed) {
		_speed = newSpeed;
	}

	public void SnapToPosition (Transform newPosition) {
		_startPosition = newPosition;
		_targetPosition = newPosition;
		GetComponent<Transform>().position = newPosition.position;
		GetComponent<Transform>().rotation = newPosition.rotation;
	}

	public void MoveToPosition (Transform newPosition) {
		_startPosition = GetComponent<Transform>();
		_sTime = Time.time;
		_targetPosition = newPosition;
	}

	void Update() {
		if (Vector3.Distance(_startPosition.GetComponent<Transform>().position, _targetPosition.GetComponent<Transform>().position) != 0f) {
			GetComponent<Transform>().position = Vector3.Lerp(_startPosition.position, _targetPosition.position, 
				(Time.time-_sTime)*_speed*Time.deltaTime/Vector3.Distance(_startPosition.GetComponent<Transform>().position, _targetPosition.GetComponent<Transform>().position));
			GetComponent<Transform>().rotation = Quaternion.Lerp(_startPosition.rotation, _targetPosition.rotation, 
				(Time.time-_sTime)*_speed*Time.deltaTime/Vector3.Distance(_startPosition.GetComponent<Transform>().position, _targetPosition.GetComponent<Transform>().position));
		} else {
			_startPosition = _targetPosition;
		}
	}

	void Start() {
		SnapToPosition (_initialPosition);
	}
}
