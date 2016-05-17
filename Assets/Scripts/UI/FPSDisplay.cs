﻿// Script by Dave Hampson

using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour {

	float deltaTime = 0.0f;

	void Update () {
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}

	void OnGUI() {
		if (CameraControl.instance.state != CameraControl.State.Free) {
			int w = Screen.width, h = Screen.height;

			GUIStyle style = new GUIStyle();

			Rect rect = new Rect (0, 0, w, h * 2 /100);
			style.alignment = TextAnchor.UpperLeft;
			style.fontSize = h * 2 /100;
			style.normal.textColor = new Color (0f, 0f, 1f, 1f);
			//float msec = deltaTime * 1000f;
			float fps = 1f / deltaTime;
			//string text = string.Format("{0:0000.0} ms ({1:0000.} fps)", msec, fps);
			string text = fps.ToString ("0000.") + "fps";
			GUI.Label(rect, text, style);
		}
	}
}
