using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraView {

	public enum View {
		OutsideCar,
		Driving,
		Radio,
		Chase
	}

	public enum CameraPlacementMode {
		Fixed,
		RandomSky,
		RandomGround
	}

	public enum CameraFollowMode {
		Lead, // Points in front of target
		Static,
		Shaky
	}

	public string name;
	public Transform transform;
	public float fov; // field of view
	public CameraFollowMode followMode; // type of camera following
	public CameraPlacementMode placementMode;
	public Vector3 pos; // current position of camera
	public Quaternion rot; // current rotation of camera

	public Vector3 targetPos; // target transform
	public Quaternion targetRot;

	public float lag; // how tightly camera follows (lower = tighter)
	public float shake;
}

