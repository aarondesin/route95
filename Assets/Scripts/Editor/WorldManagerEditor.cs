using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Custom editor for WorldManager.
/// </summary>
[CustomEditor(typeof(WorldManager))]
public class WorldManagerEditor : Editor {

	public override void OnInspectorGUI () {

		DrawDefaultInspector();

		// Button to show debug colors
		if (GUILayout.Button("Show constraints")) {
			((WorldManager)target).DebugTerrain();
		}

		if (GUILayout.Button("Print vertex map")) {
			((WorldManager)target).PrintVertexMap();
		}
	}
}
