using UnityEngine;
using System.Collections;

public class Tooltip : MonoBehaviour {

	public string text;

	bool mouseOver = false;

	void Update () {
		Vector2 mouse = Input.mousePosition;
		Vector3[] corners = new Vector3[4];
		GetComponent<RectTransform>().GetWorldCorners(corners);

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
}
