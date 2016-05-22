using UnityEngine;
using System.Collections;

/// <summary>
/// Class to handle generic tooltips.
/// </summary>
public class Tooltip : MonoBehaviour {

	#region Tooltip Vars

	[Tooltip("Text to show in tooltip.")]
	public string text;

	bool mouseOver = false; // Is the mouse over a tooltip object

	#endregion
	#region Unity Callbacks

	void Update () {
		Vector2 mouse = Input.mousePosition;
		Vector3[] corners = new Vector3[4];
		gameObject.RectTransform().GetWorldCorners(corners);

		if (mouse.x >= corners[0].x && mouse.x < corners[2].x &&
			mouse.y >= corners[0].y && mouse.y < corners[2].y) {
			if (!mouseOver) {
				GameManager.instance.ShowTooltip(text);
				mouseOver = true;
			}
		} else {
			if (mouseOver) {
				GameManager.instance.HideTooltip();
				mouseOver = false;
			}
		}
	}

	#endregion
}
