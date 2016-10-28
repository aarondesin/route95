// CameraView.cs
// ©2016 Team 95

using UnityEngine;

namespace Route95.World {

	/// <summary>
	/// Structure to hold all camera view information.
	/// </summary>
	public class CameraView {

		#region CameraView Enums

		/// <summary>
		/// Setup mode camera views.
		/// </summary>
		public enum View {
			OutsideCar,
			Driving,
			Radio,
			Chase
		}

		/// <summary>
		/// Type of camera placement.
		/// </summary>
		public enum CameraPlacementMode {
			Fixed,
			RandomSky,
			RandomGround
		}

		/// <summary>
		/// Type of camera follow.
		/// </summary>
		public enum CameraFollowMode {
			Lead, // Points in front of target
			Static,
			Shaky
		}

		#endregion
		#region CameraView Vars

		/// <summary>
		/// Camera view name.
		/// </summary>
		public string name;

		/// <summary>
		/// Transform to use.
		/// </summary>
		public Transform transform;

		/// <summary>
		/// Field of view.
		/// </summary>
		public float fov;

		/// <summary>
		/// Type of camera follow movement.
		/// </summary>
		public CameraFollowMode followMode;

		/// <summary>
		/// Type of camera placement.
		/// </summary>
		public CameraPlacementMode placementMode;

		/// <summary>
		/// Current position of camera.
		/// </summary>
		public Vector3 pos;

		/// <summary>
		/// Current rotation of camera.
		/// </summary>
		public Quaternion rot;

		/// <summary>
		/// Target transform.
		/// </summary>
		public Vector3 targetPos;

		/// <summary>
		/// Target rotation.
		/// </summary>
		public Quaternion targetRot;

		/// <summary>
		/// How tightly the camera follows this view.
		/// </summary>
		public float lag;

		/// <summary>
		/// Amout of camera shake.
		/// </summary>
		public float shake;

		#endregion
	}
}

