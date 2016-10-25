// UIExtension.cs
// ©2016 Team 95

using UnityEngine;
using UnityEngine.UI;

namespace Route95.UI {

	/// <summary>
	/// Class for UI object extensions.
	/// </summary>
	public static class UIExtension {

		#region RectTransform Extensions

		/// <summary>
		/// Anchors at point.
		/// </summary>
		public static void AnchorAtPoint(this RectTransform tr, float x, float y) {
			Vector2 anchorPoint = new Vector2(x, y);
			tr.anchorMax = anchorPoint;
			tr.anchorMin = anchorPoint;
		}

		/// <summary>
		/// Anchors at point.
		/// </summary>
		public static void AnchorAtPoint(this RectTransform tr, Vector2 anchorPoint) {
			tr.anchorMax = anchorPoint;
			tr.anchorMin = anchorPoint;
		}

		/// <summary>
		/// Resets local scale and rotation to defaults.
		/// </summary>
		public static void ResetScaleRot(this RectTransform tr) {
			tr.localScale = Vector3.one;
			tr.localRotation = Quaternion.Euler(Vector3.zero);
		}

		#endregion
		#region MaskableGraphic Extensions

		/// <summary>
		/// Sets the alpha of this MaskableGraphic.
		/// </summary>
		public static void SetAlpha(this MaskableGraphic gr, float alpha) {
			Color color = gr.color;
			color.a = alpha;
			gr.color = color;
		}

		#endregion
	}
}